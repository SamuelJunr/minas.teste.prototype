using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.MVVM.ViewModel;
using minas.teste.prototype.Service;
using TextBox = System.Windows.Forms.TextBox; // Especifica o TextBox do Windows Forms


namespace minas.teste.prototype.MVVM.View
{
    public partial class Tela_Bombas : Form
    {

        private struct ParameterRowInfo
        {
            public string DisplayName { get; }
            public string SourceTextBoxName { get; }
            public string FormattingType { get; }

            public ParameterRowInfo(string displayName, string sourceTextBoxName, string formattingType)
            {
                DisplayName = displayName;
                SourceTextBoxName = sourceTextBoxName;
                FormattingType = formattingType;
            }
        }

        private List<ParameterRowInfo> _staticDataGridViewParameters;

        private Tela_BombasVM _viewModel;
        private List<EtapaData> _dadosColetados = new List<EtapaData>();
        public Dictionary<string, System.Windows.Forms.TextBox> sensorMap;
        public Dictionary<string, string> sensorMapmedida;
        private List<string> dadosSensoresSelecionados = new List<string>();
        public List<SensorData> dadosSensores;
        private Timer monitoramentoTimer;
        private Timer timerCronometro;
        private TextBoxTrackBarSynchronizer _synchronizerNivel;

        private SerialManager _serialManager;
        private Timer _updateUiTimer;
        private StringBuilder serialDataBuffer = new StringBuilder();
        private readonly object serialBufferLock = new object();
        private readonly object readingsLock = new object();

        private Dictionary<string, double> _currentNumericSensorReadings = new Dictionary<string, double>();
        private Dictionary<string, string> _arduinoKeyToSensorIdMap;

        private const double BAR_TO_PSI_CONVERSION = 14.5;
        private const double LPM_TO_GPM_USER_CONVERSION = 3.98;
        private const double BAR_CONVERSION_PILOT = 1.705;

        // <<< NOVOS MARCADORES DE PACOTE >>>
        private const string PACKET_START_KEY = "HA1:";
        private const string PACKET_END_KEY = "VZ4:";

        private int _timeCounterSecondsRampa = 0;
        private bool _fechamentoControladoPeloUsuario = false;
        private bool _isMonitoring = false;
        public string Inicioteste;
        public string Fimteste;
        private int etapaAtual = 1;
        public string TbNomeCliente { get; set; }
        public string TbNomeBomba { get; set; }
        public string TbOrdemServico { get; set; }
        private int valorDefinidoCronometro;
        private bool cronometroIniciado = false;
        private bool tempoCronometroDefinidoManualmente = false;

        private List<ReadingData> allReadingsData;
        private ConfigurationResult currentConfiguration;
        private Button currentlyActiveTestButton = null;
        private Color defaultButtonColor;

        private Dictionary<string, (TextBox valueTextBox, Label unitLabel)> sensorControlsMap;
        private ToolTip sensorToolTip;


        public Tela_Bombas()
        {
            InitializeComponent();
            InitializeStaticDataGridViewParameters();
            SetupStaticDataGridView();

            _viewModel = new Tela_BombasVM();

            Control textBox2Control = this.Controls.Find("textBox2", true).FirstOrDefault();
            Control trackBar1Control = this.Controls.Find("trackBar1", true).FirstOrDefault();

            if (textBox2Control is System.Windows.Forms.TextBox tb2 && trackBar1Control is TrackBar tb1)
            {
                _synchronizerNivel = new TextBoxTrackBarSynchronizer(tb2, tb1, 1, 7);
            }
            else
            {
                Debug.WriteLine("Aviso: textBox2 ou trackBar1 não encontrados no formulário para TextBoxTrackBarSynchronizer.");
            }

            dadosSensores = new List<SensorData>();

            timerCronometro = new Timer();
            timerCronometro.Interval = 1000;
            timerCronometro.Tick += TimerCronometro_Tick;

            _serialManager = ConnectionSettingsApplication.PersistentSerialManager;

            _updateUiTimer = new Timer();
            _updateUiTimer.Interval = 150;
            _updateUiTimer.Tick += UpdateUiTimer_Tick;

            InitializeAllCharts();
            InitializeArduinoToSensorIdMapping();

            SetButtonState(btniniciarteste, true);
            SetButtonState(btngravar, false);
            SetButtonState(bntFinalizar, false);
            SetButtonState(btnreset, false);
            SetButtonState(btnrelatoriobomba, false);
            InitializeSensorConfigurationSystem();
        }

        private void InitializeArduinoToSensorIdMapping()
        {
            _arduinoKeyToSensorIdMap = new Dictionary<string, string>
            {
                { "PR1", "sensor_MA1_press" }, { "PR2", "sensor_MA2_press" }, { "PR3", "sensor_MB1_press" }, { "PR4", "sensor_MB2_press" },
                { "PL1", "sensor_P1_piloto" }, { "PL2", "sensor_P2_piloto" }, { "PL3", "sensor_P3_piloto" }, { "PL4", "sensor_P4_piloto" },
                { "VZ1", "sensor_V1_vazao" }, { "VZ2", "sensor_V2_vazao" }, { "VZ3", "sensor_V3_vazao" }, { "VZ4", "sensor_V4_vazao" },
                { "DR1", "sensor_DR1_dreno" }, { "DR2", "sensor_DR2_dreno" },
                { "HA1", "sensor_HA1_hidro" }, { "HA2", "sensor_HA2_hidro" }, { "HB1", "sensor_HB1_hidro" }, { "HB2", "sensor_HB2_hidro" },
                { "MA1", "sensor_MA1_motor" }, { "MA2", "sensor_MA2_motor" }, { "MB1", "sensor_MB1_motor" }, { "MB2", "sensor_MB2_motor" },
                { "TEM", "sensor_CELSUS_temp" }, { "ROT", "sensor_RPM_rot" }
            };
        }

        private void InitializeSensorConfigurationSystem()
        {
            sensorToolTip = new ToolTip();
            InitializeAllReadingsDataFromSpec();
            InitializeSensorControlsMap();
            currentConfiguration = new ConfigurationResult();

            if (btnConfigCircuitoAberto != null)
            {
                defaultButtonColor = btnConfigCircuitoAberto.BackColor;
            }
            else
            {
                defaultButtonColor = SystemColors.Control;
            }

            if (btnConfigCircuitoAberto != null)
                btnConfigCircuitoAberto.Click += (sender, e) => HandleConfigButtonClick(sender as Button, "Bombas Axiais de Circuito Aberto");
            if (btnConfigCircuitoFechado != null)
                btnConfigCircuitoFechado.Click += (sender, e) => HandleConfigButtonClick(sender as Button, "Bombas e Motores Axiais de Circuito Fechado");
            if (btnConfigEngrenagem != null)
                btnConfigEngrenagem.Click += (sender, e) => HandleConfigButtonClick(sender as Button, "Bombas de Engrenagem, Palheta e Cartucho");

            SetInitialButtonStates();
            UpdateTelaBombasDisplay();
        }

        private void InitializeAllReadingsDataFromSpec()
        {
            allReadingsData = new List<ReadingData>
            {
                new ReadingData("sensor_MA1_press", "P1", 0, "pressure", "bar", "sensor_P1", "LbPressao1", "P1"),
                new ReadingData("sensor_MA2_press", "P2", 0, "pressure", "bar", "sensor_P2", "LbPressao2", "P2"),
                new ReadingData("sensor_MB1_press", "P3", 0, "pressure", "bar", "sensor_P3", "LbPressao3", "P3"),
                new ReadingData("sensor_MB2_press", "P4", 0, "pressure", "bar", "sensor_P4", "LbPressao4", "P4"),
                new ReadingData("sensor_P1_piloto", "Pressão Piloto 1", 0, "pressure", "bar", "sensor_PR1", "LbPilotagem1", "Pressão Piloto 1"),
                new ReadingData("sensor_P2_piloto", "Pressão Piloto 2", 0, "pressure", "bar", "sensor_PR2", "LbPilotagem2", "Pressão Piloto 2"),
                new ReadingData("sensor_P3_piloto", "Pressão Piloto 3", 0, "pressure", "bar", "sensor_PR3", "LbPilotagem3", "Pressão Piloto 3"),
                new ReadingData("sensor_P4_piloto", "Pressão Piloto 4", 0, "pressure", "bar", "sensor_PR4", "LbPilotagem4", "Pressão Piloto 4"),
                new ReadingData("sensor_V1_vazao", "Vazão 1", 0, "flow", "lpm", "sensor_V1", "LbVazao1", "Vazão 1"),
                new ReadingData("sensor_V2_vazao", "Vazão 2", 0, "flow", "lpm", "sensor_V2", "LbVazao2", "Vazão 2"),
                new ReadingData("sensor_V3_vazao", "Vazão 3", 0, "flow", "lpm", "sensor_V3", "LbVazao3", "Vazão 3"),
                new ReadingData("sensor_V4_vazao", "Vazão 4", 0, "flow", "lpm", "sensor_V4", "LbVazao4", "Vazão 4"),
                new ReadingData("sensor_DR1_dreno", "Dreno 1", 0, "flow", "lpm", "sensor_DR1", "LbDreno1", "Dreno 1"),
                new ReadingData("sensor_DR2_dreno", "Dreno 2", 0, "flow", "lpm", "sensor_DR2", "LbDreno2", "Dreno 2"),
                new ReadingData("sensor_HA1_hidro", "Hidro A1", 0, "pressure", "bar", "sensor_HA1", "LbHidroA1", "Hidro A1"),
                new ReadingData("sensor_HA2_hidro", "Hidro A2", 0, "pressure", "bar", "sensor_HA2", "LbHidroA2", "Hidro A2"),
                new ReadingData("sensor_HB1_hidro", "Hidro B1", 0, "pressure", "bar", "sensor_HB1", "LbHidroB1", "Hidro B1"),
                new ReadingData("sensor_HB2_hidro", "Hidro B2", 0, "pressure", "bar", "sensor_HB2", "LbHidroB2", "Hidro B2"),
                new ReadingData("sensor_MA1_motor", "Motor A1", 0, "pressure", "bar", "sensor_MA1", "LbMotorA1", "Motor A1"),
                new ReadingData("sensor_MA2_motor", "Motor A2", 0, "pressure", "bar", "sensor_MA2", "LbMotorA2", "Motor A2"),
                new ReadingData("sensor_MB1_motor", "Motor B1", 0, "pressure", "bar", "sensor_MB1", "LbMotorB1", "Motor B1"),
                new ReadingData("sensor_MB2_motor", "Motor B2", 0, "pressure", "bar", "sensor_MB2", "LbMotorB2", "Motor B2"),
                new ReadingData("sensor_CELSUS_temp", "Temperatura", 0, "fixed", "°C", "sensor_CELSUS", "LbTemp", "Temperatura"),
                new ReadingData("sensor_RPM_rot", "Rotação", 0, "fixed", "rpm", "sensor_RPM", "LbRota", "Rotação")
            };
        }

