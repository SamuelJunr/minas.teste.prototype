using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading; // Para CancellationTokenSource
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.Service;
using System.Diagnostics;
using System.Text.RegularExpressions; // Para Regex na validação

namespace minas.teste.prototype.MVVM.View
{
    public partial class conexao : Form
    {
        // Usar a instância persistente de ConnectionSettingsApplication
        private readonly SerialManager _serialManager;
        private readonly StringBuilder _dataBuffer = new StringBuilder();
        private readonly System.Windows.Forms.Timer _textBoxUpdateTimer;
        private readonly object _bufferLock = new object();
        private Form _ownerForm; // Para retornar ao formulário anterior
        private bool _fechamentoForcado = false;

        private CancellationTokenSource _validationCts; // Para cancelar a validação de dados
        private bool _dadosValidosRecebidos = false; // Flag para o resultado da validação

        // Padrão Regex simplificado para verificar o início "P1:" e o final "|temp:" (ou final da linha com temp)
        // Ajuste este Regex para ser mais preciso conforme o formato exato da Tela_Bombas
        private static readonly Regex ExpectedDataPattern = new Regex(@"^P1:.*?\|temp:.*$", RegexOptions.Compiled);


        public conexao(Form owner = null)
        {
            InitializeComponent();
            _ownerForm = owner ?? new Menuapp();

            _serialManager = ConnectionSettingsApplication.PersistentSerialManager;
            // As subscrições de eventos são feitas em InitializeSerialManagerHandlers

            InitializeSerialManagerHandlers(true); // Subscreve os eventos
            ConfigureUI();

            _textBoxUpdateTimer = new System.Windows.Forms.Timer { Interval = 200 }; // Atualiza o TextBox mais rápido
            _textBoxUpdateTimer.Tick += TextBoxUpdateTimer_Tick;
            _textBoxUpdateTimer.Start();
        }

        private void InitializeSerialManagerHandlers(bool subscribe)
        {
            if (subscribe)
            {
                _serialManager.DataReceived += SerialManager_DataReceived;
                _serialManager.ConnectionStatusChanged += SerialManager_ConnectionStatusChanged;
                _serialManager.ErrorOccurred += SerialManager_ErrorOccurred;
            }
            else
            {
                _serialManager.DataReceived -= SerialManager_DataReceived;
                _serialManager.ConnectionStatusChanged -= SerialManager_ConnectionStatusChanged;
                _serialManager.ErrorOccurred -= SerialManager_ErrorOccurred;
            }
        }
        private void ConfigureUI()
        {
            comboBoxBaudRate.DataSource = null; // Limpa antes de repopular
            comboBoxBaudRate.DataSource = SerialManager.CommonBaudRates.ToList(); // Use ToList() para criar uma nova coleção

            // Seleciona o Baud Rate salvo ou um valor padrão
            int currentBaudRate = ConnectionSettingsApplication.CurrentBaudRate; // Use CurrentBaudRate
            comboBoxBaudRate.SelectedItem = currentBaudRate != 0 ? currentBaudRate : (object)115200;

            RefreshPortsList(); // Popula e tenta selecionar a porta

            UpdateConnectionStatusUI(_serialManager.IsConnected);
        }

