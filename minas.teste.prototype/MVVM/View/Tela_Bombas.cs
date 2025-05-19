using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.MVVM.ViewModel;
using minas.teste.prototype.Service;

// Certifique-se que Tela_BombasVM.Datapoint_Bar_Lpm está acessível.
// Se estiver dentro de Tela_BombasVM, e Tela_BombasVM for uma classe,
// você precisará de uma instância ou torná-la estática, ou mover a struct.
// Exemplo: public struct Datapoint_Bar_Lpm { public double FlowLpm; public double PressureBar; }
// pode ser definida dentro deste namespace ou em um arquivo comum.

namespace minas.teste.prototype.MVVM.View
{
    public partial class Tela_Bombas : Form
    {
        private Tela_BombasVM _viewModel;
        private List<EtapaData> _dadosColetados = new List<EtapaData>();
        public Dictionary<string, TextBox> sensorMap;
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

        private Dictionary<string, string> _latestRawSensorData = new Dictionary<string, string>();
        private Dictionary<string, double> _currentNumericSensorReadings = new Dictionary<string, double>();

        private Dictionary<string, string> _serialDataKeys = new Dictionary<string, string>()
        {
            {"P1", "Pressao Principal"}, {"fluxo1", "Vazao Principal"},
            {"Piloto1", "Pilotagem"}, {"dreno1", "Vazao Dreno"},
            {"RPM", "Rotacao"}, {"temp", "Temperatura"}
        };

        private const double BAR_TO_PSI_CONVERSION = 14.5;
        private const double LPM_TO_GPM_USER_CONVERSION = 3.98;
        private const double BAR_CONVERSION_PILOT = 1.705;

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
        private Dictionary<string, string> sensorControlMap;

        public Tela_Bombas()
        {
            InitializeComponent();
            Inciaizador_listas();

            _viewModel = new Tela_BombasVM();

            if (this.Controls.ContainsKey("textBox2") && this.Controls.ContainsKey("trackBar1"))
            {
                _synchronizerNivel = new TextBoxTrackBarSynchronizer(textBox2, trackBar1, 1, 7);
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

            SetButtonState(btniniciarteste, true);
            SetButtonState(btngravar, false);
            SetButtonState(bntFinalizar, false);
            SetButtonState(btnreset, false);
            SetButtonState(btnrelatoriobomba, false);
        }

        private void SetButtonState(Button button, bool enabled)
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
            }
        }

        public void Inciaizador_listas()
        {
            sensorMap = new Dictionary<string, TextBox>()
            {
                {"Piloto1", sensor_bar_PL}, {"dreno1", sensor_lpm_DR},
                {"P1", sensor_Press_BAR}, {"RPM", sensor_rotacao_RPM},
                {"fluxo1", sensor_Vazao_LPM}, {"temp", sensor_Temp_C}
            };
            sensorMapmedida = new Dictionary<string, string>()
            {
                {"Piloto1", "BAR"}, {"dreno1", "LPM"}, {"P1", "BAR"},
                {"RPM", "RPM"}, {"fluxo1", "LPM"}, {"temp", "Celsius"}
            };
            sensorControlMap = new Dictionary<string, string>
            {
                {"Pilotagem PSI", "sensor_psi_PL"}, {"Pilotagem BAR", "sensor_bar_PL"},
                {"Dreno GPM", "sensor_gpm_DR"}, {"Dreno LPM", "sensor_lpm_DR"},
                {"Pressao PSI", "sensor_Press_PSI"}, {"Pressao BAR", "sensor_Press_BAR"},
                {"Rotação RPM", "sensor_rotacao_RPM"},
                {"Vazão GPM", "sensor_Vazao_GPM"}, {"Vazão LPM", "sensor_Vazao_LPM"},
                {"Temperatura Celsius", "sensor_Temp_C"}
            };

            ResetNumericSensorReadings();

            dadosSensoresSelecionados.Clear();
            dadosSensoresSelecionados.AddRange(_serialDataKeys.Keys);
        }