        private void InitializeSensorControlsMap()
        {
            sensorControlsMap = new Dictionary<string, (TextBox valueTextBox, Label unitLabel)>();
            foreach (ReadingData rd in allReadingsData)
            {
                Control[] tbControls = this.Controls.Find(rd.ValueTextBoxName, true);
                TextBox valueTb = tbControls.Length > 0 ? tbControls[0] as TextBox : null;
                Control[] ulControls = this.Controls.Find(rd.UnitLabelName, true);
                Label unitL = ulControls.Length > 0 ? ulControls[0] as Label : null;

                if (valueTb != null && unitL != null)
                {
                    sensorToolTip.SetToolTip(valueTb, rd.NameLabelText);
                    sensorControlsMap[rd.Id] = (valueTb, unitL);
                }
                else
                {
                    Debug.WriteLine($"Controles não encontrados para o ID da Leitura: {rd.Id}");
                }
            }
        }

        #region Serial Communication and Data Processing

        private void StartSerialConnection()
        {
            try
            {
                string portToConnect = ConnectionSettingsApplication.CurrentPortName;
                int baudRateToConnect = ConnectionSettingsApplication.CurrentBaudRate;

                if (string.IsNullOrEmpty(portToConnect) || baudRateToConnect <= 0)
                {
                    MessageBox.Show(this, "Configurações de porta serial não definidas.", "Configuração Serial Ausente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LogHistoricalEvent("ERRO - Configurações de porta serial não definidas.", Color.Red);
                    btnParar_Click(this, EventArgs.Empty);
                    return;
                }
                if (_serialManager == null)
                {
                    LogHistoricalEvent("Erro crítico: SerialManager não inicializado.", Color.Red);
                    btnParar_Click(this, EventArgs.Empty);
                    return;
                }

                _serialManager.DataReceived -= SerialManager_DataReceived;
                _serialManager.DataReceived += SerialManager_DataReceived;
                _serialManager.ErrorOccurred -= SerialManager_ErrorOccurred;
                _serialManager.ErrorOccurred += SerialManager_ErrorOccurred;

                if (_serialManager.IsConnected && (_serialManager.PortName != portToConnect || _serialManager.BaudRate != baudRateToConnect))
                {
                    _serialManager.Disconnect();
                }

                if (!_serialManager.IsConnected)
                {
                    _serialManager.Connect(portToConnect, baudRateToConnect);
                }

                if (_serialManager.IsConnected)
                {
                    LogHistoricalEvent($"Conectado à porta serial {portToConnect} ({baudRateToConnect} baud).", Color.Green);
                    StartUpdateUiTimer();
                }
                else
                {
                    string errorMessage = $"ERRO ao conectar à porta serial {portToConnect}.";
                    LogHistoricalEvent(errorMessage, Color.Red);
                    MessageBox.Show(this, errorMessage, "Erro de Conexão Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnParar_Click(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = $"Erro durante a tentativa de conexão serial: {ex.Message}";
                MessageBox.Show(this, errorMessage, "Erro de Conexão Serial Exceção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogHistoricalEvent(errorMessage, Color.Red);
                btnParar_Click(this, EventArgs.Empty);
            }
        }

        private void StopSerialConnection()
        {
            StopUpdateUiTimer();
            if (_serialManager != null)
            {
                _serialManager.DataReceived -= SerialManager_DataReceived;
                _serialManager.ErrorOccurred -= SerialManager_ErrorOccurred;
                if (_serialManager.IsConnected)
                {
                    try
                    {
                        _serialManager.Disconnect();
                        LogHistoricalEvent("Desconectado da porta serial.", Color.Orange);
                    }
                    catch (Exception ex)
                    {
                        LogHistoricalEvent($"ERRO ao desconectar da porta serial: {ex.Message}", Color.Red);
                    }
                }
            }
        }

        private void SerialManager_ErrorOccurred(object sender, string errorMessage)
        {
            if (this.IsDisposed || this.Disposing) return;
            BeginInvoke((MethodInvoker)delegate
            {
                if (!this.IsDisposed && !this.Disposing)
                {
                    LogHistoricalEvent($"ERRO SERIAL: {errorMessage}", Color.Red);
                }
            });
        }

        private void SerialManager_DataReceived(object sender, string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            lock (serialBufferLock)
            {
                serialDataBuffer.Append(data);
                ProcessSerialBuffer();
            }
        }

        private void ProcessSerialBuffer()
        {
            string bufferContent = serialDataBuffer.ToString();

            while (true)
            {
                // 1. Encontrar o marcador de início "HA1:"
                int startIndex = bufferContent.IndexOf(PACKET_START_KEY);
                if (startIndex == -1)
                {
                    // Nenhum início de pacote encontrado, não há o que fazer.
                    // Limpar o buffer se houver dados antes do marcador de início para evitar acúmulo de lixo.
                    if (serialDataBuffer.Length > 0)
                    {
                        serialDataBuffer.Clear();
                    }
                    return;
                }

                // 2. Descartar dados corrompidos antes do marcador de início
                // Se o PACKET_START_KEY não estiver no início do buffer, remove os caracteres anteriores.
                if (startIndex > 0)
                {
                    serialDataBuffer.Remove(0, startIndex);
                    bufferContent = serialDataBuffer.ToString();
                    // O novo início agora é na posição 0, pois removemos o lixo.
                }

                // 3. Procurar a chave de fim "VZ4:" após o início
                int VZ4_KeyIndex = bufferContent.IndexOf(PACKET_END_KEY);
                if (VZ4_KeyIndex == -1)
                {
                    // Pacote incompleto, aguardar mais dados.
                    return;
                }

                // 4. Encontrar o final do pacote, que é a sequência \r\n após a chave VZ4 e seu dado.
                // O pacote válido deve conter HA1: ... VZ4:valor\r\n
                // Procuramos por \r\n a partir do início da chave VZ4.
                int crlfIndex = bufferContent.IndexOf("\r\n", VZ4_KeyIndex);

                if (crlfIndex == -1)
                {
                    // O marcador de fim de linha não foi encontrado, pacote incompleto.
                    return;
                }

                // Extrai o pacote completo, incluindo o HA1: e indo até ANTES do \r\n
                // O pacote para parsear não deve incluir o \r\n, mas ele é usado para delimitar o fim.
                string completePacket = bufferContent.Substring(0, crlfIndex);

                // Pacote válido encontrado!
                Debug.WriteLine($"[PROCESS] Pacote válido detectado, terminando em \\r\\n.");

                // Envia para análise
                ParseAndStoreSensorData(completePacket);

                // Remove o pacote processado do buffer, incluindo o \r\n
                // O +2 é para remover também os caracteres '\r' e '\n'.
                serialDataBuffer.Remove(0, crlfIndex + 2);
                bufferContent = serialDataBuffer.ToString(); // Atualiza a string local para a próxima iteração
                                                             // Continua o loop para o caso de haver múltiplos pacotes no buffer.
            }
        }

        private void ParseAndStoreSensorData(string packet)
        {
            string[] pairs = packet.Split('|');
            var parsedReadings = new Dictionary<string, double>();
            bool success = true;

            foreach (string pair in pairs)
            {
                if (string.IsNullOrWhiteSpace(pair)) continue;

                string[] keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    string valueStr = keyValue[1].Trim();

                    if (double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        parsedReadings[key] = value;
                    }
                    else
                    {
                        Debug.WriteLine($"[PARSE_ERROR] Valor numérico inválido para a chave '{key}': '{valueStr}'");
                        success = false;
                        break;
                    }
                }
                else
                {
                    Debug.WriteLine($"[PARSE_ERROR] Par chave-valor malformado no pacote: '{pair}'");
                    success = false;
                    break;
                }
            }

            if (success && parsedReadings.Any())
            {
                lock (readingsLock)
                {
                    foreach (var entry in parsedReadings)
                    {
                        _currentNumericSensorReadings[entry.Key] = entry.Value;
                    }
                }
            }
        }


        private void StartUpdateUiTimer()
        {
            if (_updateUiTimer != null && !_updateUiTimer.Enabled)
            {
                _updateUiTimer.Start();
            }
        }

        private void StopUpdateUiTimer()
        {
            _updateUiTimer?.Stop();
        }

        private void UpdateUiTimer_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
            if (_isMonitoring)
            {
                _timeCounterSecondsRampa++;
                AddDataPointsToCharts();
            }
        }

        private void UpdateDisplay()
        {
            if (this.IsDisposed || this.Disposing) return;

            if (this.InvokeRequired)
            {
                try { this.BeginInvoke((MethodInvoker)UpdateTextBoxes); }
                catch (Exception) { /* Ignora erros se o form for fechado */ }
            }
            else
            {
                UpdateTextBoxes();
            }
        }

        private void UpdateTextBoxes()
        {
            if (this.IsDisposed || this.Disposing) return;

            Dictionary<string, double> readingsSnapshot;
            lock (readingsLock)
            {
                readingsSnapshot = new Dictionary<string, double>(_currentNumericSensorReadings);
            }

            if (!readingsSnapshot.Any()) return;

            foreach (var rd in allReadingsData)
            {
                string arduinoKey = _arduinoKeyToSensorIdMap.FirstOrDefault(kvp => kvp.Value == rd.Id).Key;

                if (arduinoKey != null && readingsSnapshot.TryGetValue(arduinoKey, out double numericValue))
                {
                    if (sensorControlsMap.TryGetValue(rd.Id, out var controls))
                    {
                        string formattedValue;

                        if (rd.Type.Equals("rpm", StringComparison.OrdinalIgnoreCase) || rd.Type.Equals("temp", StringComparison.OrdinalIgnoreCase))
                        {
                            formattedValue = numericValue.ToString("F0", CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            formattedValue = numericValue.ToString("F2", CultureInfo.InvariantCulture);
                        }

                        UpdateTextBoxIfAvailable(controls.valueTextBox, formattedValue);
                    }
                }
                else
                {
                    if (sensorControlsMap.TryGetValue(rd.Id, out var controls))
                    {
                        UpdateTextBoxIfAvailable(controls.valueTextBox, "N/A");
                    }
                }
            }
        }

        private void UpdateTextBoxIfAvailable(TextBox textBox, string value)
        {
            if (textBox != null && !textBox.IsDisposed)
            {
                textBox.Text = value;
            }
        }

        #endregion

        // O restante do código (eventos de botões, cronômetro, gráficos, etc.) permanece o mesmo.
        // ...
        // [O código para HandleConfigButtonClick, OpenConfigModal, UpdateTelaBombasDisplay, etc. foi omitido por brevidade]
        // ...
        #region placeholder
        private void HandleConfigButtonClick(Button clickedButton, string testTypeDescription)
        {
            if (clickedButton == null) return;

            if (currentlyActiveTestButton == null)
            {
                OpenConfigModal(testTypeDescription, clickedButton);
            }
            else if (clickedButton == currentlyActiveTestButton)
            {
                DialogResult dr = MessageBox.Show("Deseja resetar a configuração atual e selecionar um novo tipo de ensaio?",
                                                 "Resetar Ensaio", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    ResetActiveTestConfiguration();
                }
            }
            else
            {
                MessageBox.Show($"O ensaio '{currentlyActiveTestButton.Tag?.ToString() ?? "ativo"}' já está configurado.\n" +
                                "Para mudar o tipo de ensaio, clique no botão do teste ativo (verde) e confirme o reset.",
                                "Ensaio Ativo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void OpenConfigModal(string testTypeDescription, Button activatingButton)
        {
            var configForModal = new ConfigurationResult
            {
                SelectedPressureUnit = currentConfiguration.SelectedPressureUnit,
                SelectedFlowUnit = currentConfiguration.SelectedFlowUnit,
                SelectedReadingIds = new List<string>(currentConfiguration.SelectedReadingIds)
            };

            using (ConfigFormBomb configDialog = new ConfigFormBomb(testTypeDescription, allReadingsData, configForModal))
            {
                if (configDialog.ShowDialog(this) == DialogResult.OK)
                {
                    currentConfiguration = configDialog.CurrentConfiguration;
                    UpdateTelaBombasDisplay();

                    if (currentlyActiveTestButton != null && currentlyActiveTestButton != activatingButton)
                    {
                        currentlyActiveTestButton.BackColor = defaultButtonColor;
                        currentlyActiveTestButton.ForeColor = SystemColors.ControlText;
                        currentlyActiveTestButton.Enabled = true;
                    }

                    currentlyActiveTestButton = activatingButton;
                    currentlyActiveTestButton.BackColor = Color.DarkGreen;
                    currentlyActiveTestButton.ForeColor = Color.White;
                    currentlyActiveTestButton.Tag = testTypeDescription;

                    var configButtons = new[] { btnConfigCircuitoAberto, btnConfigCircuitoFechado, btnConfigEngrenagem };
                    foreach (Button btn in configButtons)
                    {
                        if (btn != null && btn != currentlyActiveTestButton)
                        {
                            btn.Enabled = false;
                            btn.BackColor = defaultButtonColor;
                            btn.ForeColor = SystemColors.ControlText;
                        }
                    }
                    if (btniniciarteste != null) btniniciarteste.Enabled = true;
                }
            }
        }
        private void UpdateTelaBombasDisplay()
        {
            if (allReadingsData == null || sensorControlsMap == null) return;

            Dictionary<GroupBox, List<(TextBox valueTb, Label unitL)>> groupBoxReadingsMap =
                new Dictionary<GroupBox, List<(TextBox valueTb, Label unitL)>>();

            foreach (ReadingData rd in allReadingsData)
            {
                if (sensorControlsMap.TryGetValue(rd.Id, out var controls))
                {
                    bool isSelected = currentConfiguration.SelectedReadingIds.Contains(rd.Id);

                    controls.valueTextBox.Visible = isSelected;
                    controls.unitLabel.Visible = isSelected;

                    if (isSelected)
                    {
                        string unitText = "";
                        if (rd.Type == "pressure") unitText = currentConfiguration.SelectedPressureUnit.ToUpper();
                        else if (rd.Type == "flow") unitText = currentConfiguration.SelectedFlowUnit.ToUpper();
                        else unitText = rd.OriginalUnit.ToUpper();

                        controls.unitLabel.Text = unitText;
                        controls.unitLabel.Font = new Font(controls.unitLabel.Font, FontStyle.Bold);

                        GroupBox parentGroup = FindParentGroupBox(controls.valueTextBox);
                        if (parentGroup != null)
                        {
                            if (!groupBoxReadingsMap.ContainsKey(parentGroup))
                            {
                                groupBoxReadingsMap[parentGroup] = new List<(TextBox valueTb, Label unitL)>();
                            }
                            groupBoxReadingsMap[parentGroup].Add(controls);
                        }
                    }
                }
            }

            foreach (var kvp in groupBoxReadingsMap)
            {
                LayoutControlsInGroup(kvp.Key, kvp.Value);
            }

            var allGroupBoxesOnForm = this.Controls.OfType<Panel>()
                                     .Where(p => p.Name == "panel1")
                                     .SelectMany(p => p.Controls.OfType<GroupBox>())
                                     .Concat(this.Controls.OfType<GroupBox>());

            foreach (var groupControl in allGroupBoxesOnForm)
            {
                if (!groupBoxReadingsMap.ContainsKey(groupControl))
                {
                    foreach (var rd in allReadingsData)
                    {
                        if (sensorControlsMap.TryGetValue(rd.Id, out var sensorPair))
                        {
                            if (FindParentGroupBox(sensorPair.valueTextBox) == groupControl)
                            {
                                sensorPair.valueTextBox.Visible = false;
                                sensorPair.unitLabel.Visible = false;
                            }
                        }
                    }
                }
            }
        }
        private GroupBox FindParentGroupBox(Control control)
        {
            Control current = control;
            while (current != null)
            {
                if (current is GroupBox gb)
                {
                    return gb;
                }
                current = current.Parent;
            }
            return null;
        }
        private void LayoutControlsInGroup(GroupBox groupBox, List<(TextBox valueTb, Label unitL)> selectedItemsInGroup)
        {
            if (groupBox == null || selectedItemsInGroup == null) return;

            groupBox.SuspendLayout();

            foreach (var readingEntry in sensorControlsMap)
            {
                if (FindParentGroupBox(readingEntry.Value.valueTextBox) == groupBox)
                {
                    bool isCurrentlySelectedForThisGroup = selectedItemsInGroup.Any(item => item.valueTb == readingEntry.Value.valueTextBox);

                    readingEntry.Value.valueTextBox.Visible = isCurrentlySelectedForThisGroup;
                    readingEntry.Value.unitLabel.Visible = isCurrentlySelectedForThisGroup;
                }
            }

            var itemsToLayout = selectedItemsInGroup.Where(item => item.valueTb.Visible).ToList();

            if (!itemsToLayout.Any())
            {
                groupBox.ResumeLayout(true);
                return;
            }

            int controlRowHeight = 35;
            if (itemsToLayout.Any())
            {
                controlRowHeight = itemsToLayout.First().valueTb.Height;
            }

            Rectangle displayRect = groupBox.DisplayRectangle;

            int yPosForRow = displayRect.Y;
            if (displayRect.Height > controlRowHeight)
            {
                yPosForRow += (displayRect.Height - controlRowHeight) / 2;
            }

            yPosForRow = Math.Max(displayRect.Y, yPosForRow);

            const int internalHorizontalPadding = 5;
            int availableContentWidth = displayRect.Width - (2 * internalHorizontalPadding);
            const int spacingBetweenSensorItemsOnSameRow = 15;
            const int spacingWithinSensorItem = 3;

            int totalWidthOfItemsInThisRow = 0;
            for (int i = 0; i < itemsToLayout.Count; i++)
            {
                var itemTuple = itemsToLayout[i];
                itemTuple.unitL.AutoSize = true;
                Size unitLabelActualSize = TextRenderer.MeasureText(itemTuple.unitL.Text, itemTuple.unitL.Font);

                totalWidthOfItemsInThisRow += itemTuple.valueTb.Width + spacingWithinSensorItem + unitLabelActualSize.Width;
                if (i < itemsToLayout.Count - 1)
                {
                    totalWidthOfItemsInThisRow += spacingBetweenSensorItemsOnSameRow;
                }
            }

            int startX = displayRect.X + internalHorizontalPadding;
            if (availableContentWidth > totalWidthOfItemsInThisRow)
            {
                startX += (availableContentWidth - totalWidthOfItemsInThisRow) / 2;
            }

            int currentXInRow = startX;

            for (int i = 0; i < itemsToLayout.Count; i++)
            {
                var currentItemTuple = itemsToLayout[i];
                TextBox valueTextBox = currentItemTuple.valueTb;
                Label unitLabel = currentItemTuple.unitL;

                valueTextBox.Visible = true;
                unitLabel.Visible = true;
                unitLabel.AutoSize = true;

                int tbY = yPosForRow;
                int lblY = tbY + (valueTextBox.Height - unitLabel.PreferredHeight) / 2;

                tbY = Math.Max(displayRect.Y, tbY);
                lblY = Math.Max(displayRect.Y, lblY);

                valueTextBox.Location = new Point(currentXInRow, tbY);
                unitLabel.Location = new Point(valueTextBox.Right + spacingWithinSensorItem, lblY);

                currentXInRow = unitLabel.Right + spacingBetweenSensorItemsOnSameRow;
            }

            groupBox.ResumeLayout(true);
        }
        private void ResetActiveTestConfiguration()
        {
            currentConfiguration = new ConfigurationResult();
            UpdateTelaBombasDisplay();

            if (currentlyActiveTestButton != null)
            {
                currentlyActiveTestButton.BackColor = defaultButtonColor;
                currentlyActiveTestButton.ForeColor = SystemColors.ControlText;
                currentlyActiveTestButton.Tag = null;
            }
            currentlyActiveTestButton = null;

            var configButtons = new[] { btnConfigCircuitoAberto, btnConfigCircuitoFechado, btnConfigEngrenagem };
            foreach (Button btn in configButtons)
            {
                if (btn != null)
                {
                    btn.Enabled = true;
                    btn.BackColor = defaultButtonColor;
                    btn.ForeColor = SystemColors.ControlText;
                }
            }
            if (btniniciarteste != null) btniniciarteste.Enabled = false;
        }
        private void SetInitialButtonStates()
        {
            var configButtons = new[] { btnConfigCircuitoAberto, btnConfigCircuitoFechado, btnConfigEngrenagem };
            foreach (Button btn in configButtons)
            {
                if (btn != null)
                {
                    btn.Enabled = true;
                    btn.BackColor = defaultButtonColor;
                    btn.ForeColor = SystemColors.ControlText;
                }
            }
            if (btniniciarteste != null) btniniciarteste.Enabled = false;
        }
        public string GetConfiguredSensorValue(string sensorId)
        {
            if (currentConfiguration.SelectedReadingIds.Contains(sensorId) &&
                sensorControlsMap.TryGetValue(sensorId, out var controls))
            {
                return controls.valueTextBox.Text;
            }
            return "N/D";
        }
        public string GetConfiguredSensorUnit(string sensorId)
        {
            if (currentConfiguration.SelectedReadingIds.Contains(sensorId) &&
               sensorControlsMap.TryGetValue(sensorId, out var controls))
            {
                return controls.unitLabel.Text;
            }
            return "";
        }
        private void btnLimparConfigTelaBombas_Click(object sender, EventArgs e)
        {
            ResetActiveTestConfiguration();
        }
        private void InitializeStaticDataGridViewParameters()
        {
            _staticDataGridViewParameters = new List<ParameterRowInfo>
            {
                new ParameterRowInfo("P1", "sensor_P1", "float"),
                new ParameterRowInfo("P2", "sensor_P2", "float"),
                new ParameterRowInfo("P3", "sensor_P3", "float"),
                new ParameterRowInfo("P4", "sensor_P4", "float"),
                new ParameterRowInfo("Pressão Piloto 1", "sensor_PR1", "float"),
                new ParameterRowInfo("Pressão Piloto 2", "sensor_PR2", "float"),
                new ParameterRowInfo("Pressão Piloto 3", "sensor_PR3", "float"),
                new ParameterRowInfo("Pressão Piloto 4", "sensor_PR4", "float"),
                new ParameterRowInfo("Vazão 1", "sensor_V1", "float"),
                new ParameterRowInfo("Vazão 2", "sensor_V2", "float"),
                new ParameterRowInfo("Vazão 3", "sensor_V3", "float"),
                new ParameterRowInfo("Vazão 4", "sensor_V4", "float"),
                new ParameterRowInfo("Dreno 1", "sensor_DR1", "float"),
                new ParameterRowInfo("Dreno 2", "sensor_DR2", "float"),
                new ParameterRowInfo("Hidro A1", "sensor_HA1", "float"),
                new ParameterRowInfo("Hidro A2", "sensor_HA2", "float"),
                new ParameterRowInfo("Hidro B1", "sensor_HB1", "float"),
                new ParameterRowInfo("Hidro B2", "sensor_HB2", "float"),
                new ParameterRowInfo("Motor A1", "sensor_MA1", "float"),
                new ParameterRowInfo("Motor A2", "sensor_MA2", "float"),
                new ParameterRowInfo("Motor B1", "sensor_MB1", "float"),
                new ParameterRowInfo("Motor B2", "sensor_MB2", "float"),
                new ParameterRowInfo("Temperatura", "sensor_CELSUS", "temp"),
                new ParameterRowInfo("Rotação", "sensor_RPM", "rpm")
            };
        }
        private void SetupStaticDataGridView()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed) return;

            dataGridView1.SuspendLayout();

            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font ?? SystemFonts.DefaultFont, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            DataGridViewTextBoxColumn parametroCol = new DataGridViewTextBoxColumn
            {
                Name = "Parametro",
                HeaderText = "Parâmetro",
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5, 2, 5, 2)
                }
            };
            dataGridView1.Columns.Add(parametroCol);

            int numberOfEtapaColumns = 7;
            for (int i = 1; i <= numberOfEtapaColumns; i++)
            {
                dataGridView1.Columns.Add($"Etapa{i}", $"Etapa {i}");
            }

            if (_staticDataGridViewParameters != null)
            {
                foreach (var paramInfo in _staticDataGridViewParameters)
                {
                    dataGridView1.Rows.Add(paramInfo.DisplayName);
                }
            }

            if (dataGridView1.Rows.Count == 0 || dataGridView1.Columns.Count == 0)
            {
                dataGridView1.ResumeLayout();
                return;
            }

            int availableClientWidth = dataGridView1.ClientSize.Width;
            int availableClientHeight = dataGridView1.ClientSize.Height;

            int maxParametroTextWidth = 0;
            using (Graphics g = dataGridView1.CreateGraphics())
            {
                Font paramCellFont = parametroCol.DefaultCellStyle.Font ?? dataGridView1.DefaultCellStyle.Font ?? dataGridView1.Font ?? SystemFonts.DefaultFont;
                Font headerFont = dataGridView1.ColumnHeadersDefaultCellStyle.Font ?? dataGridView1.Font ?? SystemFonts.DefaultFont;

                int headerTextWidth = TextRenderer.MeasureText(g, parametroCol.HeaderText, headerFont).Width;
                maxParametroTextWidth = headerTextWidth;

                if (_staticDataGridViewParameters != null)
                {
                    foreach (var paramInfo in _staticDataGridViewParameters)
                    {
                        int itemTextWidth = TextRenderer.MeasureText(g, paramInfo.DisplayName, paramCellFont).Width;
                        if (itemTextWidth > maxParametroTextWidth)
                        {
                            maxParametroTextWidth = itemTextWidth;
                        }
                    }
                }
            }

            int parametroColHorizontalPadding = parametroCol.DefaultCellStyle.Padding.Horizontal;
            int calculatedParametroColWidth = maxParametroTextWidth + parametroColHorizontalPadding + 10;

            int minParametroColWidth = 100;
            parametroCol.Width = Math.Max(minParametroColWidth, calculatedParametroColWidth);

            int minWidthPerEtapaCol = 30;
            if (parametroCol.Width > availableClientWidth - (numberOfEtapaColumns * minWidthPerEtapaCol))
            {
                parametroCol.Width = Math.Max(minParametroColWidth, availableClientWidth - (numberOfEtapaColumns * minWidthPerEtapaCol));
                if (parametroCol.Width < minParametroColWidth) parametroCol.Width = minParametroColWidth;
                if (parametroCol.Width < 0) parametroCol.Width = 0;
            }

            int remainingWidthForEtapas = availableClientWidth - parametroCol.Width;
            if (remainingWidthForEtapas < 0) remainingWidthForEtapas = 0;

            if (numberOfEtapaColumns > 0 && remainingWidthForEtapas > 0)
            {
                int baseEtapaWidth = remainingWidthForEtapas / numberOfEtapaColumns;
                int remainderEtapaWidth = remainingWidthForEtapas % numberOfEtapaColumns;

                for (int i = 0; i < numberOfEtapaColumns; i++)
                {
                    DataGridViewColumn etapaCol = dataGridView1.Columns[i + 1];
                    etapaCol.Width = baseEtapaWidth + (i < remainderEtapaWidth ? 1 : 0);
                    if (etapaCol.Width < minWidthPerEtapaCol && baseEtapaWidth >= minWidthPerEtapaCol) etapaCol.Width = minWidthPerEtapaCol;
                    else if (etapaCol.Width < 0) etapaCol.Width = 0;
                }
            }
            else if (numberOfEtapaColumns > 0)
            {
                for (int i = 0; i < numberOfEtapaColumns; i++)
                {
                    dataGridView1.Columns[i + 1].Width = Math.Max(0, Math.Min(minWidthPerEtapaCol, availableClientWidth / (numberOfEtapaColumns + 1)));
                }
            }

            int currentTotalColWidth = 0;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Visible) currentTotalColWidth += col.Width;
            }

            if (currentTotalColWidth < availableClientWidth && dataGridView1.Columns.Count > 1)
            {
                int slack = availableClientWidth - currentTotalColWidth;
                dataGridView1.Columns[dataGridView1.Columns.Count - 1].Width += slack;
            }
            else if (currentTotalColWidth > availableClientWidth && dataGridView1.Columns.Count > 1)
            {
                int overflow = currentTotalColWidth - availableClientWidth;
                for (int i = dataGridView1.Columns.Count - 1; i > 0 && overflow > 0; i--)
                {
                    DataGridViewColumn etapaCol = dataGridView1.Columns[i];
                    int reduction = Math.Min(overflow, etapaCol.Width - minWidthPerEtapaCol);
                    if (reduction > 0)
                    {
                        etapaCol.Width -= reduction;
                        overflow -= reduction;
                    }
                }
            }

            int columnHeadersHeight = dataGridView1.ColumnHeadersVisible ? dataGridView1.ColumnHeadersHeight : 0;
            int availableHeightForRows = availableClientHeight - columnHeadersHeight;
            if (availableHeightForRows < 0) availableHeightForRows = 0;

            int numberOfRows = dataGridView1.Rows.Count;

            if (numberOfRows > 0 && availableHeightForRows > 0)
            {
                Font rowFont = dataGridView1.DefaultCellStyle.Font ?? dataGridView1.Font ?? SystemFonts.DefaultFont;
                int cellVerticalPadding = dataGridView1.DefaultCellStyle.Padding.Vertical;
                int minPracticalRowHeight = rowFont.Height + cellVerticalPadding + 4;

                int baseRowHeight = availableHeightForRows / numberOfRows;
                int remainderRowHeight = availableHeightForRows % numberOfRows;

                int actualRowHeight = Math.Max(minPracticalRowHeight, baseRowHeight);

                int totalAppliedRowHeight = 0;
                for (int i = 0; i < numberOfRows; i++)
                {
                    if (!dataGridView1.Rows[i].IsNewRow)
                    {
                        int heightForRow = actualRowHeight;
                        if (baseRowHeight >= minPracticalRowHeight && i < remainderRowHeight)
                        {
                            heightForRow++;
                        }
                        dataGridView1.Rows[i].Height = heightForRow;
                        totalAppliedRowHeight += heightForRow;
                    }
                }

                if (totalAppliedRowHeight < availableHeightForRows && numberOfRows > 0)
                {
                    int diff = availableHeightForRows - totalAppliedRowHeight;
                    for (int i = 0; i < Math.Min(diff, numberOfRows); i++)
                    {
                        dataGridView1.Rows[i].Height++;
                    }
                }
                else if (totalAppliedRowHeight > availableHeightForRows && numberOfRows > 0)
                {
                    int diff = totalAppliedRowHeight - availableHeightForRows;
                    for (int i = 0; i < Math.Min(diff, numberOfRows); i++)
                    {
                        if (dataGridView1.Rows[i].Height > minPracticalRowHeight)
                        {
                            dataGridView1.Rows[i].Height--;
                        }
                    }
                }

                dataGridView1.RowTemplate.Height = (numberOfRows > 0 && availableHeightForRows > 0) ?
                                                   Math.Max(minPracticalRowHeight, availableHeightForRows / numberOfRows) :
                                                   minPracticalRowHeight;
            }
            else if (numberOfRows > 0)
            {
                int minHeight = dataGridView1.Font?.Height ?? 10;
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow) row.Height = Math.Max(1, minHeight / 2);
                }
                dataGridView1.RowTemplate.Height = Math.Max(1, minHeight / 2);
            }

