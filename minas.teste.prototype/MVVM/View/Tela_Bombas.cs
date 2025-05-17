using System.Drawing;
using System;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete; // Assumindo que EtapaData e SensorData estão aqui E ConnectionSettingsApplication
using minas.teste.prototype.MVVM.ViewModel;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting; // Required for Chart
using minas.teste.prototype.Service; // Assumindo SerialManager está aqui
using static minas.teste.prototype.MVVM.ViewModel.Tela_BombasVM;
using System.Linq;
using System.Text; // Adicionado para StringBuilder
using System.Collections.Concurrent; // Considerado para async mais robusto, mas usando buffer simples por agora


namespace minas.teste.prototype.MVVM.View
{
    public partial class Tela_Bombas : Form
    {
        // Declaração dos controles Chart (assumindo que foram adicionados via designer)
        // Se você adicionou os charts via código, remova estas linhas
        //chart1;  Gráfico de Pressão vs Vazão
        //chart2;  Gráfico de Vazamento Interno / Dreno (Vazão Dreno vs Rotação)
        //chart3;  Novo gráfico de Vazamento (Vazão Dreno vs Pressão Carga)
        //chart4;  Novo gráfico de Eficiência vs Vazão
        //chart5;  Novo gráfico de Rampa de Amaciamento e Ciclagem
        //chart6;  Novo gráfico de Curva de Deslocamento / Vazão Real


        //          objetos GLOBAIS          //
        //-------------------------------------//
        private Tela_BombasVM _viewModel;
        private apresentacao _fechar_box; // Assuming 'apresentacao' is a form
        private List<EtapaData> _dadosColetados = new List<EtapaData>();
        public Dictionary<string, TextBox> sensorMap;
        public Dictionary<string, string> sensorMapmedida;
        private List<string> dadosSensoresSelecionados = new List<string>();
        public List<SensorData> dadosSensores; // Used for the visualizador DataGridView
        private Timer monitoramentoTimer; // Timer para monitoramento de ranges (used for validation)
        private Timer timer; // Timer para o cronômetro
        private TextBoxTrackBarSynchronizer _synchronizerNivel; // Assuming TextBoxTrackBarSynchronizer exists

        // Use the SerialManager from ConnectionSettingsApplication
        private SerialManager _serialManager;

        // Timer para atualização da UI em tempo real (a cada 1 segundo)
        private Timer _updateTimer;

        // Buffer para acumular os dados recebidos da porta serial
        private StringBuilder serialDataBuffer = new StringBuilder();
        // Objeto para sincronização de acesso ao buffer (necessário pois DataReceived roda em outra thread)
        private readonly object serialBufferLock = new object();


        // Storage for latest received serial data
        private Dictionary<string, string> _latestRawSensorData = new Dictionary<string, string>();
        private Dictionary<string, double> _currentNumericSensorReadings = new Dictionary<string, double>();

        // Dictionary for listing character labels (corrigido '1' para 'l')
        private Dictionary<string, string> _serialDataKeys = new Dictionary<string, string>()
        {
            {"P1", "Pressao Principal"},
            {"fluxo1", "Vazao Principal"},     // CORRIGIDO: fluxo1 -> fluxol
            {"Piloto1", "Pilotagem"},      // CORRIGIDO: Piloto1 -> Pilotol
            {"dreno1", "Vazao Dreno"},       // CORRIGIDO: dreno1 -> drenol
            {"RPM", "Rotacao"},
            {"temp", "Temperatura"}
        };

        // Conversion factors (based on user request)
        private const double BAR_TO_PSI_CONVERSION = 14.5; // Based on "Pilotol:10 * 14.5" and "P1:0.00 * 14.5"
        // The user requested "drenol:0*3.98" for GPM and "fluxol:8223*3.98" for GPM.
        // Assuming drenol and fluxol are received in LPM, the conversion factor from LPM to GPM is approx 0.264172.
        // However, the user explicitly provided 3.98. We will use the user's factor for GPM calculation.
        private const double LPM_TO_GPM_USER_CONVERSION = 3.98;
        private const double BAR_CONVERSION_PILOT = 1.705;


        // Contador de tempo para o Chart 5 (em segundos)
        private int _timeCounterSeconds = 0;

        //-------------------------------------//
        //          variaveis GLOBAIS          //
        //-------------------------------------//

        private bool _fechamentoForcado;
        private bool _isMonitoring = false; // Indicates if the test is active and monitoring
        public string Inicioteste;
        public string Fimteste;
        private int etapaAtual = 1;
        public string StatusText;
        public string TbNomeCliente { get; set; } //textbox nome cliente
        public string TbNomeBomba { get; set; } //
        public string TbOrdemServico { get; set; }
        private int valorDefinido;
        private bool cronometroIniciado = false;
        private int tempoTotalSegundos;
        private bool valorDefinidoManualmente = false;
        private Dictionary<string, string> sensorControlMap; // Used for validation logic


