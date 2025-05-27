using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
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
        // Estrutura para mapear parâmetros do DataGridView estático
        private struct ParameterRowInfo
        {
            public string DisplayName { get; } // Nome a ser exibido na primeira coluna
            public string SourceTextBoxName { get; } // Nome do TextBox de onde o dado virá
            public string FormattingType { get; } // "float", "temp", "rpm"

            public ParameterRowInfo(string displayName, string sourceTextBoxName, string formattingType)
            {
                DisplayName = displayName;
                SourceTextBoxName = sourceTextBoxName;
                FormattingType = formattingType;
            }
        }

        // --- INÍCIO: Nova estrutura para mapeamento de sensores, TextBoxes e calibração ---
        private struct SensorDisplayInfo
        {
            public string SerialKey { get; }         // Chave no pacote serial (ex: "HA1", "TEM")
            public string TextBoxName { get; }       // Nome do controle TextBox no formulário (pode ser null se não houver display)
            public string DisplayFormat { get; }     // Formato para exibição (ex: "F2" para 2 casas decimais, "F0" para inteiro)
            public string ChartDataKey { get; }      // Chave a ser usada para _currentNumericSensorReadings e gráficos

            public SensorDisplayInfo(string serialKey, string textBoxName, string displayFormat, string chartDataKey = null)
            {
                SerialKey = serialKey;
                TextBoxName = textBoxName;
                DisplayFormat = displayFormat;
                ChartDataKey = chartDataKey ?? serialKey; // Por padrão, usa a SerialKey para os dados numéricos
            }
        }
        private List<SensorDisplayInfo> _sensorDisplayMap;
        private Dictionary<string, double> _calibrationCoefficients = new Dictionary<string, double>();
        // --- FIM: Nova estrutura ---

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
        private readonly object readingsLock = new object(); // Usado para _latestRawSensorData e _currentNumericSensorReadings

        private Dictionary<string, string> _latestRawSensorData = new Dictionary<string, string>(); // Armazena os últimos valores brutos como string
        private Dictionary<string, double> _currentNumericSensorReadings = new Dictionary<string, double>(); // Armazena valores numéricos (calibrados)


        private const double BAR_TO_PSI_CONVERSION = 14.5;
        private const double LPM_TO_GPM_USER_CONVERSION = 0.264172; // Conversão padrão de LPM para GPM (US)
        private const double BAR_CONVERSION_PILOT = 1.705; // Mantido para conversão específica do gráfico 3 para "PL1"

        private int _timeCounterSecondsRampa = 0;
        private bool _fechamentoControladoPeloUsuario = false;
        private bool _isMonitoring = false;
        public string Inicioteste;
        public string Fimteste;
        private int etapaAtual = 1; // Controla a coluna da etapa atual (1 a 7) para o dataGridView1
        public string TbNomeCliente { get; set; }
        public string TbNomeBomba { get; set; }
        public string TbOrdemServico { get; set; }
        private int valorDefinidoCronometro;
        private bool cronometroIniciado = false;
        private bool tempoCronometroDefinidoManualmente = false;
        private Dictionary<string, string> sensorControlMap;
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
                _synchronizerNivel = new TextBoxTrackBarSynchronizer(tb2, tb1, 1, 7); // Assumindo que o nível vai de 1 a 7 etapas
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
            _updateUiTimer.Interval = 150; // Intervalo de atualização da UI (mantido)
            _updateUiTimer.Tick += UpdateUiTimer_Tick; // ATIVADO O TICK HANDLER

            InitializeAllCharts();
            InitializeChartClickEvents();

            SetButtonState(btniniciarteste, true);
            SetButtonState(btngravar, false);
            SetButtonState(bntFinalizar, false);
            SetButtonState(btnreset, false);
            SetButtonState(btnrelatoriobomba, false);
            InitializeSensorConfigurationSystem();

            InitializeSensorDisplayMap(); // NOVO: Inicializa o mapeamento de sensores
            LoadCalibrationCoefficients(); // NOVO: Carrega/define coeficientes de calibração
        }

        // --- INÍCIO: Mapeamento de Sensores, Calibração e Formato de Display ---
        private void InitializeSensorDisplayMap()
        {
            _sensorDisplayMap = new List<SensorDisplayInfo>
            {
                // SerialKey, TextBoxName, DisplayFormat, ChartDataKey (se diferente de SerialKey)
                new SensorDisplayInfo("HA1", "sensor_HA1", "F2"),
                new SensorDisplayInfo("HA2", "sensor_HA2", "F2"),
                new SensorDisplayInfo("HB1", "sensor_HB1", "F2"),
                new SensorDisplayInfo("HB2", "sensor_HB2", "F2"),

                new SensorDisplayInfo("MA1", "sensor_MA1", "F2", "MA1"), // ChartDataKey explícito
                new SensorDisplayInfo("MA2", "sensor_MA2", "F2", "MA2"),
                new SensorDisplayInfo("MB1", "sensor_MB1", "F2", "MB1"),
                new SensorDisplayInfo("MB2", "sensor_MB2", "F2", "MB2"),

                new SensorDisplayInfo("TEM", "sensor_CELSUS", "F1", "TEM"),
                new SensorDisplayInfo("ROT", "sensor_RPM", "F0", "ROT"), // Rotação como inteiro para display

                new SensorDisplayInfo("DR1", "sensor_DR1", "F2", "DR1"),
                new SensorDisplayInfo("DR2", "sensor_DR2", "F2", "DR2"),
                new SensorDisplayInfo("DR3", null, "F2", "DR3"), // Sem TextBox, mas pode ser usado em _currentNumericSensorReadings
                new SensorDisplayInfo("DR4", null, "F2", "DR4"), // Sem TextBox

                new SensorDisplayInfo("PL1", "sensor_P1", "F2", "PL1"),
                new SensorDisplayInfo("PL2", "sensor_P2", "F2", "PL2"),
                new SensorDisplayInfo("PL3", "sensor_P3", "F2", "PL3"),
                new SensorDisplayInfo("PL4", "sensor_P4", "F2", "PL4"),

                new SensorDisplayInfo("PR1", "sensor_PR1", "F2", "PR1"),
                new SensorDisplayInfo("PR2", "sensor_PR2", "F2", "PR2"),
                new SensorDisplayInfo("PR3", "sensor_PR3", "F2", "PR3"),
                new SensorDisplayInfo("PR4", "sensor_PR4", "F2", "PR4"),

                new SensorDisplayInfo("VZ1", "sensor_V1", "F2", "VZ1"),
                new SensorDisplayInfo("VZ2", "sensor_V2", "F2", "VZ2"),
                new SensorDisplayInfo("VZ3", "sensor_V3", "F2", "VZ3"),
                new SensorDisplayInfo("VZ4", "sensor_V4", "F2", "VZ4")
            };
        }

        private void LoadCalibrationCoefficients()
        {
            // Exemplo - Carregue de um arquivo de configuração ou UI em uma aplicação real
            _calibrationCoefficients.Clear(); // Limpa coeficientes antigos se recarregado
            _calibrationCoefficients["HA1"] = 1.0;
            _calibrationCoefficients["HA2"] = 1.0;
            _calibrationCoefficients["HB1"] = 1.0;
            _calibrationCoefficients["HB2"] = 1.0;
            _calibrationCoefficients["MA1"] = 1.02; // Ex: Calibrar MA1 em +2%
            _calibrationCoefficients["MA2"] = 1.0;
            _calibrationCoefficients["MB1"] = 1.0;
            _calibrationCoefficients["MB2"] = 1.0;
            _calibrationCoefficients["TEM"] = 0.99; // Ex: Calibrar TEM em -1%
            _calibrationCoefficients["ROT"] = 1.0;
            _calibrationCoefficients["DR1"] = 1.01;
            _calibrationCoefficients["DR2"] = 1.0;
            _calibrationCoefficients["DR3"] = 1.0; // Mesmo sem TextBox, pode ter calibração
            _calibrationCoefficients["DR4"] = 1.0;
            _calibrationCoefficients["PL1"] = 1.05; // Ex: Calibrar PL1 em +5%
            _calibrationCoefficients["PL2"] = 1.0;
            _calibrationCoefficients["PL3"] = 1.0;
            _calibrationCoefficients["PL4"] = 1.0;
            _calibrationCoefficients["PR1"] = 1.0;
            _calibrationCoefficients["PR2"] = 1.0;
            _calibrationCoefficients["PR3"] = 1.0;
            _calibrationCoefficients["PR4"] = 1.0;
            _calibrationCoefficients["VZ1"] = 0.98; // Ex: Calibrar VZ1 em -2%
            _calibrationCoefficients["VZ2"] = 1.0;
            _calibrationCoefficients["VZ3"] = 1.0;
            _calibrationCoefficients["VZ4"] = 1.0;
            // Sensores não listados aqui ou com coeficiente 0 ou 1.0 não terão sua leitura alterada pela calibração.
        }

        private double? GetCalibrationCoefficient(string sensorKey)
        {
            if (_calibrationCoefficients.TryGetValue(sensorKey, out double coeff))
            {
                // "caso seja nulos, vazios ou 0 exibe os dados da forma que foram recebidos"
                // Um coeficiente de 0 efetivamente zeraria o dado, o que pode não ser o desejado.
                // Se for 0, retornamos null para indicar "usar valor raw".
                if (coeff != 0.0)
                {
                    return coeff;
                }
            }
            return null; // Coeficiente não encontrado ou é 0.0
        }
        // --- FIM: Mapeamento de Sensores ---

        private void InitializeChartClickEvents()
        {
            if (this.chart1 != null && !this.chart1.IsDisposed) this.chart1.Click += new System.EventHandler(this.ShowChartDetailModal_Click);
            if (this.chart2 != null && !this.chart2.IsDisposed) this.chart2.Click += new System.EventHandler(this.ShowChartDetailModal_Click);
            if (this.chart3 != null && !this.chart3.IsDisposed) this.chart3.Click += new System.EventHandler(this.ShowChartDetailModal_Click);
        }

        private void ShowChartDetailModal_Click(object sender, EventArgs e)
        {
            Chart clickedChart = sender as Chart;
            if (clickedChart != null && clickedChart.Series.Count > 0 && clickedChart.ChartAreas.Count > 0)
            {
                string chartTitle = "Detalhes do Gráfico";
                if (clickedChart.Titles.Count > 0 && !string.IsNullOrEmpty(clickedChart.Titles[0].Text))
                {
                    chartTitle = clickedChart.Titles[0].Text;
                }

                
                ChartDetailForm detailForm = new ChartDetailForm(clickedChart.Series, chartTitle, clickedChart.ChartAreas[0], this.Icon);

                
                detailForm.Show(this);
            }
            else if (clickedChart != null)
            {
                MessageBox.Show("Não há dados para exibir em detalhe para este gráfico.", "Gráfico Vazio", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void InitializeSensorConfigurationSystem()
        {
            sensorToolTip = new ToolTip(); // Inicializa o ToolTip
            InitializeAllReadingsDataFromSpec();
            InitializeSensorControlsMap();
            currentConfiguration = new ConfigurationResult();

            if (btnConfigCircuitoAberto != null)
            {
                defaultButtonColor = btnConfigCircuitoAberto.BackColor;
            }
            else
            {
                defaultButtonColor = SystemColors.Control; // Padrão
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
                    Debug.WriteLine($"Controles não encontrados para o ID da Leitura: {rd.Id} (TextBox: {rd.ValueTextBoxName}, RótuloUnidade: {rd.UnitLabelName})");
                }
            }
        }

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
                if (dr == DialogResult.Yes) ResetActiveTestConfiguration();
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
            Dictionary<GroupBox, List<(TextBox valueTb, Label unitL)>> groupBoxReadingsMap = new Dictionary<GroupBox, List<(TextBox valueTb, Label unitL)>>();

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
                                groupBoxReadingsMap[parentGroup] = new List<(TextBox valueTb, Label unitL)>();
                            groupBoxReadingsMap[parentGroup].Add(controls);
                        }
                    }
                }
            }
            foreach (var kvp in groupBoxReadingsMap) LayoutControlsInGroup(kvp.Key, kvp.Value);
            var allGroupBoxesOnForm = this.Controls.OfType<Panel>()
                                     .Where(p => p.Name == "panel1")
                                     .SelectMany(p => p.Controls.OfType<GroupBox>())
                                     .Concat(this.Controls.OfType<GroupBox>());
            foreach (var groupControl in allGroupBoxesOnForm)
            {
                if (!groupBoxReadingsMap.ContainsKey(groupControl))
                {
                    foreach (var rd in allReadingsData)
                        if (sensorControlsMap.TryGetValue(rd.Id, out var sensorPair))
                            if (FindParentGroupBox(sensorPair.valueTextBox) == groupControl)
                            {
                                sensorPair.valueTextBox.Visible = false;
                                sensorPair.unitLabel.Visible = false;
                            }
                }
            }
        }

        private GroupBox FindParentGroupBox(Control control)
        {
            Control current = control;
            while (current != null) { if (current is GroupBox gb) return gb; current = current.Parent; }
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
            if (!itemsToLayout.Any()) { groupBox.ResumeLayout(true); return; }

            int controlRowHeight = 35; if (itemsToLayout.Any()) controlRowHeight = itemsToLayout.First().valueTb.Height;
            Rectangle displayRect = groupBox.DisplayRectangle;
            int yPosForRow = displayRect.Y; if (displayRect.Height > controlRowHeight) yPosForRow += (displayRect.Height - controlRowHeight) / 2;
            yPosForRow = Math.Max(displayRect.Y, yPosForRow);

            const int internalHorizontalPadding = 5; int availableContentWidth = displayRect.Width - (2 * internalHorizontalPadding);
            const int spacingBetweenSensorItemsOnSameRow = 15; const int spacingWithinSensorItem = 3;
            int totalWidthOfItemsInThisRow = 0;
            for (int i = 0; i < itemsToLayout.Count; i++)
            {
                var itemTuple = itemsToLayout[i]; itemTuple.unitL.AutoSize = true;
                Size unitLabelActualSize = TextRenderer.MeasureText(itemTuple.unitL.Text, itemTuple.unitL.Font);
                totalWidthOfItemsInThisRow += itemTuple.valueTb.Width + spacingWithinSensorItem + unitLabelActualSize.Width;
                if (i < itemsToLayout.Count - 1) totalWidthOfItemsInThisRow += spacingBetweenSensorItemsOnSameRow;
            }
            int startX = displayRect.X + internalHorizontalPadding;
            if (availableContentWidth > totalWidthOfItemsInThisRow) startX += (availableContentWidth - totalWidthOfItemsInThisRow) / 2;
            int currentXInRow = startX;
            for (int i = 0; i < itemsToLayout.Count; i++)
            {
                var currentItemTuple = itemsToLayout[i]; TextBox valueTextBox = currentItemTuple.valueTb; Label unitLabel = currentItemTuple.unitL;
                valueTextBox.Visible = true; unitLabel.Visible = true; unitLabel.AutoSize = true;
                int tbY = yPosForRow; int lblY = tbY + (valueTextBox.Height - unitLabel.PreferredHeight) / 2;
                tbY = Math.Max(displayRect.Y, tbY); lblY = Math.Max(displayRect.Y, lblY);
                valueTextBox.Location = new Point(currentXInRow, tbY);
                unitLabel.Location = new Point(valueTextBox.Right + spacingWithinSensorItem, lblY);
                currentXInRow = unitLabel.Right + spacingBetweenSensorItemsOnSameRow;
            }
            groupBox.ResumeLayout(true);
        }

        private void ResetActiveTestConfiguration()
        {
            currentConfiguration = new ConfigurationResult(); UpdateTelaBombasDisplay();
            if (currentlyActiveTestButton != null)
            {
                currentlyActiveTestButton.BackColor = defaultButtonColor;
                currentlyActiveTestButton.ForeColor = SystemColors.ControlText;
                currentlyActiveTestButton.Tag = null;
            }
            currentlyActiveTestButton = null;
            var configButtons = new[] { btnConfigCircuitoAberto, btnConfigCircuitoFechado, btnConfigEngrenagem };
            foreach (Button btn in configButtons) if (btn != null) { btn.Enabled = true; btn.BackColor = defaultButtonColor; btn.ForeColor = SystemColors.ControlText; }
            if (btniniciarteste != null) btniniciarteste.Enabled = false;
        }

        private void SetInitialButtonStates()
        {
            var configButtons = new[] { btnConfigCircuitoAberto, btnConfigCircuitoFechado, btnConfigEngrenagem };
            foreach (Button btn in configButtons) if (btn != null) { btn.Enabled = true; btn.BackColor = defaultButtonColor; btn.ForeColor = SystemColors.ControlText; }
            if (btniniciarteste != null) btniniciarteste.Enabled = false;
        }

        public string GetConfiguredSensorValue(string sensorId)
        {
            if (currentConfiguration.SelectedReadingIds.Contains(sensorId) &&
                sensorControlsMap.TryGetValue(sensorId, out var controls)) return controls.valueTextBox.Text;
            return "N/D";
        }

        public string GetConfiguredSensorUnit(string sensorId)
        {
            if (currentConfiguration.SelectedReadingIds.Contains(sensorId) &&
               sensorControlsMap.TryGetValue(sensorId, out var controls)) return controls.unitLabel.Text;
            return "";
        }

        private void btnLimparConfigTelaBombas_Click(object sender, EventArgs e) { ResetActiveTestConfiguration(); }

        // --- FIM: Métodos para o sistema de configuração de ensaio ---

        private void InitializeStaticDataGridViewParameters()
        {
            // Os nomes em SourceTextBoxName devem corresponder aos nomes dos TextBoxes que serão atualizados.
            // Removidos os "_dup" pois não há mapeamento serial claro para eles.
            // Se MA1/MA2/etc. forem para P1/P2, o mapeamento é indireto via o TextBox correspondente.
            _staticDataGridViewParameters = new List<ParameterRowInfo>
            {
                new ParameterRowInfo("P1", "sensor_MA1", "float"), // DGV "P1" usa valor do TextBox "sensor_MA1" (Serial: MA1)
                new ParameterRowInfo("P2", "sensor_MA2", "float"), // DGV "P2" usa valor do TextBox "sensor_MA2" (Serial: MA2)
                new ParameterRowInfo("P3", "sensor_MB1", "float"), // DGV "P3" usa valor do TextBox "sensor_MB1" (Serial: MB1)
                new ParameterRowInfo("P4", "sensor_MB2", "float"), // DGV "P4" usa valor do TextBox "sensor_MB2" (Serial: MB2)
                new ParameterRowInfo("Pressão Piloto 1", "sensor_P1", "float"), // DGV "Pressão Piloto 1" usa "sensor_P1" (Serial: PL1)
                new ParameterRowInfo("Pressão Piloto 2", "sensor_P2", "float"), // DGV "Pressão Piloto 2" usa "sensor_P2" (Serial: PL2)
                new ParameterRowInfo("Pressão Piloto 3", "sensor_P3", "float"), // DGV "Pressão Piloto 3" usa "sensor_P3" (Serial: PL3)
                new ParameterRowInfo("Pressão Piloto 4", "sensor_P4", "float"), // DGV "Pressão Piloto 4" usa "sensor_P4" (Serial: PL4)
                new ParameterRowInfo("Vazão 1", "sensor_V1", "float"),         // DGV "Vazão 1" usa "sensor_V1" (Serial: VZ1)
                new ParameterRowInfo("Vazão 2", "sensor_V2", "float"),         // DGV "Vazão 2" usa "sensor_V2" (Serial: VZ2)
                new ParameterRowInfo("Vazão 3", "sensor_V3", "float"),         // DGV "Vazão 3" usa "sensor_V3" (Serial: VZ3)
                new ParameterRowInfo("Vazão 4", "sensor_V4", "float"),         // DGV "Vazão 4" usa "sensor_V4" (Serial: VZ4)
                new ParameterRowInfo("Dreno 1", "sensor_DR1", "float"),        // DGV "Dreno 1" usa "sensor_DR1" (Serial: DR1)
                new ParameterRowInfo("Dreno 2", "sensor_DR2", "float"),        // DGV "Dreno 2" usa "sensor_DR2" (Serial: DR2)
                new ParameterRowInfo("Hidro A1", "sensor_HA1", "float"),       // DGV "Hidro A1" usa "sensor_HA1" (Serial: HA1)
                new ParameterRowInfo("Hidro A2", "sensor_HA2", "float"),
                new ParameterRowInfo("Hidro B1", "sensor_HB1", "float"),
                new ParameterRowInfo("Hidro B2", "sensor_HB2", "float"),
                // "Motor Ax" usará os mesmos TextBoxes de Px se forem os mesmos valores
                new ParameterRowInfo("Motor A1", "sensor_MA1", "float"), // Assumindo que Motor A1 é o mesmo que P1
                new ParameterRowInfo("Motor A2", "sensor_MA2", "float"), // Assumindo que Motor A2 é o mesmo que P2
                new ParameterRowInfo("Motor B1", "sensor_MB1", "float"),
                new ParameterRowInfo("Motor B2", "sensor_MB2", "float"),
                new ParameterRowInfo("Temperatura", "sensor_CELSUS", "temp"),  // DGV "Temperatura" usa "sensor_CELSUS" (Serial: TEM)
                new ParameterRowInfo("Rotação", "sensor_RPM", "rpm")           // DGV "Rotação" usa "sensor_RPM" (Serial: ROT)
            };
        }


        // SetupStaticDataGridView - SEM ALTERAÇÕES SIGNIFICATIVAS NA LÓGICA DE DIMENSIONAMENTO
        // ... (código existente de SetupStaticDataGridView)
        // MANTIDO IGUAL AO FORNECIDO NO PROMPT ANTERIOR
        private void SetupStaticDataGridView()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed) return;
            dataGridView1.SuspendLayout();
            dataGridView1.AllowUserToAddRows = false; dataGridView1.ReadOnly = true; dataGridView1.RowHeadersVisible = false;
            dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None; dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.Rows.Clear(); dataGridView1.Columns.Clear();
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font ?? SystemFonts.DefaultFont, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DataGridViewTextBoxColumn parametroCol = new DataGridViewTextBoxColumn { Name = "Parametro", HeaderText = "Parâmetro", DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft, Padding = new Padding(5, 2, 5, 2) } };
            dataGridView1.Columns.Add(parametroCol);
            int numberOfEtapaColumns = 7; for (int i = 1; i <= numberOfEtapaColumns; i++) dataGridView1.Columns.Add($"Etapa{i}", $"Etapa {i}");
            if (_staticDataGridViewParameters != null) foreach (var paramInfo in _staticDataGridViewParameters) dataGridView1.Rows.Add(paramInfo.DisplayName);
            if (dataGridView1.Rows.Count == 0 || dataGridView1.Columns.Count == 0) { dataGridView1.ResumeLayout(); return; }
            int availableClientWidth = dataGridView1.ClientSize.Width; int availableClientHeight = dataGridView1.ClientSize.Height; int maxParametroTextWidth = 0;
            using (Graphics g = dataGridView1.CreateGraphics())
            {
                Font paramCellFont = parametroCol.DefaultCellStyle.Font ?? dataGridView1.DefaultCellStyle.Font ?? dataGridView1.Font ?? SystemFonts.DefaultFont; Font headerFont = dataGridView1.ColumnHeadersDefaultCellStyle.Font ?? dataGridView1.Font ?? SystemFonts.DefaultFont;
                int headerTextWidth = TextRenderer.MeasureText(g, parametroCol.HeaderText, headerFont).Width; maxParametroTextWidth = headerTextWidth;
                if (_staticDataGridViewParameters != null) foreach (var paramInfo in _staticDataGridViewParameters) { int itemTextWidth = TextRenderer.MeasureText(g, paramInfo.DisplayName, paramCellFont).Width; if (itemTextWidth > maxParametroTextWidth) maxParametroTextWidth = itemTextWidth; }
            }
            int parametroColHorizontalPadding = parametroCol.DefaultCellStyle.Padding.Horizontal; int calculatedParametroColWidth = maxParametroTextWidth + parametroColHorizontalPadding + 10;
            int minParametroColWidth = 100; parametroCol.Width = Math.Max(minParametroColWidth, calculatedParametroColWidth);
            int minWidthPerEtapaCol = 30; if (parametroCol.Width > availableClientWidth - (numberOfEtapaColumns * minWidthPerEtapaCol)) { parametroCol.Width = Math.Max(minParametroColWidth, availableClientWidth - (numberOfEtapaColumns * minWidthPerEtapaCol)); if (parametroCol.Width < minParametroColWidth) parametroCol.Width = minParametroColWidth; if (parametroCol.Width < 0) parametroCol.Width = 0; }
            int remainingWidthForEtapas = availableClientWidth - parametroCol.Width; if (remainingWidthForEtapas < 0) remainingWidthForEtapas = 0;
            if (numberOfEtapaColumns > 0 && remainingWidthForEtapas > 0)
            {
                int baseEtapaWidth = remainingWidthForEtapas / numberOfEtapaColumns; int remainderEtapaWidth = remainingWidthForEtapas % numberOfEtapaColumns;
                for (int i = 0; i < numberOfEtapaColumns; i++) { DataGridViewColumn etapaCol = dataGridView1.Columns[i + 1]; etapaCol.Width = baseEtapaWidth + (i < remainderEtapaWidth ? 1 : 0); if (etapaCol.Width < minWidthPerEtapaCol && baseEtapaWidth >= minWidthPerEtapaCol) etapaCol.Width = minWidthPerEtapaCol; else if (etapaCol.Width < 0) etapaCol.Width = 0; }
            }
            else if (numberOfEtapaColumns > 0) { for (int i = 0; i < numberOfEtapaColumns; i++) dataGridView1.Columns[i + 1].Width = Math.Max(0, Math.Min(minWidthPerEtapaCol, availableClientWidth / (numberOfEtapaColumns + 1))); }
            int currentTotalColWidth = 0; foreach (DataGridViewColumn col in dataGridView1.Columns) if (col.Visible) currentTotalColWidth += col.Width;
            if (currentTotalColWidth < availableClientWidth && dataGridView1.Columns.Count > 1) { int slack = availableClientWidth - currentTotalColWidth; dataGridView1.Columns[dataGridView1.Columns.Count - 1].Width += slack; }
            else if (currentTotalColWidth > availableClientWidth && dataGridView1.Columns.Count > 1)
            {
                int overflow = currentTotalColWidth - availableClientWidth;
                for (int i = dataGridView1.Columns.Count - 1; i > 0 && overflow > 0; i--) { DataGridViewColumn etapaCol = dataGridView1.Columns[i]; int reduction = Math.Min(overflow, etapaCol.Width - minWidthPerEtapaCol); if (reduction > 0) { etapaCol.Width -= reduction; overflow -= reduction; } }
            }
            int columnHeadersHeight = dataGridView1.ColumnHeadersVisible ? dataGridView1.ColumnHeadersHeight : 0; int availableHeightForRows = availableClientHeight - columnHeadersHeight; if (availableHeightForRows < 0) availableHeightForRows = 0;
            int numberOfRows = dataGridView1.Rows.Count;
            if (numberOfRows > 0 && availableHeightForRows > 0)
            {
                Font rowFont = dataGridView1.DefaultCellStyle.Font ?? dataGridView1.Font ?? SystemFonts.DefaultFont; int cellVerticalPadding = dataGridView1.DefaultCellStyle.Padding.Vertical; int minPracticalRowHeight = rowFont.Height + cellVerticalPadding + 4;
                int baseRowHeight = availableHeightForRows / numberOfRows; int remainderRowHeight = availableHeightForRows % numberOfRows; int actualRowHeight = Math.Max(minPracticalRowHeight, baseRowHeight); int totalAppliedRowHeight = 0;
                for (int i = 0; i < numberOfRows; i++) { if (!dataGridView1.Rows[i].IsNewRow) { int heightForRow = actualRowHeight; if (baseRowHeight >= minPracticalRowHeight && i < remainderRowHeight) heightForRow++; dataGridView1.Rows[i].Height = heightForRow; totalAppliedRowHeight += heightForRow; } }
                if (totalAppliedRowHeight < availableHeightForRows && numberOfRows > 0) { int diff = availableHeightForRows - totalAppliedRowHeight; for (int i = 0; i < Math.Min(diff, numberOfRows); i++) dataGridView1.Rows[i].Height++; }
                else if (totalAppliedRowHeight > availableHeightForRows && numberOfRows > 0) { int diff = totalAppliedRowHeight - availableHeightForRows; for (int i = 0; i < Math.Min(diff, numberOfRows); i++) if (dataGridView1.Rows[i].Height > minPracticalRowHeight) dataGridView1.Rows[i].Height--; }
                dataGridView1.RowTemplate.Height = (numberOfRows > 0 && availableHeightForRows > 0) ? Math.Max(minPracticalRowHeight, availableHeightForRows / numberOfRows) : minPracticalRowHeight;
            }
            else if (numberOfRows > 0) { int minHeight = dataGridView1.Font?.Height ?? 10; foreach (DataGridViewRow row in dataGridView1.Rows) if (!row.IsNewRow) row.Height = Math.Max(1, minHeight / 2); dataGridView1.RowTemplate.Height = Math.Max(1, minHeight / 2); }
            dataGridView1.ResumeLayout(true);
        }


        private void SetButtonState(Control button, bool enabled)
        {
            if (button != null && !button.IsDisposed)
            {
                if (button.InvokeRequired) button.BeginInvoke((MethodInvoker)delegate { button.Enabled = enabled; });
                else button.Enabled = enabled;
                if (button is CuoreUI.Controls.cuiButton cuiButton) { /* Adapte conforme necessidade */ }
            }
        }


        // InitializeAllCharts, InitializeChart, InitializeChartWithSecondaryAxis - SEM ALTERAÇÕES DIRETAS NA LÓGICA INTERNA
        // Os títulos dos eixos e séries podem precisar de ajuste se os dados mapeados mudarem significativamente.
        private void InitializeAllCharts()
        {
            InitializeChart(chart1, "Vazão (LPM)", "Pressão (BAR)", "Curva de Desempenho Principal",
                new List<SeriesConfig> { new SeriesConfig("Pre.x Vaz.", SeriesChartType.FastLine, Color.Blue) },
                0, 100, 0, 300);
            InitializeChart(chart2, "Rotação (RPM)", "Dreno (LPM)", "Vazamento Interno / Dreno",
                new List<SeriesConfig> { new SeriesConfig("Vaz.In.X Rot", SeriesChartType.FastLine, Color.Red) },
                0, 3000, 0, 100);
            InitializeChart(chart3, "Pressão Piloto (BAR)", "Dreno (LPM)", "Curva de Pressão Piloto", // Ajustado título do eixo X
                new List<SeriesConfig> { new SeriesConfig("Vaz. x Pres.Piloto", SeriesChartType.FastLine, Color.Orange) }, // Ajustado nome da série
                0, 50, 0, 10);
        }
        private void InitializeChart(Chart chart, string xAxisTitle, string yAxisTitle, string chartTitle, List<SeriesConfig> seriesConfigs, double xMin, double xMax, double yMin, double yMax) { if (chart == null || chart.IsDisposed) return; chart.Series.Clear(); chart.ChartAreas.Clear(); ChartArea chartArea = new ChartArea(chartTitle.Replace(" ", "") + "Area"); chartArea.AxisX.Title = xAxisTitle; chartArea.AxisX.Minimum = xMin; if (xMax > xMin) chartArea.AxisX.Maximum = xMax; else chartArea.AxisX.Maximum = xMin + 1; chartArea.AxisX.LabelStyle.Format = "F0"; chartArea.AxisY.Title = yAxisTitle; chartArea.AxisY.Minimum = yMin; if (yMax > yMin) chartArea.AxisY.Maximum = yMax; else chartArea.AxisY.Maximum = yMin + 1; chartArea.AxisY.LabelStyle.Format = "F0"; chartArea.CursorX.IsUserEnabled = true; chartArea.CursorX.IsUserSelectionEnabled = true; chartArea.CursorY.IsUserEnabled = true; chartArea.CursorY.IsUserSelectionEnabled = true; chartArea.AxisX.ScaleView.Zoomable = true; chartArea.AxisY.ScaleView.Zoomable = true; chart.ChartAreas.Add(chartArea); foreach (var sc in seriesConfigs) { Series series = new Series(sc.Name) { ChartType = sc.Type, Color = sc.Color, BorderWidth = 2 }; chart.Series.Add(series); } chart.Titles.Clear(); chart.Titles.Add(new Title(chartTitle, Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black)); }
        private void InitializeChartWithSecondaryAxis(Chart chart, string xAxisTitle, string yAxisTitlePrimary, string yAxisTitleSecondary, string chartTitle, List<SeriesConfig> seriesConfigs, double xMin, double xMax, double yMinPrimary, double yMaxPrimary, double yMinSecondary, double yMaxSecondary) { if (chart == null || chart.IsDisposed) return; chart.Series.Clear(); chart.ChartAreas.Clear(); ChartArea chartArea = new ChartArea(chartTitle.Replace(" ", "") + "Area"); chartArea.AxisX.Title = xAxisTitle; chartArea.AxisX.Minimum = xMin; if (xMax > xMin) chartArea.AxisX.Maximum = xMax; else chartArea.AxisX.Maximum = xMin + 1; chartArea.AxisX.LabelStyle.Format = "F0"; chartArea.AxisY.Title = yAxisTitlePrimary; chartArea.AxisY.Minimum = yMinPrimary; if (yMaxPrimary > yMinPrimary) chartArea.AxisY.Maximum = yMaxPrimary; else chartArea.AxisY.Maximum = yMinPrimary + 1; chartArea.AxisY.LineColor = Color.Black; chartArea.AxisY.LabelStyle.ForeColor = Color.Black; chartArea.AxisY.TitleForeColor = Color.Black; chartArea.AxisY.LabelStyle.Format = "F0"; chartArea.AxisY2.Enabled = AxisEnabled.True; chartArea.AxisY2.Title = yAxisTitleSecondary; chartArea.AxisY2.Minimum = yMinSecondary; if (yMaxSecondary > yMinSecondary) chartArea.AxisY2.Maximum = yMaxSecondary; else chartArea.AxisY2.Maximum = yMinSecondary + 1; chartArea.AxisY2.LineColor = Color.DarkRed; chartArea.AxisY2.LabelStyle.ForeColor = Color.DarkRed; chartArea.AxisY2.TitleForeColor = Color.DarkRed; chartArea.AxisY2.MajorGrid.Enabled = false; chartArea.AxisY2.LabelStyle.Format = "F0"; chartArea.CursorX.IsUserEnabled = true; chartArea.CursorX.IsUserSelectionEnabled = true; chartArea.CursorY.IsUserEnabled = true; chartArea.CursorY.IsUserSelectionEnabled = true; chartArea.AxisX.ScaleView.Zoomable = true; chartArea.AxisY.ScaleView.Zoomable = true; chartArea.AxisY2.ScaleView.Zoomable = true; chart.ChartAreas.Add(chartArea); foreach (var sc in seriesConfigs) { Series series = new Series(sc.Name) { ChartType = sc.Type, Color = sc.Color, BorderWidth = 2, YAxisType = sc.Axis }; chart.Series.Add(series); } chart.Titles.Clear(); chart.Titles.Add(new Title(chartTitle, Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black)); }


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
        private void CloseWindows_Click(object sender, EventArgs e) { btnretornar_Click(sender, e); }
        private void Tela_Bombas_FormClosing(object sender, FormClosingEventArgs e) { if (!_fechamentoControladoPeloUsuario) { DialogResult dr = MessageBox.Show("Deseja fechar toda a aplicação?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Question); if (dr == DialogResult.Yes) StopAllOperationsAndQuit(true); else e.Cancel = true; } else StopAllOperationsAndQuit(false); }
        private void StopAllOperationsAndQuit(bool exitApplication = true) { StopTimers(); StopSerialConnection(); _updateUiTimer?.Dispose(); _updateUiTimer = null; monitoramentoTimer?.Dispose(); monitoramentoTimer = null; timerCronometro?.Dispose(); timerCronometro = null; if (exitApplication) { ConnectionSettingsApplication.CloseAllConnections(); Environment.Exit(Environment.ExitCode); } }
        #endregion

        #region INCIO_TESTE
        public void btnIniciar_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox tb6 = this.Controls.Find("textBox6", true).FirstOrDefault() as System.Windows.Forms.TextBox;
            System.Windows.Forms.TextBox tb5 = this.Controls.Find("textBox5", true).FirstOrDefault() as System.Windows.Forms.TextBox;
            System.Windows.Forms.TextBox tb4 = this.Controls.Find("textBox4", true).FirstOrDefault() as System.Windows.Forms.TextBox;
            if (!_viewModel.cabecalhoinicial(tb6, tb5, tb4)) { MessageBox.Show("Favor preencher os campos obrigatórios em DADOS DE ENSAIO.", "Campos Obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning); _viewModel.PiscarLabelsVermelhoSync(label6, label5, label4, 1000); return; }
            _isMonitoring = true; Inicioteste = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            if (Stage_box_bomba != null) _viewModel.IniciarTesteBomba(Stage_box_bomba);

            // InicializarMonitoramento(); // Originalmente comentado, mantido assim.
            _timeCounterSecondsRampa = 0; etapaAtual = 1;
            if (tempoCronometroDefinidoManualmente && valorDefinidoCronometro > 0)
            {
                cronometroIniciado = true; int tempoTotal = valorDefinidoCronometro * 60;
                if (circularProgressBar1 != null) { circularProgressBar1.Maximum = tempoTotal > 0 ? tempoTotal : 1; circularProgressBar1.Minimum = 0; circularProgressBar1.Value = tempoTotal; circularProgressBar1.Invalidate(); }
                timerCronometro.Start();
            }
            else if (circularProgressBar1 != null) { circularProgressBar1.Value = 0; circularProgressBar1.Maximum = 100; }
            SetButtonState(btngravar, true); SetButtonState(bntFinalizar, true); SetButtonState(btnreset, true); SetButtonState(btnrelatoriobomba, false); SetButtonState(btniniciarteste, false);
            LogHistoricalEvent("INICIADO ENSAIO DE BOMBAS", Color.Blue);

            ClearCharts();
            _viewModel.ResetChartDataLogic();
            ClearStaticDataGridViewCells();
            StartSerialConnection(); // Inicia a comunicação serial e o timer de UI
        }

        private void InicializarMonitoramento()
        {
            if (monitoramentoTimer == null)
            {
                monitoramentoTimer = new System.Windows.Forms.Timer();
                monitoramentoTimer.Interval = 1000;
                // monitoramentoTimer.Tick += MonitoramentoTimer_Tick;
            }
            // monitoramentoTimer.Start();
        }

        private void PararMonitoramento()
        {
            monitoramentoTimer?.Stop();
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
                    StartUpdateUiTimer(); // Inicia o timer para atualizar a UI com dados seriais
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

            while (true)
            {
                lock (serialBufferLock)
                {
                    bufferContent = serialDataBuffer.ToString();
                    newlineIndex = bufferContent.IndexOf('\n');
                }

                if (newlineIndex >= 0)
                {
                    string completeMessage = bufferContent.Substring(0, newlineIndex + 1).Trim();

                    lock (serialBufferLock)
                    {
                        serialDataBuffer.Remove(0, newlineIndex + 1);
                    }

                    if (!string.IsNullOrWhiteSpace(completeMessage))
                    {
                        Debug.WriteLine($"[PROCESS] Linha completa: {completeMessage}");
                        ParseAndStoreSensorData(completeMessage); // Chama o novo parser
                    }
                }
                else
                {
                    break;
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

        // NOVO: Método para parsear os dados e armazenar
        private void ParseAndStoreSensorData(string data)
        {
            // Formato esperado: HA1:000.00|HA2:000.00|...
            string[] pairs = data.Split('|');
            Dictionary<string, string> rawReadingsUpdate = new Dictionary<string, string>();

            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();
                    rawReadingsUpdate[key] = value; // Armazena o valor string como recebido
                }
                else
                {
                    Debug.WriteLine($"[PARSE_ERROR] Par chave-valor malformado no pacote serial: {pair}");
                }
            }

            if (rawReadingsUpdate.Count > 0)
            {
                lock (readingsLock)
                {
                    // Atualiza _latestRawSensorData com os novos valores
                    foreach (var entry in rawReadingsUpdate)
                    {
                        _latestRawSensorData[entry.Key] = entry.Value;
                    }
                }
            }
        }


        private void StartUpdateUiTimer()
        {
            if (_updateUiTimer == null)
            {
                _updateUiTimer = new Timer();
                _updateUiTimer.Interval = 150;
                _updateUiTimer.Tick += UpdateUiTimer_Tick; // Certifique-se que o handler está atribuído
            }
            if (!_updateUiTimer.Enabled)
            {
                _updateUiTimer.Start();
            }
        }

        private void StopUpdateUiTimer()
        {
            _updateUiTimer?.Stop();
        }

        // ATIVADO E MODIFICADO: Timer para atualizar a UI
        private void UpdateUiTimer_Tick(object sender, EventArgs e)
        {
            UpdateDisplay();
            if (_isMonitoring)
            {
                _timeCounterSecondsRampa++;
                AddDataPointsToCharts();
            }
        }

        // ATIVADO E MODIFICADO: Método para chamar atualização dos TextBoxes
        private void UpdateDisplay()
        {
            if (this.IsDisposed || this.Disposing) return;
            if (this.InvokeRequired)
            {
                try { this.BeginInvoke((MethodInvoker)UpdateTextBoxesAndNumericReadings); } // Chama o método renomeado/modificado
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
            }
            else
            {
                UpdateTextBoxesAndNumericReadings(); // Chama o método renomeado/modificado
            }
        }

        // ATIVADO E MODIFICADO: Lógica de atualização dos TextBoxes e _currentNumericSensorReadings
        private void UpdateTextBoxesAndNumericReadings()
        {
            if (this.IsDisposed || this.Disposing) return;

            Dictionary<string, string> rawDataSnapshot;
            lock (readingsLock)
            {
                rawDataSnapshot = new Dictionary<string, string>(_latestRawSensorData);
            }

            Dictionary<string, double> numericReadingsUpdate = new Dictionary<string, double>();

            foreach (var sensorInfo in _sensorDisplayMap)
            {
                string displayValue = "N/A";
                double finalNumericValue = 0.0; // Valor padrão ou para quando não há leitura

                if (rawDataSnapshot.TryGetValue(sensorInfo.SerialKey, out string rawValueStr))
                {
                    if (double.TryParse(rawValueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsedValue))
                    {
                        double? calibCoeff = GetCalibrationCoefficient(sensorInfo.SerialKey);
                        double calibratedValue = parsedValue;

                        if (calibCoeff.HasValue) // Coeficiente existe e não é 0
                        {
                            calibratedValue = parsedValue * calibCoeff.Value;
                        }
                        // Se calibCoeff for null (não encontrado ou 0), calibratedValue permanece como parsedValue (raw)

                        finalNumericValue = calibratedValue;
                        displayValue = calibratedValue.ToString(sensorInfo.DisplayFormat, CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        Debug.WriteLine($"[UI_UPDATE_ERROR] Não foi possível converter valor '{rawValueStr}' para double para a chave '{sensorInfo.SerialKey}'.");
                        finalNumericValue = 0.0; // Ou algum valor de erro/padrão
                    }
                }

                // Armazena o valor numérico (calibrado ou raw) para gráficos e outras lógicas
                numericReadingsUpdate[sensorInfo.ChartDataKey] = finalNumericValue;


                // Atualiza o TextBox se houver um mapeado
                if (!string.IsNullOrEmpty(sensorInfo.TextBoxName))
                {
                    Control[] foundControls = this.Controls.Find(sensorInfo.TextBoxName, true);
                    if (foundControls.Length > 0 && foundControls[0] is TextBox targetTextBox)
                    {
                        UpdateTextBoxIfAvailable(targetTextBox, displayValue);
                    }
                    else
                    {
                        // Se o TextBox não for encontrado, pode ser um campo da classe
                        var field = this.GetType().GetField(sensorInfo.TextBoxName,
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance);
                        if (field != null && field.GetValue(this) is TextBox tbInstance)
                        {
                            UpdateTextBoxIfAvailable(tbInstance, displayValue);
                        }
                        else if (sensorInfo.TextBoxName != null) // Log apenas se um nome de textbox foi especificado
                        {
                            Debug.WriteLine($"[UI_UPDATE_WARN] TextBox '{sensorInfo.TextBoxName}' não encontrado para a chave '{sensorInfo.SerialKey}'.");
                        }
                    }
                }
            }

            // Atualiza _currentNumericSensorReadings com todos os valores processados
            if (numericReadingsUpdate.Count > 0)
            {
                lock (readingsLock)
                {
                    foreach (var entry in numericReadingsUpdate)
                    {
                        _currentNumericSensorReadings[entry.Key] = entry.Value;
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

        // ATIVADO: Lógica para adicionar pontos aos gráficos
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

        // ATIVADO E MODIFICADO: Lógica interna para adicionar pontos aos gráficos
        private void AddDataPointsToChartsInternal()
        {
            if (this.IsDisposed || this.Disposing) return;
            Dictionary<string, double> readingsSnapshot;
            lock (readingsLock)
            {
                readingsSnapshot = new Dictionary<string, double>(_currentNumericSensorReadings);
            }

            // Chart 1: Pre.x Vaz. (MA1 vs VZ1)
            if (readingsSnapshot.TryGetValue("MA1", out double pressaoBar_MA1) &&
                readingsSnapshot.TryGetValue("VZ1", out double vazaoLpm_VZ1))
            {
                // A lógica do ViewModel para Chart 1 foi removida para simplificar, adicionando diretamente.
                // Se precisar da lógica de rotação constante, ela precisaria ser adaptada e re-adicionada.
                if (chart1 != null && !chart1.IsDisposed && chart1.Series.Count > 0 && chart1.Series.IndexOf("Pre.x Vaz.") != -1)
                {
                    chart1.Series["Pre.x Vaz."].Points.AddXY(vazaoLpm_VZ1, pressaoBar_MA1);
                }
            }

            // Chart 2: Vaz.In.X Rot (ROT vs DR1)
            if (readingsSnapshot.TryGetValue("ROT", out double rotacaoRpm_ROT) &&
                readingsSnapshot.TryGetValue("DR1", out double drenoLpm_DR1_chart2))
            {
                if (chart2 != null && !chart2.IsDisposed && chart2.Series.Count > 0 && chart2.Series.IndexOf("Vaz.In.X Rot") != -1)
                {
                    chart2.Series["Vaz.In.X Rot"].Points.AddXY(rotacaoRpm_ROT, drenoLpm_DR1_chart2);
                }
            }

            // Chart 3: Vaz. x Pres.Piloto (PL1 vs DR1)
            // "PL1" é a pressão piloto, "DR1" é o dreno.
            // A constante BAR_CONVERSION_PILOT será aplicada a PL1.
            if (readingsSnapshot.TryGetValue("PL1", out double pilotagemRaw_PL1) &&
                readingsSnapshot.TryGetValue("DR1", out double drenoLpm_DR1_chart3))
            {
                double pilotagemBarConvertida_PL1 = pilotagemRaw_PL1 * BAR_CONVERSION_PILOT;

                if (chart3 != null && !chart3.IsDisposed && chart3.Series.Count > 0 && chart3.Series.IndexOf("Vaz. x Pres.Piloto") != -1)
                {
                    chart3.Series["Vaz. x Pres.Piloto"].Points.AddXY(pilotagemBarConvertida_PL1, drenoLpm_DR1_chart3);
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
                    HistoricalEvents.BeginInvoke((MethodInvoker)delegate { AppendLogMessage(message, color); });
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
        #endregion

        #region FIM_TESTE
        private void btnParar_Click(object sender, EventArgs e)
        {
           // 1.Para todos os processos de teste em andamento para congelar o estado atual
            _isMonitoring = false;
            StopTimers();
            StopSerialConnection(); // Para o fluxo de dados antes de perguntar sobre salvar, para consistência
            cronometroIniciado = false;

            // 2. Registra o horário oficial de término do teste
            Fimteste = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); // Lógica similar no btnParar_Click existente

            // 3. Atualiza indicadores visuais e registra o início da finalização
            if (Stage_box_bomba != null && _viewModel != null)
            {
                _viewModel.FinalizarTesteBomba(Stage_box_bomba); // Do btnParar_Click existente
            }
            LogHistoricalEvent("ENSAIO DE BOMBAS FINALIZANDO...", Color.Orange);

            // 4. Pergunta ao usuário se deseja salvar os dados e gerar um relatório
            DialogResult dr = MessageBox.Show("Deseja salvar os dados deste ensaio e gerar o relatório?",
                                             "Finalizar Ensaio e Salvar Relatório",
                                             MessageBoxButtons.YesNo,
                                             MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                // Verifica se uma janela de relatório já está aberta
                if (Application.OpenForms.OfType<Realatoriobase>().Any(f => !f.IsDisposed)) // Do btnrelatoriobomba_Click
                {
                    MessageBox.Show("Uma janela de relatório já está aberta. Feche-a antes de gerar um novo relatório.", "Relatório Aberto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    Application.OpenForms.OfType<Realatoriobase>().First(f => !f.IsDisposed).Focus();
                }
                else
                {
                    Realatoriobase relatorioForm = new Realatoriobase();

                    // Coleta os dados para o relatório
                    // Nome do Teste (de textBox6, conforme solicitado)
                    string nomeTeste = this.Controls.Find("textBox6", true).FirstOrDefault() is TextBox tbNomeTeste ? tbNomeTeste.Text : "N/A";
                    // Nome do Cliente (de textBox5, conforme solicitado)
                    string nomeCliente = this.Controls.Find("textBox5", true).FirstOrDefault() is TextBox tbCliente ? tbCliente.Text : "N/A";
                    // Ordem de Serviço (de textBox4, conforme solicitado)
                    string ordemServico = this.Controls.Find("textBox4", true).FirstOrDefault() is TextBox tbOS ? tbOS.Text : "N/A";

                    // Dados do DataGridView
                    List<List<string>> tabelaDadosParaRelatorio = new List<List<string>>();
                    if (dataGridView1 != null && !dataGridView1.IsDisposed)
                    {
                        List<string> headers = new List<string>();
                        foreach (DataGridViewColumn col in dataGridView1.Columns) { headers.Add(col.HeaderText); }
                        tabelaDadosParaRelatorio.Add(headers);

                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            if (row.IsNewRow) continue;
                            List<string> rowData = new List<string>();
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                rowData.Add(cell.Value?.ToString() ?? string.Empty);
                            }
                            tabelaDadosParaRelatorio.Add(rowData);
                        }
                    }

                    // Imagens dos Gráficos
                    List<Image> graficos = new List<Image>();
                    try
                    {
                        if (chart1 != null && !chart1.IsDisposed && chart1.Series.Cast<Series>().Any(s => s.Points.Count > 0)) { using (var ms = new System.IO.MemoryStream()) { chart1.SaveImage(ms, ChartImageFormat.Png); graficos.Add(Image.FromStream(ms)); } } else { graficos.Add(null); }
                        if (chart2 != null && !chart2.IsDisposed && chart2.Series.Cast<Series>().Any(s => s.Points.Count > 0)) { using (var ms = new System.IO.MemoryStream()) { chart2.SaveImage(ms, ChartImageFormat.Png); graficos.Add(Image.FromStream(ms)); } } else { graficos.Add(null); }
                        if (chart3 != null && !chart3.IsDisposed && chart3.Series.Cast<Series>().Any(s => s.Points.Count > 0)) { using (var ms = new System.IO.MemoryStream()) { chart3.SaveImage(ms, ChartImageFormat.Png); graficos.Add(Image.FromStream(ms)); } } else { graficos.Add(null); }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Erro ao salvar imagens dos gráficos para relatório: {ex.Message}");
                        while (graficos.Count < 3) graficos.Add(null); // Garante 3 entradas, mesmo que nulas
                    }

                    // Passa os dados para a instância de Realatoriobase.
                    // Usando a assinatura de SetDadosEnsaio como inferido do seu código comentado em btnrelatoriobomba_Click,
                    // adaptando nomeCliente (era textBox6) para nomeTeste (textBox6) e nomeBomba (era textBox5) para nomeCliente (textBox5).
                    if (relatorioForm.IsHandleCreated || !relatorioForm.IsDisposed)
                    {
                        // O método em Realatoriobase.cs que recebe esses dados precisa existir.
                        // Assumindo: public void SetDadosEnsaio(string inicio, string fim, string cliente, string nomeDoTeste, string os, List<List<string>> dadosGrid, List<EtapaData> dadosEtapasDetalhados, List<Image> imgsGraficos)
                        relatorioForm.SetDadosEnsaio(
                            Inicioteste,
                            Fimteste,
                            nomeCliente,        // De textBox5
                            nomeTeste,          // De textBox6
                            ordemServico,       // De textBox4
                            tabelaDadosParaRelatorio,
                            _dadosColetados,    // Esta é sua List<EtapaData>
                            graficos
                        );

                        relatorioForm.Show(this.Owner ?? this); // Mostra o relatório de forma não modal
                        LogHistoricalEvent("Dados do ensaio enviados para o formulário de relatório.", Color.Green);
                    }
                    else
                    {
                        MessageBox.Show("Não foi possível preparar ou exibir o formulário de relatório.", "Erro de Relatório", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            // else if dr == DialogResult.No (Usuário escolheu não salvar e gerar relatório agora)
            // {
            //     LogHistoricalEvent("Usuário optou por NÃO salvar o relatório ao finalizar o ensaio.", Color.DarkGoldenrod);
            // }

            // 5. Define o estado final dos botões
            SetButtonState(btngravar, false);        // Ensaio terminou
            SetButtonState(bntFinalizar, false);     // Ensaio terminou
            SetButtonState(btnreset, true);          // Permite resetar para uma nova sessão
            SetButtonState(btnrelatoriobomba, true); // Permite gerar o relatório manualmente depois, se necessário
            SetButtonState(btniniciarteste, true);   // Pronto para um novo ensaio

            LogHistoricalEvent("ENSAIO DE BOMBAS FINALIZADO.", Color.Red); // Do btnParar_Click existente
        }
        #endregion

        #region RESET
        private void btnretornar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Tem certeza que deseja reiniciar o processo e retornar ao menu?\nTodos os dados não salvos em relatório serão perdidos!",
                "Confirmação de Reinício", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result == DialogResult.No) return;
            _fechamentoControladoPeloUsuario = true;
            StopTimers(); StopSerialConnection(); _isMonitoring = false; cronometroIniciado = false;
            _timeCounterSecondsRampa = 0; etapaAtual = 1;
            if (dadosSensores != null) dadosSensores.Clear();
            if (_dadosColetados != null) _dadosColetados.Clear();
            Inicioteste = string.Empty; Fimteste = string.Empty;
            lock (serialBufferLock) { serialDataBuffer.Clear(); }
            lock (readingsLock) { _currentNumericSensorReadings.Clear(); _latestRawSensorData.Clear(); } // Limpa também _latestRawSensorData

            UpdateDisplay(); // Chama para limpar os TextBoxes com "N/A"
            ClearStaticDataGridViewCells();

            if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
            {
                circularProgressBar1.Value = 0;
                circularProgressBar1.Maximum = (tempoCronometroDefinidoManualmente && valorDefinidoCronometro > 0) ? (valorDefinidoCronometro * 60) : 100;
                circularProgressBar1.Invalidate();
            }
            ClearCharts(); _viewModel.ResetChartDataLogic();
            if (Stage_box_bomba != null) _viewModel.FinalizarTesteBomba(Stage_box_bomba);
            LogHistoricalEvent("AGUARDANDO INÍCIO DO ENSAIO...", Color.DarkGreen);
            SetButtonState(btniniciarteste, true); SetButtonState(btngravar, false); SetButtonState(bntFinalizar, false); SetButtonState(btnreset, false); SetButtonState(btnrelatoriobomba, false);
            try
            {
                var menuAppInstance = Menuapp.Instance;
                if (menuAppInstance != null && !menuAppInstance.IsDisposed) { menuAppInstance.Show(); menuAppInstance.BringToFront(); }
                else { Debug.WriteLine("btnretornar_Click: Menuapp.Instance não disponível ou descartado."); }
            }
            catch (Exception ex) { Debug.WriteLine($"btnretornar_Click: Exceção ao tentar mostrar Menuapp: {ex.Message}"); }
            this.Close();
        }

        private void btnreset_Click(object sender, EventArgs e)
        {
            DialogResult confirmResult = MessageBox.Show(
                "Tem certeza que deseja resetar o ensaio atual?\nTodos os dados coletados serão perdidos e um novo ciclo de ensaio será iniciado imediatamente com as configurações e dados de cabeçalho atuais.",
                "Confirmar Reset e Reinício Imediato",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                // --- 1. Parar atividades do teste atual em andamento ---
                _isMonitoring = false;
                StopTimers();

                // --- 2. Limpar todos os dados da execução anterior ---
                ClearCharts();
                if (_viewModel != null) _viewModel.ResetChartDataLogic();

                lock (readingsLock)
                {
                    _latestRawSensorData.Clear();
                    _currentNumericSensorReadings.Clear();
                }
                // UpdateDisplay() será chamado após a reinicialização

                ClearStaticDataGridViewCells();

                if (_dadosColetados != null) _dadosColetados.Clear();

                // --- 3. Reinicializar o sistema para o estado de um novo teste recém-iniciado ---
                _isMonitoring = true;
                Inicioteste = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                Fimteste = string.Empty;

                if (Stage_box_bomba != null && _viewModel != null)
                {
                    _viewModel.IniciarTesteBomba(Stage_box_bomba);
                }

                // REINICIAR CONTADOR DE TEMPO DE EXECUÇÃO
                _timeCounterSecondsRampa = 0;
                etapaAtual = 1;

                // REINICIAR CRONÔMETRO
                cronometroIniciado = false;
                if (timerCronometro != null) timerCronometro.Stop();

                if (tempoCronometroDefinidoManualmente && valorDefinidoCronometro > 0)
                {
                    cronometroIniciado = true;
                    int tempoTotalDefinido = valorDefinidoCronometro * 60;
                    if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
                    {
                        circularProgressBar1.Maximum = tempoTotalDefinido > 0 ? tempoTotalDefinido : 1;
                        circularProgressBar1.Value = tempoTotalDefinido;
                        circularProgressBar1.Invalidate();
                    }
                    if (timerCronometro != null) timerCronometro.Start();
                }
                else
                {
                    if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
                    {
                        circularProgressBar1.Value = 0;
                        circularProgressBar1.Maximum = 100;
                        circularProgressBar1.Invalidate();
                    }
                }

                if (labelCronometro_bomba != null && !labelCronometro_bomba.IsDisposed)
                {
                    string initialCronometroText = "00:00:00";
                    if (labelCronometro_bomba.InvokeRequired)
                    {
                        labelCronometro_bomba.BeginInvoke((MethodInvoker)delegate { labelCronometro_bomba.Text = initialCronometroText; });
                    }
                    else
                    {
                        labelCronometro_bomba.Text = initialCronometroText;
                    }
                }

                LogHistoricalEvent("ENSAIO RESETADO E REINICIADO AUTOMATICAMENTE.", Color.Blue);

                UpdateDisplay();

                StartSerialConnection();

                // --- 4. Definir o estado dos botões para corresponder ao estado PÓS-clique bem-sucedido em "Iniciar" ---
                SetButtonState(btngravar, true);
                SetButtonState(bntFinalizar, true);
                SetButtonState(btnreset, true);
                SetButtonState(btnrelatoriobomba, false);
                SetButtonState(btniniciarteste, false);

                MessageBox.Show("O ensaio foi resetado e um novo ciclo foi iniciado.", "Reset e Reinício Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion

        private void ClearStaticDataGridViewCells()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed) return;
            Action action = () => {
                for (int colIndex = 1; colIndex < dataGridView1.Columns.Count; colIndex++)
                {
                    for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count; rowIndex++)
                    {
                        if (dataGridView1.Rows[rowIndex].Cells.Count > colIndex && dataGridView1.Rows[rowIndex].Cells[colIndex] != null)
                        {
                            dataGridView1.Rows[rowIndex].Cells[colIndex].Value = string.Empty;
                        }
                    }
                }
            };
            if (dataGridView1.InvokeRequired) { try { dataGridView1.Invoke(action); } catch (ObjectDisposedException) { } }
            else { action(); }
        }


        public struct SeriesConfig
        {
            public string Name; public SeriesChartType Type; public Color Color; public AxisType Axis;
            public SeriesConfig(string name, SeriesChartType type, Color color, AxisType axis = AxisType.Primary)
            { Name = name; Type = type; Color = color; Axis = axis; }
        }

        private void GravarDadoSensor(string nomeSensor, string valor, string unidade)
        {
            if (string.IsNullOrEmpty(valor) || valor == "N/A")
            { MessageBox.Show($"Valor do sensor '{nomeSensor}' não disponível para gravação.", "Dados Ausentes", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (_isMonitoring)
            {
                if (dadosSensores == null) dadosSensores = new List<SensorData>();
                dadosSensores.Add(new SensorData { Sensor = nomeSensor, Valor = valor, Medidas = unidade });
            }
            else
            { MessageBox.Show("Teste não iniciado. Favor iniciar o teste para capturar os dados.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
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
            { MessageBox.Show(this, erroMsg + $"\nA verificação de limites para {nomeUnidade} foi desativada.", $"Erro de Validação - {nomeUnidade}", MessageBoxButtons.OK, MessageBoxIcon.Warning); checkBox.Checked = false; return false; }
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
            { circularProgressBar1.Value--; }
            else
            { timerCronometro.Stop(); cronometroIniciado = false; if (_isMonitoring) { LogHistoricalEvent("Tempo do cronômetro esgotado. Finalizando teste automaticamente.", Color.Orange); btnParar_Click(this, EventArgs.Empty); } }
        }


        private void btnDefinir_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox inputTempoCronometro = this.Controls.Find("textBox1_tempoCronometro", true).FirstOrDefault() as System.Windows.Forms.TextBox;
            if (inputTempoCronometro == null || inputTempoCronometro.IsDisposed)
            { MessageBox.Show(this, "Controle para definir tempo do cronômetro não encontrado ou foi descartado.", "Erro UI", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (!cronometroIniciado)
            {
                if (int.TryParse(inputTempoCronometro.Text, out int valorMinutos) && valorMinutos > 0)
                {
                    valorDefinidoCronometro = valorMinutos; tempoCronometroDefinidoManualmente = true;
                    if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
                    { circularProgressBar1.Maximum = valorDefinidoCronometro * 60 > 0 ? valorDefinidoCronometro * 60 : 1; circularProgressBar1.Value = valorDefinidoCronometro * 60; circularProgressBar1.Invalidate(); }
                    MessageBox.Show(this, $"Tempo do cronômetro definido para {valorDefinidoCronometro} minutos.", "Cronômetro Definido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else { MessageBox.Show(this, "Por favor, insira um valor numérico inteiro positivo válido em minutos.", "Erro de Entrada", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
            else { MessageBox.Show(this, "O cronômetro já está em execução. Limpe ou finalize o teste atual para definir um novo tempo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox inputTempoCronometro = this.Controls.Find("textBox1_tempoCronometro", true).FirstOrDefault() as System.Windows.Forms.TextBox;
            if (inputTempoCronometro != null && !inputTempoCronometro.IsDisposed && !cronometroIniciado)
            {
                inputTempoCronometro.Text = "0"; valorDefinidoCronometro = 0; tempoCronometroDefinidoManualmente = false;
                if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed) { circularProgressBar1.Value = 0; circularProgressBar1.Maximum = 100; circularProgressBar1.Invalidate(); }
                MessageBox.Show(this, "Tempo do cronômetro limpo.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (cronometroIniciado) { MessageBox.Show(this, "O cronômetro está em execução. Finalize o teste atual para limpar o tempo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
        }

        private void btn_gravar_Click(object sender, EventArgs e)
        {
            if (!_isMonitoring)
            {
                MessageBox.Show("O teste não foi iniciado. Por favor, inicie o teste para gravar os dados.", "Teste Não Iniciado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // MODIFICADO: Limite de etapas dinâmico com base no textBox2 (Nível de Teste)
            int maxEtapasDefinidas = 7; // Valor padrão caso textBox2 não seja encontrado ou inválido
            Control textBox2Control = this.Controls.Find("textBox2", true).FirstOrDefault();
            if (textBox2Control is TextBox tb2Nivel && int.TryParse(tb2Nivel.Text, out int nivelDefinido) && nivelDefinido > 0)
            {
                maxEtapasDefinidas = nivelDefinido;
            }
            else
            {
                Debug.WriteLine($"[Gravar] Não foi possível ler o número de etapas de textBox2. Usando padrão: {maxEtapasDefinidas}.");
            }


            if (tempoCronometroDefinidoManualmente && !cronometroIniciado && valorDefinidoCronometro > 0 && (circularProgressBar1 == null || circularProgressBar1.Value <= 0))
            {
                MessageBox.Show("O tempo definido para o teste encerrou ou o cronômetro não está ativo. Não é possível gravar novas etapas.", "Tempo Esgotado ou Cronômetro Inativo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetButtonState(btngravar, false);
                return;
            }

            if (etapaAtual > maxEtapasDefinidas) // Usa o valor dinâmico
            {
                MessageBox.Show($"Limite de {maxEtapasDefinidas} etapas (definido em 'Nível de Teste') foi atingido.", "Limite de Etapas Atingido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetButtonState(btngravar, false);
                return;
            }

            GravarDadosNoDataGridViewEstatico();

            var currentEtapaData = new EtapaData { Etapa = etapaAtual, leituras = new List<SensorData>() };
            Dictionary<string, double> readingsSnapshot;
            lock (readingsLock) { readingsSnapshot = new Dictionary<string, double>(_currentNumericSensorReadings); }

            if (_staticDataGridViewParameters != null)
            {
                foreach (var paramInfo in _staticDataGridViewParameters)
                {
                    // Tenta encontrar a leitura correspondente baseando-se no _sensorDisplayMap
                    // para obter a SerialKey/ChartDataKey associada ao TextBoxName
                    string chartKeyForParam = _sensorDisplayMap
                                              .FirstOrDefault(s => s.TextBoxName == paramInfo.SourceTextBoxName)
                                              .ChartDataKey; // Pode ser null se não houver mapeamento direto

                    if (chartKeyForParam == null && paramInfo.SourceTextBoxName == "sensor_CELSUS") chartKeyForParam = "TEM";
                    if (chartKeyForParam == null && paramInfo.SourceTextBoxName == "sensor_RPM") chartKeyForParam = "ROT";


                    string valorParaRelatorio = "N/D";
                    // Determina a unidade (simplificado, idealmente viria de um mapa de unidades mais robusto)
                    string unidadeParaRelatorio = paramInfo.FormattingType == "temp" ? "°C" : (paramInfo.FormattingType == "rpm" ? "RPM" : "Unid.");
                    if (paramInfo.DisplayName.ToLower().Contains("pressão") || paramInfo.DisplayName.ToLower().Contains("hidro") || paramInfo.DisplayName.ToLower().Contains("p1") || paramInfo.DisplayName.ToLower().Contains("p2") || paramInfo.DisplayName.ToLower().Contains("p3") || paramInfo.DisplayName.ToLower().Contains("p4")) unidadeParaRelatorio = "bar";
                    if (paramInfo.DisplayName.ToLower().Contains("vazão") || paramInfo.DisplayName.ToLower().Contains("dreno")) unidadeParaRelatorio = "lpm";


                    if (chartKeyForParam != null && readingsSnapshot.TryGetValue(chartKeyForParam, out double sensorNumericValue))
                    {
                        if (paramInfo.FormattingType == "temp" || paramInfo.FormattingType == "rpm")
                            valorParaRelatorio = ((int)Math.Round(sensorNumericValue)).ToString(CultureInfo.InvariantCulture);
                        else
                            valorParaRelatorio = sensorNumericValue.ToString("F2", CultureInfo.InvariantCulture);
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

            if (etapaAtual > maxEtapasDefinidas) // Verifica novamente com o valor dinâmico
            {
                MessageBox.Show($"Todas as {maxEtapasDefinidas} etapas (definidas em 'Nível de Teste') foram preenchidas.", "Tabela Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetButtonState(btngravar, false);
            }
        }


        private void GravarDadosNoDataGridViewEstatico()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed || _staticDataGridViewParameters == null) return;
            if (etapaAtual < 1 || etapaAtual > (dataGridView1.Columns.Count - 1))
            { Debug.WriteLine($"[GravarDGV] Etapa atual ({etapaAtual}) fora do intervalo válido."); return; }

            int targetColumnIndexInDGV = etapaAtual;

            for (int rowIndex = 0; rowIndex < _staticDataGridViewParameters.Count; rowIndex++)
            {
                if (rowIndex >= dataGridView1.Rows.Count) { Debug.WriteLine($"[GravarDGV] rowIndex {rowIndex} fora do intervalo."); break; }
                ParameterRowInfo paramInfo = _staticDataGridViewParameters[rowIndex];
                System.Windows.Forms.TextBox sourceTextBox = null;
                Control[] foundControls = this.Controls.Find(paramInfo.SourceTextBoxName, true);
                if (foundControls.Length > 0 && foundControls[0] is TextBox tb) { sourceTextBox = tb; }
                else
                {
                    var field = this.GetType().GetField(paramInfo.SourceTextBoxName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (field != null && field.GetValue(this) is TextBox tbInstance) { sourceTextBox = tbInstance; }
                }

                string formattedValue = "ERRO";
                if (sourceTextBox != null && !sourceTextBox.IsDisposed)
                {
                    string rawValue = sourceTextBox.Text;
                    if (string.IsNullOrWhiteSpace(rawValue) || rawValue.Equals("N/A", StringComparison.OrdinalIgnoreCase)) { formattedValue = "-"; }
                    else
                    {
                        if (double.TryParse(rawValue.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
                        {
                            switch (paramInfo.FormattingType.ToLower())
                            {
                                case "float": formattedValue = numericValue.ToString("0.00", CultureInfo.InvariantCulture); break;
                                case "temp": formattedValue = ((int)Math.Round(numericValue)).ToString("D", CultureInfo.InvariantCulture); break;
                                case "rpm": formattedValue = ((int)Math.Round(numericValue)).ToString("D", CultureInfo.InvariantCulture); break;
                                default: formattedValue = numericValue.ToString(CultureInfo.InvariantCulture); break;
                            }
                        }
                        else { formattedValue = "Inválido"; }
                    }
                }
                else { Debug.WriteLine($"[GravarDGV] TextBox '{paramInfo.SourceTextBoxName}' não encontrado."); formattedValue = "N/D"; }

                if (dataGridView1.Rows[rowIndex].Cells.Count > targetColumnIndexInDGV && dataGridView1.Rows[rowIndex].Cells[targetColumnIndexInDGV] != null)
                {
                    if (dataGridView1.InvokeRequired)
                    { int r = rowIndex; int c = targetColumnIndexInDGV; string val = formattedValue; try { dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Rows[r].Cells[c].Value = val; }); } catch (ObjectDisposedException) { } }
                    else { dataGridView1.Rows[rowIndex].Cells[targetColumnIndexInDGV].Value = formattedValue; }
                }
            }
        }


        private void btnrelatoriobomba_Click(object sender, EventArgs e)
        {
            if (Application.OpenForms.OfType<Realatoriobase>().Any(f => !f.IsDisposed))
            { MessageBox.Show("Uma janela de relatório já está aberta.", "Relatório Aberto", MessageBoxButtons.OK, MessageBoxIcon.Warning); Application.OpenForms.OfType<Realatoriobase>().First(f => !f.IsDisposed).Focus(); return; }

            Realatoriobase relatorioForm = new Realatoriobase();
            string nomeCliente = this.Controls.Find("textBox6", true).FirstOrDefault() is TextBox tbCliente ? tbCliente.Text : "N/A";
            string nomeBomba = this.Controls.Find("textBox5", true).FirstOrDefault() is TextBox tbBomba ? tbBomba.Text : "N/A";
            string ordemServico = this.Controls.Find("textBox4", true).FirstOrDefault() is TextBox tbOS ? tbOS.Text : "N/A";

            List<List<string>> tabelaDadosParaRelatorio = new List<List<string>>();
            if (dataGridView1 != null && !dataGridView1.IsDisposed)
            {
                List<string> headers = new List<string>();
                foreach (DataGridViewColumn col in dataGridView1.Columns) { headers.Add(col.HeaderText); }
                tabelaDadosParaRelatorio.Add(headers);
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (row.IsNewRow) continue; List<string> rowData = new List<string>();
                    foreach (DataGridViewCell cell in row.Cells) { rowData.Add(cell.Value?.ToString() ?? string.Empty); }
                    tabelaDadosParaRelatorio.Add(rowData);
                }
            }

            List<Image> graficos = new List<Image>();
            try
            {
                if (chart1 != null && !chart1.IsDisposed && chart1.Series.Cast<Series>().Any(s => s.Points.Count > 0)) { using (var ms = new System.IO.MemoryStream()) { chart1.SaveImage(ms, ChartImageFormat.Png); graficos.Add(Image.FromStream(ms)); } } else { graficos.Add(null); }
                if (chart2 != null && !chart2.IsDisposed && chart2.Series.Cast<Series>().Any(s => s.Points.Count > 0)) { using (var ms = new System.IO.MemoryStream()) { chart2.SaveImage(ms, ChartImageFormat.Png); graficos.Add(Image.FromStream(ms)); } } else { graficos.Add(null); }
                if (chart3 != null && !chart3.IsDisposed && chart3.Series.Cast<Series>().Any(s => s.Points.Count > 0)) { using (var ms = new System.IO.MemoryStream()) { chart3.SaveImage(ms, ChartImageFormat.Png); graficos.Add(Image.FromStream(ms)); } } else { graficos.Add(null); }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar imagens dos gráficos: {ex.Message}");
                // Adiciona placeholders nulos se houver erro para manter a contagem de gráficos
                while (graficos.Count < 3) graficos.Add(null);
            }


            relatorioForm.SetDadosEnsaio(Inicioteste, Fimteste, nomeCliente, nomeBomba, ordemServico, tabelaDadosParaRelatorio, _dadosColetados, graficos);
            relatorioForm.Show();
        }
    }
}