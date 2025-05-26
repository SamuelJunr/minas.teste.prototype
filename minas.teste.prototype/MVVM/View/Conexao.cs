using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading; // Para CancellationTokenSource
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete; // Supondo que esta e a próxima using são necessárias
using minas.teste.prototype.Service;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CuoreUI.Controls; // Para Regex na validação

namespace minas.teste.prototype.MVVM.View
{
    public partial class conexao : Form
    {
        private readonly SerialManager _serialManager;
        private readonly StringBuilder _dataBuffer = new StringBuilder();
        private readonly System.Windows.Forms.Timer _textBoxUpdateTimer;
        private readonly object _bufferLock = new object();
        private Form _ownerForm;
        private bool _fechamentoForcado = false;

        private CancellationTokenSource _validationCts;
        private bool _dadosValidosRecebidos = false;

        // Ajuste este Regex conforme o formato exato esperado da Tela_Bombas
        private static readonly Regex ExpectedDataPattern = new Regex(@"^HA1:.*?\|VZ4:.*$", RegexOptions.Compiled);

        public conexao(Form owner = null)
        {
            InitializeComponent();
            _ownerForm = owner ?? new Menuapp(); // Certifique-se que Menuapp é um Form válido

            _serialManager = ConnectionSettingsApplication.PersistentSerialManager;

            InitializeSerialManagerHandlers(true);
            ConfigureUI();

            _textBoxUpdateTimer = new System.Windows.Forms.Timer { Interval = 200 }; // Atualiza o TextBox
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
            btnSalvarAplicarConfiguracoes.Enabled = enabled;
            // O botão Conectar/Desconectar (btnTestarConexao) é gerenciado por UpdateConnectionStatusUI
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
                // Certifique-se que textBox1 existe no seu formulário e está acessível.
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
                    catch (Exception ex) // Captura outras exceções para depuração
                    {
                        Debug.WriteLine($"Erro ao atualizar textBox1: {ex.Message}");
                    }
                }
            }
        }
        private void SerialManager_DataReceived(object sender, string data)
        {
            string cleanData = FilterNonPrintableChars(data);

            if (!string.IsNullOrEmpty(cleanData))
            {
                lock (_bufferLock)
                {
                    _dataBuffer.Append(cleanData);
                }
                // Debug.WriteLine($"Dados bufferizados: {_dataBuffer.Length} chars"); // Para depuração

                if (_validationCts != null && !_validationCts.IsCancellationRequested)
                {
                    // A linha this.Invoke((Action)(() => { /* UI update logic */ })); foi removida pois estava vazia.
                    // Se havia uma intenção específica para ela, precisaria ser reimplementada.

                    string currentBufferSnapshot;
                    lock (_bufferLock)
                    {
                        currentBufferSnapshot = _dataBuffer.ToString();
                    }

                    string[] lines = currentBufferSnapshot.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
                        if (ExpectedDataPattern.IsMatch(line.Trim()))
                        {
                            _dadosValidosRecebidos = true;
                            if (!_validationCts.IsCancellationRequested) // Evita erro se já cancelado
                            {
                                _validationCts.Cancel();
                            }
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
                btnTestarConexao.Text = isConnected ? "Desconectar" : "Conectar";

                if (btnTestarConexao is cuiButton cuiButtonTestarConexao)
                {
                    cuiButtonTestarConexao.NormalBackground = isConnected ? Color.Crimson : Color.DarkGreen;
                    cuiButtonTestarConexao.NormalForeColor = Color.White;
                }

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
                btnSalvarAplicarConfiguracoes.Enabled = enableControlsWhenDisconnected;
            }

            if (cuiButton2 != null && !cuiButton2.IsDisposed) // Botão de desconectar dedicado
            {
                cuiButton2.Enabled = isConnected;
            }
        }

        private void SerialManager_ErrorOccurred(object sender, string errorMessage)
        {
            if (this.IsDisposed || this.Disposing) return;
            BeginInvoke((MethodInvoker)delegate
            {
                if (!this.IsDisposed && !this.Disposing && lblStatusConexao != null) // Checar lblStatusConexao
                {
                    MessageBox.Show(this, errorMessage, "Erro na Comunicação Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatusConexao.Text = "Erro na conexão.";
                    lblStatusConexao.ForeColor = Color.Red;
                }
            });
        }

        private async void btnConectarDesconectar_Click(object sender, EventArgs e) // btnTestarConexao.Click
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

                lblStatusConexao.Text = $"Tentando conectar a {port}@{baudRate}...";
                lblStatusConexao.ForeColor = Color.Orange;
                btnTestarConexao.Enabled = false;
                Application.DoEvents(); // Use com cautela

                bool success = ConnectionSettingsApplication.UpdateConnection(port, baudRate);

                if (success)
                {
                    lblStatusConexao.Text = "Conectado. Validando dados...";
                    lblStatusConexao.ForeColor = Color.BlueViolet;
                    Application.DoEvents(); // Use com cautela

                    _dadosValidosRecebidos = false;
                    _validationCts?.Cancel(); // Cancela token anterior
                    _validationCts?.Dispose(); // Libera recursos do token anterior
                    _validationCts = new CancellationTokenSource();

                    try
                    {
                        // Espera por dados válidos por um período (ex: 3 segundos)
                        // Se _validationCts.Cancel() for chamado por SerialManager_DataReceived
                        // (porque dados válidos foram encontrados), TaskCanceledException será lançada.
                        await Task.Delay(TimeSpan.FromSeconds(3), _validationCts.Token);
                    }
                    catch (TaskCanceledException)
                    {
                        // Normal se a validação for bem-sucedida e o token cancelado
                        Debug.WriteLine("Validação interrompida por cancelamento (dados válidos recebidos ou outra razão).");
                    }
                    // O finally não é estritamente necessário aqui se _validationCts é gerenciado após o bloco try-catch.

                    if (_dadosValidosRecebidos)
                    {
                        lblStatusConexao.Text = $"Conectado e dados válidos! {port}@{baudRate}";
                        lblStatusConexao.ForeColor = Color.Green;
                        SalvarConfiguracoesPersistentes(port, baudRate); // Salva após validação bem-sucedida
                        MessageBox.Show(this, $"Conectado a {port}@{baudRate}. Dados no formato esperado recebidos e configurações salvas.", "Conexão Bem-sucedida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        // Se chegou aqui, ou o Task.Delay completou sem _dadosValidosRecebidos = true,
                        // ou foi cancelado por outra razão sem que _dadosValidosRecebidos fosse true.
                        lblStatusConexao.Text = $"Porta {port} aberta, mas dados não estão no formato esperado. Desconectando.";
                        lblStatusConexao.ForeColor = Color.DarkOrange;
                        MessageBox.Show(this, $"A porta {port} foi aberta, mas os dados recebidos não correspondem ao padrão esperado. A conexão será desfeita.", "Dados Inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        _serialManager.Disconnect();
                    }
                }
                else
                {
                    lblStatusConexao.Text = $"Falha ao conectar a {port}.";
                    lblStatusConexao.ForeColor = Color.Red;
                    // SerialManager_ErrorOccurred ou UpdateConnectionStatusUI já deve ter lidado com a UI
                }

                _validationCts?.Dispose(); // Garante a liberação
                _validationCts = null;
                btnTestarConexao.Enabled = true;
                UpdateConnectionStatusUI(_serialManager.IsConnected); // Atualiza estado final da UI
            }
        }

        private void btnDesconectarSeparado_Click(object sender, EventArgs e) // cuiButton2.Click
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

        private void btnSalvarConfiguracoesClick(object sender, EventArgs e) // btnSalvarAplicarConfiguracoes.Click (era cuiButton3)
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
            bool aplicarNovasConfig = false;

            if (_serialManager.IsConnected)
            {
                if (ConnectionSettingsApplication.CurrentPortName != port || ConnectionSettingsApplication.CurrentBaudRate != baudRate)
                {
                    mensagemConfirmacao += "\nIsso também tentará aplicar e reconectar com estas novas configurações.";
                    aplicarNovasConfig = true;
                }
            }
            else // Não está conectado
            {
                // Apenas salva, não tenta conectar daqui. O usuário conecta pelo botão "Conectar".
            }


            var result = MessageBox.Show(this, mensagemConfirmacao, "Salvar Configurações", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Salva permanentemente
                SalvarConfiguracoesPersistentes(port, baudRate);

                // Atualiza as configurações na sessão atual via ConnectionSettingsApplication
                // Se estiver conectado e as configs mudaram, UpdateConnection vai tentar reconectar.
                // Se não estiver conectado, apenas atualiza as configs para a próxima tentativa de conexão.
                if (aplicarNovasConfig || !_serialManager.IsConnected)
                {
                    // Atualiza as configurações na memória para serem usadas na próxima conexão ou reconexão
                    ConnectionSettingsApplication.UpdateCurrentSettings(port, baudRate);

                    if (aplicarNovasConfig && _serialManager.IsConnected)
                    {
                        // Desconecta primeiro para que a nova configuração seja aplicada na reconexão
                        _serialManager.Disconnect();
                        // O usuário precisará clicar em "Conectar" novamente para usar as novas configs,
                        // ou você pode tentar reconectar automaticamente aqui (chamar btnConectarDesconectar_Click programaticamente, por exemplo)
                        // Para simplificar, vamos apenas informar.
                        MessageBox.Show(this, "Configurações salvas e atualizadas. Por favor, reconecte para usar as novas configurações.", "Configurações Salvas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show(this, "Configurações salvas com sucesso!", "Configurações Salvas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show(this, "Configurações salvas com sucesso! (Nenhuma alteração aplicada à conexão ativa).", "Configurações Salvas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Atualiza a UI para refletir as configurações selecionadas (que agora são as salvas)
                // e o estado da conexão.
                ConfigureUI(); // Recarrega comboBoxes com valores salvos e atualiza status.
            }
        }

        // Método auxiliar em ConnectionSettingsApplication (exemplo)
        // public static void UpdateCurrentSettings(string portName, int baudRate)
        // {
        //     CurrentPortName = portName; // Supondo que estas propriedades têm setters ou um método para atualizá-las
        //     CurrentBaudRate = baudRate;
        // }


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
            InitializeSerialManagerHandlers(false);

            _validationCts?.Cancel();
            _validationCts?.Dispose();

            if (!_fechamentoForcado)
            {
                DialogResult dr = MessageBox.Show("Deseja fechar toda a aplicação?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    ConnectionSettingsApplication.CloseAllConnections();
                    Application.Exit();
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void btnVoltar_Click(object sender, EventArgs e) // Bntvoltar.Click
        {
            _fechamentoForcado = true;
            if (_ownerForm != null && !_ownerForm.IsDisposed)
            {
                _ownerForm.Show();
            }
            else
            {
                // Se _ownerForm for nulo ou descartado, decidir o que fazer (ex: novo Menuapp ou fechar app)
                // new Menuapp().Show(); // Opção
            }
            this.Close();
        }

        private void conexao_Load(object sender, EventArgs e)
        {
            // ConfigureUI é chamado no construtor.
            // Adicionar um Label lblInfoAdicional no designer se desejar usar estas mensagens.
            if (lblInfoAdicional != null) // Checar se o label existe
            {
                if (_serialManager.IsConnected)
                {
                    // lblStatusConexao já informa o status.
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
            }
        }

        private void btnAtualizarPortas_Click(object sender, EventArgs e)
        {
            if (lblStatusConexao != null) lblStatusConexao.Text = "Atualizando portas...";
            Application.DoEvents(); // Use com cautela
            RefreshPortsList();
            if (lblStatusConexao != null && comboBoxPortaCOM.Enabled)
            {
                lblStatusConexao.Text = "Lista de portas COM atualizada.";
            }
            else if (lblStatusConexao != null)
            {
                // Mantém o status anterior se as portas não estiverem habilitadas (ou seja, conectado)
                UpdateConnectionStatusUI(_serialManager.IsConnected);
            }
        }

        // Certifique-se de que os componentes como btnTestarConexao, cuiButton2, 
        // btnSalvarAplicarConfiguracoes, textBox1, lblStatusConexao, lblInfoAdicional
        // comboBoxPortaCOM, comboBoxBaudRate existem no seu formulário.
        // Se btnTestarConexao e cuiButton2 são System.Windows.Forms.Button,
        // propriedades como .Content e .NormalBackground precisam ser ajustadas para .Text e .BackColor.
        // O código acima tentou fazer alguns ajustes genéricos para System.Windows.Forms.Button.
    }
}