        private void RefreshPortsList()
        {
            try
            {
                comboBoxPortaCOM.BeginUpdate();
                string previousSelection = comboBoxPortaCOM.SelectedItem?.ToString();
                _serialManager.RefreshPorts(); // Pede ao SerialManager para atualizar sua lista interna
                comboBoxPortaCOM.DataSource = null; // Limpa antes de repopular
                comboBoxPortaCOM.DataSource = _serialManager.AvailablePorts.ToList(); // Usa a lista do SerialManager

                if (_serialManager.AvailablePorts.Any())
                {
                    string portToSelect = null;
                    if (!string.IsNullOrEmpty(ConnectionSettingsApplication.CurrentPortName) && // Use CurrentPortName
                        _serialManager.AvailablePorts.Contains(ConnectionSettingsApplication.CurrentPortName))
                    {
                        portToSelect = ConnectionSettingsApplication.CurrentPortName;
                    }
                    else if (!string.IsNullOrEmpty(previousSelection) &&
                             _serialManager.AvailablePorts.Contains(previousSelection))
                    {
                        portToSelect = previousSelection;
                    }
                    else
                    {
                        portToSelect = _serialManager.AvailablePorts[0];
                    }
                    comboBoxPortaCOM.SelectedItem = portToSelect;
                    SetControlsEnabled(true);
                }
                else
                {
                    comboBoxPortaCOM.Items.Add("Nenhuma porta COM encontrada");
                    comboBoxPortaCOM.SelectedIndex = 0;
                    SetControlsEnabled(false);
                    lblStatusConexao.Text = "Nenhuma porta COM.";
                }
                comboBoxPortaCOM.EndUpdate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao atualizar portas: {ex.Message}");
                MessageBox.Show($"Erro ao atualizar lista de portas: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetControlsEnabled(false);
            }
        }

        private void SetControlsEnabled(bool enabled)
        {
            comboBoxPortaCOM.Enabled = enabled;
            comboBoxBaudRate.Enabled = enabled; // Habilita/desabilita baudrate junto com a porta
            // O botão Conectar/Desconectar (btnTestarConexao) é gerenciado por UpdateConnectionStatusUI
            // O botão Salvar (btnSalvarAplicarConfiguracoes) pode ser sempre habilitado ou depender da porta.
            btnSalvarAplicarConfiguracoes.Enabled = enabled;
        }


        private void TextBoxUpdateTimer_Tick(object sender, EventArgs e)
        {
            string dataToAppend = null;
            lock (_bufferLock)
            {
                if (_dataBuffer.Length > 0)
                {
                    dataToAppend = _dataBuffer.ToString();
                    _dataBuffer.Clear();
                }
            }

            if (!string.IsNullOrEmpty(dataToAppend))
            {
                if (textBox1 != null && !textBox1.IsDisposed && !textBox1.Disposing)
                {
                    try
                    {
                        Action appendAction = () =>
                        {
                            if (textBox1.TextLength > 20000) textBox1.Clear(); // Limpa se muito grande
                            textBox1.AppendText(dataToAppend);
                            textBox1.ScrollToCaret();
                        };

                        if (textBox1.InvokeRequired)
                        {
                            textBox1.BeginInvoke(appendAction);
                        }
                        else
                        {
                            appendAction();
                        }
                    }
                    catch (Exception ex) when (ex is ObjectDisposedException || ex is InvalidOperationException)
                    { /* Ignora se o controle foi descartado */ }
                }
            }
        }
        private void SerialManager_DataReceived(object sender, string data)
        {
            // Filtra caracteres não legíveis, exceto nova linha e retorno de carro.
            // Isso ajuda a garantir que apenas texto "limpo" seja bufferizado e validado.
            string cleanData = FilterNonPrintableChars(data);

            if (!string.IsNullOrEmpty(cleanData))
            {
                lock (_bufferLock)
                {
                    _dataBuffer.Append(cleanData); // Append em vez de AppendLine para ter controle sobre novas linhas
                }

                // Se a validação de dados estiver em andamento, verifica o padrão
                if (_validationCts != null && !_validationCts.IsCancellationRequested)
                {
                    if (!this.IsDisposed && !this.Disposing)
                    {
                        this.Invoke((Action)(() => { /* UI update logic */ }));
                    }
                    // Processa o buffer atual para linhas completas para validação
                    string currentBufferSnapshot;
                    lock (_bufferLock) // Re-lock para ler o buffer que pode ter sido modificado
                    {
                        currentBufferSnapshot = _dataBuffer.ToString();
                    }

                    // Quebra em linhas para validar cada uma
                    string[] lines = currentBufferSnapshot.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
                        if (ExpectedDataPattern.IsMatch(line.Trim()))
                        {
                            _dadosValidosRecebidos = true;
                            _validationCts.Cancel(); // Encontrou dados válidos, pode parar a validação
                            Debug.WriteLine($"Dados válidos recebidos: {line.Trim()}");
                            break;
                        }
                    }
                }
            }
        }

        private string FilterNonPrintableChars(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                // Permite caracteres imprimíveis, nova linha, retorno de carro e tabulação.
                // Você pode ajustar esta lógica se precisar permitir outros caracteres de controle específicos.
                if (char.IsControl(c) && c != '\n' && c != '\r' && c != '\t')
                    continue;
                sb.Append(c);
            }
            return sb.ToString();
        }


        private void SerialManager_ConnectionStatusChanged(object sender, bool isConnected)
        {
            if (this.IsDisposed || this.Disposing) return;

            Action updateAction = () => UpdateConnectionStatusUI(isConnected);

            if (InvokeRequired)
            {
                BeginInvoke(updateAction);
            }
            else
            {
                updateAction();
            }
        }

