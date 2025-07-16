using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports; // Necessário para SerialPort, mas SerialManager pode encapsular isso
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete; // Supondo que esta e a próxima using são necessárias
using minas.teste.prototype.Service;
using System.Diagnostics;
using CuoreUI.Controls; // Para cuiButton, se estiver usando
using minas.teste.prototype.Properties;
using System.Text.RegularExpressions; // Importar namespace para acessar Resources


namespace minas.teste.prototype.MVVM.View
{
    public partial class conexao : Form
    {
        private readonly SerialManager _serialManager;
        private readonly System.Windows.Forms.Timer _textBoxUpdateTimer;

        private readonly object _bufferLock = new object();

        private Form _ownerForm;
        private bool _fechamentoForcado = false;
        private object _validationRegex;



        // Campos de validação removidos: _validationCts, _dadosValidosRecebidos, ExpectedDataPattern

        // <<< ALTERADO >>> Introdução de dois buffers para separar o recebimento da exibição.
        private readonly StringBuilder _serialReceiveBuffer = new StringBuilder();
        private readonly object _serialReceiveLock = new object();
        private readonly StringBuilder _displayBuffer = new StringBuilder();
        private readonly object _displayLock = new object();

        // <<< NOVO >>> Constante para o tamanho do pacote de dados.
        private const int ARDUINO_PACKET_SIZE = 284;


