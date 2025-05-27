using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports; // Necessário para SerialPort, mas SerialManager pode encapsular isso
using System.Linq;
using System.Text;
// using System.Threading; // CancellationTokenSource não é mais necessário
// using System.Threading.Tasks; // Task.Delay não é mais necessário nesta capacidade
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete; // Supondo que esta e a próxima using são necessárias
using minas.teste.prototype.Service;
using System.Diagnostics;
// using System.Text.RegularExpressions; // Não é mais necessário para ExpectedDataPattern
using CuoreUI.Controls; // Para cuiButton, se estiver usando

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

        // Campos de validação removidos: _validationCts, _dadosValidosRecebidos, ExpectedDataPattern

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
            // btnSalvarAplicarConfiguracoes agora é habilitado/desabilitado com base na conexão também
            btnSalvarAplicarConfiguracoes.Enabled = !(_serialManager.IsConnected && !enabled); // Habilitado se desconectado ou se 'enabled' for true
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
        private void SerialManager_DataReceived(object sender, string data)
        {
            

            string cleanData = FilterNonPrintableChars(data);

            if (!string.IsNullOrEmpty(cleanData))
            {
                lock (_bufferLock)
                {
                    _dataBuffer.Append(cleanData);
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
                //btnTestarConexao.Text = isConnected ? "Desconectar" : "Conectar"; // Para System.Windows.Forms.Button
                if (btnTestarConexao is cuiButton cuiBtn) // Para CuoreUI.Controls.cuiButton
                {
                    cuiBtn.Content = isConnected ? "Desconectar" : "Conectar";
                    cuiBtn.NormalBackground = isConnected ? Color.Crimson : Color.DarkGreen;
                    cuiBtn.ForeColor = Color.White; // Opcional, se quiser mudar a cor do texto
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

        private async void btnConectarDesconectar_Click(object sender, EventArgs e) // btnTestarConexao.Click
        {
            if (_serialManager.IsConnected)
            {
                _serialManager.Disconnect();
                // A UI será atualizada pelo evento ConnectionStatusChanged
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

                bool success = ConnectionSettingsApplication.UpdateConnection(port, baudRate);

                if (success)
                {
                    // Não há mais validação de formato de dados. Apenas conectar.
                    // lblStatusConexao será atualizado por ConnectionStatusChanged
                    // para "Conectado: PORTA @ BAUDRATE"

                    // Salva as configurações que resultaram em uma conexão bem-sucedida
                    SalvarConfiguracoesPersistentes(port, baudRate);
                    // Atualiza as configurações na memória da aplicação (se ConnectionSettingsApplication.UpdateConnection não fizer isso internamente)
                    ConnectionSettingsApplication.UpdateCurrentSettings(port, baudRate); // Presumindo que este método existe.

                    MessageBox.Show(this, $"Conectado a {port}@{baudRate}.\nAs configurações foram salvas.\nOs dados recebidos serão exibidos.", "Conexão Bem-sucedida", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // lblStatusConexao será atualizado por ConnectionStatusChanged para "Desconectado" ou SerialManager_ErrorOccurred mostrará erro
                    // MessageBox.Show(this, $"Falha ao conectar a {port}.", "Falha na Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error); // Opcional, pois SerialManager_ErrorOccurred deve tratar
                }

                if (btnTestarConexao is cuiButton cuiBtn2) cuiBtn2.Enabled = true;
                UpdateConnectionStatusUI(_serialManager.IsConnected); // Garante que a UI reflita o estado final
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
            }
        }

        private void btnAtualizarPortas_Click(object sender, EventArgs e)
        {
            if (lblStatusConexao != null) lblStatusConexao.Text = "Atualizando portas...";
            Application.DoEvents();
            RefreshPortsList();
            // O status da conexão não muda, então UpdateConnectionStatusUI irá restaurar o texto correto do lblStatusConexao
            UpdateConnectionStatusUI(_serialManager.IsConnected);
        }
    }
}