        private void UpdateConnectionStatusUI(bool isConnected)
        {
            if (lblStatusConexao != null && !lblStatusConexao.IsDisposed)
            {
                // btnTestarConexao é o seu "cuiButton1" com texto "Conectar"
                btnTestarConexao.Content = isConnected ? "Desconectar" : "Conectar";
                btnTestarConexao.NormalBackground = isConnected ? Color.Crimson : Color.DarkGreen;

                lblStatusConexao.Text = isConnected ?
                    $"Conectado: {ConnectionSettingsApplication.CurrentPortName} @ {ConnectionSettingsApplication.CurrentBaudRate}" :
                    "Desconectado";
                lblStatusConexao.ForeColor = isConnected ? Color.Green : Color.Red;
            }

            if (comboBoxPortaCOM != null && !comboBoxPortaCOM.IsDisposed)
            {
                comboBoxPortaCOM.Enabled = !isConnected && (_serialManager.AvailablePorts?.Any(p => p != "Nenhuma porta COM encontrada") ?? false);
            }
            if (comboBoxBaudRate != null && !comboBoxBaudRate.IsDisposed)
            {
                comboBoxBaudRate.Enabled = !isConnected && (_serialManager.AvailablePorts?.Any(p => p != "Nenhuma porta COM encontrada") ?? false);
            }
            if (btnSalvarAplicarConfiguracoes != null && !btnSalvarAplicarConfiguracoes.IsDisposed)
            {
                btnSalvarAplicarConfiguracoes.Enabled = !isConnected && (_serialManager.AvailablePorts?.Any(p => p != "Nenhuma porta COM encontrada") ?? false);
            }

            // O botão de Desconectar separado (cuiButton2) pode ser desabilitado/habilitado aqui também,
            // ou completamente removido se btnTestarConexao (cuiButton1) lida com ambas as ações.
            if (cuiButton2 != null && !cuiButton2.IsDisposed) // Assumindo que cuiButton2 é o botão de desconectar dedicado
            {
                cuiButton2.Enabled = isConnected;
            }
        }