        public Tela_Bombas()
        {
            InitializeComponent();
            Inciaizador_listas(); // Initialize sensorMap, sensorMapmedida, sensorControlMap, and _currentNumericSensorReadings

            _viewModel = new Tela_BombasVM();
            _fechar_box = new apresentacao();
            _synchronizerNivel = new TextBoxTrackBarSynchronizer(textBox2, trackBar1, 1, 7);
            dadosSensores = new List<SensorData>(); // Used for the visualizador DataGridView
            timer = new Timer(); // Timer para o cronômetro (mantém como está)
            timer.Interval = 1000; // Intervalo de 1 segundo
            timer.Tick += Timer_Tick;

            // Obtém a instância persistente do SerialManager
            _serialManager = ConnectionSettingsApplication.PersistentSerialManager;
            // Garante que a subscrição do evento DataReceived usa o handler correto
            // Este handler acumulará os dados no buffer.
            _serialManager.DataReceived += SerialManager_DataReceived;


            // Inicializa o timer de atualização da UI
            _updateTimer = new Timer();
            // *** ALTERAÇÃO: Intervalo para 1 SEGUNDO (1000 ms) ***
            _updateTimer.Interval = 1000; // 1 segundo para atualizações da UI
            // Este timer irá processar o buffer acumulado e atualizar a UI.
            _updateTimer.Tick += UpdateTimer_Tick;


            // --- Chart 1 Initialization (Pressão vs Vazão) ---
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            if (chart1.ChartAreas.Count == 0)
            {
                chart1.ChartAreas.Add(new ChartArea());
            }

            chart1.ChartAreas[0].AxisX.Title = "Vazão (LPM)"; // Eixo X: Vazão
            chart1.ChartAreas[0].AxisY.Title = "Pressão (BAR)"; // Eixo Y: Pressão

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 100; // Example max flow
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 300; // Example max pressure in BAR

            Series performanceSeries = new Series("Pre.x Vaz.");
            performanceSeries.ChartType = SeriesChartType.FastLine;
            performanceSeries.Color = Color.Blue;
            performanceSeries.BorderWidth = 2;
            chart1.Series.Add(performanceSeries);

            chart1.Titles.Clear();
            chart1.Titles.Add("Curva de Desempenho Principal");

            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            // --- End Chart 1 Initialization ---


            // --- Chart 2 Initialization (Vazamento Interno / Dreno - Vazão Dreno vs Rotação) ---
            chart2.Series.Clear();
            chart2.ChartAreas.Clear();

            if (chart2.ChartAreas.Count == 0)
            {
                chart2.ChartAreas.Add(new ChartArea());
            }

            chart2.ChartAreas[0].AxisX.Title = "Rotação (RPM)"; // Eixo X: Rotação
            chart2.ChartAreas[0].AxisY.Title = "Dreno (LPM)"; // Eixo Y: Vazão de Dreno

            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Maximum = 3000; // Example max RPM
            chart2.ChartAreas[0].AxisY.Minimum = 0;
            chart2.ChartAreas[0].AxisY.Maximum = 100; // Example max Dreno LPM

            Series drainRotationSeries = new Series("Vaz.In.X Rot");
            drainRotationSeries.ChartType = SeriesChartType.FastLine;
            drainRotationSeries.Color = Color.Red;
            drainRotationSeries.BorderWidth = 2;
            chart2.Series.Add(drainRotationSeries);

            chart2.Titles.Clear();
            chart2.Titles.Add("Vazamento Interno / Dreno");

            chart2.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart2.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart2.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            // --- End Chart 2 Initialization ---

            // --- Chart 3 Initialization (Vazamento Interno / Dreno - Vazão Dreno vs Pressão Carga) ---
            chart3.Series.Clear();
            chart3.ChartAreas.Clear();

            if (chart3.ChartAreas.Count == 0)
            {
                chart3.ChartAreas.Add(new ChartArea());
            }

            chart3.ChartAreas[0].AxisX.Title = "Pressão de Carga (BAR)"; // Eixo X: Pressão de Carga
            chart3.ChartAreas[0].AxisY.Title = "Dreno (LPM)"; // Eixo Y: Vazão de Dreno

            chart3.ChartAreas[0].AxisX.Minimum = 0;
            chart3.ChartAreas[0].AxisX.Maximum = 300; // Example max pressure
            chart3.ChartAreas[0].AxisY.Minimum = 0;
            chart3.ChartAreas[0].AxisY.Maximum = 10; // Example max Dreno LPM

            Series drainPressureSeries = new Series("Vaz. x Pres.");
            drainPressureSeries.ChartType = SeriesChartType.FastLine;
            drainPressureSeries.Color = Color.Orange;
            drainPressureSeries.BorderWidth = 2;
            chart3.Series.Add(drainPressureSeries);

            chart3.Titles.Clear();
            chart3.Titles.Add("Vazamento (Dreno) do Circuito Fechado");

            chart3.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart3.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart3.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart3.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart3.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart3.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            // --- End Chart 3 Initialization ---

            // --- Chart 4 Initialization (Eficiência vs Vazão) ---
            chart4.Series.Clear();
            chart4.ChartAreas.Clear();

            if (chart4.ChartAreas.Count == 0)
            {
                chart4.ChartAreas.Add(new ChartArea());
            }

            chart4.ChartAreas[0].AxisX.Title = "Vazão (LPM)"; // Eixo X: Vazão
            chart4.ChartAreas[0].AxisY.Title = "Eficiência (%)"; // Eixo Y: Eficiência

            chart4.ChartAreas[0].AxisX.Minimum = 0;
            chart4.ChartAreas[0].AxisX.Maximum = 100; // Example max flow
            chart4.ChartAreas[0].AxisY.Minimum = 0;
            chart4.ChartAreas[0].AxisY.Maximum = 100; // Example max efficiency

            Series globalEfficiencySeries = new Series("Rend. Global");
            globalEfficiencySeries.ChartType = SeriesChartType.FastLine;
            globalEfficiencySeries.Color = Color.Green;
            globalEfficiencySeries.BorderWidth = 2;
            chart4.Series.Add(globalEfficiencySeries);

            Series volumetricEfficiencySeries = new Series("Ef. Volumetrica");
            volumetricEfficiencySeries.ChartType = SeriesChartType.FastLine;
            volumetricEfficiencySeries.Color = Color.Purple;
            volumetricEfficiencySeries.BorderWidth = 2;
            chart4.Series.Add(volumetricEfficiencySeries);


            chart4.Titles.Clear();
            chart4.Titles.Add("Curvas de Rendimento Global e Volumétrico");

            chart4.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart4.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart4.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart4.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart4.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart4.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            // --- End Chart 4 Initialization ---


            // --- Chart 5 Initialization (Rampa de Amaciamento e Ciclagem) ---
            chart5.Series.Clear();
            chart5.ChartAreas.Clear();

            if (chart5.ChartAreas.Count == 0)
            {
                chart5.ChartAreas.Add(new ChartArea());
            }

            chart5.ChartAreas[0].AxisX.Title = "Tempo (segundos)"; // Eixo X: Tempo

            chart5.ChartAreas[0].AxisY.Title = "Temperatura (°C)"; // Eixo Y1: Temperatura
            chart5.ChartAreas[0].AxisY.Minimum = 0;
            chart5.ChartAreas[0].AxisY.Maximum = 100;

            chart5.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
            chart5.ChartAreas[0].AxisY2.Title = "Pressão (BAR) / Vazão (LPM)"; // Eixo Y2: Pressão/Vazão
            chart5.ChartAreas[0].AxisY2.Minimum = 0;
            chart5.ChartAreas[0].AxisY2.Maximum = 300;
            chart5.ChartAreas[0].AxisY2.LineColor = Color.Red;
            chart5.ChartAreas[0].AxisY2.LabelStyle.ForeColor = Color.Red;
            chart5.ChartAreas[0].AxisY2.TitleForeColor = Color.Red;


            Series temperatureSeries = new Series("Temperatura");
            temperatureSeries.ChartType = SeriesChartType.FastLine;
            temperatureSeries.Color = Color.Blue;
            temperatureSeries.BorderWidth = 2;
            temperatureSeries.YAxisType = AxisType.Primary; // Map to Primary Y-axis
            chart5.Series.Add(temperatureSeries);

            Series pressureRampSeries = new Series("Pressão Rampa");
            pressureRampSeries.ChartType = SeriesChartType.FastLine;
            pressureRampSeries.Color = Color.Red;
            pressureRampSeries.BorderWidth = 2;
            pressureRampSeries.YAxisType = AxisType.Secondary; // Map to Secondary Y-axis
            chart5.Series.Add(pressureRampSeries);

            Series flowRampSeries = new Series("Vazão Rampa");
            flowRampSeries.ChartType = SeriesChartType.FastLine;
            flowRampSeries.Color = Color.Green;
            flowRampSeries.BorderWidth = 2;
            flowRampSeries.YAxisType = AxisType.Secondary; // Map to Secondary Y-axis
            chart5.Series.Add(flowRampSeries);


            chart5.Titles.Clear();
            chart5.Titles.Add("Rampa de Amaciamento e Ciclagem");

            chart5.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart5.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart5.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart5.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart5.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart5.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart5.ChartAreas[0].AxisY2.ScaleView.Zoomable = true;
            // --- End Chart 5 Initialization ---

            // --- Chart 6 Initialization (Curva de Deslocamento / Vazão Real) ---
            chart6.Series.Clear();
            chart6.ChartAreas.Clear();

            if (chart6.ChartAreas.Count == 0)
            {
                chart6.ChartAreas.Add(new ChartArea());
            }

            chart6.ChartAreas[0].AxisX.Title = "Rotação (RPM)"; // Eixo X: Rotação
            chart6.ChartAreas[0].AxisY.Title = "Vazão Real (LPM)"; // Eixo Y: Vazão Real (Vazão Principal)

            chart6.ChartAreas[0].AxisX.Minimum = 0;
            chart6.ChartAreas[0].AxisX.Maximum = 3000; // Example max RPM
            chart6.ChartAreas[0].AxisY.Minimum = 0;
            chart6.ChartAreas[0].AxisY.Maximum = 100; // Example max Real Flow

            Series realFlowSeries = new Series("Vazão Real");
            realFlowSeries.ChartType = SeriesChartType.FastLine;
            realFlowSeries.Color = Color.Blue;
            realFlowSeries.BorderWidth = 2;
            chart6.Series.Add(realFlowSeries);


            chart6.Titles.Clear();
            chart6.Titles.Add("Curva de Deslocamento / Vazão Real");

            chart6.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart6.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart6.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart6.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart6.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart6.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            // --- End Chart 6 Initialization ---


            // Inicialmente desabilita botões que exigem o teste em execução
            btngravar.Enabled = false;
            bntFinalizar.Enabled = false;
            btnreset.Enabled = false;
            btnrelatoriobomba.Enabled = false;

            // Obtém a instância persistente do SerialManager
            _serialManager = ConnectionSettingsApplication.PersistentSerialManager;
            // Assina o evento DataReceived do SerialManager
            // Este handler irá AGORA acumular os dados no buffer.
            _serialManager.DataReceived += SerialManager_DataReceived;


            // Inicializa o timer de atualização da UI
            _updateTimer = new Timer();
            // *** ALTERAÇÃO: Intervalo para 1 SEGUNDO (1000 ms) ***
            _updateTimer.Interval = 1000; // 1 segundo para atualizações da UI
            // Este timer irá AGORA processar o buffer acumulado e atualizar a UI.
            _updateTimer.Tick += UpdateTimer_Tick;
        }

        public void Inciaizador_listas()
        {
            // Inicializa sensorMap para mapear chaves de dados serial para TextBoxes
            sensorMap = new Dictionary<string, TextBox>();
            sensorMap.Add("Pilotol", sensor_bar_PL);  // CORRIGIDO: Piloto1 -> Pilotol
            sensorMap.Add("drenol", sensor_lpm_DR);   // CORRIGIDO: dreno1 -> drenol
            sensorMap.Add("P1", sensor_Press_BAR);
            sensorMap.Add("RPM", sensor_rotacao_RPM);
            sensorMap.Add("fluxol", sensor_Vazao_LPM); // CORRIGIDO: fluxo1 -> fluxol
            sensorMap.Add("temp", sensor_Temp_C);    // Mapeia chave serial para textbox Temperatura

            // Inicializa sensorMapmedida - descreve a unidade do valor *bruto* serial
            // Isso é útil para saber a unidade da leitura direta, antes das conversões para exibição
            sensorMapmedida = new Dictionary<string, string>();
            sensorMapmedida.Add("Pilotol", "BAR");   // CORRIGIDO: Pilotol - Assumindo que o bruto é BAR
            sensorMapmedida.Add("drenol", "LPM");   // CORRIGIDO: drenol - Assumindo que o bruto é LPM
            sensorMapmedida.Add("P1", "BAR");       // Assumindo que o bruto é BAR
            sensorMapmedida.Add("RPM", "RPM");      // Assumindo que o bruto é RPM
            sensorMapmedida.Add("fluxol", "LPM");   // CORRIGIDO: fluxol - Assumindo que o bruto é LPM
            sensorMapmedida.Add("temp", "Celsius");// Assumindo que o bruto é Celsius

            // sensorControlMap parece mapear nomes descritivos para nomes de controle para lógica de validação/UI
            sensorControlMap = new Dictionary<string, string>();
            sensorControlMap.Add("Pilotagem PSI", "sensor_psi_PL");
            sensorControlMap.Add("Pilotagem BAR", "sensor_bar_PL");
            sensorControlMap.Add("Dreno GPM", "sensor_gpm_DR");
            sensorControlMap.Add("Dreno LPM", "sensor_lpm_DR");
            sensorControlMap.Add("Pressao PSI", "sensor_Press_PSI");
            sensorControlMap.Add("Pressao BAR", "sensor_Press_BAR");
            sensorControlMap.Add("Rotação RPM", "sensor_rotacao_RPM");
            sensorControlMap.Add("Vazão GPM", "sensor_Vazao_GPM");
            sensorControlMap.Add("Vazão LPM", "sensor_Vazao_LPM");
            sensorControlMap.Add("Temperatura Celsius", "sensor_Temp_C");


            // Inicializa o dicionário para armazenar as últimas leituras numéricas
            // Baseado nas chaves do formato serial
            _currentNumericSensorReadings = new Dictionary<string, double>();
            _currentNumericSensorReadings.Add("P1", 0.0);
            _currentNumericSensorReadings.Add("fluxol", 0.0);  // CORRIGIDO: fluxo1 -> fluxol
            _currentNumericSensorReadings.Add("Pilotol", 0.0); // CORRIGIDO: Piloto1 -> Pilotol
            _currentNumericSensorReadings.Add("drenol", 0.0);  // CORRIGIDO: dreno1 -> drenol
            _currentNumericSensorReadings.Add("RPM", 0.0);
            _currentNumericSensorReadings.Add("temp", 0.0);


            // Estas são provavelmente as chaves dos sensores esperadas para salvar ou outro processamento
            dadosSensoresSelecionados.AddRange(_serialDataKeys.Keys);

        }