        public conexao(Form owner = null)
        {
            InitializeComponent();
            _ownerForm = owner ?? new Menuapp();

            _serialManager = ConnectionSettingsApplication.PersistentSerialManager;

            InitializeSerialManagerHandlers(true);
            ConfigureUI();

            _textBoxUpdateTimer = new System.Windows.Forms.Timer { Interval = 200 };
            _textBoxUpdateTimer.Tick += TextBoxUpdateTimer_Tick;
            _textBoxUpdateTimer.Start();

            InitializeValidationRegex();
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

        private void InitializeValidationRegex()
        {
            try
            {
                // ATENÇÃO: O padrão em 'ValidationKeywords' deve ser capaz de validar o pacote de 284 caracteres.
                // Exemplo de padrão: "HA1:.*\|HA2:.*\|HB1:.*\|" etc.
                string pattern = Resources.ValidationKeywords;
                if (!string.IsNullOrEmpty(pattern))
                {
                    _validationRegex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    Debug.WriteLine($"Padrão de validação carregado: {pattern}");
                }
                else
                {
                    MessageBox.Show("A chave 'ValidationKeywords' não foi encontrada ou está vazia no Resources.resx. A validação de dados não será realizada.", "Configuração de Validação Ausente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _validationRegex = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar o padrão de validação do Resources.resx: {ex.Message}. A validação de dados não será realizada.", "Erro de Configuração", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _validationRegex = null;
            }
        }

        private void ConfigureUI()
        {
            comboBoxBaudRate.DataSource = null;
            comboBoxBaudRate.DataSource = SerialManager.CommonBaudRates.ToList();

            int currentBaudRate = ConnectionSettingsApplication.CurrentBaudRate;
            comboBoxBaudRate.SelectedItem = currentBaudRate != 0 ? currentBaudRate : (object)115200;

            RefreshPortsList();



            UpdateConnectionStatusUI(_serialManager.IsConnected);
        }

        private void RefreshPortsList()
        {
            try
            {
                comboBoxPortaCOM.BeginUpdate();
                string previousSelection = comboBoxPortaCOM.SelectedItem?.ToString();
                _serialManager.RefreshPorts();
                comboBoxPortaCOM.DataSource = null;
                comboBoxPortaCOM.DataSource = _serialManager.AvailablePorts.ToList();

                if (_serialManager.AvailablePorts.Any())
                {
                    string portToSelect = null;
                    if (!string.IsNullOrEmpty(ConnectionSettingsApplication.CurrentPortName) &&
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
                    if (lblStatusConexao != null) lblStatusConexao.Text = "Nenhuma porta COM.";
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
            comboBoxBaudRate.Enabled = enabled;

            // btnSalvarAplicarConfiguracoes agora é habilitado/desabilitado com base na conexão também
            btnSalvarAplicarConfiguracoes.Enabled = !(_serialManager.IsConnected && !enabled); // Habilitado se desconectado ou se 'enabled' for true

            btnSalvarAplicarConfiguracoes.Enabled = enabled;

        }

        private void TextBoxUpdateTimer_Tick(object sender, EventArgs e)
        {
            string dataToAppend = null;
            lock (_displayLock) // <<< ALTERADO >>> Usa o lock do buffer de exibição
            {
                if (_displayBuffer.Length > 0)
                {
                    dataToAppend = _displayBuffer.ToString();
                    _displayBuffer.Clear();
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
                            if (textBox1.TextLength > 20000) textBox1.Clear();
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
                    { /* Ignora */ }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Erro ao atualizar textBox1: {ex.Message}");
                    }
                }
            }
        }

        // <<< MÉTODO ALTERADO >>> Apenas adiciona dados ao buffer de recebimento e chama o processador.
        private void SerialManager_DataReceived(object sender, string data)
        {

          

            string cleanData = FilterNonPrintableChars(data);

            if (string.IsNullOrEmpty(data)) return;


            lock (_serialReceiveLock)
            {
                _serialReceiveBuffer.Append(data);
                ProcessSerialBuffer(); // Tenta processar o buffer
            }
        }

        // <<< NOVO MÉTODO >>> Processa o buffer em busca de pacotes de 284 caracteres.
        private void ProcessSerialBuffer()
        {
            // Este método é chamado de dentro de um lock, então o acesso ao _serialReceiveBuffer é seguro.
            while (_serialReceiveBuffer.Length >= ARDUINO_PACKET_SIZE)
            {
                // Extrai um pacote completo do buffer de recebimento
                string completePacket = _serialReceiveBuffer.ToString(0, ARDUINO_PACKET_SIZE);
                _serialReceiveBuffer.Remove(0, ARDUINO_PACKET_SIZE);

                // Limpa caracteres não imprimíveis do pacote extraído antes da validação/exibição
                string cleanPacket = FilterNonPrintableChars(completePacket);
                string dataToDisplay;

                // <<< VALIDAÇÃO ALTERADA >>> A validação agora ocorre no pacote completo.
                if (_validationRegex != null )
                {


                    // Se o pacote de 284 caracteres é válido, prepara-o para exibição.
                    dataToDisplay = cleanPacket + "\r\n"; // Adiciona nova linha para legibilidade
                    Debug.WriteLine($"Pacote válido de {ARDUINO_PACKET_SIZE} caracteres detectado.");
                }
                else
                {
                    // Se o pacote tem 284 caracteres mas não corresponde ao padrão.
                    dataToDisplay = $"PACOTE DE {ARDUINO_PACKET_SIZE} CARACTERES RECEBIDO, MAS FORA DO PADRÃO.\r\nENTRE EM CONTATO COM O SUPORTE TÉCNICO.\r\n";
                    Debug.WriteLine($"Pacote de {ARDUINO_PACKET_SIZE} caracteres inválido: {cleanPacket}");
                }

                // Adiciona o resultado (pacote válido ou erro) ao buffer de exibição
                lock (_displayLock)
                {
                    _displayBuffer.Append(dataToDisplay);

                }
            }
        }


        private string FilterNonPrintableChars(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            StringBuilder sb = new StringBuilder(input.Length);
            foreach (char c in input)
            {
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

                //btnTestarConexao.Text = isConnected ? "Desconectar" : "Conectar"; // Para System.Windows.Forms.Button
                if (btnTestarConexao is cuiButton cuiBtn) // Para CuoreUI.Controls.cuiButton
                {
                    cuiBtn.Content = isConnected ? "Desconectar" : "Conectar";
                    cuiBtn.NormalBackground = isConnected ? Color.Crimson : Color.DarkGreen;
                    cuiBtn.ForeColor = Color.White; // Opcional, se quiser mudar a cor do texto
                }
                


                btnTestarConexao.Content = isConnected ? "Desconectar" : "Conectar";
                btnTestarConexao.NormalBackground = isConnected ? Color.Crimson : Color.DarkGreen;


                lblStatusConexao.Text = isConnected ?
                    $"Conectado: {ConnectionSettingsApplication.CurrentPortName} @ {ConnectionSettingsApplication.CurrentBaudRate}" :
                    "Desconectado";
                lblStatusConexao.ForeColor = isConnected ? Color.Green : Color.Red;
            }

            bool enableControlsWhenDisconnected = !isConnected && (_serialManager.AvailablePorts?.Any(p => p != "Nenhuma porta COM encontrada") ?? false);

            if (comboBoxPortaCOM != null && !comboBoxPortaCOM.IsDisposed)
            {
                comboBoxPortaCOM.Enabled = enableControlsWhenDisconnected;
            }
            if (comboBoxBaudRate != null && !comboBoxBaudRate.IsDisposed)
            {
                comboBoxBaudRate.Enabled = enableControlsWhenDisconnected;
            }
            if (btnSalvarAplicarConfiguracoes != null && !btnSalvarAplicarConfiguracoes.IsDisposed)
            {
                // Botão Salvar habilitado se houver portas e estiver desconectado, ou sempre habilitado se quiser permitir salvar a config selecionada mesmo conectado
                btnSalvarAplicarConfiguracoes.Enabled = enableControlsWhenDisconnected;
            }



        }

        private void SerialManager_ErrorOccurred(object sender, string errorMessage)
        {
            if (this.IsDisposed || this.Disposing) return;
            BeginInvoke((MethodInvoker)delegate
            {
                if (!this.IsDisposed && !this.Disposing && lblStatusConexao != null)
                {
                    MessageBox.Show(this, errorMessage, "Erro na Comunicação Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatusConexao.Text = "Erro na conexão.";
                    lblStatusConexao.ForeColor = Color.Red;
                }
            });
        }

        private void btnConectarDesconectar_Click(object sender, EventArgs e)

        {
            if (_serialManager.IsConnected)
            {
                _serialManager.Disconnect();

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

                if (btnTestarConexao is cuiButton cuiBtn) cuiBtn.Enabled = false;

                lblStatusConexao.Text = $"Tentando conectar a {port}@{baudRate}...";
                lblStatusConexao.ForeColor = Color.Orange;

                Application.DoEvents(); // Use com cautela

                btnTestarConexao.Enabled = false;
                Application.DoEvents();


                Task.Run(() =>
                {

                    // Não há mais validação de formato de dados. Apenas conectar.
                    // lblStatusConexao será atualizado por ConnectionStatusChanged
                    // para "Conectado: PORTA @ BAUDRATE"

                    // Salva as configurações que resultaram em uma conexão bem-sucedida
                    SalvarConfiguracoesPersistentes(port, baudRate);
                    // Atualiza as configurações na memória da aplicação (se ConnectionSettingsApplication.UpdateConnection não fizer isso internamente)
                    ConnectionSettingsApplication.UpdateCurrentSettings(port, baudRate); // Presumindo que este método existe.

                    MessageBox.Show(this, $"Conectado a {port}@{baudRate}.\nAs configurações foram salvas.\nOs dados recebidos serão exibidos.", "Conexão Bem-sucedida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                });
               

                if (btnTestarConexao is cuiButton cuiBtn2) cuiBtn2.Enabled = true;
                UpdateConnectionStatusUI(_serialManager.IsConnected); // Garante que a UI reflita o estado final
            }
        }

       
        private void btnSalvarConfiguracoesClick(object sender, EventArgs e)
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

            string mensagemConfirmacao = $"Deseja salvar Porta: {port} e Baud Rate: {baudRate} como padrão?";
            bool aplicarNovasConfig = false; // Flag para saber se precisa desconectar e reconectar

            if (_serialManager.IsConnected)
            {
                if (ConnectionSettingsApplication.CurrentPortName != port || ConnectionSettingsApplication.CurrentBaudRate != baudRate)
                {
                    // Se conectado E as configurações selecionadas são DIFERENTES das ativas
                    mensagemConfirmacao += "\nA conexão atual será desfeita para aplicar as novas configurações. Você precisará conectar novamente.";
                    aplicarNovasConfig = true;
                }
                // Se conectado E as configurações selecionadas são IGUAIS às ativas, apenas salva sem mensagem adicional.
            }
            // Se não estiver conectado, apenas salva.

            var result = MessageBox.Show(this, mensagemConfirmacao, "Salvar Configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {

                SalvarConfiguracoesPersistentes(port, baudRate); // Salva nas Properties.Settings
                ConnectionSettingsApplication.UpdateCurrentSettings(port, baudRate); // Atualiza as configurações na memória da aplicação

                bool success = ConnectionSettingsApplication.UpdateConnection(port, baudRate);


                if (aplicarNovasConfig && _serialManager.IsConnected)
                {
                    _serialManager.Disconnect(); // Desconecta para forçar o uso das novas configs na próxima conexão
                    MessageBox.Show(this, "Configurações salvas e aplicadas. A conexão foi desfeita. Por favor, conecte novamente.", "Configurações Salvas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "Configurações salvas com sucesso!", "Configurações Salvas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                ConfigureUI(); // Atualiza os ComboBoxes para refletir as configs salvas


                UpdateConnectionStatusUI(_serialManager.IsConnected);

            }
        }


        private void SalvarConfiguracoesPersistentes(string port, int baudRate)
        {
            try
            {
                // Supondo que você tenha essas configurações no seu Properties.Settings
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
    Debug.WriteLine($"[CONEXAO.conexao_FormClosing] Formulário fechando. FechamentoForcado: {_fechamentoForcado}, Razão: {e.CloseReason}");

    // Limpeza de recursos sempre que o formulário estiver fechando
    _textBoxUpdateTimer?.Stop();
    _textBoxUpdateTimer?.Dispose();

    InitializeSerialManagerHandlers(false); // Crucial para desinscrever eventos

    // Se o fechamento é devido ao Application.Exit() já ter sido chamado,
    // ou se o fechamento foi intencionalmente forçado (pelo botão Voltar),
    // não fazemos mais nada aqui, apenas permitimos o fechamento.
    if (e.CloseReason == CloseReason.ApplicationExitCall)
    {
        Debug.WriteLine("[CONEXAO.conexao_FormClosing] Fechamento devido a ApplicationExitCall, permitindo fechar sem nova pergunta.");
        return; // Permite o fechamento sem mais interações
    }

    if (_fechamentoForcado)
    {
        Debug.WriteLine("[CONEXAO.conexao_FormClosing] Fechamento forçado (ex: botão Voltar), permitindo fechar sem pergunta.");
        return; // Permite o fechamento
    }

    // Se chegou aqui, _fechamentoForcado é false e e.CloseReason NÃO é ApplicationExitCall.
    // Geralmente, será CloseReason.UserClosing (usuário clicou no 'X' deste formulário).
    // Também pode ser TaskManagerClosing, WindowsShutDown, etc., onde a pergunta de salvar/sair da aplicação é válida.

    // Faz a pergunta apenas se não for um fechamento forçado e não for uma consequência de Application.Exit()
    DialogResult dr = MessageBox.Show("Deseja fechar toda a aplicação?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
    Debug.WriteLine($"[CONEXAO.conexao_FormClosing] Diálogo de Sair (UserClosing ou similar): Resultado={dr}");

    if (dr == DialogResult.Yes)
    {
        Debug.WriteLine("[CONEXAO.conexao_FormClosing] Usuário escolheu sair da aplicação.");
        ConnectionSettingsApplication.CloseAllConnections(); // Fecha conexões gerenciadas
        Application.Exit(); // Inicia o processo de fechamento da aplicação.
                            // Este e outros formulários receberão FormClosing com CloseReason.ApplicationExitCall.
    }
    else
    {
        Debug.WriteLine("[CONEXAO.conexao_FormClosing] Usuário cancelou o fechamento da aplicação.");
        e.Cancel = true; // Cancela o fechamento DESTE formulário

        InitializeSerialManagerHandlers(false);

        if (!_fechamentoForcado)
        {
            DialogResult result = MessageBox.Show("Deseja fechar toda a aplicação?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                ConnectionSettingsApplication.CloseAllConnections();
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }

        }
    }
}

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            _fechamentoForcado = true;
            if (_ownerForm != null && !_ownerForm.IsDisposed)
            {
                _ownerForm.Show();
            }
            else
            {
                // new Menuapp().Show(); // Considerar alternativa
            }
            this.Close();
        }

        private void conexao_Load(object sender, EventArgs e)
{

    if (lblInfoAdicional != null)
    {
        if (_serialManager.IsConnected)
        {
            lblInfoAdicional.Text = $"Conexão ativa: {ConnectionSettingsApplication.CurrentPortName}";
        }
        else if (!string.IsNullOrEmpty(ConnectionSettingsApplication.CurrentPortName))
        {
            lblInfoAdicional.Text = $"Última config salva: {ConnectionSettingsApplication.CurrentPortName}@{ConnectionSettingsApplication.CurrentBaudRate}.";
        }
        else
        {
            lblInfoAdicional.Text = "Nenhuma configuração salva. Selecione as opções e conecte.";
        }

        if (_serialManager.IsConnected)
        {
            // UI já atualizada pelo UpdateConnectionStatusUI
        }
        else if (!string.IsNullOrEmpty(ConnectionSettingsApplication.CurrentPortName))
        {
            lblInfoAdicional.Text = $"Última config: {ConnectionSettingsApplication.CurrentPortName}@{ConnectionSettingsApplication.CurrentBaudRate}.";
        }
        else
        {
            lblInfoAdicional.Text = "Nenhuma config salva.\n Selecione e conecte.";

        }
    }
}

        private void btnAtualizarPortas_Click(object sender, EventArgs e)
        {
            if (lblStatusConexao != null) lblStatusConexao.Text = "Atualizando portas...";
            Application.DoEvents();
            RefreshPortsList();

            // O status da conexão não muda, então UpdateConnectionStatusUI irá restaurar o texto correto do lblStatusConexao
            UpdateConnectionStatusUI(_serialManager.IsConnected);

            if (comboBoxPortaCOM.Enabled)
            {
                lblStatusConexao.Text = "Lista de portas COM atualizada.";
            }

        }
    }
}