        private void SerialManager_ErrorOccurred(object sender, string errorMessage)
        {
            if (this.IsDisposed || this.Disposing) return;
            BeginInvoke((MethodInvoker)delegate
            {
                if (!this.IsDisposed && !this.Disposing)
                {
                    MessageBox.Show(this, errorMessage, "Erro na Comunicação Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatusConexao.Text = "Erro na conexão.";
                    lblStatusConexao.ForeColor = Color.Red;
                }
            });
        }

        // Evento do seu cuiButton1 ("Conectar"/"Desconectar")
        // No designer, btnTestarConexao.Click += new System.EventHandler(this.btnConectarDesconectar_Click);
        private async void btnConectarDesconectar_Click(object sender, EventArgs e) // Renomeado de btnConnect_Click
        {
            if (_serialManager.IsConnected)
            {
                _serialManager.Disconnect();
                // UpdateConnectionStatusUI será chamado pelo evento ConnectionStatusChanged
            }
            else
            {
                if (comboBoxPortaCOM.SelectedItem == null || comboBoxPortaCOM.SelectedItem.ToString() == "Nenhuma porta COM encontrada")
                {
                    MessageBox.Show(this, "Por favor, selecione uma porta COM válida.", "Seleção Necessária", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (comboBoxBaudRate.SelectedItem == null)
                {
                    MessageBox.Show(this, "Por favor, selecione uma taxa de Baud Rate válida.", "Seleção Necessária", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var port = comboBoxPortaCOM.SelectedItem.ToString();
                var baudRate = (int)comboBoxBaudRate.SelectedItem;

                lblStatusConexao.Text = $"Tentando conectar a {port}@{baudRate}...";
                lblStatusConexao.ForeColor = Color.Orange;
                btnTestarConexao.Enabled = false; // Desabilita durante a tentativa
                Application.DoEvents();

                bool success = ConnectionSettingsApplication.UpdateConnection(port, baudRate);

                if (success)
                {
                    lblStatusConexao.Text = "Conectado. Validando dados...";
                    lblStatusConexao.ForeColor = Color.BlueViolet;
                    Application.DoEvents();

                    // Inicia a validação dos dados
                    _dadosValidosRecebidos = false;
                    _validationCts?.Cancel(); // Cancela validação anterior, se houver
                    _validationCts = new CancellationTokenSource();

                    try
                    {
                        // Espera por dados válidos por um curto período (ex: 3 segundos)
                        await Task.Delay(TimeSpan.FromSeconds(3), _validationCts.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        // Ocorre se _dadosValidosRecebidos se tornou true e _validationCts.Cancel() foi chamado,
                        // ou se o token foi cancelado por outra razão (ex: desconexão).
                    }

                    if (_dadosValidosRecebidos)
                    {
                        lblStatusConexao.Text = $"Conectado e dados válidos! {port}@{baudRate}";
                        lblStatusConexao.ForeColor = Color.Green;
                        // Os settings em ConnectionSettingsApplication já foram atualizados por UpdateConnection.
                        // Agora, salve permanentemente.
                        SalvarConfiguracoesPersistentes(port, baudRate);
                        MessageBox.Show(this, $"Conectado a {port}@{baudRate}. Dados no formato esperado recebidos e configurações salvas.", "Conexão Bem-sucedida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        lblStatusConexao.Text = $"Porta {port} aberta, mas dados não estão no formato esperado. Desconectando.";
                        lblStatusConexao.ForeColor = Color.DarkOrange;
                        MessageBox.Show(this, $"A porta {port} foi aberta, mas os dados recebidos não correspondem ao padrão esperado para a Tela_Bombas. A conexão será desfeita.", "Dados Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        _serialManager.Disconnect(); // Desconecta se os dados não forem válidos
                    }
                }
                else
                {
                    // A falha na conexão já é tratada pelo evento ErrorOccurred ou pelo status em UpdateConnectionStatusUI
                    // Mas podemos reforçar a mensagem aqui.
                    lblStatusConexao.Text = $"Falha ao conectar a {port}.";
                    lblStatusConexao.ForeColor = Color.Red;
                    // MessageBox.Show já deve ter sido mostrado por SerialManager_ErrorOccurred se houve erro.
                }
                _validationCts?.Dispose();
                _validationCts = null;
                btnTestarConexao.Enabled = true; // Reabilita o botão após a tentativa
                UpdateConnectionStatusUI(_serialManager.IsConnected); // Garante que a UI reflita o estado final
            }
        }

        // Evento do seu cuiButton2 ("Desconectar")
        // No designer, cuiButton2.Click += new System.EventHandler(this.btnDesconectarSeparado_Click);
        private void btnDesconectarSeparado_Click(object sender, EventArgs e) // Renomeado de btnDesconnect_Click
        {
            if (_serialManager.IsConnected)
            {
                _serialManager.Disconnect();
            }
            else
            {
                MessageBox.Show(this, "Nenhuma conexão ativa para desconectar.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        // The error CS0272 indicates that the property `ConnectionSettingsApplication.CurrentBaudRate`
        // does not have a public setter, and therefore cannot be assigned directly.
        // To fix this, you need to use an appropriate method or mechanism provided by the `ConnectionSettingsApplication` class
        // to update the `CurrentBaudRate` value. Based on the provided context, the `UpdateConnection` method
        // seems to be the correct way to update both the port and baud rate.

        private void btnSalvarConfiguracoesClick(object sender, EventArgs e) // Renomeado de cuiButton3_Click
        {
            if (comboBoxPortaCOM.SelectedItem == null || comboBoxPortaCOM.SelectedItem.ToString() == "Nenhuma porta COM encontrada")
            {
                MessageBox.Show(this, "Por favor, selecione uma porta COM válida para salvar.", "Seleção Necessária", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboBoxBaudRate.SelectedItem == null)
            {
                MessageBox.Show(this, "Por favor, selecione uma taxa de Baud Rate válida para salvar.", "Seleção Necessária", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var port = comboBoxPortaCOM.SelectedItem.ToString();
            var baudRate = (int)comboBoxBaudRate.SelectedItem;

            // Pergunta se quer salvar. Se já estiver conectado à mesma porta/baud, apenas confirma.
            // Se a conexão ativa for diferente, ou se não estiver conectado, pergunta se quer salvar E aplicar.
            string mensagemConfirmacao = $"Deseja salvar Porta: {port} e Baud Rate: {baudRate} como padrão?";
            if (_serialManager.IsConnected && (ConnectionSettingsApplication.CurrentPortName != port || ConnectionSettingsApplication.CurrentBaudRate != baudRate))
            {
                mensagemConfirmacao += "\nIsso também tentará aplicar e reconectar com estas novas configurações.";
            }
            else if (!_serialManager.IsConnected)
            {
                mensagemConfirmacao += "\nVocê precisará conectar manualmente após salvar.";
            }

            var result = MessageBox.Show(this, mensagemConfirmacao, "Salvar Configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Use the UpdateConnection method to save and apply the new settings.
                bool success = ConnectionSettingsApplication.UpdateConnection(port, baudRate);

                if (success)
                {
                    MessageBox.Show(this, "Configurações salvas com sucesso!\nUse o botão 'Conectar' para ativar esta configuração se necessário.", "Configurações Salvas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "Falha ao salvar as configurações. Verifique as configurações e tente novamente.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Atualiza a UI caso a porta salva seja a atualmente conectada, ou se estava desconectado.
                UpdateConnectionStatusUI(_serialManager.IsConnected);
            }
        }

        private void SalvarConfiguracoesPersistentes(string port, int baudRate)
        {
            try
            {
                Properties.Settings.Default.PortaCOMSelecionada = port;
                Properties.Settings.Default.BaudRateSelecionado = baudRate;
                Properties.Settings.Default.Save();
                Debug.WriteLine($"Configurações persistentes salvas: Porta={port}, BaudRate={baudRate}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar configurações persistentes: {ex.Message}");
                MessageBox.Show(this, $"Ocorreu um erro ao salvar as configurações: {ex.Message}", "Erro de Salvamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void conexao_FormClosing(object sender, FormClosingEventArgs e)
        {
            _textBoxUpdateTimer?.Stop();
            _textBoxUpdateTimer?.Dispose();
            InitializeSerialManagerHandlers(false); // Cancela subscrição dos eventos

            // NÃO desconecte o PersistentSerialManager aqui automaticamente,
            // a menos que seja um fechamento da aplicação inteira.
            // A conexão persistente deve sobreviver ao fechamento desta tela de config.
            // _serialManager?.Disconnect(); // REMOVIDO - Não desconectar aqui.

            if (!_fechamentoForcado)
            {
                // Se o usuário clicou no 'X' do formulário, trata como fechamento da aplicação.
                DialogResult dr = MessageBox.Show("Deseja fechar toda a aplicação?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    ConnectionSettingsApplication.CloseAllConnections(); // Agora sim, fecha tudo
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true; // Cancela o fechamento do formulário se o usuário disser não.
                }
            }
            // Se _fechamentoForcado (pelo botão Voltar), o owner form já foi mostrado.
        }

        // Evento do seu button1 ("Retornar")
        // No designer, Bntvoltar.Click += new System.EventHandler(this.btnVoltar_Click);
        private void btnVoltar_Click(object sender, EventArgs e) // Renomeado de button1_Click
        {
            _fechamentoForcado = true;
            _ownerForm.Show();
            this.Close();
        }

        private void conexao_Load(object sender, EventArgs e)
        {
            // O método ConfigureUI já é chamado no construtor e lida com
            // a configuração inicial baseada em ConnectionSettingsApplication.
            // Mensagens informativas:
            if (_serialManager.IsConnected)
            {
                // Não precisa de MessageBox aqui, UpdateConnectionStatusUI já atualiza o label.
                // lblStatusConexao já mostrará o status conectado.
            }
            else if (!string.IsNullOrEmpty(ConnectionSettingsApplication.CurrentPortName))
            {
                lblInfoAdicional.Text = $"Última config: {ConnectionSettingsApplication.CurrentPortName}@{ConnectionSettingsApplication.CurrentBaudRate}."; // Supondo um lblInfoAdicional
            }
            else
            {
                lblInfoAdicional.Text = "Nenhuma config salva. Selecione e conecte."; // Supondo um lblInfoAdicional
            }
        }
        // Adicione um Label chamado lblInfoAdicional no seu designer para essas mensagens.

        // Adicione este botão ao seu designer se ainda não o fez (btnAtualizarPortas)
        private void btnAtualizarPortas_Click(object sender, EventArgs e)
        {
            lblStatusConexao.Text = "Atualizando portas...";
            Application.DoEvents();
            RefreshPortsList();
            if (comboBoxPortaCOM.Enabled)
            {
                lblStatusConexao.Text = "Lista de portas COM atualizada.";
            }
            // O status da conexão não é alterado, então não precisa mexer no lblStatusConexao principal
        }
    }
}