        #region LOADS_JANELA
        private void Tela_Bombas_Load(object sender, EventArgs e)
        {
            _viewModel.Carregar_configuracao(this); // Carrega estilo do formulário
            _viewModel.Stage_signal(Stage_box_bomba); // Configura imagem do estágio do teste
            _viewModel.VincularRelogioLabel(LabelHorariotela); // Configura label do relógio
            HistoricalEvents.Text = "AGUARDANDO INÍCIO DO ENSAIO...";
            HistoricalEvents.ForeColor = System.Drawing.Color.DarkGreen;

            // Desabilita botões que exigem o teste em execução (já feito no construtor, mas pode reiterar)
            btngravar.Enabled = false;
            bntFinalizar.Enabled = false;
            btnreset.Enabled = false;
            btnrelatoriobomba.Enabled = false;

            // A configuração da porta serial é feita por ConnectionSettingsApplication.PersistentSerialManager.
            // A conexão será aberta quando o teste iniciar (btnIniciar_Click).
        }

        #endregion

        #region EVENTOS_FECHAMANETO
        private void CloseWindows_Click(object sender, EventArgs e)
        {
            _fechamentoForcado = true; // Indica que é um fechamento controlado
            // Para a porta serial e timers antes de fechar
            StopTimers(); // Para todos os timers (incluindo _updateTimer)
            StopSerialConnection(); // Desconecta usando SerialManager (e para _updateTimer)

            // Assumindo que Menuapp é o formulário principal para retornar
            // Verifica se Menuapp.Instance é acessível ou passa uma referência
            // Para simplificar, mantém o código existente assumindo Menuapp.Instance está disponível
            if (Menuapp.Instance != null)
            {
                Menuapp.Instance.Show();
            }
            this.Close();
        }

        private void Tela_Bombas_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Para a porta serial e timers e descarta recursos
            StopTimers(); // Para todos os timers
            StopSerialConnection(); // Desconecta usando SerialManager e para _updateTimer

            // Descarta timers
            _updateTimer?.Dispose(); // Descarta o timer de atualização da UI
            monitoramentoTimer?.Dispose(); // Descarta o timer de monitoramento
            timer?.Dispose(); // Descarta o timer do cronômetro

            // Descarta o SerialManager se este formulário for responsável pelo seu ciclo de vida (improvável com PersistentManager)
            // Se PersistentSerialManager deve viver durante toda a aplicação, NÃO o descarte aqui.
            // Assumindo que PersistentSerialManager é gerenciado em outro lugar e NÃO deve ser descartado aqui.


            // Apenas sai da aplicação se NÃO for um fechamento controlado
            if (!_fechamentoForcado)
            {
                // Assumindo que apresentacao lida com a lógica final de encerramento da aplicação
                if (_fechar_box != null)
                {
                    // Se você quer que a aplicação feche completamente, pode precisar de Application.Exit()
                    // em vez de depender de _fechar_box se ele apenas oculta a janela principal.
                    // _fechar_box.apresentacao_FormClosing(sender, e); // Comentei ou remova esta linha se o comportamento padrão de fechamento for desejado
                    Application.Exit(); // Garante que a aplicação fecha
                }
                else
                {
                    // Se _fechar_box não estiver disponível, garante que a aplicação saia
                    Application.Exit();
                }
            }
            else
            {
                // Se for um fechamento controlado (retornando ao menu), apenas garante que o menu principal seja mostrado
                if (Menuapp.Instance != null && !Menuapp.Instance.Visible)
                {
                    Menuapp.Instance.Show();
                }
            }
        }
        #endregion

        #region INCIO_TESTE
        public async void btnIniciar_Click(object sender, EventArgs e)
        {
            // Realiza validação do cabeçalho
            if (!_viewModel.cabecalhoinicial(textBox6, textBox5, textBox4))
            {
                MessageBox.Show("Favor preencher os campos obrigatórios em DADOS DE ENSAIO.");
                await _viewModel.PiscarLabelsVermelho(label6, label5, label4, 1000);
                return;
            }

            _isMonitoring = true;
            _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5, panel2, panel6, panel11, panel9);
            Inicioteste = DateTime.Now.ToString(); // Garante que Inicioteste é acessível
            _viewModel.IniciarTesteBomba(Stage_box_bomba); // Lógica específica de início no ViewModel

            // Inicia o timer de monitoramento de ranges (validações)
            InicializarMonitoramento();

            // Reinicia o contador de tempo para o Chart 5
            _timeCounterSeconds = 0;

            // Inicia o cronômetro se definido
            if (valorDefinidoManualmente)
            {
                cronometroIniciado = true;
                int tempoTotalSegundos = valorDefinido * 60;
                circularProgressBar1.Maximum = tempoTotalSegundos;
                circularProgressBar1.Minimum = 0;
                circularProgressBar1.Value = tempoTotalSegundos;
                circularProgressBar1.Invalidate();
                timer.Start(); // Inicia o timer do cronômetro
            }
            else
            {
                MessageBox.Show("O cronômetro não foi definido. O teste não será finalizado automaticamente.", "Aviso");
            }

            // Habilita/desabilita botões
            btngravar.Enabled = true;
            bntFinalizar.Enabled = true;
            btnreset.Enabled = true;
            btnrelatoriobomba.Enabled = true;
            btniniciarteste.Enabled = false;
            HistoricalEvents.Text = "INICIADO ENSAIO DE BOMBAS";

            // Limpa e reseta gráficos
            ClearCharts();
            _viewModel.ResetChartDataLogic(); // Reseta a lógica de dados dos gráficos no VM (ex: rotação anterior para chart1)