            dataGridView1.ResumeLayout(true);
        }
        private void SetButtonState(Control button, bool enabled)
        {
            if (button != null && !button.IsDisposed)
            {
                if (button.InvokeRequired)
                {
                    button.BeginInvoke((MethodInvoker)delegate { button.Enabled = enabled; });
                }
                else
                {
                    button.Enabled = enabled;
                }

                if (button is CuoreUI.Controls.cuiButton cuiButton)
                {
                }
            }
        }
        private void InitializeAllCharts()
        {
            InitializeChart(chart1, "Vazão (LPM)", "Pressão (BAR)", "Curva de Desempenho Principal",
                new List<SeriesConfig> { new SeriesConfig("Pre.x Vaz.", SeriesChartType.FastLine, Color.Blue) },
                0, 100, 0, 300);
            InitializeChart(chart2, "Rotação (RPM)", "Dreno (LPM)", "Vazamento Interno / Dreno",
                new List<SeriesConfig> { new SeriesConfig("Vaz.In.X Rot", SeriesChartType.FastLine, Color.Red) },
                0, 3000, 0, 100);
            InitializeChart(chart3, "Pressão de Carga (BAR)", "Dreno (LPM)", "Curva de Pressão",
                new List<SeriesConfig> { new SeriesConfig("Vaz. x Pres.", SeriesChartType.FastLine, Color.Orange) },
                0, 300, 0, 10);

        }
        private void InitializeChart(Chart chart, string xAxisTitle, string yAxisTitle, string chartTitle, List<SeriesConfig> seriesConfigs, double xMin, double xMax, double yMin, double yMax)
        {
            if (chart == null || chart.IsDisposed) return;
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            ChartArea chartArea = new ChartArea(chartTitle.Replace(" ", "") + "Area");

            chartArea.AxisX.Title = xAxisTitle;
            chartArea.AxisX.Minimum = xMin;
            if (xMax > xMin) chartArea.AxisX.Maximum = xMax; else chartArea.AxisX.Maximum = xMin + 1;
            chartArea.AxisX.LabelStyle.Format = "F0";

            chartArea.AxisY.Title = yAxisTitle;
            chartArea.AxisY.Minimum = yMin;
            if (yMax > yMin) chartArea.AxisY.Maximum = yMax; else chartArea.AxisY.Maximum = yMin + 1;
            chartArea.AxisY.LabelStyle.Format = "F0";

            chartArea.CursorX.IsUserEnabled = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.CursorY.IsUserEnabled = true;
            chartArea.CursorY.IsUserSelectionEnabled = true;
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisY.ScaleView.Zoomable = true;

            chart.ChartAreas.Add(chartArea);

            foreach (var sc in seriesConfigs)
            {
                Series series = new Series(sc.Name)
                {
                    ChartType = sc.Type,
                    Color = sc.Color,
                    BorderWidth = 2
                };
                chart.Series.Add(series);
            }
            chart.Titles.Clear();
            chart.Titles.Add(new Title(chartTitle, Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black));
        }
        private void InitializeChartWithSecondaryAxis(Chart chart, string xAxisTitle, string yAxisTitlePrimary, string yAxisTitleSecondary, string chartTitle, List<SeriesConfig> seriesConfigs, double xMin, double xMax, double yMinPrimary, double yMaxPrimary, double yMinSecondary, double yMaxSecondary)
        {
            if (chart == null || chart.IsDisposed) return;
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            ChartArea chartArea = new ChartArea(chartTitle.Replace(" ", "") + "Area");

            chartArea.AxisX.Title = xAxisTitle;
            chartArea.AxisX.Minimum = xMin;
            if (xMax > xMin) chartArea.AxisX.Maximum = xMax; else chartArea.AxisX.Maximum = xMin + 1;
            chartArea.AxisX.LabelStyle.Format = "F0";


            chartArea.AxisY.Title = yAxisTitlePrimary;
            chartArea.AxisY.Minimum = yMinPrimary;
            if (yMaxPrimary > yMinPrimary) chartArea.AxisY.Maximum = yMaxPrimary; else chartArea.AxisY.Maximum = yMinPrimary + 1;
            chartArea.AxisY.LineColor = Color.Black;
            chartArea.AxisY.LabelStyle.ForeColor = Color.Black;
            chartArea.AxisY.TitleForeColor = Color.Black;
            chartArea.AxisY.LabelStyle.Format = "F0";


            chartArea.AxisY2.Enabled = AxisEnabled.True;
            chartArea.AxisY2.Title = yAxisTitleSecondary;
            chartArea.AxisY2.Minimum = yMinSecondary;
            if (yMaxSecondary > yMinSecondary) chartArea.AxisY2.Maximum = yMaxSecondary; else chartArea.AxisY2.Maximum = yMinSecondary + 1;
            chartArea.AxisY2.LineColor = Color.DarkRed;
            chartArea.AxisY2.LabelStyle.ForeColor = Color.DarkRed;
            chartArea.AxisY2.TitleForeColor = Color.DarkRed;
            chartArea.AxisY2.MajorGrid.Enabled = false;
            chartArea.AxisY2.LabelStyle.Format = "F0";


            chartArea.CursorX.IsUserEnabled = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.CursorY.IsUserEnabled = true;
            chartArea.CursorY.IsUserSelectionEnabled = true;

            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisY.ScaleView.Zoomable = true;
            chartArea.AxisY2.ScaleView.Zoomable = true;

            chart.ChartAreas.Add(chartArea);

            foreach (var sc in seriesConfigs)
            {
                Series series = new Series(sc.Name)
                {
                    ChartType = sc.Type,
                    Color = sc.Color,
                    BorderWidth = 2,
                    YAxisType = sc.Axis
                };
                chart.Series.Add(series);
            }
            chart.Titles.Clear();
            chart.Titles.Add(new Title(chartTitle, Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black));
        }
        private void Tela_Bombas_Load(object sender, EventArgs e)
        {
            _viewModel.Carregar_configuracao(this);
            if (Stage_box_bomba != null) _viewModel.Stage_signal(Stage_box_bomba);
            if (LabelHorariotela != null) _viewModel.VincularRelogioLabel(LabelHorariotela);
            LogHistoricalEvent("AGUARDANDO INÍCIO DO ENSAIO...", Color.DarkGreen);
        }
        private void CloseWindows_Click(object sender, EventArgs e)
        {
            btnretornar_Click(sender, e);
        }
        private void Tela_Bombas_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_fechamentoControladoPeloUsuario)
            {
                DialogResult dr = MessageBox.Show("Deseja fechar toda a aplicação?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    StopAllOperationsAndQuit(true);
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                StopAllOperationsAndQuit(false);
            }
        }
        private void StopAllOperationsAndQuit(bool exitApplication = true)
        {
            StopTimers();
            StopSerialConnection();

            _updateUiTimer?.Dispose();
            _updateUiTimer = null;
            monitoramentoTimer?.Dispose();
            monitoramentoTimer = null;
            timerCronometro?.Dispose();
            timerCronometro = null;

            if (exitApplication)
            {
                ConnectionSettingsApplication.CloseAllConnections();
                Environment.Exit(Environment.ExitCode);
            }
        }
        public void btnIniciar_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox tb6 = this.Controls.Find("textBox6", true).FirstOrDefault() as System.Windows.Forms.TextBox;
            System.Windows.Forms.TextBox tb5 = this.Controls.Find("textBox5", true).FirstOrDefault() as System.Windows.Forms.TextBox;
            System.Windows.Forms.TextBox tb4 = this.Controls.Find("textBox4", true).FirstOrDefault() as System.Windows.Forms.TextBox;

            if (!_viewModel.cabecalhoinicial(tb6, tb5, tb4))
            {

                MessageBox.Show("Favor preencher os campos obrigatórios em DADOS DE ENSAIO.", "Campos Obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _viewModel.PiscarLabelsVermelhoSync(label6, label5, label4, 1000);
                return;
            }

            _isMonitoring = true;
            Inicioteste = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            if (Stage_box_bomba != null) _viewModel.IniciarTesteBomba(Stage_box_bomba);

            InicializarMonitoramento();
            _timeCounterSecondsRampa = 0;
            etapaAtual = 1;

            if (tempoCronometroDefinidoManualmente && valorDefinidoCronometro > 0)
            {
                cronometroIniciado = true;
                int tempoTotal = valorDefinidoCronometro * 60;
                if (circularProgressBar1 != null)
                {
                    circularProgressBar1.Maximum = tempoTotal > 0 ? tempoTotal : 1;
                    circularProgressBar1.Minimum = 0;
                    circularProgressBar1.Value = tempoTotal;
                    circularProgressBar1.Invalidate();
                }
                timerCronometro.Start();
            }
            else
            {
                if (circularProgressBar1 != null)
                {
                    circularProgressBar1.Value = 0;
                    circularProgressBar1.Maximum = 100;
                }
            }

            SetButtonState(btngravar, true);
            SetButtonState(bntFinalizar, true);
            SetButtonState(btnreset, true);
            SetButtonState(btnrelatoriobomba, false);
            SetButtonState(btniniciarteste, false);
            LogHistoricalEvent("INICIADO ENSAIO DE BOMBAS", Color.Blue);

            ClearCharts();
            _viewModel.ResetChartDataLogic();
            ClearStaticDataGridViewCells();
            StartSerialConnection();
        }
        private void InicializarMonitoramento()
        {
            if (monitoramentoTimer == null)
            {
                monitoramentoTimer = new System.Windows.Forms.Timer();
                monitoramentoTimer.Interval = 1000;
            }
        }
        private void PararMonitoramento()
        {
            monitoramentoTimer?.Stop();
        }
        private void btnParar_Click(object sender, EventArgs e)
        {
            StopTimers();
            StopSerialConnection();

            cronometroIniciado = false;
            _isMonitoring = false;
            Fimteste = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            SetButtonState(btngravar, false);
            SetButtonState(bntFinalizar, false);
            SetButtonState(btnreset, true);
            SetButtonState(btnrelatoriobomba, true);
            SetButtonState(btniniciarteste, true);

            LogHistoricalEvent("ENSAIO DE BOMBAS FINALIZADO", Color.Red);
            if (Stage_box_bomba != null) _viewModel.FinalizarTesteBomba(Stage_box_bomba);
        }
        private void btnretornar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Tem certeza que deseja reiniciar o processo e retornar ao menu?\nTodos os dados não salvos em relatório serão perdidos!",
                "Confirmação de Reinício",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                return;
            }

            _fechamentoControladoPeloUsuario = true;

            StopTimers();
            StopSerialConnection();
            _isMonitoring = false;
            cronometroIniciado = false;

            _timeCounterSecondsRampa = 0;
            etapaAtual = 1;
            if (dadosSensores != null) dadosSensores.Clear();
            if (_dadosColetados != null) _dadosColetados.Clear();


            Inicioteste = string.Empty;
            Fimteste = string.Empty;

            lock (serialBufferLock)
            {
                serialDataBuffer.Clear();
            }
            lock (readingsLock)
            {
                _currentNumericSensorReadings.Clear();
            }

            UpdateTextBoxes();
            ClearStaticDataGridViewCells();

            if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
            {
                circularProgressBar1.Value = 0;
                circularProgressBar1.Maximum = (tempoCronometroDefinidoManualmente && valorDefinidoCronometro > 0) ? (valorDefinidoCronometro * 60) : 100;
                circularProgressBar1.Invalidate();
            }

            ClearCharts();
            _viewModel.ResetChartDataLogic();

            if (Stage_box_bomba != null) _viewModel.FinalizarTesteBomba(Stage_box_bomba);
            LogHistoricalEvent("AGUARDANDO INÍCIO DO ENSAIO...", Color.DarkGreen);

            SetButtonState(btniniciarteste, true);
            SetButtonState(btngravar, false);
            SetButtonState(bntFinalizar, false);
            SetButtonState(btnreset, false);
            SetButtonState(btnrelatoriobomba, false);

            try
            {
                var menuAppInstance = Menuapp.Instance;
                if (menuAppInstance != null && !menuAppInstance.IsDisposed)
                {
                    menuAppInstance.Show();
                    menuAppInstance.BringToFront();
                }
                else
                {
                    Debug.WriteLine("btnretornar_Click: Menuapp.Instance não disponível ou descartado.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"btnretornar_Click: Exceção ao tentar mostrar Menuapp: {ex.Message}");
            }
            this.Close();
        }
        private void ClearStaticDataGridViewCells()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed) return;

            Action action = () => {
                for (int colIndex = 1; colIndex < dataGridView1.Columns.Count; colIndex++)
                {
                    for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count; rowIndex++)
                    {
                        if (dataGridView1.Rows[rowIndex].Cells.Count > colIndex &&
                            dataGridView1.Rows[rowIndex].Cells[colIndex] != null)
                        {
                            dataGridView1.Rows[rowIndex].Cells[colIndex].Value = string.Empty;
                        }
                    }
                }
            };

            if (dataGridView1.InvokeRequired)
            {
                try { dataGridView1.Invoke(action); } catch (ObjectDisposedException) { }
            }
            else
            {
                action();
            }
        }
        private void AddDataPointsToCharts()
        {
            if (this.IsDisposed || this.Disposing) return;

            if (this.InvokeRequired)
            {
                try { this.BeginInvoke((MethodInvoker)AddDataPointsToChartsInternal); }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
            }
            else
            {
                AddDataPointsToChartsInternal();
            }
        }
        private void AddDataPointsToChartsInternal()
        {
            if (this.IsDisposed || this.Disposing) return;
            Dictionary<string, double> readingsSnapshot;
            lock (readingsLock)
            {
                readingsSnapshot = new Dictionary<string, double>(_currentNumericSensorReadings);
            }

            if (readingsSnapshot.TryGetValue("PR1", out double pressaoBar) &&
                readingsSnapshot.TryGetValue("VZ1", out double vazaoLpm))
            {
                if (chart1 != null && !chart1.IsDisposed && chart1.Series.IndexOf("Pre.x Vaz.") != -1)
                {
                    chart1.Series["Pre.x Vaz."].Points.AddXY(vazaoLpm, pressaoBar);
                }
            }

            if (readingsSnapshot.TryGetValue("ROT", out double rotacaoRpm) &&
                readingsSnapshot.TryGetValue("DR1", out double drenoLpm))
            {
                if (chart2 != null && !chart2.IsDisposed && chart2.Series.IndexOf("Vaz.In.X Rot") != -1)
                {
                    chart2.Series["Vaz.In.X Rot"].Points.AddXY(rotacaoRpm, drenoLpm);
                }
            }

            if (readingsSnapshot.TryGetValue("PL1", out double pilotagemBar) &&
                readingsSnapshot.TryGetValue("DR1", out double drenoLpmForChart3))
            {
                if (chart3 != null && !chart3.IsDisposed && chart3.Series.IndexOf("Vaz. x Pres.") != -1)
                {
                    chart3.Series["Vaz. x Pres."].Points.AddXY(pilotagemBar, drenoLpmForChart3);
                }
            }
        }
        private void ClearCharts()
        {
            if (this.IsDisposed || this.Disposing) return;

            var chartToCheck = chart1 ?? chart2 ?? chart3;
            if (chartToCheck != null && !chartToCheck.IsDisposed && chartToCheck.InvokeRequired)
            {
                try { this.BeginInvoke((MethodInvoker)ClearChartsInternal); }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
            }
            else
            {
                ClearChartsInternal();
            }
        }
        private void ClearChartsInternal()
        {
            if (this.IsDisposed || this.Disposing) return;
            var charts = new List<Chart> { chart1, chart2, chart3 };
            foreach (Chart chart in charts)
            {
                if (chart != null && !chart.IsDisposed)
                {
                    foreach (var series in chart.Series)
                    {
                        series.Points.Clear();
                    }
                }
            }
        }
        private void StopTimers()
        {
            timerCronometro?.Stop();
            monitoramentoTimer?.Stop();
            StopUpdateUiTimer();
        }
        private void LogHistoricalEvent(string message, Color? color = null)
        {
            if (HistoricalEvents == null || HistoricalEvents.IsDisposed || this.IsDisposed || this.Disposing) return;

            if (HistoricalEvents.InvokeRequired)
            {
                try
                {
                    HistoricalEvents.BeginInvoke((MethodInvoker)delegate {
                        AppendLogMessage(message, color);
                    });
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
            }
            else
            {
                AppendLogMessage(message, color);
            }
        }
        private void AppendLogMessage(string message, Color? color = null)
        {
            if (HistoricalEvents == null || HistoricalEvents.IsDisposed) return;

            if (HistoricalEvents.TextLength > 15000)
            {
                HistoricalEvents.Text = HistoricalEvents.Text.Substring(HistoricalEvents.TextLength - 5000);
            }

            HistoricalEvents.SelectionStart = HistoricalEvents.TextLength;
            HistoricalEvents.SelectionLength = 0;

            HistoricalEvents.AppendText($"{DateTime.Now:G}: {message}" + Environment.NewLine);

            HistoricalEvents.ScrollToCaret();
        }
        public struct SeriesConfig
        {
            public string Name;
            public SeriesChartType Type;
            public Color Color;
            public AxisType Axis;

            public SeriesConfig(string name, SeriesChartType type, Color color, AxisType axis = AxisType.Primary)
            {
                Name = name;
                Type = type;
                Color = color;
                Axis = axis;
            }
        }
        private void GravarDadoSensor(string nomeSensor, string valor, string unidade)
        {
            if (string.IsNullOrEmpty(valor) || valor == "N/A")
            {
                MessageBox.Show($"Valor do sensor '{nomeSensor}' não disponível para gravação.", "Dados Ausentes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isMonitoring)
            {
                if (dadosSensores == null) dadosSensores = new List<SensorData>();
                dadosSensores.Add(new SensorData { Sensor = nomeSensor, Valor = valor, Medidas = unidade });
            }
            else
            {
                MessageBox.Show("Teste não iniciado. Favor iniciar o teste para capturar os dados.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private bool ValidarMinMaxCheckBoxLocal(System.Windows.Forms.CheckBox checkBox, System.Windows.Forms.TextBox minTextBox, System.Windows.Forms.TextBox maxTextBox, string nomeUnidade)
        {
            if (checkBox == null || minTextBox == null || maxTextBox == null) return false;

            if (!checkBox.Checked) return true;

            bool minOk = decimal.TryParse(minTextBox.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMinimo);
            bool maxOk = decimal.TryParse(maxTextBox.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMaximo);
            string erroMsg = null;

            if (!minOk || !maxOk) erroMsg = $"Valores de Mínimo e Máximo para {nomeUnidade} devem ser numéricos.";
            else if (valorMinimo < 0 || valorMaximo < 0) erroMsg = $"Valores de Mínimo e Máximo para {nomeUnidade} não podem ser negativos.";
            else if (valorMinimo > valorMaximo) erroMsg = $"Valor Mínimo para {nomeUnidade} não pode ser maior que o Valor Máximo.";

            if (erroMsg != null)
            {
                MessageBox.Show(this, erroMsg + $"\nA verificação de limites para {nomeUnidade} foi desativada.", $"Erro de Validação - {nomeUnidade}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkBox.Checked = false;
                return false;
            }
            return true;
        }
        private void checkBox_psi_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox9, textBox8, "PSI"); }
        private void checkBox_gpm_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox14, textBox12, "GPM"); }
        private void checkBox_bar_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox11, textBox10, "BAR"); }
        private void checkBox_rotacao_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox18, textBox17, "Rotação (RPM)"); }
        private void checkBox_lpm_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox16, textBox15, "LPM"); }
        private void checkBox_temperatura_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox20, textBox19, "Temperatura (°C)"); }
        private void TimerCronometro_Tick(object sender, EventArgs e)
        {
            if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed && circularProgressBar1.Value > 0)
            {
                circularProgressBar1.Value--;
            }
            else
            {
                timerCronometro.Stop();
                cronometroIniciado = false;
                if (_isMonitoring)
                {
                    LogHistoricalEvent("Tempo do cronômetro esgotado. Finalizando teste automaticamente.", Color.Orange);
                    btnParar_Click(this, EventArgs.Empty);
                }
            }
        }
        private void btnDefinir_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox inputTempoCronometro = this.Controls.Find("textBox1_tempoCronometro", true).FirstOrDefault() as System.Windows.Forms.TextBox;

            if (inputTempoCronometro == null || inputTempoCronometro.IsDisposed)
            {
                MessageBox.Show(this, "Controle para definir tempo do cronômetro não encontrado ou foi descartado.", "Erro UI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!cronometroIniciado)
            {
                if (int.TryParse(inputTempoCronometro.Text, out int valorMinutos) && valorMinutos > 0)
                {
                    valorDefinidoCronometro = valorMinutos;
                    tempoCronometroDefinidoManualmente = true;
                    if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
                    {
                        circularProgressBar1.Maximum = valorDefinidoCronometro * 60 > 0 ? valorDefinidoCronometro * 60 : 1;
                        circularProgressBar1.Value = valorDefinidoCronometro * 60;
                        circularProgressBar1.Invalidate();
                    }
                    MessageBox.Show(this, $"Tempo do cronômetro definido para {valorDefinidoCronometro} minutos.", "Cronômetro Definido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "Por favor, insira um valor numérico inteiro positivo válido em minutos.", "Erro de Entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "O cronômetro já está em execução. Limpe ou finalize o teste atual para definir um novo tempo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btnLimpar_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox inputTempoCronometro = this.Controls.Find("textBox1_tempoCronometro", true).FirstOrDefault() as System.Windows.Forms.TextBox;

            if (inputTempoCronometro != null && !inputTempoCronometro.IsDisposed && !cronometroIniciado)
            {
                inputTempoCronometro.Text = "0";
                valorDefinidoCronometro = 0;
                tempoCronometroDefinidoManualmente = false;
                if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
                {
                    circularProgressBar1.Value = 0;
                    circularProgressBar1.Maximum = 100;
                    circularProgressBar1.Invalidate();
                }
                MessageBox.Show(this, "Tempo do cronômetro limpo.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (cronometroIniciado)
            {
                MessageBox.Show(this, "O cronômetro está em execução. Finalize o teste atual para limpar o tempo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        private void btn_gravar_Click(object sender, EventArgs e)
        {
            if (!_isMonitoring)
            {
                MessageBox.Show("O teste não foi iniciado. Por favor, inicie o teste para gravar os dados.", "Teste Não Iniciado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (tempoCronometroDefinidoManualmente && !cronometroIniciado && valorDefinidoCronometro > 0 && (circularProgressBar1 == null || circularProgressBar1.Value <= 0))
            {
                MessageBox.Show("O tempo definido para o teste encerrou ou o cronômetro não está ativo. Não é possível gravar novas etapas.", "Tempo Esgotado ou Cronômetro Inativo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetButtonState(btngravar, false);
                return;
            }

            int maxEtapasTabela = 7;
            if (etapaAtual > maxEtapasTabela)
            {
                MessageBox.Show($"Limite de {maxEtapasTabela} etapas para a tabela foi atingido.", "Limite de Etapas da Tabela", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetButtonState(btngravar, false);
                return;
            }

            GravarDadosNoDataGridViewEstatico();

            var currentEtapaData = new EtapaData
            {
                Etapa = etapaAtual,
                leituras = new List<SensorData>()
            };

            Dictionary<string, double> readingsSnapshot;
            lock (readingsLock)
            {
                readingsSnapshot = new Dictionary<string, double>(_currentNumericSensorReadings);
            }

            if (_staticDataGridViewParameters != null)
            {
                foreach (var paramInfo in _staticDataGridViewParameters)
                {
                    string serialKeyForParam = _arduinoKeyToSensorIdMap
                        .FirstOrDefault(kvp =>
                            sensorControlsMap.ContainsKey(kvp.Value) &&
                            sensorControlsMap[kvp.Value].valueTextBox.Name == paramInfo.SourceTextBoxName)
                        .Key;

                    if (string.IsNullOrEmpty(serialKeyForParam))
                    {
                        serialKeyForParam = paramInfo.SourceTextBoxName.Replace("sensor_", "");
                    }

                    string valorParaRelatorio = "N/D";
                    string unidadeParaRelatorio = "";

                    if (readingsSnapshot.TryGetValue(serialKeyForParam, out double sensorNumericValue))
                    {
                        valorParaRelatorio = sensorNumericValue.ToString("F2", CultureInfo.InvariantCulture);

                        var readingData = allReadingsData.FirstOrDefault(rd => _arduinoKeyToSensorIdMap.ContainsKey(serialKeyForParam) && _arduinoKeyToSensorIdMap[serialKeyForParam] == rd.Id);
                        if (readingData != null)
                        {
                            if (readingData.Type == "pressure") unidadeParaRelatorio = currentConfiguration.SelectedPressureUnit.ToUpper();
                            else if (readingData.Type == "flow") unidadeParaRelatorio = currentConfiguration.SelectedFlowUnit.ToUpper();
                            else unidadeParaRelatorio = readingData.OriginalUnit;
                        }
                    }

                    currentEtapaData.leituras.Add(new SensorData
                    {
                        Sensor = paramInfo.DisplayName,
                        Valor = valorParaRelatorio,
                        Medidas = unidadeParaRelatorio
                    });
                }
            }
            _dadosColetados.Add(currentEtapaData);

            LogHistoricalEvent($"Dados da Etapa {etapaAtual} gravados.", Color.DarkCyan);
            etapaAtual++;

            if (etapaAtual > maxEtapasTabela)
            {
                MessageBox.Show("Todas as etapas da tabela foram preenchidas.", "Tabela Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetButtonState(btngravar, false);
            }
        }
        private void GravarDadosNoDataGridViewEstatico()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed || _staticDataGridViewParameters == null) return;
            if (etapaAtual < 1 || etapaAtual > (dataGridView1.Columns.Count - 1))
            {
                Debug.WriteLine($"[GravarDGV] Etapa atual ({etapaAtual}) fora do intervalo válido para colunas (1 a {dataGridView1.Columns.Count - 1}).");
                return;
            }

            int targetColumnIndexInDGV = etapaAtual;

            for (int rowIndex = 0; rowIndex < _staticDataGridViewParameters.Count; rowIndex++)
            {
                if (rowIndex >= dataGridView1.Rows.Count)
                {
                    Debug.WriteLine($"[GravarDGV] rowIndex {rowIndex} fora do intervalo para dataGridView1.Rows.Count {dataGridView1.Rows.Count}");
                    break;
                }

                ParameterRowInfo paramInfo = _staticDataGridViewParameters[rowIndex];
                System.Windows.Forms.TextBox sourceTextBox = null;

                Control[] foundControls = this.Controls.Find(paramInfo.SourceTextBoxName, true);
                if (foundControls.Length > 0 && foundControls[0] is TextBox tb)
                {
                    sourceTextBox = tb;
                }
                else
                {
                    var field = this.GetType().GetField(paramInfo.SourceTextBoxName,
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.Instance);
                    if (field != null && field.GetValue(this) is TextBox tbInstance)
                    {
                        sourceTextBox = tbInstance;
                    }
                }

                string formattedValue = "ERRO";

                if (sourceTextBox != null && !sourceTextBox.IsDisposed)
                {
                    string rawValue = sourceTextBox.Text;
                    if (string.IsNullOrWhiteSpace(rawValue) || rawValue.Equals("N/A", StringComparison.OrdinalIgnoreCase))
                    {
                        formattedValue = "-";
                    }
                    else
                    {
                        if (double.TryParse(rawValue.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
                        {
                            switch (paramInfo.FormattingType.ToLower())
                            {
                                case "float":
                                    formattedValue = numericValue.ToString("0.00", CultureInfo.InvariantCulture);
                                    break;
                                case "temp":
                                    formattedValue = ((int)Math.Round(numericValue)).ToString("D", CultureInfo.InvariantCulture);
                                    break;
                                case "rpm":
                                    formattedValue = ((int)Math.Round(numericValue)).ToString("D", CultureInfo.InvariantCulture);
                                    break;
                                default:
                                    formattedValue = numericValue.ToString(CultureInfo.InvariantCulture);
                                    break;
                            }
                        }
                        else
                        {
                            formattedValue = "Inválido";
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"[GravarDGV] TextBox de origem '{paramInfo.SourceTextBoxName}' para o parâmetro '{paramInfo.DisplayName}' não foi encontrado ou está descartado.");
                    formattedValue = "N/D";
                }

                if (dataGridView1.Rows[rowIndex].Cells.Count > targetColumnIndexInDGV &&
                    dataGridView1.Rows[rowIndex].Cells[targetColumnIndexInDGV] != null)
                {
                    if (dataGridView1.InvokeRequired)
                    {
                        int r = rowIndex; int c = targetColumnIndexInDGV; string val = formattedValue;
                        try { dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Rows[r].Cells[c].Value = val; }); }
                        catch (ObjectDisposedException) { }
                    }
                    else
                    {
                        dataGridView1.Rows[rowIndex].Cells[targetColumnIndexInDGV].Value = formattedValue;
                    }
                }
            }
        }
        #endregion

    }
}