        private void ResetNumericSensorReadings()
        {
            lock (readingsLock)
            {
                _currentNumericSensorReadings = new Dictionary<string, double>()
                {
                    {"P1", 0.0}, {"fluxo1", 0.0}, {"Piloto1", 0.0},
                    {"dreno1", 0.0}, {"RPM", 0.0}, {"temp", 0.0}
                };
                _latestRawSensorData.Clear();
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
            InitializeChart(chart3, "Pressão de Carga (BAR)", "Dreno (LPM)", "Vazamento (Dreno) do Circuito Fechado",
                new List<SeriesConfig> { new SeriesConfig("Vaz. x Pres.", SeriesChartType.FastLine, Color.Orange) },
                0, 300, 0, 10);
            InitializeChart(chart4, "Vazão (LPM)", "Eficiência (%)", "Curvas de Rendimento",
                new List<SeriesConfig> {
                    new SeriesConfig("Rend. Global", SeriesChartType.FastLine, Color.Green),
                    new SeriesConfig("Ef. Volumetrica", SeriesChartType.FastLine, Color.Purple)
                }, 0, 100, 0, 100);
            InitializeChartWithSecondaryAxis(chart5, "Tempo (segundos)", "Temperatura (°C)", "Pressão (BAR) / Vazão (LPM)", "Rampa de Amaciamento e Ciclagem",
                new List<SeriesConfig> {
                    new SeriesConfig("Temperatura", SeriesChartType.FastLine, Color.Blue, AxisType.Primary),
                    new SeriesConfig("Pressão Rampa", SeriesChartType.FastLine, Color.Red, AxisType.Secondary),
                    new SeriesConfig("Vazão Rampa", SeriesChartType.FastLine, Color.DarkGreen, AxisType.Secondary)
                }, 0, 3600,
                 0, 100,
                 0, 350);
            InitializeChart(chart6, "Rotação (RPM)", "Vazão Real (LPM)", "Curva de Deslocamento / Vazão Real",
                 new List<SeriesConfig> { new SeriesConfig("Vazão Real", SeriesChartType.FastLine, Color.Blue) },
                 0, 3000, 0, 100);
        }

        private void InitializeChart(Chart chart, string xAxisTitle, string yAxisTitle, string chartTitle, List<SeriesConfig> seriesConfigs, double xMin, double xMax, double yMin, double yMax)
        {
            if (chart == null || chart.IsDisposed) return;
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            ChartArea chartArea = new ChartArea(chartTitle.Replace(" ", "") + "Area");

            chartArea.AxisX.Title = xAxisTitle;
            chartArea.AxisY.Title = yAxisTitle;
            chartArea.AxisX.Minimum = xMin;
            if (xMax > xMin) chartArea.AxisX.Maximum = xMax;
            chartArea.AxisY.Minimum = yMin;
            if (yMax > yMin) chartArea.AxisY.Maximum = yMax;

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
            chart.Titles.Add(chartTitle);
        }
        private void InitializeChartWithSecondaryAxis(Chart chart, string xAxisTitle, string yAxisTitlePrimary, string yAxisTitleSecondary, string chartTitle, List<SeriesConfig> seriesConfigs, double xMin, double xMax, double yMinPrimary, double yMaxPrimary, double yMinSecondary, double yMaxSecondary)
        {
            if (chart == null || chart.IsDisposed) return;
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            ChartArea chartArea = new ChartArea(chartTitle.Replace(" ", "") + "Area");

            chartArea.AxisX.Title = xAxisTitle;
            chartArea.AxisX.Minimum = xMin;
            if (xMax > xMin) chartArea.AxisX.Maximum = xMax;

            chartArea.AxisY.Title = yAxisTitlePrimary;
            chartArea.AxisY.Minimum = yMinPrimary;
            if (yMaxPrimary > yMinPrimary) chartArea.AxisY.Maximum = yMaxPrimary;
            chartArea.AxisY.LineColor = Color.Black;
            chartArea.AxisY.LabelStyle.ForeColor = Color.Black;
            chartArea.AxisY.TitleForeColor = Color.Black;

            chartArea.AxisY2.Enabled = AxisEnabled.True;
            chartArea.AxisY2.Title = yAxisTitleSecondary;
            chartArea.AxisY2.Minimum = yMinSecondary;
            if (yMaxSecondary > yMinSecondary) chartArea.AxisY2.Maximum = yMaxSecondary;
            chartArea.AxisY2.LineColor = Color.DarkRed;
            chartArea.AxisY2.LabelStyle.ForeColor = Color.DarkRed;
            chartArea.AxisY2.TitleForeColor = Color.DarkRed;
            chartArea.AxisY2.MajorGrid.Enabled = false;

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
            chart.Titles.Add(chartTitle);
        }

        #region LOADS_JANELA
        private void Tela_Bombas_Load(object sender, EventArgs e)
        {
            _viewModel.Carregar_configuracao(this);
            if (Stage_box_bomba != null) _viewModel.Stage_signal(Stage_box_bomba);
            if (LabelHorariotela != null) _viewModel.VincularRelogioLabel(LabelHorariotela);
            LogHistoricalEvent("AGUARDANDO INÍCIO DO ENSAIO...", Color.DarkGreen);
        }
        #endregion

        #region EVENTOS_FECHAMANETO
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
        #endregion

        #region INCIO_TESTE
        public void btnIniciar_Click(object sender, EventArgs e)
        {
            if (textBox6 == null || textBox5 == null || textBox4 == null || !_viewModel.cabecalhoinicial(textBox6, textBox5, textBox4))
            {
                MessageBox.Show("Favor preencher os campos obrigatórios em DADOS DE ENSAIO.", "Campos Obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isMonitoring = true;
            // if (panel4 != null) _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5, panel2, panel6, panel11, panel9);
            Inicioteste = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            if (Stage_box_bomba != null) _viewModel.IniciarTesteBomba(Stage_box_bomba);

            InicializarMonitoramento();
            _timeCounterSecondsRampa = 0;

            if (tempoCronometroDefinidoManualmente)
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
                MessageBox.Show("O cronômetro não foi definido. O teste não será finalizado automaticamente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            SetButtonState(btngravar, true);
            SetButtonState(bntFinalizar, true);
            SetButtonState(btnreset, true);
            SetButtonState(btnrelatoriobomba, false);
            SetButtonState(btniniciarteste, false);
            LogHistoricalEvent("INICIADO ENSAIO DE BOMBAS", Color.Blue);

            ClearCharts();
            _viewModel.ResetChartDataLogic();
            StartSerialConnection();
        }

        private void InicializarMonitoramento()
        {
            if (monitoramentoTimer == null)
            {
                monitoramentoTimer = new System.Windows.Forms.Timer();
                monitoramentoTimer.Interval = 1000;
                monitoramentoTimer.Tick += MonitoramentoTimer_Tick;
            }
            monitoramentoTimer.Start();
        }

        private void PararMonitoramento()
        {
            monitoramentoTimer?.Stop();
        }

        private void MonitoramentoTimer_Tick(object sender, EventArgs e)
        {
            // Adapte esta chamada aos seus controles reais
            // _viewModel.MonitorarDados(...);
            if (_viewModel != null && sensor_psi_PL != null && textBox9 != null && textBox8 != null && checkBox_psi != null && panel4 != null && HistoricalEvents != null &&
               sensor_Press_PSI != null && panel5 != null && sensor_gpm_DR != null && textBox14 != null && textBox12 != null && checkBox_gpm != null && panel2 != null &&
               sensor_Vazao_GPM != null && panel6 != null && sensor_Press_BAR != null && textBox11 != null && textBox10 != null && checkBox_bar != null &&
               sensor_bar_PL != null && sensor_rotacao_RPM != null && textBox18 != null && textBox17 != null && checkBox_rotacao != null && panel9 != null &&
               sensor_Vazao_LPM != null && textBox16 != null && textBox15 != null && checkBox_lpm != null && sensor_lpm_DR != null &&
               sensor_Temp_C != null && textBox20 != null && textBox19 != null && checkBox_temperatura != null && panel11 != null)
            {
                _viewModel.MonitorarDados(
                    sensor_psi_PL.Text, textBox9.Text, textBox8.Text, checkBox_psi.Checked, panel4, HistoricalEvents,
                    sensor_Press_PSI.Text, textBox9.Text, textBox8.Text, checkBox_psi.Checked, panel5, // Repetido textBox9, textBox8, checkBox_psi intencionalmente?
                    sensor_gpm_DR.Text, textBox14.Text, textBox12.Text, checkBox_gpm.Checked, panel2,
                    sensor_Vazao_GPM.Text, textBox14.Text, textBox12.Text, checkBox_gpm.Checked, panel6, // Repetido textBox14, textBox12, checkBox_gpm
                    sensor_Press_BAR.Text, textBox11.Text, textBox10.Text, checkBox_bar.Checked, panel5,
                    sensor_bar_PL.Text, textBox11.Text, textBox10.Text, checkBox_bar.Checked, panel4,   // Repetido textBox11, textBox10, checkBox_bar
                    sensor_rotacao_RPM.Text, textBox18.Text, textBox17.Text, checkBox_rotacao.Checked, panel9,
                    sensor_Vazao_LPM.Text, textBox16.Text, textBox15.Text, checkBox_lpm.Checked, panel6,
                    sensor_lpm_DR.Text, textBox16.Text, textBox15.Text, checkBox_lpm.Checked, panel2,    // Repetido textBox16, textBox15, checkBox_lpm
                    sensor_Temp_C.Text, textBox20.Text, textBox19.Text, checkBox_temperatura.Checked, panel11
                );
            }
        }
        #endregion

        #region Serial Communication and Data Processing

        private void StartSerialConnection()
        {
            try
            {
                string portToConnect = ConnectionSettingsApplication.CurrentPortName;
                int baudRateToConnect = ConnectionSettingsApplication.CurrentBaudRate;

                if (string.IsNullOrEmpty(portToConnect) || baudRateToConnect <= 0)
                {
                    string errorMessage = "Configurações de porta serial (Nome da Porta e/ou Baud Rate) não definidas.";
                    MessageBox.Show(this, errorMessage, "Configuração Serial Ausente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LogHistoricalEvent($"ERRO - {errorMessage}", Color.Red);
                    btnParar_Click(this, EventArgs.Empty);
                    return;
                }

                if (_serialManager == null)
                {
                    LogHistoricalEvent("Erro crítico: SerialManager não inicializado antes de StartSerialConnection.", Color.Red);
                    btnParar_Click(this, EventArgs.Empty);
                    return;
                }

                _serialManager.DataReceived -= SerialManager_DataReceived;
                _serialManager.DataReceived += SerialManager_DataReceived;
                _serialManager.ErrorOccurred -= SerialManager_ErrorOccurred;
                _serialManager.ErrorOccurred += SerialManager_ErrorOccurred;

                if (!_serialManager.IsConnected ||
                    _serialManager.PortName != portToConnect ||
                    _serialManager.BaudRate != baudRateToConnect)
                {
                    if (_serialManager.IsConnected) _serialManager.Disconnect();
                    _serialManager.Connect(portToConnect, baudRateToConnect);
                }

                if (_serialManager.IsConnected)
                {
                    LogHistoricalEvent($"Conectado à porta serial {portToConnect}.", Color.Green);
                    StartUpdateUiTimer();
                }
                else
                {
                    string errorMessage = $"ERRO ao conectar à porta serial {portToConnect}. Verifique as configurações ou se a porta está disponível.";
                    LogHistoricalEvent(errorMessage, Color.Red);
                    MessageBox.Show(this, errorMessage, "Erro de Conexão Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnParar_Click(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                string attemptedPortName = ConnectionSettingsApplication.CurrentPortName;
                string errorMessage = $"Erro durante a tentativa de conexão serial {(string.IsNullOrEmpty(attemptedPortName) ? "" : $"à porta {attemptedPortName}")}: {ex.Message}";
                MessageBox.Show(this, errorMessage, "Erro de Conexão Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        LogHistoricalEvent($"Desconectado da porta serial.", Color.Orange);
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
            string cleanData = FilterNonPrintableChars(data);
            if (string.IsNullOrEmpty(cleanData)) return;

            Debug.WriteLine($"[RAW_CLEAN] Dados: {cleanData.Replace("\n", "\\n").Replace("\r", "\\r")}");

            lock (serialBufferLock)
            {
                serialDataBuffer.Append(cleanData);
                ProcessSerialBuffer();
            }
        }

        private void ProcessSerialBuffer()
        {
            string bufferContent;
            int newlineIndex;

            lock (serialBufferLock)
            {
                bufferContent = serialDataBuffer.ToString();
            }

            while ((newlineIndex = bufferContent.IndexOf('\n')) >= 0)
            {
                string completeMessage = bufferContent.Substring(0, newlineIndex + 1).Trim();

                lock (serialBufferLock)
                {
                    serialDataBuffer.Remove(0, newlineIndex + 1);
                    bufferContent = serialDataBuffer.ToString();
                }

                if (!string.IsNullOrWhiteSpace(completeMessage))
                {
                    Debug.WriteLine($"[PROCESS] Linha completa: {completeMessage}");
                    ParseAndStoreSensorData(completeMessage);
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

        private void ParseAndStoreSensorData(string data)
        {
            string cleanedData = data.Trim();
            if (string.IsNullOrEmpty(cleanedData))
            {
                Debug.WriteLine("[PARSE] Mensagem vazia após trim.");
                return;
            }
            Debug.WriteLine($"[PARSE] Processando: {cleanedData}");

            string[] sensorReadings = cleanedData.Split('|');
            if (sensorReadings.Length != 6 ||
                !sensorReadings[0].Trim().StartsWith("P1:") ||
                !sensorReadings[5].Trim().StartsWith("temp:"))
            {
                // Log de formato inválido apenas para Debug, não para HistoricalEvents
                Debug.WriteLine($"[PARSE_ERROR_FORMAT] Formato inválido. Recebido: {cleanedData}");
                return; // Descarta a mensagem silenciosamente para o usuário
            }

            Dictionary<string, double> parsedValuesThisMessage = new Dictionary<string, double>();
            List<string> expectedKeysOrder = new List<string> { "P1", "fluxo1", "Piloto1", "dreno1", "RPM", "temp" };
            bool allFieldsValid = true;

            for (int i = 0; i < sensorReadings.Length; i++)
            {
                string reading = sensorReadings[i].Trim();
                string[] parts = reading.Split(':');

                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string valueString = parts[1].Trim();

                    if (i >= expectedKeysOrder.Count || key != expectedKeysOrder[i] || !_serialDataKeys.ContainsKey(key))
                    {
                        allFieldsValid = false;
                        Debug.WriteLine($"[PARSE_ERROR_KEY] Chave inesperada ou fora de ordem: '{key}' na posição {i}. Esperada: '{(i < expectedKeysOrder.Count ? expectedKeysOrder[i] : "FORA_DO_INDICE")}'");
                        break;
                    }
                    if (double.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
                    {
                        parsedValuesThisMessage[key] = numericValue;
                    }
                    else
                    {
                        allFieldsValid = false;
                        Debug.WriteLine($"[PARSE_ERROR_VALUE] Valor inválido para chave '{key}': '{valueString}'");
                        break;
                    }
                }
                else
                {
                    allFieldsValid = false;
                    Debug.WriteLine($"[PARSE_ERROR_PAIR] Par chave-valor malformado: '{reading}'");
                    break;
                }
            }

            if (allFieldsValid && parsedValuesThisMessage.Count == 6)
            {
                lock (readingsLock)
                {
                    foreach (var parsedEntry in parsedValuesThisMessage)
                    {
                        _currentNumericSensorReadings[parsedEntry.Key] = parsedEntry.Value;
                    }
                }
                Debug.WriteLine("[PARSE_SUCCESS] _currentNumericSensorReadings atualizado.");
            }
            else
            {
                // Log de erro de parsing apenas para Debug
                Debug.WriteLine($"[PARSE_ERROR_FINAL] Falha ao parsear todos os campos ou ordem/formato incorreto na mensagem: {cleanedData}");
            }
        }

        private void StartUpdateUiTimer()
        {
            if (_updateUiTimer == null)
            {
                _updateUiTimer = new Timer();
                _updateUiTimer.Interval = 150;
                _updateUiTimer.Tick += UpdateUiTimer_Tick;
            }
            if (!_updateUiTimer.Enabled)
            {
                _updateUiTimer.Start();
                LogHistoricalEvent("Timer de atualização da UI iniciado.", Color.Gray);
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
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
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

            UpdateTextBoxIfAvailable(sensor_bar_PL, readingsSnapshot.TryGetValue("Piloto1", out double pVal) ? pVal.ToString("F2", CultureInfo.InvariantCulture) : "N/A");
            UpdateTextBoxIfAvailable(sensor_psi_PL, readingsSnapshot.TryGetValue("Piloto1", out pVal) ? (pVal * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture) : "N/A");
            UpdateTextBoxIfAvailable(sensor_lpm_DR, readingsSnapshot.TryGetValue("dreno1", out double dVal) ? dVal.ToString("F2", CultureInfo.InvariantCulture) : "N/A");
            UpdateTextBoxIfAvailable(sensor_gpm_DR, readingsSnapshot.TryGetValue("dreno1", out dVal) ? (dVal * LPM_TO_GPM_USER_CONVERSION).ToString("F2", CultureInfo.InvariantCulture) : "N/A");
            UpdateTextBoxIfAvailable(sensor_Press_BAR, readingsSnapshot.TryGetValue("P1", out double prVal) ? prVal.ToString("F2", CultureInfo.InvariantCulture) : "N/A");
            UpdateTextBoxIfAvailable(sensor_Press_PSI, readingsSnapshot.TryGetValue("P1", out prVal) ? (prVal * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture) : "N/A");
            UpdateTextBoxIfAvailable(sensor_rotacao_RPM, readingsSnapshot.TryGetValue("RPM", out double rpmVal) ? rpmVal.ToString("F0", CultureInfo.InvariantCulture) : "N/A");
            UpdateTextBoxIfAvailable(sensor_Vazao_LPM, readingsSnapshot.TryGetValue("fluxo1", out double fVal) ? fVal.ToString("F2", CultureInfo.InvariantCulture) : "N/A");
            UpdateTextBoxIfAvailable(sensor_Vazao_GPM, readingsSnapshot.TryGetValue("fluxo1", out fVal) ? (fVal * LPM_TO_GPM_USER_CONVERSION).ToString("F2", CultureInfo.InvariantCulture) : "N/A");
            UpdateTextBoxIfAvailable(sensor_Temp_C, readingsSnapshot.TryGetValue("temp", out double tVal) ? tVal.ToString("F1", CultureInfo.InvariantCulture) : "N/A");
        }

        private void UpdateTextBoxIfAvailable(TextBox textBox, string value)
        {
            if (textBox != null && !textBox.IsDisposed)
            {
                textBox.Text = value;
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

            // Debug: Verificar se os dados estão chegando aqui
            // Debug.WriteLine($"[CHARTS] Tentando adicionar pontos. P1: {readingsSnapshot.GetValueOrDefault("P1", -1)}, fluxo1: {readingsSnapshot.GetValueOrDefault("fluxo1", -1)}");

            if (readingsSnapshot.TryGetValue("P1", out double pressaoBar) &&
                readingsSnapshot.TryGetValue("fluxo1", out double vazaoLpm) &&
                readingsSnapshot.TryGetValue("Piloto1", out double pilotagemRaw) &&
                readingsSnapshot.TryGetValue("dreno1", out double drenoLpm) &&
                readingsSnapshot.TryGetValue("RPM", out double rotacaoRpm) &&
                readingsSnapshot.TryGetValue("temp", out double temperaturaC))
            {
                // Chart 1
                Tela_BombasVM.Datapoint_Bar_Lpm? dataPoint = _viewModel.GetChartDataIfRotationConstant(
                      pressaoBar.ToString(CultureInfo.InvariantCulture),
                      vazaoLpm.ToString(CultureInfo.InvariantCulture),
                      rotacaoRpm.ToString(CultureInfo.InvariantCulture)
                   );
                if (chart1 != null && !chart1.IsDisposed && chart1.Series.IndexOf("Pre.x Vaz.") != -1 && dataPoint.HasValue)
                {
                    chart1.Series["Pre.x Vaz."].Points.AddXY(dataPoint.Value.FlowLpm, dataPoint.Value.PressureBar);
                    Debug.WriteLine($"[CHART1] Ponto adicionado: X={dataPoint.Value.FlowLpm}, Y={dataPoint.Value.PressureBar}");
                }

                // Chart 2
                if (chart2 != null && !chart2.IsDisposed && chart2.Series.IndexOf("Vaz.In.X Rot") != -1)
                {
                    chart2.Series["Vaz.In.X Rot"].Points.AddXY(rotacaoRpm, drenoLpm);
                }
                // Chart 3
                double pilotagemBar = pilotagemRaw * BAR_CONVERSION_PILOT;
                if (chart3 != null && !chart3.IsDisposed && chart3.Series.IndexOf("Vaz. x Pres.") != -1)
                {
                    chart3.Series["Vaz. x Pres."].Points.AddXY(pilotagemBar, drenoLpm);
                }
                // Chart 4
                double eficienciaVolumetrica = (vazaoLpm + drenoLpm > 0.001) ? (vazaoLpm / (vazaoLpm + drenoLpm)) * 100 : 0;
                eficienciaVolumetrica = Math.Max(0, Math.Min(100, eficienciaVolumetrica));
                double rendimentoGlobal = eficienciaVolumetrica * (1 - (pressaoBar / 400.0));
                rendimentoGlobal = Math.Max(0, Math.Min(eficienciaVolumetrica, rendimentoGlobal));

                if (chart4 != null && !chart4.IsDisposed && chart4.Series.IndexOf("Rend. Global") != -1) chart4.Series["Rend. Global"].Points.AddXY(vazaoLpm, rendimentoGlobal);
                if (chart4 != null && !chart4.IsDisposed && chart4.Series.IndexOf("Ef. Volumetrica") != -1) chart4.Series["Ef. Volumetrica"].Points.AddXY(vazaoLpm, eficienciaVolumetrica);

                // Chart 5
                if (chart5 != null && !chart5.IsDisposed && chart5.Series.IndexOf("Temperatura") != -1) chart5.Series["Temperatura"].Points.AddXY(_timeCounterSecondsRampa, temperaturaC);
                if (chart5 != null && !chart5.IsDisposed && chart5.Series.IndexOf("Pressão Rampa") != -1) chart5.Series["Pressão Rampa"].Points.AddXY(_timeCounterSecondsRampa, pressaoBar);
                if (chart5 != null && !chart5.IsDisposed && chart5.Series.IndexOf("Vazão Rampa") != -1) chart5.Series["Vazão Rampa"].Points.AddXY(_timeCounterSecondsRampa, vazaoLpm);

                // Chart 6
                if (chart6 != null && !chart6.IsDisposed && chart6.Series.IndexOf("Vazão Real") != -1) chart6.Series["Vazão Real"].Points.AddXY(rotacaoRpm, vazaoLpm);
            }
            else
            {
                // Debug.WriteLine("[CHARTS] Dados insuficientes para popular todos os gráficos.");
            }
        }

        private void ClearCharts()
        {
            if (this.IsDisposed || this.Disposing) return;
            var chartToCheck = chart1 ?? chart2 ?? chart3 ?? chart4 ?? chart5 ?? chart6;
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
            var charts = new List<Chart> { chart1, chart2, chart3, chart4, chart5, chart6 };
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
            if (HistoricalEvents.TextLength > 10000) HistoricalEvents.Clear();
            HistoricalEvents.SelectionStart = HistoricalEvents.TextLength;
            HistoricalEvents.SelectionLength = 0;
            HistoricalEvents.SelectionColor = color ?? SystemColors.ControlText;
            HistoricalEvents.AppendText($"{DateTime.Now:G}: {message}" + Environment.NewLine);
            HistoricalEvents.SelectionColor = HistoricalEvents.ForeColor;
            HistoricalEvents.ScrollToCaret();
        }
        #endregion

        #region FIM_TESTE
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
        #endregion

        #region RESET
        private void btnretornar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Tem certeza que deseja reiniciar o processo e retornar ao menu?\nTodos os dados coletados serão perdidos!",
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

            ResetNumericSensorReadings();

            Inicioteste = string.Empty;
            Fimteste = string.Empty;

            lock (serialBufferLock)
            {
                serialDataBuffer.Clear();
            }

            UpdateTextBoxes();

            if (dataGridView1 != null && !dataGridView1.IsDisposed)
            {
                if (dataGridView1.InvokeRequired)
                {
                    dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Rows.Clear(); });
                }
                else
                {
                    dataGridView1.Rows.Clear();
                }
            }
            if (visualizador != null && !visualizador.IsDisposed)
            {
                Action clearVisualizador = () => {
                    if (visualizador.DataSource is List<SensorData> list) list.Clear();
                    visualizador.DataSource = null;
                    if (dadosSensores != null) visualizador.DataSource = dadosSensores;
                    visualizador.Refresh();
                };
                if (visualizador.InvokeRequired) visualizador.Invoke(clearVisualizador); else clearVisualizador();
            }

            if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
            {
                circularProgressBar1.Value = 0;
                circularProgressBar1.Maximum = tempoCronometroDefinidoManualmente && valorDefinidoCronometro > 0 ? (valorDefinidoCronometro * 60) : 100;
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
        #endregion

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

        private void unidade_medidapilotagem1_Click(object sender, EventArgs e) { GravarDadoSensor("pilotagem", sensor_psi_PL?.Text, "psi"); }
        private void unidade_medidapilotagem2_Click(object sender, EventArgs e) { GravarDadoSensor("pilotagem", sensor_bar_PL?.Text, "bar"); }
        private void unidade_medidadreno1_Click(object sender, EventArgs e) { GravarDadoSensor("dreno", sensor_gpm_DR?.Text, "gpm"); }
        private void unidade_medidadreno2_Click(object sender, EventArgs e) { GravarDadoSensor("dreno", sensor_lpm_DR?.Text, "lpm"); }
        private void unidade_medidapressao1_Click(object sender, EventArgs e) { GravarDadoSensor("pressão", sensor_Press_PSI?.Text, "psi"); }
        private void unidade_medidapressao2_Click(object sender, EventArgs e) { GravarDadoSensor("pressão", sensor_Press_BAR?.Text, "bar"); }
        private void unidade_medidarota_Click(object sender, EventArgs e) { GravarDadoSensor("rotação", sensor_rotacao_RPM?.Text, "rpm"); }
        private void unidade_medidasvazao1_Click(object sender, EventArgs e) { GravarDadoSensor("vazão", sensor_Vazao_GPM?.Text, "gpm"); }
        private void unidade_medidasvazao2_Click(object sender, EventArgs e) { GravarDadoSensor("vazão", sensor_Vazao_LPM?.Text, "lpm"); }
        private void unidade_medidatemp_Click(object sender, EventArgs e) { GravarDadoSensor("temperatura", sensor_Temp_C?.Text, "celsius"); }


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
                if (visualizador != null && !visualizador.IsDisposed) _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private bool ValidarMinMaxCheckBoxLocal(CheckBox checkBox, TextBox minTextBox, TextBox maxTextBox, string nomeUnidade)
        {
            if (checkBox == null || minTextBox == null || maxTextBox == null) return false;

            if (!checkBox.Checked) return true;

            bool minOk = decimal.TryParse(minTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMinimo);
            bool maxOk = decimal.TryParse(maxTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMaximo);
            string erroMsg = null;

            if (!minOk || !maxOk) erroMsg = $"Valores de Mín/Máx para {nomeUnidade} devem ser numéricos.";
            else if (valorMinimo < 0 || valorMaximo < 0) erroMsg = $"Valores de Mín/Máx para {nomeUnidade} não podem ser < 0.";
            else if (valorMinimo > valorMaximo) erroMsg = $"Mínimo para {nomeUnidade} não pode ser > Máximo.";

            if (erroMsg != null)
            {
                checkBox.Checked = false;
                MessageBox.Show(this, erroMsg + $"\nA verificação {nomeUnidade} foi desativada.", $"Erro Validação - {nomeUnidade}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                minTextBox.Clear(); maxTextBox.Clear();
                if (!minOk || (minOk && valorMinimo < 0) || (minOk && maxOk && valorMinimo > valorMaximo)) minTextBox.Focus(); else maxTextBox.Focus();
                return false;
            }
            return true;
        }
        private void checkBox_psi_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as CheckBox, textBox9, textBox8, "PSI"); }
        private void checkBox_gpm_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as CheckBox, textBox14, textBox12, "GPM"); }
        private void checkBox_bar_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as CheckBox, textBox11, textBox10, "BAR"); }
        private void checkBox_rotacao_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as CheckBox, textBox18, textBox17, "Rotação (RPM)"); }
        private void checkBox_lpm_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as CheckBox, textBox16, textBox15, "LPM"); }
        private void checkBox_temperatura_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as CheckBox, textBox20, textBox19, "Temperatura (°C)"); }

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
                    LogHistoricalEvent("Tempo do cronômetro esgotado. Finalizando teste.", Color.Orange);
                    btnParar_Click(this, EventArgs.Empty);
                }
            }
        }

        // Assumindo que o TextBox para definir o tempo do cronômetro se chama 'textBoxTempoCronometro' no designer
        private void btnDefinir_Click(object sender, EventArgs e)
        {
            TextBox inputTempoCronometro = textBox1_tempoCronometro; // Use o nome correto do seu TextBox

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
                    MessageBox.Show(this, $"Tempo definido para {valorDefinidoCronometro} minutos.", "Cronômetro Definido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "Por favor, insira um valor numérico inteiro positivo válido em minutos.", "Erro de Entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "O cronômetro já está em execução. Limpe ou finalize o teste para definir um novo tempo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Assumindo que o botão para limpar o tempo do cronômetro se chama 'btnLimparTempoCronometro'
        private void btnLimpar_Click(object sender, EventArgs e)
        {
            TextBox inputTempoCronometro = textBox1_tempoCronometro; // Use o nome correto do seu TextBox

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
                MessageBox.Show(this, "O cronômetro está em execução. Finalize o teste para limpar o tempo definido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_gravar_Click(object sender, EventArgs e)
        {
            if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para gravar os dados.");
                return;
            }

            // Verificação do cronômetro para bloquear gravação de etapas
            if (tempoCronometroDefinidoManualmente && !cronometroIniciado) // Significa que o cronômetro já rodou e parou
            {
                MessageBox.Show("O tempo definido para o teste encerrou. Não é possível gravar novas etapas.", "Tempo Esgotado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }


            int maxEtapas = 1;
            if (trackBar1 != null && !trackBar1.IsDisposed)
            {
                maxEtapas = trackBar1.Maximum;
            }

            if (etapaAtual <= maxEtapas)
            {
                if (dataGridView1 != null && !dataGridView1.IsDisposed)
                {
                    dataGridViewLoad();
                    AtualizarDataGridView();
                }

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

                foreach (var reading in readingsSnapshot)
                {
                    string sensorSerialKey = reading.Key;
                    double sensorNumericValue = reading.Value;
                    _serialDataKeys.TryGetValue(sensorSerialKey, out string logicalSensorName);
                    sensorMapmedida.TryGetValue(sensorSerialKey, out string unit);

                    currentEtapaData.leituras.Add(new SensorData
                    {
                        Sensor = logicalSensorName ?? sensorSerialKey,
                        Valor = sensorNumericValue.ToString(CultureInfo.InvariantCulture),
                        Medidas = unit ?? "N/A"
                    });
                }
                _dadosColetados.Add(currentEtapaData);
                etapaAtual++;
                LogHistoricalEvent($"Dados gravados para Etapa {etapaAtual - 1}.", Color.Blue);
            }
            else
            {
                MessageBox.Show($"Limite de etapas ({maxEtapas}) atingido.", "Limite de Etapas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetButtonState(btngravar, false);
            }
        }
        public void dataGridViewLoad()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed) return;

            if (dataGridView1.ColumnCount == 0)
            {
                dataGridView1.Columns.Clear();
                dataGridView1.Columns.Add("Etapa", "Etapa");
                dataGridView1.Columns.Add("PilotagemPSI", "Pilotagem PSI");
                dataGridView1.Columns.Add("PilotagemBAR", "Pilotagem BAR");
                dataGridView1.Columns.Add("DrenoGPM", "Dreno GPM");
                dataGridView1.Columns.Add("DrenoLPM", "Dreno LPM");
                dataGridView1.Columns.Add("PressaoPSI", "Pressão PSI");
                dataGridView1.Columns.Add("PressaoBAR", "Pressão BAR");
                dataGridView1.Columns.Add("RotacaoRPM", "Rotação RPM");
                dataGridView1.Columns.Add("VazaoGPM", "Vazão GPM");
                dataGridView1.Columns.Add("VazaoLPM", "Vazão LPM");
                dataGridView1.Columns.Add("TemperaturaC", "Temperatura °C");
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        public void AtualizarDataGridView()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed) return;

            Dictionary<string, double> readingsSnapshot;
            lock (readingsLock)
            {
                readingsSnapshot = new Dictionary<string, double>(_currentNumericSensorReadings);
            }

            readingsSnapshot.TryGetValue("Piloto1", out double piloto1Value);
            readingsSnapshot.TryGetValue("dreno1", out double dreno1Value);
            readingsSnapshot.TryGetValue("P1", out double p1Value);
            readingsSnapshot.TryGetValue("RPM", out double rpmValue);
            readingsSnapshot.TryGetValue("fluxo1", out double fluxo1Value);
            readingsSnapshot.TryGetValue("temp", out double tempValue);

            string pilotagemPsi = (piloto1Value * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            string pilotagemBar = piloto1Value.ToString("F2", CultureInfo.InvariantCulture);
            string drenoGpm = (dreno1Value * LPM_TO_GPM_USER_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            string drenoLpm = dreno1Value.ToString("F2", CultureInfo.InvariantCulture);
            string pressaoPsi = (p1Value * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            string pressaoBar = p1Value.ToString("F2", CultureInfo.InvariantCulture);
            string rotacaoRpm = rpmValue.ToString("F0", CultureInfo.InvariantCulture);
            string vazaoGpm = (fluxo1Value * LPM_TO_GPM_USER_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            string vazaoLpm = fluxo1Value.ToString("F2", CultureInfo.InvariantCulture);
            string temperaturaCelsius = tempValue.ToString("F1", CultureInfo.InvariantCulture);

            string[] novaLinha = new string[]
            {
                $"Etapa {etapaAtual}", pilotagemPsi, pilotagemBar, drenoGpm, drenoLpm,
                pressaoPsi, pressaoBar, rotacaoRpm, vazaoGpm, vazaoLpm, temperaturaCelsius
            };

            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Rows.Add(novaLinha); });
            }
            else
            {
                dataGridView1.Rows.Add(novaLinha);
            }
        }
        private void btnrelatoriobomba_Click(object sender, EventArgs e)
        {
            Realatoriobase relatorioForm = new Realatoriobase();
            // relatorioForm.SetDadosEnsaio(_dadosColetados, Inicioteste, Fimteste, 
            //                                       TbNomeCliente, TbNomeBomba, TbOrdemServico);
            relatorioForm.Show();
        }
    }
}