            // *** Inicia a conexão serial e o timer de atualização da UI ***
            StartSerialConnection(); // Este método AGORA também inicia o timer de atualização da UI em caso de sucesso
        }

        private void InicializarMonitoramento()
        {
            // Este timer é para monitoramento de ranges e rodará a cada 1 segundo
            if (monitoramentoTimer == null)
            {
                monitoramentoTimer = new System.Windows.Forms.Timer();
                monitoramentoTimer.Interval = 1000; // Intervalo de 1 segundo para monitoramento
                monitoramentoTimer.Tick += MonitoramentoTimer_Tick;
            }
            monitoramentoTimer.Start();
        }

        private void PararMonitoramento()
        {
            if (monitoramentoTimer != null && monitoramentoTimer.Enabled)
            {
                monitoramentoTimer.Stop();
                // Não descarta aqui, descarta no fechamento do formulário
            }
        }

        private void MonitoramentoTimer_Tick(object sender, EventArgs e)
        {
            // A lógica de monitoramento verificará os valores *atuais* exibidos nos TextBoxes.
            // Estes TextBoxes são atualizados pelo _updateTimer (que obtém dados do serial).
            // Não há necessidade de chamar UpdateDisplay() aqui, pois o _updateTimer faz isso.
            _viewModel.MonitorarDados(
                sensor_psi_PL.Text, textBox9.Text, textBox8.Text, checkBox_psi.Checked, panel4, HistoricalEvents,
                sensor_Press_PSI.Text, textBox9.Text, textBox8.Text, checkBox_psi.Checked, panel5,
                sensor_gpm_DR.Text, textBox14.Text, textBox12.Text, checkBox_gpm.Checked, panel2,
                sensor_Vazao_GPM.Text, textBox14.Text, textBox12.Text, checkBox_gpm.Checked, panel6,
                sensor_Press_BAR.Text, textBox11.Text, textBox10.Text, checkBox_bar.Checked, panel5,
                sensor_bar_PL.Text, textBox11.Text, textBox10.Text, checkBox_bar.Checked, panel4,
                sensor_rotacao_RPM.Text, textBox18.Text, textBox17.Text, checkBox_rotacao.Checked, panel9,
                sensor_Vazao_LPM.Text, textBox16.Text, textBox15.Text, checkBox_lpm.Checked, panel6,
                sensor_lpm_DR.Text, textBox16.Text, textBox15.Text, checkBox_lpm.Checked, panel2,
                sensor_Temp_C.Text, textBox20.Text, textBox19.Text, checkBox_temperatura.Checked, panel11
            );
        }

        #endregion

        #region Serial Communication and Data Processing

        private void StartSerialConnection()
        {
            // Usa o SerialManager para conectar
            try
            {
                // Assumindo que ConnectionSettingsApplication mantém o nome da porta e baud rate
                // Garante que estes estejam definidos antes de chamar Connect
                string portToConnect = ConnectionSettingsApplication.PortName;
                int baudRateToConnect = ConnectionSettingsApplication.BaudRate;


                if (!string.IsNullOrEmpty(portToConnect) && baudRateToConnect > 0)
                {
                    // A subscrição ao DataReceived é feita no construtor.
                    // _serialManager.DataReceived += SerialManager_DataReceived; // Garante apenas uma subscrição

                    _serialManager.Connect(portToConnect, baudRateToConnect);

                    // Verifica se a conexão foi bem-sucedida usando _serialManager.IsConnected
                    if (_serialManager.IsConnected)
                    {

                        LogHistoricalEvent($"Conectado à porta serial {ConnectionSettingsApplication.PortName}.");

                        // *** Inicia o timer de atualização da UI SOMENTE após a conexão bem-sucedida ***
                        StartUpdateTimer(); // Movido o início do timer para cá
                    }
                    else
                    {
                        // Em caso de erro na conexão, usa o nome da porta que tentou conectar
                        string errorMessage = $"ERRO ao conectar à porta serial {portToConnect}. Verifique as configurações ou se a porta está disponível.";
                        LogHistoricalEvent(errorMessage);
                        MessageBox.Show(errorMessage, "Erro de Conexão Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        // Considera parar o teste ou desabilitar funcionalidades dependentes do serial
                        btnParar_Click(this, EventArgs.Empty); // Para o teste se a conexão falhar
                    }
                }
                else
                {
                    string errorMessage = "Configurações de porta serial (Nome da Porta e/ou Baud Rate) não definidas. Por favor, configure a conexão serial.";
                    MessageBox.Show(errorMessage, "Configuração Serial Ausente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LogHistoricalEvent($"ERRO - {errorMessage}");
                    // Considera parar o teste ou desabilitar funcionalidades dependentes do serial
                    btnParar_Click(this, EventArgs.Empty); // Para o teste se a configuração estiver faltando
                }
            }
            catch (Exception ex)
            {
                // Em caso de exceção durante a tentativa de conexão, usa o nome da porta que tentou se for conhecido
                string attemptedPortName = ConnectionSettingsApplication.PortName;
                string errorMessage = $"Erro durante a tentativa de conexão serial {(string.IsNullOrEmpty(attemptedPortName) ? "" : $"à porta {attemptedPortName}")}: {ex.Message}";
                MessageBox.Show(errorMessage, "Erro de Conexão Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogHistoricalEvent(errorMessage);
                // Considera parar o teste ou desabilitar funcionalidades dependentes do serial
                btnParar_Click(this, EventArgs.Empty); // Para o teste em caso de exceção
            }
        }

        private void StopSerialConnection()
        {
            // Usa o SerialManager para desconectar
            if (_serialManager != null && _serialManager.IsConnected)
            {
                try
                {
                    // Desinscreve do evento DataReceived ao desconectar para evitar chamadas após o fechamento da porta
                    _serialManager.DataReceived -= SerialManager_DataReceived; // Desinscreve

                    _serialManager.Disconnect();
                    LogHistoricalEvent($"Desconectado da porta serial {ConnectionSettingsApplication.PortName}.");

                    // *** Para o timer de atualização da UI ao desconectar ***
                    StopUpdateTimer(); // Para o timer de atualização da UI

                }
                catch (Exception ex)
                {
                    // Mesmo que a desconexão falhe, loga o erro e prossegue
                    LogHistoricalEvent($"ERRO ao desconectar da porta serial {ConnectionSettingsApplication.PortName}: {ex.Message}");
                }
            }
            else if (_serialManager != null && !_serialManager.IsConnected)
            {
                // Se o SerialManager existe mas não estava conectado, apenas loga que já estava desconectado
                LogHistoricalEvent("Porta serial já estava desconectada.");
            }
            // Se _serialManager for nulo, nenhuma ação é necessária.
        }

        // Manipulador do evento DataReceived do SerialManager
        // Este método é chamado em uma thread secundária fornecida pela porta serial.
        private void SerialManager_DataReceived(object sender, string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            // Anexa os dados recebidos ao buffer
            lock (serialBufferLock)
            {
                serialDataBuffer.Append(data);
            }

            // Processa o buffer para extrair e tratar mensagens completas.
            // O processamento e atualização das variáveis globais deve ser rápido aqui
            // ou usar Invoke para a thread da UI se as atualizações forem para controles de UI.
            ProcessSerialBuffer();
        }

        /// <summary>
        /// Processa o buffer de dados serial, extraindo mensagens completas
        /// e enviando-as para serem parseadas.
        /// </summary>
        private void ProcessSerialBuffer()
        {
            lock (serialBufferLock)
            {
                string bufferContent = serialDataBuffer.ToString();
                int newlineIndex;

                // Processa todas as mensagens completas encontradas no buffer.
                // Assumimos que cada mensagem termina com um caractere de nova linha ('\n').
                while ((newlineIndex = bufferContent.IndexOf('\n')) != -1)
                {
                    // Extrai uma mensagem completa (incluindo a nova linha)
                    string completeMessage = bufferContent.Substring(0, newlineIndex + 1);

                    // Remove a mensagem processada do início do buffer
                    serialDataBuffer.Remove(0, newlineIndex + 1);

                    // Envia a mensagem completa para ser parseada e armazenada.
                    // Esta chamada deve ser segura para threads se atualizar variáveis compartilhadas.
                    ParseAndStoreSensorData(completeMessage);

                    // Atualiza bufferContent para o restante do buffer para a próxima iteração do while
                    bufferContent = serialDataBuffer.ToString();
                }
                // Após o loop, partes de mensagens incompletas permanecem no buffer.
            }
        }


        /// <summary>
        /// Parseia uma única string de dados serial (mensagem completa) no formato KEY:VALUE|...
        /// e armazena os valores numéricos e brutos em dicionários.
        /// </summary>
        /// <param name="data">A string da mensagem serial completa a ser parseada.</param>
        // Renomeado e ligeiramente modificado ProcessSerialDataString para ParseAndStoreSensorData
        private void ParseAndStoreSensorData(string data)
        {
            if (string.IsNullOrEmpty(data)) return;

            // Remove espaços em branco e caracteres de fim de linha da mensagem
            string cleanedData = data.Trim();
            if (string.IsNullOrEmpty(cleanedData)) return;


            // Dicionários temporários para armazenar valores desta mensagem específica
            var tempNumericReadings = new Dictionary<string, double>();
            var tempRawData = new Dictionary<string, string>();

            // Divide a mensagem limpa pelo separador de pares '|'
            string[] sensorReadings = cleanedData.Split('|');

            foreach (string reading in sensorReadings)
            {
                string[] parts = reading.Split(':');
                // Verifica se a parte tem exatamente duas subpartes (chave e valor)
                if (parts.Length == 2)
                {
                    string key = parts[0].Trim();
                    string valueString = parts[1].Trim();

                    // Verifica se a chave está na nossa lista de chaves esperadas para processamento
                    if (_serialDataKeys.ContainsKey(key))
                    {
                        // Armazena o valor bruto
                        tempRawData[key] = valueString;

                        // Tenta parsear o valor numérico
                        // Usa InvariantCulture para lidar com pontos decimais de forma consistente (ex: "0.00")
                        if (double.TryParse(valueString, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
                        {
                            tempNumericReadings[key] = numericValue;
                        }
                        else
                        {
                            // Lida com erros de parseamento - loga um aviso nos eventos históricos
                            // O log é feito de forma segura para a thread da UI.
                            LogHistoricalEvent($"AVISO - Falha ao converter valor numérico para '{key}': '{valueString}'");
                            // Opcional: manter o valor anterior ou definir um padrão em caso de erro.
                            // Isso exigiria ler de _currentNumericSensorReadings antes de limpar.
                            // Por simplicidade, se o parse falhar, a chave não será adicionada a tempNumericReadings,
                            // e o valor anterior em _currentNumericSensorReadings persistirá até uma nova leitura válida.
                        }
                    }
                }
                // Partes que não seguem o formato KEY:VALUE ou com chaves não esperadas são simplesmente ignoradas.
            }

            // Atualiza os dicionários principais (_currentNumericSensorReadings e _latestRawSensorData)
            // que são lidos pelo timer de atualização da UI.
            // É MAIS SEGURO atualizar estes dicionários compartilhados na thread da UI usando Invoke.
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    UpdateSensorReadingDictionaries(tempNumericReadings, tempRawData);
                });
            }
            else
            {
                // Se já estiver na thread da UI (menos comum para DataReceived)
                UpdateSensorReadingDictionaries(tempNumericReadings, tempRawData);
            }
        }

        /// <summary>
        /// Atualiza os dicionários principais de leituras de sensores com os dados parseados.
        /// Este método DEVE ser chamado na thread da UI.
        /// </summary>
        private void UpdateSensorReadingDictionaries(Dictionary<string, double> numericReadings, Dictionary<string, string> rawData)
        {
            // Limpa e atualiza os dicionários com os novos dados.
            // Isso substitui os valores anteriores pelos desta mensagem.
            _latestRawSensorData.Clear();
            foreach (var item in rawData) _latestRawSensorData[item.Key] = item.Value;

            _currentNumericSensorReadings.Clear();
            foreach (var item in numericReadings) _currentNumericSensorReadings[item.Key] = item.Value;

            // Opcional: Logar recebimento bem-sucedido (evitar logs muito frequentes)
            // LogHistoricalEvent("Dados serial processados."); // Muito frequente a cada segundo
        }


        /// <summary>
        /// Inicia o timer responsável por atualizar a UI (TextBoxes e Charts)
        /// a cada segundo com base nos últimos dados recebidos.
        /// </summary>
        private void StartUpdateTimer()
        {
            if (_updateTimer == null)
            {
                _updateTimer = new Timer();
                _updateTimer.Interval = 1000; // Intervalo de 1 segundo
                _updateTimer.Tick += UpdateTimer_Tick;
            }

            if (!_updateTimer.Enabled)
            {
                _updateTimer.Start();
                LogHistoricalEvent("Timer de atualização da UI iniciado (intervalo: 1 segundo).");
            }
        }

        /// <summary>
        /// Para o timer de atualização da UI.
        /// </summary>
        private void StopUpdateTimer()
        {
            if (_updateTimer != null && _updateTimer.Enabled)
            {
                _updateTimer.Stop();
                LogHistoricalEvent("Timer de atualização da UI parado.");
            }
        }

        // Manipulador de evento Tick para o timer de atualização da UI (roda a cada 1 segundo)
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            // Este método roda na thread da UI.
            // Atualiza os TextBoxes e gráficos com os últimos dados processados.
            UpdateDisplay();

            // Incrementa o contador de tempo para o Chart 5 (Rampa) a cada segundo agora
            _timeCounterSeconds++; // Incrementa em 1 segundo a cada tick

            // Adiciona pontos de dados aos gráficos com base nos valores atuais
            AddDataPointsToCharts();

            // Este timer também aciona a lógica de monitoramento implicitamente
            // ao atualizar os TextBoxes que o MonitoramentoTimer_Tick lê.
            // Não há necessidade de chamar MonitoramentoTimer_Tick explicitamente aqui.
        }

        /// <summary>
        /// Garante que a atualização dos TextBoxes ocorra na thread da UI.
        /// </summary>
        private void UpdateDisplay()
        {
            // Garante que estamos na thread da UI para atualizar os TextBoxes
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)delegate {
                    UpdateTextBoxes();
                });
            }
            else
            {
                UpdateTextBoxes();
            }
        }

        /// <summary>
        /// Atualiza o texto dos TextBoxes no formulário com base nos valores
        /// armazenados em _currentNumericSensorReadings, aplicando as conversões.
        /// Este método DEVE ser chamado na thread da UI.
        /// </summary>
        private void UpdateTextBoxes()
        {
            // Atualiza TextBoxes com base nos valores em _currentNumericSensorReadings
            // Aplica conversões conforme especificado pelo usuário

            // Usa TryGetValue para acessar os dados de forma segura e evitar exceções
            // Pilotol (valor bruto assumido para exibição em BAR, convertido para PSI)
            // CORRIGIDO: Chave "Pilotol" estava com espaço extra ou digitada incorretamente
            if (_currentNumericSensorReadings.TryGetValue("Piloto1", out double piloto1Value))
            {
                // Exibe em BAR (assumindo que o valor bruto já representa BAR ou uma unidade proporcional)
                sensor_bar_PL.Text = piloto1Value.ToString("F2", CultureInfo.InvariantCulture);
                // Converte BAR para PSI usando o fator 14.5 (conforme a nota do usuário)
                sensor_psi_PL.Text = (piloto1Value * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);

                // Se o valor bruto de Pilotol precisar ser convertido para BAR usando BAR_CONVERSION_PILOT (1.705), ajuste aqui.
                // double piloto1Bar = piloto1Value * BAR_CONVERSION_PILOT;
                // sensor_bar_PL.Text = piloto1Bar.ToString("F2", CultureInfo.InvariantCulture);
                // sensor_psi_PL.Text = (piloto1Bar * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            }
            else
            {
                // Define como "N/A" se a chave não for encontrada
                sensor_bar_PL.Text = "N/A";
                sensor_psi_PL.Text = "N/A";
            }

            // drenol (valor bruto assumido como LPM, convertido para GPM usando o fator 3.98 do usuário)
            // CORRIGIDO: Chave "drenol" estava digitada incorretamente
            if (_currentNumericSensorReadings.TryGetValue("dreno1", out double dreno1Value))
            {
                // Exibe em LPM
                sensor_lpm_DR.Text = dreno1Value.ToString("F2", CultureInfo.InvariantCulture);
                // Converte LPM para GPM usando o fator 3.98 (incomum, mas segue instrução do usuário)
                sensor_gpm_DR.Text = (dreno1Value * LPM_TO_GPM_USER_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            }
            else
            {
                // Define como "N/A" se a chave não for encontrada
                sensor_lpm_DR.Text = "N/A";
                sensor_gpm_DR.Text = "N/A";
            }

            // P1 (valor bruto assumido como BAR, convertido para PSI usando 14.5)
            if (_currentNumericSensorReadings.TryGetValue("P1", out double p1Value))
            {
                // Exibe em BAR
                sensor_Press_BAR.Text = p1Value.ToString("F2", CultureInfo.InvariantCulture);
                // Converte BAR para PSI usando o fator 14.5
                sensor_Press_PSI.Text = (p1Value * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            }
            else
            {
                // Define como "N/A" se a chave não for encontrada
                sensor_Press_BAR.Text = "N/A";
                sensor_Press_PSI.Text = "N/A";
            }

            // RPM (valor bruto assumido como RPM)
            if (_currentNumericSensorReadings.TryGetValue("RPM", out double rpmValue))
            {
                // Exibe em RPM (geralmente sem casas decimais)
                sensor_rotacao_RPM.Text = rpmValue.ToString("F0", CultureInfo.InvariantCulture);
            }
            else
            {
                // Define como "N/A" se a chave não for encontrada
                sensor_rotacao_RPM.Text = "N/A";
            }

            // fluxol (valor bruto assumido como LPM, convertido para GPM usando o fator 3.98 do usuário)
            // CORRIGIDO: Chave "fluxol" estava digitada incorretamente
            if (_currentNumericSensorReadings.TryGetValue("fluxo1", out double fluxo1Value))
            {
                // Exibe em LPM
                sensor_Vazao_LPM.Text = fluxo1Value.ToString("F2", CultureInfo.InvariantCulture);
                // Converte LPM para GPM usando o fator 3.98 (incomum, mas segue instrução do usuário)
                sensor_Vazao_GPM.Text = (fluxo1Value * LPM_TO_GPM_USER_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            }
            else
            {
                // Define como "N/A" se a chave não for encontrada
                sensor_Vazao_LPM.Text = "N/A";
                sensor_Vazao_GPM.Text = "N/A";
            }

            // temp (valor bruto assumido como Celsius)
            if (_currentNumericSensorReadings.TryGetValue("temp", out double tempValue))
            {
                // Exibe em Celsius (geralmente com 1 casa decimal)
                sensor_Temp_C.Text = tempValue.ToString("F1", CultureInfo.InvariantCulture);
            }
            else
            {
                // Define como "N/A" se a chave não for encontrada
                sensor_Temp_C.Text = "N/A";
            }

            // Você pode adicionar tratamento de erro ou valores padrão se uma chave estiver faltando em _currentNumericSensorReadings
        }


        /// <summary>
        /// Adiciona pontos aos gráficos com base nos últimos valores numéricos dos sensores.
        /// Este método DEVE ser chamado na thread da UI.
        /// </summary>
        private void AddDataPointsToCharts()
        {
            // Garante que estamos na thread da UI para atualizar os Charts
            if (chart1.InvokeRequired)
            {
                chart1.Invoke((MethodInvoker)delegate {
                    AddDataPointsToChartsInternal();
                });
            }
            else
            {
                AddDataPointsToChartsInternal();
            }
        }

        /// <summary>
        /// Lógica interna para adicionar pontos aos gráficos na thread da UI.
        /// </summary>
        private void AddDataPointsToChartsInternal()
        {
            // Obtém os valores numéricos atuais (já atualizados em UpdateTextBoxes ou diretamente de _currentNumericSensorReadings)
            // Usa TryGetValue para acessar os dados de forma segura.
            // NOTA: Usando as chaves corrigidas ('l' em vez de '1')
            if (_currentNumericSensorReadings.TryGetValue("P1", out double pressaoBar) &&
                _currentNumericSensorReadings.TryGetValue("fluxo1", out double vazaoLpm) && // CORRIGIDO: fluxo1 -> fluxol
                _currentNumericSensorReadings.TryGetValue("Piloto1", out double pilotagemRaw) && // CORRIGIDO: Piloto1 -> Pilotol - Usando valor bruto de Pilotol
                _currentNumericSensorReadings.TryGetValue("dreno1", out double drenoLpm) && // CORRIGIDO: dreno1 -> drenol
                _currentNumericSensorReadings.TryGetValue("RPM", out double rotacaoRpm) &&
                _currentNumericSensorReadings.TryGetValue("temp", out double temperaturaC))
            {
                // Chart 1 (Pressão vs Vazão) - X=Vazão (LPM), Y=Pressão (BAR)
                // Obtém ponto de dados se a rotação for constante (lógica no ViewModel)
                Datapoint_Bar_Lpm? dataPoint = _viewModel.GetChartDataIfRotationConstant(
                      pressaoBar.ToString(CultureInfo.InvariantCulture), // Passa como string para o VM se necessário
                      vazaoLpm.ToString(CultureInfo.InvariantCulture),
                      rotacaoRpm.ToString(CultureInfo.InvariantCulture)
                   );

                if (dataPoint.HasValue)
                {
                    // Verifica se a série existe antes de adicionar pontos
                    if (chart1.Series["Pre.x Vaz."] != null) // Uso ContainsKey para robustez
                    {
                        chart1.Series["Pre.x Vaz."].Points.AddXY(dataPoint.Value.FlowLpm, dataPoint.Value.PressureBar);
                    }
                }


                // Chart 2 (Vazamento Interno / Dreno - Vazão Dreno vs Rotação) - X=Rotação (RPM), Y=Dreno (LPM)
                if (chart2.Series["Vaz.In.X Rot"] != null)
                {
                    chart2.Series["Vaz.In.X Rot"].Points.AddXY(rotacaoRpm, drenoLpm);
                }

                // Chart 3 (Vazamento Interno / Dreno - Vazão Dreno vs Pressão Carga) - X=Pressão Carga (BAR), Y=Dreno (LPM)
                // Assumindo que 'Pressão de Carga' para o Chart 3 é o valor de Pilotagem em BAR
                double pilotagemBar = pilotagemRaw * BAR_CONVERSION_PILOT; // Usa o fator de conversão para Pilotagem BAR
                if (chart3.Series["Vaz. x Pres."] != null)
                {
                    chart3.Series["Vaz. x Pres."].Points.AddXY(pilotagemBar, drenoLpm);
                }

                // Chart 4 (Eficiência vs Vazão) - X=Vazão (LPM), Y=Eficiência (%)
                double eficienciaVolumetrica = 0;
                // Evita divisão por zero
                if (vazaoLpm + drenoLpm > 0)
                {
                    eficienciaVolumetrica = (vazaoLpm / (vazaoLpm + drenoLpm)) * 100;
                }
                // Limita a eficiência a 100% e 0%
                if (eficienciaVolumetrica > 100) eficienciaVolumetrica = 100;
                if (eficienciaVolumetrica < 0) eficienciaVolumetrica = 0;

                // Cálculo simplificado do rendimento global (ajuste conforme requisitos reais)
                // Exemplo: Eficiência Volumétrica * (1 - (Pressão / PressãoMáximaTeórica))
                double rendimentoGlobal = eficienciaVolumetrica * (1 - (pressaoBar / 400.0)); // Exemplo simplificado
                // Garante que o rendimento global não seja negativo e não exceda a eficiência volumétrica
                if (rendimentoGlobal < 0) rendimentoGlobal = 0;
                if (rendimentoGlobal > eficienciaVolumetrica) rendimentoGlobal = eficienciaVolumetrica;


                if (chart4.Series["Rend. Global"] != null)
                {
                    chart4.Series["Rend. Global"].Points.AddXY(vazaoLpm, rendimentoGlobal);
                }
                if (chart4.Series["Ef. Volumetrica"] != null)
                {
                    // Recalcula a eficiência volumétrica se necessário, ou usa a calculada acima
                    chart4.Series["Ef. Volumetrica"].Points.AddXY(vazaoLpm, eficienciaVolumetrica);
                }

                // Chart 5 (Rampa de Amaciamento) - X=Tempo (segundos), Y1=Temp (°C), Y2=Press (BAR), Y2=Vazao (LPM)
                // Usa _timeCounterSeconds (agora incrementando a cada segundo)
                if (chart5.Series["Temperatura"] != null)
                {
                    chart5.Series["Temperatura"].Points.AddXY(_timeCounterSeconds, temperaturaC);
                }
                if (chart5.Series["Pressão Rampa"] != null)
                {
                    chart5.Series["Pressão Rampa"].Points.AddXY(_timeCounterSeconds, pressaoBar); // Usando pressão principal
                }
                if (chart5.Series["Vazão Rampa"] != null)
                {
                    chart5.Series["Vazão Rampa"].Points.AddXY(_timeCounterSeconds, vazaoLpm); // Usando vazão principal
                }

                // Chart 6 (Vazão Real vs Rotação) - X=Rotação (RPM), Y=Vazão Real (LPM)
                if (chart6.Series["Vazão Real"] != null)
                {
                    chart6.Series["Vazão Real"].Points.AddXY(rotacaoRpm, vazaoLpm); // Assumindo Vazão Real é Vazão Principal
                }
            }
            else
            {
                // Logar um aviso se dados essenciais estiverem faltando para os gráficos (evitar logs frequentes)
                // Console.WriteLine($"{DateTime.Now:G}: AVISO - Dados essenciais para gráficos ausentes ou inválidos.");
            }
        }

        /// <summary>
        /// Limpa todos os pontos dos gráficos.
        /// Este método DEVE ser chamado na thread da UI.
        /// </summary>
        private void ClearCharts()
        {
            // Garante que estamos na thread da UI para limpar os Charts
            if (chart1.InvokeRequired)
            {
                chart1.Invoke((MethodInvoker)delegate {
                    ClearChartsInternal();
                });
            }
            else
            {
                ClearChartsInternal();
            }
        }

        /// <summary>
        /// Lógica interna para limpar os pontos dos gráficos na thread da UI.
        /// </summary>
        private void ClearChartsInternal()
        {
            // Limpa os pontos de todas as séries em cada gráfico
            foreach (var series in chart1.Series) series.Points.Clear();
            foreach (var series in chart2.Series) series.Points.Clear();
            foreach (var series in chart3.Series) series.Points.Clear();
            foreach (var series in chart4.Series) series.Points.Clear();
            foreach (var series in chart5.Series) series.Points.Clear();
            foreach (var series in chart6.Series) series.Points.Clear();

            // Re-adicionar séries se elas foram removidas (não deveriam ser se apenas os pontos foram limpos)
            // Esta parte pode ser redundante se você apenas limpa os pontos, mas é uma boa prática
            // garantir que as séries existam antes de tentar adicionar pontos.
            // Exemplo (re-adicionar séries se Count == 0):
            // if (chart1.Series.Count == 0) chart1.Series.Add(new Series("Pre.x Vaz.") { ChartType = SeriesChartType.FastLine, Color = Color.Blue, BorderWidth = 2 });
            // ... repetir para outros gráficos ...
        }


        /// <summary>
        /// Para todos os timers em execução no formulário.
        /// </summary>
        private void StopTimers()
        {
            timer?.Stop(); // Timer do cronômetro
            monitoramentoTimer?.Stop(); // Timer de monitoramento
            _updateTimer?.Stop(); // Timer de atualização da UI
        }

        /// <summary>
        /// Método auxiliar para logar eventos no RichTextBox HistoricalEvents
        /// de forma segura para a thread da UI.
        /// </summary>
        /// <param name="message">A mensagem a ser logada.</param>
        private void LogHistoricalEvent(string message)
        {
            // Verifica se é necessário invocar para a thread da UI
            if (HistoricalEvents.InvokeRequired)
            {
                // Invoca o método AppendText na thread da UI
                HistoricalEvents.Invoke((MethodInvoker)delegate {
                    HistoricalEvents.AppendText($"{DateTime.Now:G}: {message}" + Environment.NewLine);
                });
            }
            else
            {
                // Se já estiver na thread da UI, apenas anexa o texto
                HistoricalEvents.AppendText($"{DateTime.Now:G}: {message}" + Environment.NewLine);
            }
        }


        #endregion

        #region FIM_TESTE
        private void btnParar_Click(object sender, EventArgs e) // Assumindo que este é o seu btnFinalizar
        {
            StopTimers(); // Para todos os timers (inclui _updateTimer agora)
            StopSerialConnection(); // Para a comunicação serial e _updateTimer

            cronometroIniciado = false;
            _isMonitoring = false;
            _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5, panel2, panel6, panel11, panel9);
            Fimteste = DateTime.Now.ToString();
            //trackBar1.Enabled = true; // Re-habilita trackbar se necessário

            // Desabilita botões que exigem o teste em execução
            btngravar.Enabled = false;
            bntFinalizar.Enabled = false;
            btnreset.Enabled = false;
            btnrelatoriobomba.Enabled = false;
            btniniciarteste.Enabled = true; // Re-habilita botão iniciar

            LogHistoricalEvent("ENSAIO DE BOMBAS FINALIZADO"); // Usa método auxiliar para logar
            HistoricalEvents.ForeColor = System.Drawing.Color.Red; // Altera cor do log para indicar finalização

            // Os pontos existentes nos gráficos permanecerão visíveis após parar
            _viewModel.FinalizarTesteBomba(Stage_box_bomba); // Finalização específica no ViewModel
        }


        #endregion

        #region RESET
        private void btnReset_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Tem certeza que deseja reiniciar o processo?\nTodos os dados coletados serão perdidos!",
                "Confirmação de Reinício",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                // Para todos os processos
                StopTimers(); // Para todos os timers
                StopSerialConnection(); // Para a comunicação serial e _updateTimer

                _isMonitoring = false;
                _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5, panel2, panel6, panel11, panel9);


                // Limpa dados
                _timeCounterSeconds = 0; // Reseta contador de tempo
                etapaAtual = 1;
                dadosSensores.Clear(); // Limpa dados para o 'visualizador'
                _dadosColetados.Clear(); // Limpa dados de etapas coletadas
                _latestRawSensorData.Clear(); // Limpa últimos dados serial brutos
                _currentNumericSensorReadings.Clear(); // Limpa últimas leituras numéricas
                Inicioteste = string.Empty;
                Fimteste = string.Empty;
                // Mantém valorDefinidoManualmente como está, para reter o tempo definido se configurado.

                // Limpa o buffer de dados serial
                lock (serialBufferLock)
                {
                    serialDataBuffer.Clear();
                }


                // Limpa TextBoxes
                sensor_psi_PL.Text = string.Empty;
                sensor_bar_PL.Text = string.Empty;
                sensor_gpm_DR.Text = string.Empty;
                sensor_lpm_DR.Text = string.Empty;
                sensor_Press_PSI.Text = string.Empty;
                sensor_Press_BAR.Text = string.Empty;
                sensor_rotacao_RPM.Text = string.Empty;
                sensor_Vazao_GPM.Text = string.Empty;
                sensor_Vazao_LPM.Text = string.Empty;
                sensor_Temp_C.Text = string.Empty;


                // Limpa DataGridViews
                dataGridView1.Rows.Clear();
                if (visualizador.DataSource != null)
                {
                    var list = visualizador.DataSource as List<SensorData>;
                    if (list != null)
                    {
                        list.Clear();
                        visualizador.DataSource = null;
                        visualizador.DataSource = list;
                    }
                    else
                    {
                        visualizador.DataSource = null;
                    }
                }
                else
                {
                    visualizador.Rows.Clear();
                }


                // Reseta ProgressBar para o valor definido ou 0
                if (valorDefinidoManualmente)
                {
                    circularProgressBar1.Maximum = valorDefinido * 60;
                    circularProgressBar1.Value = valorDefinido * 60;
                }
                else
                {
                    circularProgressBar1.Value = 0;
                    circularProgressBar1.Maximum = 100; // Reseta max se necessário ou define um padrão
                }
                circularProgressBar1.Invalidate();

                // Limpa gráficos
                ClearCharts();
                _viewModel.ResetChartDataLogic(); // Reseta lógica relacionada aos gráficos no VM

                // Reseta indicador visual do estágio do teste
                _viewModel.FinalizarTesteBomba(Stage_box_bomba); // Assumindo que isso define a imagem do estágio para 'off'

                LogHistoricalEvent("ENSAIO DE BOMBAS RESETADO"); // Usa método auxiliar para logar
                HistoricalEvents.ForeColor = System.Drawing.Color.DarkOrange; // Altera cor para indicar reset

                // Re-habilita botão iniciar e desabilita outros
                btniniciarteste.Enabled = true;
                btngravar.Enabled = false;
                bntFinalizar.Enabled = false;
                btnreset.Enabled = false;
                btnrelatoriobomba.Enabled = false;


                MessageBox.Show("Processo reiniciado com sucesso!", "Reinício Completo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Nota: Reset NÃO reinicia automaticamente o teste.
                // O usuário precisa clicar em 'Iniciar' novamente.
            }
        }

        #endregion


        #region BOTÕES_MEDIDAS
        // Estes botões capturam os valores *atuais* exibidos nos TextBoxes
        // Os TextBoxes são atualizados a cada segundo pelo _updateTimer
        private void unidade_medidapilotagem1_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (_isMonitoring && double.TryParse(sensor_psi_PL.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valorAtual))
            {
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pilotagem", // Nome lógico do sensor
                    Valor = valorAtual.ToString("F2", CultureInfo.InvariantCulture), // Armazena valor numérico como string
                    Medidas = "psi" // Unidade
                });

                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }


        private void unidade_medidapilotagem2_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (_isMonitoring && double.TryParse(sensor_bar_PL.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valorAtual))
            {
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pilotagem",
                    Valor = valorAtual.ToString("F2", CultureInfo.InvariantCulture),
                    Medidas = "bar"
                });

                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidadreno1_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (_isMonitoring && double.TryParse(sensor_gpm_DR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valorAtual))
            {
                dadosSensores.Add(new SensorData
                {
                    Sensor = "dreno",
                    Valor = valorAtual.ToString("F2", CultureInfo.InvariantCulture),
                    Medidas = "gpm"
                });

                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidadreno2_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (_isMonitoring && double.TryParse(sensor_lpm_DR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valorAtual))
            {
                dadosSensores.Add(new SensorData
                {
                    Sensor = "dreno",
                    Valor = valorAtual.ToString("F2", CultureInfo.InvariantCulture),
                    Medidas = "lpm"
                });

                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidapressao1_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (_isMonitoring && double.TryParse(sensor_Press_PSI.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valorAtual))
            {
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pressão",
                    Valor = valorAtual.ToString("F2", CultureInfo.InvariantCulture),
                    Medidas = "psi"
                });

                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidapressao2_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (_isMonitoring && double.TryParse(sensor_Press_BAR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valorAtual))
            {
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pressão",
                    Valor = valorAtual.ToString("F2", CultureInfo.InvariantCulture),
                    Medidas = "bar"
                });

                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados."); // Corrigido para MessageBox.Show
            }
        }

        private void unidade_medidarota_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (_isMonitoring && double.TryParse(sensor_rotacao_RPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valorAtual))
            {
                dadosSensores.Add(new SensorData
                {
                    Sensor = "rotação",
                    Valor = valorAtual.ToString("F0", CultureInfo.InvariantCulture),
                    Medidas = "rpm"
                });

                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidasvazao1_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (_isMonitoring && double.TryParse(sensor_Vazao_GPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valorAtual))
            {
                dadosSensores.Add(new SensorData
                {
                    Sensor = "vazão",
                    Valor = valorAtual.ToString("F2", CultureInfo.InvariantCulture),
                    Medidas = "gpm"
                });

                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidasvazao2_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (_isMonitoring && double.TryParse(sensor_Vazao_LPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valorAtual))
            {
                dadosSensores.Add(new SensorData
                {
                    Sensor = "vazão",
                    Valor = valorAtual.ToString("F2", CultureInfo.InvariantCulture),
                    Medidas = "lpm"
                });

                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidatemp_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (_isMonitoring && double.TryParse(sensor_Temp_C.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double valorAtual))
            {
                dadosSensores.Add(new SensorData
                {
                    Sensor = "temperatura", // Nome lógico do sensor, pode usar Tag se definido no designer
                    Valor = valorAtual.ToString("F1", CultureInfo.InvariantCulture),
                    Medidas = "celsius"
                });

                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }


        private void btn_gravar_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para gravar os dados.");
                return; // Sai do método se não estiver monitorando
            }

            // Verifica se o número máximo de etapas foi atingido com base no máximo do trackBar1
            if (etapaAtual <= trackBar1.Maximum) // Usando trackBar1.Maximum como referência de maxControlEtapas
            {
                // Adiciona uma linha ao DataGridView mostrando os valores atuais dos sensores
                dataGridViewLoad(); // Garante que as colunas do DataGridView estejam configuradas
                AtualizarDataGridView(); // Adiciona uma linha ao dataGridView1 com os dados dos TextBoxes

                // Coleta dados para a etapa atual e armazena em _dadosColetados
                var currentEtapaData = new EtapaData
                {
                    Etapa = etapaAtual,
                    leituras = new List<SensorData>()
                };

                // Adiciona as leituras numéricas atuais dos sensores ao EtapaData
                // Isso usa os valores que foram parseados e armazenados dos dados serial
                // É melhor usar os valores numéricos de _currentNumericSensorReadings para salvar
                foreach (var reading in _currentNumericSensorReadings)
                {
                    string sensorSerialKey = reading.Key;
                    double sensorNumericValue = reading.Value;

                    // Encontra o nome lógico correspondente do sensor e a unidade
                    string logicalSensorName = sensorSerialKey; // Padrão
                    string unit = "N/A"; // Padrão

                    // Tenta obter o nome lógico de _serialDataKeys
                    if (_serialDataKeys.TryGetValue(sensorSerialKey, out string mappedLogicalName))
                    {
                        logicalSensorName = mappedLogicalName;
                    }

                    // Tenta encontrar a unidade com base na chave serial (usando sensorMapmedida)
                    if (sensorMapmedida.TryGetValue(sensorSerialKey, out string mappedUnit))
                    {
                        unit = mappedUnit;
                    }
                    else
                    {
                        // Tentativa de adivinhar a unidade pelo nome da chave se não encontrada em sensorMapmedida
                        if (sensorSerialKey.IndexOf("PSI", StringComparison.OrdinalIgnoreCase) != -1) unit = "PSI";
                        else if (sensorSerialKey.IndexOf("BAR", StringComparison.OrdinalIgnoreCase) != -1) unit = "BAR";
                        else if (sensorSerialKey.IndexOf("GPM", StringComparison.OrdinalIgnoreCase) != -1) unit = "GPM";
                        else if (sensorSerialKey.IndexOf("LPM", StringComparison.OrdinalIgnoreCase) != -1) unit = "LPM";
                        else if (sensorSerialKey.IndexOf("RPM", StringComparison.OrdinalIgnoreCase) != -1) unit = "RPM";
                        else if (sensorSerialKey.IndexOf("temp", StringComparison.OrdinalIgnoreCase) != -1) unit = "Celsius";
                    }


                    currentEtapaData.leituras.Add(new SensorData
                    {
                        Sensor = logicalSensorName, // Usa o nome lógico
                        Valor = sensorNumericValue.ToString(CultureInfo.InvariantCulture), // Armazena valor numérico
                        Medidas = unit // Armazena a unidade
                    });
                }

                // Adiciona os dados da etapa coletada à lista principal
                _dadosColetados.Add(currentEtapaData);

                // Incrementa a etapa atual
                etapaAtual++;
                LogHistoricalEvent($"Dados gravados para Etapa {etapaAtual - 1}."); // Usa método auxiliar para logar
            }
            else
            {
                MessageBox.Show($"Limite de etapas ({trackBar1.Maximum}) atingido.", "Limite de Etapas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Opcional: desabilitar o botão de gravação aqui
                btngravar.Enabled = false;
            }
        }

        private void btnDefinir_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (!cronometroIniciado)
            {
                if (int.TryParse(textBox1.Text, out int valor) && valor > 0) // Garante que o valor seja positivo
                {
                    valorDefinido = valor;
                    valorDefinidoManualmente = true;
                    // Habilita/desabilita botões conforme necessário após definir um valor
                    // button4.Enabled = false; // Assumindo que button4 é btnLimpar - deixa habilitado para permitir limpar
                    // button6.Enabled = true; // Assumindo que button6 é btnDefinir - talvez queira desabilitar isso também após definir, se só puder definir uma vez por teste
                    MessageBox.Show($"O valor {valorDefinido} minutos foi definido.", "Sucesso");

                    // Atualiza o máximo da ProgressBar imediatamente após definir o tempo
                    circularProgressBar1.Maximum = valorDefinido * 60;
                    circularProgressBar1.Value = valorDefinido * 60;
                    circularProgressBar1.Invalidate();
                }
                else
                {
                    MessageBox.Show("Por favor, insira um valor numérico inteiro positivo válido no TextBox para o cronômetro.", "Erro de Entrada");
                }
            }
            else
            {
                MessageBox.Show("O cronômetro já está em execução. Limpe ou finalize o teste para definir um novo tempo.", "Aviso");
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            // ... (mantém lógica existente) ...
            if (!cronometroIniciado)
            {
                textBox1.Text = "0";
                valorDefinido = 0; // Reseta o valor definido
                valorDefinidoManualmente = false;
                // Habilita/desabilita botões conforme necessário após limpar
                // button4.Enabled = true; // Assumindo que button4 é btnLimpar - deixa habilitado
                // button6.Enabled = true; // Assumindo que button6 é btnDefinir - deixa habilitado
                circularProgressBar1.Value = 0; // Reseta barra de progresso
                circularProgressBar1.Maximum = 100; // Reseta max se necessário ou define um padrão
                circularProgressBar1.Invalidate();
                MessageBox.Show("Tempo definido limpo.", "Informação");
            }
            else
            {
                MessageBox.Show("O cronômetro está em execução. Finalize o teste para limpar o tempo definido.", "Aviso");
            }
        }
        #endregion


        #region VALIDAÇÕES
        // Mantém os métodos de validação existentes, pois são usados pelo MonitoramentoTimer_Tick
        /// <summary>
        /// Valida os TextBoxes de mínimo e máximo associados a um CheckBox.
        /// Verifica se são numéricos, não negativos e se min <= max.
        /// Desmarca o CheckBox e exibe uma mensagem se a validação falhar.
        /// </summary>
        /// <param name="checkBox">O CheckBox que está sendo validado.</param>
        /// <param name="minTextBox">O TextBox que contém o valor mínimo.</param>
        /// <param name="maxTextBox">O TextBox que contém o valor máximo.</param>
        /// <param name="nomeUnidade">O nome da unidade/medição (ex: "PSI", "GPM") para usar na mensagem de erro.</param>
        /// <returns>True se a validação passar, False caso contrário.</returns>
        private bool ValidarMinMaxCheckBox(CheckBox checkBox, TextBox minTextBox, TextBox maxTextBox, string nomeUnidade)
        {
            // ... (mantém lógica existente) ...
            // Tenta converter os valores, usando InvariantCulture para consistência (ponto decimal)
            bool minOk = decimal.TryParse(minTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMinimo);
            bool maxOk = decimal.TryParse(maxTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMaximo);

            string erroMsg = null; // Armazena a mensagem de erro específica

            // 1. Valida se são numéricos
            if (!minOk || !maxOk)
            {
                erroMsg = $"Os valores de Mínimo e Máximo para {nomeUnidade} devem ser numéricos.";
            }
            // 2. Valida se não são negativos (só faz sentido se forem numéricos)
            else if (valorMinimo < 0 || valorMaximo < 0)
            {
                erroMsg = $"Os valores de Mínimo e Máximo para {nomeUnidade} não podem ser menores que 0.";
            }
            // 3. Valida se min <= max (só faz sentido se não forem negativos)
            else if (valorMinimo > valorMaximo)
            {
                erroMsg = $"O valor Mínimo para {nomeUnidade} não pode ser maior que o valor Máximo .";
            }

            // Se houve um erro (erroMsg foi definido)
            if (erroMsg != null)
            {
                // Desmarca o CheckBox que acionou a validação
                checkBox.Checked = false;

                // Exibe a mensagem de erro
                MessageBox.Show(erroMsg + $"\nA verificação {nomeUnidade} foi desativada.",
                                $"Erro de Validação - {nomeUnidade}",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                minTextBox.Clear(); // Equivalente a minTextBox.Text = "";
                maxTextBox.Clear(); // Equivalente a maxTextBox.Text = "";


                // Opcional: Foca no TextBox com o problema
                if (!minOk || (minOk && valorMinimo < 0) || (minOk && maxOk && valorMinimo > valorMaximo))
                {
                    minTextBox.Focus();
                    minTextBox.SelectAll();
                }
                else if (!maxOk || (maxOk && valorMaximo < 0))
                {
                    maxTextBox.Focus();
                    maxTextBox.SelectAll();
                }

                return false; // Indica que a validação falhou
            }

            return true; // Indica que a validação passou
        }

        private void checkBox_psi_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                ValidarMinMaxCheckBox(cb, textBox9, textBox8, "PSI");
            }
        }

        private void checkBox_gpm_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                ValidarMinMaxCheckBox(cb, textBox14, textBox12, "GPM");
            }
        }

        private void checkBox_bar_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                ValidarMinMaxCheckBox(cb, textBox11, textBox10, "BAR");
            }
        }

        private void checkBox_rotacao_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                ValidarMinMaxCheckBox(cb, textBox18, textBox17, "Rotação (RPM)");
            }
        }

        private void checkBox_lpm_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                ValidarMinMaxCheckBox(cb, textBox16, textBox15, "LPM");
            }
        }

        private void checkBox_temperatura_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                ValidarMinMaxCheckBox(cb, textBox20, textBox19, "Temperatura (°C)");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Tick do timer do cronômetro
            if (circularProgressBar1.Value > 0)
            {
                circularProgressBar1.Value--;
            }
            else
            {
                timer.Stop();
                cronometroIniciado = false; // Permite definir e limpar novamente
                btnParar_Click(this, EventArgs.Empty); // Chama o manipulador do botão Finalizar
            }
        }

        #endregion

        public int ObterValorDefinido()
        {
            return valorDefinido;
        }

        private void tableLayoutPanel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnrelatoriobomba_Click(object sender, EventArgs e)
        {
            // Nota: Você pode precisar passar os dados coletados (_dadosColetados) para o formulário de relatório
            Realatoriobase relatorio = new Realatoriobase(); // Assumindo que Realatoriobase é um formulário
            // Implemente a passagem de dados para o formulário de relatório
            // relatorio.SetReportData(_dadosColetados, Inicioteste, Fimteste, etc.);
            relatorio.Show();
            this.Hide();

        }

        public void dataGridViewLoad()
        {
            // Inicializa as colunas do DataGridView se ainda não foram
            if (dataGridView1.ColumnCount == 0)
            {
                dataGridView1.Size = new Size(852, 174);
                // Número de colunas: Etapa + número de tipos de sensores distintos sendo registrados
                // Baseado nas chaves do formato de dados serial: P1, fluxol, Pilotol, drenol, RPM, temp (6 chaves)
                // Mais quaisquer valores derivados que você queira mostrar explicitamente (PSI, GPM)
                // A exibição solicitada pelo usuário implica mostrar ambas as unidades para alguns sensores.
                // Vamos alinhar com as colunas adicionadas no AtualizarDataGridView original: 11 colunas.
                dataGridView1.ColumnCount = 11;
                dataGridView1.Columns[0].HeaderText = "Etapa";
                dataGridView1.Columns[1].HeaderText = "Pilotagem PSI"; // Derivado de Pilotol
                dataGridView1.Columns[2].HeaderText = "Pilotagem BAR"; // De Pilotol (ou derivado)
                dataGridView1.Columns[3].HeaderText = "Dreno GPM";     // Derivado de drenol
                dataGridView1.Columns[4].HeaderText = "Dreno LPM";     // De drenol
                dataGridView1.Columns[5].HeaderText = "Pressão PSI";   // Derivado de P1
                dataGridView1.Columns[6].HeaderText = "Pressão BAR";   // De P1
                dataGridView1.Columns[7].HeaderText = "Rotação RPM";   // De RPM
                dataGridView1.Columns[8].HeaderText = "Vazão GPM";     // Derivado de fluxol
                dataGridView1.Columns[9].HeaderText = "Vazão LPM";     // De fluxol
                dataGridView1.Columns[10].HeaderText = "Temperatura Celsius"; // De temp

                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            }
        }

        public void AtualizarDataGridView()
        {
            // Obtém os valores atuais dos TextBoxes (que foram atualizados pelo _updateTimer)
            // Usa TryGetValue para acessar os dados de forma segura
            // NOTA: Usando as chaves corrigidas ('l' em vez de '1')
            _currentNumericSensorReadings.TryGetValue("Piloto1", out double piloto1Value); // CORRIGIDO
            _currentNumericSensorReadings.TryGetValue("dreno1", out double dreno1Value); // CORRIGIDO
            _currentNumericSensorReadings.TryGetValue("P1", out double p1Value);
            _currentNumericSensorReadings.TryGetValue("RPM", out double rpmValue);
            _currentNumericSensorReadings.TryGetValue("fluxo1", out double fluxo1Value); // CORRIGIDO
            _currentNumericSensorReadings.TryGetValue("temp", out double tempValue);

            // Aplica conversões para exibição no DataGridView
            string pilotagemPsi = (piloto1Value * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            string pilotagemBar = piloto1Value.ToString("F2", CultureInfo.InvariantCulture); // Assumindo que o bruto é para BAR
                                                                                             // Se o bruto de Pilotol precisar de conversão para BAR usando BAR_CONVERSION_PILOT (1.705), ajuste aqui: pilotagemBar = (piloto1Value * BAR_CONVERSION_PILOT).ToString("F2", CultureInfo.InvariantCulture);

            string drenoGpm = (dreno1Value * LPM_TO_GPM_USER_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            string drenoLpm = dreno1Value.ToString("F2", CultureInfo.InvariantCulture);

            string pressaoPsi = (p1Value * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            string pressaoBar = p1Value.ToString("F2", CultureInfo.InvariantCulture);

            string rotacaoRpm = rpmValue.ToString("F0", CultureInfo.InvariantCulture);
            string vazaoGpm = (fluxo1Value * LPM_TO_GPM_USER_CONVERSION).ToString("F2", CultureInfo.InvariantCulture);
            string vazaoLpm = fluxo1Value.ToString("F2", CultureInfo.InvariantCulture);
            string temperaturaCelsius = tempValue.ToString("F1", CultureInfo.InvariantCulture);


            // Cria uma nova linha com os dados
            string[] novaLinha = new string[]
            {
                $"Etapa {etapaAtual}",
                pilotagemPsi,
                pilotagemBar,
                drenoGpm,
                drenoLpm,
                pressaoPsi,
                pressaoBar,
                rotacaoRpm,
                vazaoGpm,
                vazaoLpm,
                temperaturaCelsius
            };

            // Adiciona a nova linha ao DataGridView de forma segura para a thread da UI
            if (dataGridView1.InvokeRequired)
            {
                dataGridView1.Invoke((MethodInvoker)delegate {
                    dataGridView1.Rows.Add(novaLinha);
                });
            }
            else
            {
                dataGridView1.Rows.Add(novaLinha);
            }
        }

        // Método para o botão Retornar
        private void btnretornar_Click(object sender, EventArgs e)
        {
            // Para timers e porta serial
            StopTimers(); // Para todos os timers
            StopSerialConnection(); // Para comunicação serial e _updateTimer

            // Limpa gráficos
            ClearCharts();
            _viewModel.ResetChartDataLogic();

            // Limpa listas de dados e variáveis
            _timeCounterSeconds = 0; // Reseta contador de tempo
            etapaAtual = 1;
            dadosSensores.Clear(); // Limpa dados para o 'visualizador'
            _dadosColetados.Clear(); // Limpa dados de etapas coletadas
            _latestRawSensorData.Clear(); // Limpa últimos dados serial brutos
            _currentNumericSensorReadings.Clear(); // Limpa últimas leituras numéricas
            Inicioteste = string.Empty;
            Fimteste = string.Empty;
            // Mantém valorDefinidoManualmente como está

            // Limpa o buffer de dados serial
            lock (serialBufferLock)
            {
                serialDataBuffer.Clear();
            }


            // Limpa TextBoxes
            sensor_psi_PL.Text = string.Empty;
            sensor_bar_PL.Text = string.Empty;
            sensor_gpm_DR.Text = string.Empty;
            sensor_lpm_DR.Text = string.Empty;
            sensor_Press_PSI.Text = string.Empty;
            sensor_Press_BAR.Text = string.Empty;
            sensor_rotacao_RPM.Text = string.Empty;
            sensor_Vazao_GPM.Text = string.Empty;
            sensor_Vazao_LPM.Text = string.Empty;
            sensor_Temp_C.Text = string.Empty;

            // Limpa DataGridViews
            dataGridView1.Rows.Clear();
            if (visualizador.DataSource != null)
            {
                var list = visualizador.DataSource as List<SensorData>;
                if (list != null)
                {
                    list.Clear();
                    visualizador.DataSource = null;
                    visualizador.DataSource = list;
                }
                else
                {
                    visualizador.DataSource = null;
                }
            }
            else
            {
                visualizador.Rows.Clear();
            }

            // Reseta ProgressBar
            circularProgressBar1.Value = 0;
            if (valorDefinidoManualmente)
            {
                circularProgressBar1.Maximum = valorDefinido * 60;
            }
            else
            {
                circularProgressBar1.Maximum = 100; // Reseta max se necessário ou define um padrão
            }
            circularProgressBar1.Invalidate();


            // Reseta indicador visual
            _viewModel.FinalizarTesteBomba(Stage_box_bomba); // Assumindo que isso define a imagem do estágio para 'off'
            HistoricalEvents.Text = "AGUARDANDO INÍCIO DO ENSAIO...";
            HistoricalEvents.ForeColor = System.Drawing.Color.DarkGreen;

            // Re-habilita botão Start e desabilita outros
            btniniciarteste.Enabled = true;
            btngravar.Enabled = false;
            bntFinalizar.Enabled = false;
            btnreset.Enabled = false;
            btnrelatoriobomba.Enabled = false;


            // Lógica para retornar ao menu principal ou fechar o formulário
            _fechamentoForcado = true; // Indica que é um fechamento controlado
                                       // Verifica se Menuapp.Instance é acessível ou passa uma referência
                                       // Para simplicidade, mantendo o código existente assumindo Menuapp.Instance está disponível
            if (Menuapp.Instance != null)
            {
                Menuapp.Instance.Show();
            }
            this.Close(); // Fecha o formulário atual
        }
    }
}
