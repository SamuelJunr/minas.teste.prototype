using System.Drawing;
using System;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete; // Assumindo que EtapaData e SensorData estão aqui
using minas.teste.prototype.MVVM.ViewModel;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting; // Required for Chart
using static minas.teste.prototype.MVVM.ViewModel.SimulatedSensorDataGenerator; // Importar o EventArgs customizado do primeiro gerador
using static minas.teste.prototype.MVVM.ViewModel.SimulatedDrainDataGenerator; // Importar o EventArgs customizado do segundo gerador
using static minas.teste.prototype.MVVM.ViewModel.Tela_BombasVM; // Importar Datapoint_Bar_Lpm (se ainda usado no VM)

// Assumindo que estas classes estão em um namespace acessível
// using minas.teste.prototype.MVVM.Model.Concrete; // Para TextBoxTrackBarSynchronizer (se estiver aqui)
// using minas.teste.prototype.MVVM.View; // Para apresentacao (se estiver aqui)


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
        private apresentacao _fechar_box;
        private List<EtapaData> _dadosColetados = new List<EtapaData>();
        public Dictionary<string, TextBox> sensorMap;
        public Dictionary<string, string> sensorMapmedida;
        private List<string> dadosSensoresSelecionados = new List<string>();
        public List<SensorData> dadosSensores;
        private Timer monitoramentoTimer; // Timer para monitoramento de ranges
        private Timer timer; // Timer para o cronômetro
        private TextBoxTrackBarSynchronizer _synchronizerNivel;


        // Instâncias dos geradores de dados simulados
        private SimulatedSensorDataGenerator _dataGenerator; // Para Pressão, Vazão, Rotação, Temperatura
        private SimulatedDrainDataGenerator _drainDataGenerator; // Para Vazão de Dreno, Rotação

        // Contador de tempo para o Chart 5 (em segundos)
        private int _timeCounterSeconds = 0;

        //-------------------------------------//
        //          variaveis GLOBAIS          //
        //-------------------------------------//

        private bool _fechamentoForcado;
        private bool _isMonitoring = false;
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
        private Dictionary<string, string> sensorControlMap;


        // Move the initialization of the static dictionary to the constructor or a method,
        // as static fields cannot reference instance members directly.

        public Tela_Bombas()
        {
            InitializeComponent();
            Inciaizador_listas();

            _viewModel = new Tela_BombasVM();
            _fechar_box = new apresentacao();
            _synchronizerNivel = new TextBoxTrackBarSynchronizer(textBox2, trackBar1, 1, 7);
            dadosSensores = new List<SensorData>();
            timer = new Timer();
            timer.Interval = 1000; // Intervalo de 1 segundo
            timer.Tick += Timer_Tick;

            // --- Chart 1 Initialization (Pressão vs Vazão) ---
            // Se chart1 foi adicionado via designer, você não precisa instanciá-lo aqui.
            // Se não, remova o comentário das duas linhas abaixo e ajuste a posição/tamanho.
            // this.chart1 = new Chart(); // Placeholder: replace with your actual chart control
            // this.Controls.Add(this.chart1); // Placeholder: add to controls if not in designer layout

            // Limpa quaisquer séries e áreas de gráfico padrão existentes no designer para chart1
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            // Adiciona uma área de gráfico principal se não existir (pode já existir no designer) para chart1
            if (chart1.ChartAreas.Count == 0)
            {
                chart1.ChartAreas.Add(new ChartArea());
            }

            // Configura os títulos dos eixos para Pressão (Y) vs Vazão (X) para chart1
            chart1.ChartAreas[0].AxisX.Title = "Vazão (LPM)"; // Eixo X: Vazão
            chart1.ChartAreas[0].AxisY.Title = "Pressão (BAR)"; // Eixo Y: Pressão

            // Opcional: Configurar limites iniciais dos eixos para melhor visualização para chart1
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = 100; // Exemplo: limite máximo para Vazão
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 300; // Exemplo: limite máximo para Pressão em BAR

            // Adiciona uma série para a curva de desempenho em tempo real para chart1
            Series performanceSeries = new Series("Pre.x Vaz."); // Nome da série usado para adicionar pontos e limpar
            performanceSeries.ChartType = SeriesChartType.FastLine; // Usa FastLine para melhor performance em atualizações frequentes
            performanceSeries.Color = Color.Blue; // Define a cor da linha
            performanceSeries.BorderWidth = 2; // Define a espessura da linha
            // Você pode adicionar outras configurações de aparência aqui (marcadores, etc.)
            // performanceSeries.MarkerStyle = MarkerStyle.Circle;
            // performanceSeries.MarkerSize = 5;

            chart1.Series.Add(performanceSeries); // Adiciona a série ao gráfico chart1

            // Título principal do gráfico para chart1
            chart1.Titles.Clear();
            chart1.Titles.Add("Curva de Desempenho Principal");

            // Opcional: Habilitar funcionalidades interativas como zoom e pan para chart1
            chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            // --- End Chart 1 Initialization ---


            // --- Chart 2 Initialization (Vazamento Interno / Dreno - Vazão Dreno vs Rotação) ---
            // Se chart2 foi adicionado via designer, você não precisa instanciá-lo aqui.
            // Se não, remova o comentário das duas linhas abaixo e ajuste a posição/tamanho.
            // this.chart2 = new Chart(); // Placeholder: replace with your actual chart control
            // this.Controls.Add(this.chart2); // Placeholder: add to controls if not in designer layout

            // Limpa quaisquer séries e áreas de gráfico padrão existentes no designer para chart2
            chart2.Series.Clear();
            chart2.ChartAreas.Clear();

            // Adiciona uma área de gráfico principal se não existir (pode já existir no designer) para chart2
            if (chart2.ChartAreas.Count == 0)
            {
                chart2.ChartAreas.Add(new ChartArea());
            }

            // Configura os títulos dos eixos para Vazão de Dreno (Y) vs Rotação (X) para chart2
            chart2.ChartAreas[0].AxisX.Title = "Rotação (RPM)"; // Eixo X: Rotação
            chart2.ChartAreas[0].AxisY.Title = "Dreno (LPM)"; // Eixo Y: Vazão de Dreno

            // Opcional: Configurar limites iniciais dos eixos para melhor visualização para chart2
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisX.Maximum = 3000; // Exemplo: limite máximo para Rotação
            chart2.ChartAreas[0].AxisY.Minimum = 0;
            chart2.ChartAreas[0].AxisY.Maximum = 10; // Exemplo: limite máximo para Vazão de Dreno

            // Adiciona uma série para a curva de vazamento interno em tempo real para chart2
            Series drainRotationSeries = new Series("Vaz.In.X Rot"); // Nome da série usado para adicionar pontos e limpar
            drainRotationSeries.ChartType = SeriesChartType.FastLine; // Usa FastLine para melhor performance
            drainRotationSeries.Color = Color.Red; // Define a cor da linha (exemplo: vermelho para indicar vazamento)
            drainRotationSeries.BorderWidth = 2; // Define a espessura da linha

            chart2.Series.Add(drainRotationSeries); // Adiciona a série ao gráfico chart2

            // Título principal do gráfico para chart2
            chart2.Titles.Clear();
            chart2.Titles.Add("Vazamento Interno / Dreno");

            // Opcional: Habilitar funcionalidades interativas como zoom e pan para chart2
            chart2.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart2.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart2.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart2.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart2.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            // --- End Chart 2 Initialization ---

            // --- Chart 3 Initialization (Vazamento Interno / Dreno - Vazão Dreno vs Pressão Carga) ---
            // Se chart3 foi adicionado via designer, você não precisa instanciá-lo aqui.
            // Se não, remova o comentário das duas linhas abaixo e ajuste a posição/tamanho.
            // this.chart3 = new Chart(); // Placeholder: replace with your actual chart control
            // this.Controls.Add(this.chart3); // Placeholder: add to controls if not in designer layout

            // Limpa quaisquer séries e áreas de gráfico padrão existentes no designer para chart3
            chart3.Series.Clear();
            chart3.ChartAreas.Clear();

            // Adiciona uma área de gráfico principal se não existir (pode já existir no designer) para chart3
            if (chart3.ChartAreas.Count == 0)
            {
                chart3.ChartAreas.Add(new ChartArea());
            }

            // Configura os títulos dos eixos para Vazão de Dreno (Y) vs Pressão de Carga (X) para chart3
            chart3.ChartAreas[0].AxisX.Title = "Pressão de Carga (BAR)"; // Eixo X: Pressão de Carga
            chart3.ChartAreas[0].AxisY.Title = "Dreno (LPM)"; // Eixo Y: Vazão de Dreno

            // Opcional: Configurar limites iniciais dos eixos para melhor visualização para chart3
            chart3.ChartAreas[0].AxisX.Minimum = 0;
            chart3.ChartAreas[0].AxisX.Maximum = 300; // Exemplo: limite máximo para Pressão de Carga
            chart3.ChartAreas[0].AxisY.Minimum = 0;
            chart3.ChartAreas[0].AxisY.Maximum = 10; // Exemplo: limite máximo para Vazão de Dreno

            // Adiciona uma série para a curva de vazamento interno em tempo real para chart3
            Series drainPressureSeries = new Series("Vaz. x Pres."); // Nome da série usado para adicionar pontos e limpar
            drainPressureSeries.ChartType = SeriesChartType.FastLine; // Usa FastLine para melhor performance
            drainPressureSeries.Color = Color.Orange; // Define a cor da linha (exemplo: laranja)
            drainPressureSeries.BorderWidth = 2; // Define a espessura da linha

            chart3.Series.Add(drainPressureSeries); // Adiciona a série ao gráfico chart3

            // Título principal do gráfico para chart3
            chart3.Titles.Clear();
            chart3.Titles.Add("Vazamento (Dreno) do Circuito Fechado");

            // Opcional: Habilitar funcionalidades interativas como zoom e pan para chart3
            chart3.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart3.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart3.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart3.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart3.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart3.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            // --- End Chart 3 Initialization ---

            // --- Chart 4 Initialization (Eficiência vs Vazão) ---
            // Se chart4 foi adicionado via designer, você não precisa instanciá-lo aqui.
            // Se não, remova o comentário das duas linhas abaixo e ajuste a posição/tamanho.
            // this.chart4 = new Chart(); // Placeholder: replace with your actual chart control
            // this.Controls.Add(this.chart4); // Placeholder: add to controls if not in designer layout

            // Limpa quaisquer séries e áreas de gráfico padrão existentes no designer para chart4
            chart4.Series.Clear();
            chart4.ChartAreas.Clear();

            // Adiciona uma área de gráfico principal se não existir (pode já existir no designer) para chart4
            if (chart4.ChartAreas.Count == 0)
            {
                chart4.ChartAreas.Add(new ChartArea());
            }

            // Configura os títulos dos eixos para Eficiência (%) (Y) vs Vazão (LPM) (X) para chart4
            chart4.ChartAreas[0].AxisX.Title = "Vazão (LPM)"; // Eixo X: Vazão
            chart4.ChartAreas[0].AxisY.Title = "Eficiência (%)"; // Eixo Y: Eficiência

            // Opcional: Configurar limites iniciais dos eixos para melhor visualização para chart4
            chart4.ChartAreas[0].AxisX.Minimum = 0;
            chart4.ChartAreas[0].AxisX.Maximum = 100; // Exemplo: limite máximo para Vazão
            chart4.ChartAreas[0].AxisY.Minimum = 0;
            chart4.ChartAreas[0].AxisY.Maximum = 100; // Exemplo: limite máximo para Eficiência em %

            // Adiciona séries para Rendimento Global e Eficiência Volumétrica para chart4
            Series globalEfficiencySeries = new Series("Rend. Global"); // Nome da série para Rendimento Global
            globalEfficiencySeries.ChartType = SeriesChartType.FastLine;
            globalEfficiencySeries.Color = Color.Green; // Cor para Rendimento Global
            globalEfficiencySeries.BorderWidth = 2;
            chart4.Series.Add(globalEfficiencySeries);

            Series volumetricEfficiencySeries = new Series("Ef. Volumetrica"); // Nome da série para Eficiência Volumétrica
            volumetricEfficiencySeries.ChartType = SeriesChartType.FastLine;
            volumetricEfficiencySeries.Color = Color.Purple; // Cor para Eficiência Volumétrica
            volumetricEfficiencySeries.BorderWidth = 2;
            chart4.Series.Add(volumetricEfficiencySeries);


            // Título principal do gráfico para chart4
            chart4.Titles.Clear();
            chart4.Titles.Add("Curvas de Rendimento Global e Volumétrico");

            // Opcional: Habilitar funcionalidades interativas como zoom e pan para chart4
            chart4.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart4.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart4.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart4.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart4.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart4.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            // --- End Chart 4 Initialization ---


            // --- Chart 5 Initialization (Rampa de Amaciamento e Ciclagem) ---
            // Se chart5 foi adicionado via designer, você não precisa instanciá-lo aqui.
            // Se não, remova o comentário das duas linhas abaixo e ajuste a posição/tamanho.
            // this.chart5 = new Chart(); // Placeholder: replace with your actual chart control
            // this.Controls.Add(this.chart5); // Placeholder: add to controls if not in designer layout

            // Limpa quaisquer séries e áreas de gráfico padrão existentes no designer para chart5
            chart5.Series.Clear();
            chart5.ChartAreas.Clear();

            // Adiciona uma área de gráfico principal se não existir (pode já existir no designer) para chart5
            if (chart5.ChartAreas.Count == 0)
            {
                chart5.ChartAreas.Add(new ChartArea());
            }

            // Configura os títulos dos eixos para Tempo (X)
            chart5.ChartAreas[0].AxisX.Title = "Tempo (segundos)"; // Eixo X: Tempo

            // Configura o Eixo Y Primário (Y1) para Temperatura
            chart5.ChartAreas[0].AxisY.Title = "Temperatura (°C)"; // Eixo Y1: Temperatura
            chart5.ChartAreas[0].AxisY.Minimum = 0; // Exemplo: limite mínimo para Temperatura
            chart5.ChartAreas[0].AxisY.Maximum = 100; // Exemplo: limite máximo para Temperatura

            // Adiciona Eixos Y Secundários para Pressão e Vazão
            chart5.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
            chart5.ChartAreas[0].AxisY2.Title = "Pressão (BAR) / Vazão (LPM)"; // Eixo Y2: Pressão/Vazão
            chart5.ChartAreas[0].AxisY2.Minimum = 0; // Exemplo: limite mínimo para Pressão/Vazão
            chart5.ChartAreas[0].AxisY2.Maximum = 300; // Exemplo: limite máximo para Pressão/Vazão (ajustar conforme os dados simulados)
            chart5.ChartAreas[0].AxisY2.LineColor = Color.Red; // Cor diferente para o eixo Y2
            chart5.ChartAreas[0].AxisY2.LabelStyle.ForeColor = Color.Red;
            chart5.ChartAreas[0].AxisY2.TitleForeColor = Color.Red;


            // Adiciona séries para Temperatura, Pressão e Vazão para chart5
            Series temperatureSeries = new Series("Temperatura"); // Nome da série para Temperatura
            temperatureSeries.ChartType = SeriesChartType.FastLine;
            temperatureSeries.Color = Color.Blue; // Cor para Temperatura
            temperatureSeries.BorderWidth = 2;
            temperatureSeries.YAxisType = AxisType.Primary; // Mapeia para o Eixo Y Primário (Temperatura)
            chart5.Series.Add(temperatureSeries);

            Series pressureRampSeries = new Series("Pressão Rampa"); // Nome da série para Pressão
            pressureRampSeries.ChartType = SeriesChartType.FastLine;
            pressureRampSeries.Color = Color.Red; // Cor para Pressão
            pressureRampSeries.BorderWidth = 2;
            pressureRampSeries.YAxisType = AxisType.Secondary; // Mapeia para o Eixo Y Secundário (Pressão/Vazão)
            chart5.Series.Add(pressureRampSeries);

            Series flowRampSeries = new Series("Vazão Rampa"); // Nome da série para Vazão
            flowRampSeries.ChartType = SeriesChartType.FastLine;
            flowRampSeries.Color = Color.Green; // Cor para Vazão
            flowRampSeries.BorderWidth = 2;
            flowRampSeries.YAxisType = AxisType.Secondary; // Mapeia para o Eixo Y Secundário (Pressão/Vazão)
                                                           // Opcional: Ajustar a escala do Eixo Y2 para esta série se as unidades de Pressão e Vazão forem muito diferentes
                                                           // flowRampSeries.YValuesPerPoint = 2; // Poderia ser usado com YValueType adequado e cálculo
            chart5.Series.Add(flowRampSeries);


            // Título principal do gráfico para chart5
            chart5.Titles.Clear();
            chart5.Titles.Add("Rampa de Amaciamento e Ciclagem");

            // Opcional: Habilitar funcionalidades interativas como zoom e pan para chart5
            chart5.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart5.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart5.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart5.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart5.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart5.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart5.ChartAreas[0].AxisY2.ScaleView.Zoomable = true;
            // --- End Chart 5 Initialization ---

            // --- Chart 6 Initialization (Curva de Deslocamento / Vazão Real) ---
            // Se chart6 foi adicionado via designer, você não precisa instanciá-lo aqui.
            // Se não, remova o comentário das duas linhas abaixo e ajuste a posição/tamanho.
            // this.chart6 = new Chart(); // Placeholder: replace with your actual chart control
            // this.Controls.Add(this.chart6); // Placeholder: add to controls if not in designer layout

            // Limpa quaisquer séries e áreas de gráfico padrão existentes no designer para chart6
            chart6.Series.Clear();
            chart6.ChartAreas.Clear();

            // Adiciona uma área de gráfico principal se não existir (pode já existir no designer) para chart6
            if (chart6.ChartAreas.Count == 0)
            {
                chart6.ChartAreas.Add(new ChartArea());
            }

            // Configura os títulos dos eixos para Rotação (X) vs Vazão Real (Y) para chart6
            chart6.ChartAreas[0].AxisX.Title = "Rotação (RPM)"; // Eixo X: Rotação
            chart6.ChartAreas[0].AxisY.Title = "Vazão Real (LPM)"; // Eixo Y: Vazão Real (Vazão Principal)

            // Opcional: Configurar limites iniciais dos eixos para melhor visualização para chart6
            chart6.ChartAreas[0].AxisX.Minimum = 0;
            chart6.ChartAreas[0].AxisX.Maximum = 3000; // Exemplo: limite máximo para Rotação
            chart6.ChartAreas[0].AxisY.Minimum = 0;
            chart6.ChartAreas[0].AxisY.Maximum = 100; // Exemplo: limite máximo para Vazão Real

            // Adiciona uma série para a curva de Vazão Real em tempo real para chart6
            Series realFlowSeries = new Series("Vazão Real"); // Nome da série para Vazão Real
            realFlowSeries.ChartType = SeriesChartType.FastLine;
            realFlowSeries.Color = Color.Blue; // Cor para Vazão Real
            realFlowSeries.BorderWidth = 2;
            chart6.Series.Add(realFlowSeries);


            // Título principal do gráfico para chart6
            chart6.Titles.Clear();
            chart6.Titles.Add("Curva de Deslocamento / Vazão Real");

            // Opcional: Habilitar funcionalidades interativas como zoom e pan para chart6
            chart6.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart6.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart6.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart6.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            chart6.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
            chart6.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            // --- End Chart 6 Initialization ---


            // --- Simulated Data Generators Initialization ---
            _dataGenerator = new SimulatedSensorDataGenerator();
            // Assina o evento DataUpdated do primeiro gerador (Pressão, Vazão Principal, Rotação, Temperatura)
            _dataGenerator.DataUpdated += DataGenerator_DataUpdated;

            _drainDataGenerator = new SimulatedDrainDataGenerator();
            // Assina o evento DataUpdated do segundo gerador (Vazão de Dreno, Rotação)
            _drainDataGenerator.DataUpdated += DrainDataGenerator_DataUpdated;
            // --- End Simulated Data Generators Initialization ---

        }

        public void Inciaizador_listas()
        {

            sensorMap = new Dictionary<string, TextBox>();
            sensorMap.Add("psi_PL", sensor_psi_PL);
            sensorMap.Add("bar_PL", sensor_bar_PL);
            sensorMap.Add("gpm_DR", sensor_gpm_DR);
            sensorMap.Add("lpm_DR", sensor_lpm_DR);
            sensorMap.Add("Press_PSI", sensor_Press_PSI);
            sensorMap.Add("Press_BAR", sensor_Press_BAR);
            sensorMap.Add("rotacao_RPM", sensor_rotacao_RPM);
            sensorMap.Add("Vazao_GPM", sensor_Vazao_GPM);
            sensorMap.Add("Vazao_LPM", sensor_Vazao_LPM);
            sensorMap.Add("Temp_C", sensor_Temp_C);

            sensorMapmedida = new Dictionary<string, string>();
            sensorMapmedida.Add("psi_PL", "psi");
            sensorMapmedida.Add("bar_PL", "bar");
            sensorMapmedida.Add("gpm_DR", "gpm");
            sensorMapmedida.Add("lpm_DR", "lpm");
            sensorMapmedida.Add("Press_PSI", "psi");
            sensorMapmedida.Add("Press_BAR", "bar");
            sensorMapmedida.Add("rotacao_RPM", "rpm");
            sensorMapmedida.Add("Vazao_GPM", "gpm");
            sensorMapmedida.Add("Vazao_LPM", "lpm");
            sensorMapmedida.Add("Temp_C", "celsus");

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


            dadosSensoresSelecionados.AddRange(sensorMap.Keys);

        }

        // Removido o método inicializarchart1() duplicado


        #region LOADS_JANELA
        private void Tela_Bombas_Load(object sender, EventArgs e)
        {
            _viewModel.Carregar_configuracao(this); // Carrega o estilo do formulário
            _viewModel.Stage_signal(Stage_box_bomba);
            _viewModel.VincularRelogioLabel(LabelHorariotela);// configura a imagem de teste ligado ou desligado
            HistoricalEvents.Text = "AGUARDANDO INÍCIO DO ENSAIO...";
            HistoricalEvents.ForeColor = System.Drawing.Color.DarkGreen;// Carrega o histórico de eventos
            btngravar.Enabled = false;
            bntFinalizar.Enabled = false;
            btnreset.Enabled = false;
            btnrelatoriobomba.Enabled = false;


        }

        #endregion

        #region EVENTOS_FECHAMANETO
        private void CloseWindows_Click(object sender, EventArgs e)
        {
            _fechamentoForcado = true; // Indica que é um fechamento controlado
            // Para os geradores de dados simulados antes de fechar
            _dataGenerator?.Stop();
            _drainDataGenerator?.Stop();

            Menuapp.Instance.Show();
            this.Close();
        }

        private void Tela_Bombas_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Para os geradores de dados simulados e libera recursos
            _dataGenerator?.Stop();
            _dataGenerator?.Dispose();
            _drainDataGenerator?.Stop();
            _drainDataGenerator?.Dispose();


            // Só encerra a aplicação se não for um fechamento controlado
            if (!_fechamentoForcado)
            {
                _fechar_box.apresentacao_FormClosing(sender, e);
            }
            else
                Menuapp.Instance.Show();

        }
        #endregion

        #region INCIO_TESTE
        public async void btnIniciar_Click(object sender, EventArgs e)
        {
            if (!_viewModel.cabecalhoinicial(textBox6, textBox5, textBox4))
            {
                MessageBox.Show("Favor preencher os campos obrigatórios em DADOS DE ENSAIO.");
                await _viewModel.PiscarLabelsVermelho(label6, label5, label4, 1000);
                return;
            }

            _isMonitoring = true;
            _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5, panel2, panel6, panel11, panel9);
            Inicioteste = DateTime.Now.ToString(); // Certifique-se de que Inicioteste é uma string acessível
            _viewModel.IniciarTesteBomba(Stage_box_bomba);
            //trackBar1.Enabled = false;
            InicializarMonitoramento(); // Inicia o timer de monitoramento de ranges

            // Reinicia o contador de tempo para o Chart 5
            _timeCounterSeconds = 0;


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
            btngravar.Enabled = true;
            bntFinalizar.Enabled = true;
            btnreset.Enabled = true;
            btnrelatoriobomba.Enabled = true;
            btniniciarteste.Enabled = false;
            HistoricalEvents.Text = "INICIADO ENSAIO DE BOMBAS";

            // --- Chart 1 Start Logic ---
            // Remove e recria a série para limpar o gráfico.
            string chart1SeriesName = "Pre.x Vaz.";
            if (chart1.Series.IsUniqueName(chart1SeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart1.Series.Remove(chart1.Series[chart1SeriesName]);
            }
            if (chart1.Series.IsUniqueName(chart1SeriesName)) // Agora IsUniqueName deve ser true após a remoção (se existia)
            {
                Series performanceSeries = new Series(chart1SeriesName);
                performanceSeries.ChartType = SeriesChartType.FastLine;
                performanceSeries.Color = Color.Blue;
                performanceSeries.BorderWidth = 2;
                chart1.Series.Add(performanceSeries);
            }
            _viewModel.ResetChartDataLogic(); // Reset the previous rotation in VM (lógica específica do VM para chart1)
            // --- End Chart 1 Start Logic ---

            // --- Chart 2 Start Logic ---
            // Remove e recria a série para limpar o gráfico.
            string chart2SeriesName = "Vaz.In.X Rot";
            if (chart2.Series.IsUniqueName(chart2SeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart2.Series.Remove(chart2.Series[chart2SeriesName]);
            }
            if (chart2.Series.IsUniqueName(chart2SeriesName)) // Agora IsUniqueName deve ser true
            {
                Series drainRotationSeries = new Series(chart2SeriesName);
                drainRotationSeries.ChartType = SeriesChartType.FastLine;
                drainRotationSeries.Color = Color.Red;
                drainRotationSeries.BorderWidth = 2;
                chart2.Series.Add(drainRotationSeries);
            }
            // --- End Chart 2 Start Logic ---

            // --- Chart 3 Start Logic ---
            // Remove e recria a série para limpar o gráfico.
            string chart3SeriesName = "Vaz. x Pres.";
            if (chart3.Series.IsUniqueName(chart3SeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart3.Series.Remove(chart3.Series[chart3SeriesName]);
            }
            if (chart3.Series.IsUniqueName(chart3SeriesName)) // Agora IsUniqueName deve ser true
            {
                Series drainPressureSeries = new Series(chart3SeriesName);
                drainPressureSeries.ChartType = SeriesChartType.FastLine;
                drainPressureSeries.Color = Color.Orange;
                drainPressureSeries.BorderWidth = 2;
                chart3.Series.Add(drainPressureSeries);
            }
            // --- End Chart 3 Start Logic ---

            // --- Chart 4 Start Logic ---
            // Remove e recria as séries para limpar o gráfico de eficiência.
            string chart4GlobalSeriesName = "Rend. Global";
            if (chart4.Series.IsUniqueName(chart4GlobalSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart4.Series.Remove(chart4.Series[chart4GlobalSeriesName]);
            }
            if (chart4.Series.IsUniqueName(chart4GlobalSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series globalEfficiencySeries = new Series(chart4GlobalSeriesName);
                globalEfficiencySeries.ChartType = SeriesChartType.FastLine;
                globalEfficiencySeries.Color = Color.Green;
                globalEfficiencySeries.BorderWidth = 2;
                chart4.Series.Add(globalEfficiencySeries);
            }

            string chart4VolumetricSeriesName = "Ef. Volumetrica";
            if (chart4.Series.IsUniqueName(chart4VolumetricSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart4.Series.Remove(chart4.Series[chart4VolumetricSeriesName]);
            }
            if (chart4.Series.IsUniqueName(chart4VolumetricSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series volumetricEfficiencySeries = new Series(chart4VolumetricSeriesName);
                volumetricEfficiencySeries.ChartType = SeriesChartType.FastLine;
                volumetricEfficiencySeries.Color = Color.Purple;
                volumetricEfficiencySeries.BorderWidth = 2;
                chart4.Series.Add(volumetricEfficiencySeries);
            }
            // Não há lógica de reset específica do VM para chart4 neste momento.
            // --- End Chart 4 Start Logic ---

            // --- Chart 5 Start Logic ---
            // Remove e recria as séries para limpar o gráfico de rampa.
            string chart5TemperatureSeriesName = "Temperatura";
            if (chart5.Series.IsUniqueName(chart5TemperatureSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart5.Series.Remove(chart5.Series[chart5TemperatureSeriesName]);
            }
            if (chart5.Series.IsUniqueName(chart5TemperatureSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series temperatureSeries = new Series(chart5TemperatureSeriesName);
                temperatureSeries.ChartType = SeriesChartType.FastLine;
                temperatureSeries.Color = Color.Blue;
                temperatureSeries.BorderWidth = 2;
                temperatureSeries.YAxisType = AxisType.Primary; // Mapeia para o Eixo Y Primário (Temperatura)
                chart5.Series.Add(temperatureSeries);
            }

            string chart5PressureSeriesName = "Pressão Rampa";
            if (chart5.Series.IsUniqueName(chart5PressureSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart5.Series.Remove(chart5.Series[chart5PressureSeriesName]);
            }
            if (chart5.Series.IsUniqueName(chart5PressureSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series pressureRampSeries = new Series(chart5PressureSeriesName);
                pressureRampSeries.ChartType = SeriesChartType.FastLine;
                pressureRampSeries.Color = Color.Red;
                pressureRampSeries.BorderWidth = 2;
                pressureRampSeries.YAxisType = AxisType.Secondary; // Mapeia para o Eixo Y Secundário (Pressão/Vazão)
                chart5.Series.Add(pressureRampSeries);
            }

            string chart5FlowSeriesName = "Vazão Rampa";
            if (chart5.Series.IsUniqueName(chart5FlowSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart5.Series.Remove(chart5.Series[chart5FlowSeriesName]);
            }
            if (chart5.Series.IsUniqueName(chart5FlowSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series flowRampSeries = new Series(chart5FlowSeriesName);
                flowRampSeries.ChartType = SeriesChartType.FastLine;
                flowRampSeries.Color = Color.Green;
                flowRampSeries.BorderWidth = 2;
                flowRampSeries.YAxisType = AxisType.Secondary; // Mapeia para o Eixo Y Secundário (Pressão/Vazão)
                                                               // Opcional: Ajustar a escala do Eixo Y2 para esta série se as unidades de Pressão e Vazão forem muito diferentes
                                                               // flowRampSeries.YValuesPerPoint = 2; // Poderia ser usado com YValueType adequado e cálculo
                chart5.Series.Add(flowRampSeries);
            }
            // --- End Chart 5 Start Logic ---

            // --- Chart 6 Start Logic ---
            // Remove e recria a série para limpar o gráfico de vazão real.
            string chart6RealFlowSeriesName = "Vazão Real";
            if (chart6.Series.IsUniqueName(chart6RealFlowSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart6.Series.Remove(chart6.Series[chart6RealFlowSeriesName]);
            }
            if (chart6.Series.IsUniqueName(chart6RealFlowSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series realFlowSeries = new Series(chart6RealFlowSeriesName);
                realFlowSeries.ChartType = SeriesChartType.FastLine;
                realFlowSeries.Color = Color.Blue;
                realFlowSeries.BorderWidth = 2;
                chart6.Series.Add(realFlowSeries);
            }
            // --- End Chart 6 Start Logic ---


            // --- Simulated Data Generators Start ---
            _dataGenerator.Start(); // Inicia a geração de dados simulados para chart1, chart5, chart6 (Pressão, Vazão, Rotação, Temperatura)
            _drainDataGenerator.Start(); // Inicia a geração de dados simulados para chart2, chart3 (Vazão Dreno, Rotação)
            // --- End Simulated Data Generators Start ---
        }

        private void InicializarMonitoramento()
        {
            monitoramentoTimer = new System.Windows.Forms.Timer();
            monitoramentoTimer.Interval = 1000; // Intervalo de 1 segundo
            monitoramentoTimer.Tick += MonitoramentoTimer_Tick;
            monitoramentoTimer.Start();
        }

        private void PararMonitoramento()
        {
            if (monitoramentoTimer != null && monitoramentoTimer.Enabled)
            {
                monitoramentoTimer.Stop();
                monitoramentoTimer.Dispose();
                monitoramentoTimer = null;
            }
        }

        // Este método agora apenas chama a lógica de monitoramento do ViewModel
        // A alimentação dos gráficos é feita nos DataGenerator_DataUpdated e DrainDataGenerator_DataUpdated
        private void MonitoramentoTimer_Tick(object sender, EventArgs e)
        {
            // Obtenha os valores dos controles na View
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

            // A lógica de alimentação dos gráficos foi movida para os manipuladores de eventos dos geradores
        }

        // Handler para o evento DataUpdated do SimulatedSensorDataGenerator (Chart 1, Chart 4, Chart 5, Chart 6)
        private void DataGenerator_DataUpdated(object sender, SensorDataEventArgs e)
        {
            // Como este evento é disparado em um thread diferente da UI,
            // é necessário usar Invoke para atualizar os controles da UI de forma segura.
            if (sensor_Press_BAR.InvokeRequired)
            {
                sensor_Press_BAR.Invoke((MethodInvoker)delegate {
                    // Atualiza os TextBoxes com os dados simulados do primeiro gerador
                    sensor_Press_BAR.Text = e.PressureBar.ToString("F2", CultureInfo.InvariantCulture); // Formata com 2 casas decimais
                    sensor_Vazao_LPM.Text = e.FlowLpm.ToString("F2", CultureInfo.InvariantCulture);
                    sensor_rotacao_RPM.Text = e.RotationRpm.ToString("F2", CultureInfo.InvariantCulture); // Atualiza Rotação (para consistência)
                    sensor_Temp_C.Text = e.TemperatureC.ToString("F2", CultureInfo.InvariantCulture); // Atualiza Temperatura

                    // Incrementa o contador de tempo para Chart 5
                    _timeCounterSeconds++;


                    // Chama a lógica do ViewModel para verificar a rotação e obter o ponto do gráfico 1
                    // Esta lógica ainda usa Pressão e Vazão Principal, então mantemos a chamada aqui.
                    Datapoint_Bar_Lpm? dataPoint = _viewModel.GetChartDataIfRotationConstant(
                        sensor_Press_BAR.Text,
                        sensor_Vazao_LPM.Text,
                        sensor_rotacao_RPM.Text // Usa o valor atual do TextBox de Rotação
                    );

                    if (dataPoint.HasValue)
                    {
                        // Adiciona o ponto ao gráfico 1
                        // Usando !IsUniqueName para verificar se a série existe (comportamento similar ao Contains)
                        if (!chart1.Series.IsUniqueName("Pre.x Vaz."))
                        {
                            // Note que a ordem dos eixos X e Y aqui é Pressão (BAR) no X e Vazão (LPM) no Y,
                            // conforme a inicialização do chart1.
                            chart1.Series["Pre.x Vaz."].Points.AddXY(dataPoint.Value.PressureBar, dataPoint.Value.FlowLpm);
                        }
                    }

                    // --- Lógica de cálculo e adição de dados para Chart 4 (Eficiência) ---
                    // Obter valores atuais dos TextBoxes (já atualizados neste Invoke)
                    if (double.TryParse(sensor_Vazao_LPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double vazaoLpm) &&
                        double.TryParse(sensor_Press_BAR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double pressaoBar) &&
                        double.TryParse(sensor_rotacao_RPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double rotacaoRpm) &&
                        double.TryParse(sensor_lpm_DR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double vazaoDrenoLpm))
                    {
                        // Calcular Eficiência Volumetrica (simplificada)
                        // Evita divisão por zero se vazaoLpm e vazaoDrenoLpm forem ambos zero
                        double eficienciaVolumetrica = 0;
                        if (vazaoLpm + vazaoDrenoLpm > 0)
                        {
                            eficienciaVolumetrica = (vazaoLpm / (vazaoLpm + vazaoDrenoLpm)) * 100;
                        }
                        // Garante que a eficiência não exceda 100% devido a simulação
                        if (eficienciaVolumetrica > 100) eficienciaVolumetrica = 100;
                        if (eficienciaVolumetrica < 0) eficienciaVolumetrica = 0; // Eficiência não pode ser negativa

                        // Calcular Rendimento Global (simplificado)
                        // Esta é uma fórmula muito simplificada, pois não temos Torque de entrada.
                        // Uma abordagem comum seria (Potência Hidráulica de Saída / Potência Mecânica de Entrada) * 100
                        // Potência Hidráulica de Saída (kW) = (Pressão em bar * Vazão em LPM) / 600
                        // Potência Mecânica de Entrada (kW) = (Torque em Nm * Rotação em rad/s) / 1000
                        // Como não temos Torque, vamos usar uma relação simplificada que diminui com a pressão/vazão.
                        // Exemplo: Rendimento Global = Eficiência Volumetrica * Fator de Perda Mecânica
                        // Fator de Perda Mecânica pode ser uma função da pressão ou rotação.
                        // Para simulação, vamos usar uma fórmula que diminui com a pressão e tem um limite máximo.
                        double rendimentoGlobal = eficienciaVolumetrica * (1 - (pressaoBar / 400.0)); // Fator de perda simplificado
                        if (rendimentoGlobal < 0) rendimentoGlobal = 0; // Rendimento não pode ser negativo
                        if (rendimentoGlobal > eficienciaVolumetrica) rendimentoGlobal = eficienciaVolumetrica; // Rendimento Global <= Eficiência Volumetrica


                        // Adiciona os pontos aos gráficos de eficiência (Chart 4)
                        // O eixo X é Vazão (LPM), eixo Y é Eficiência (%)
                        // Usando !IsUniqueName para verificar se a série existe
                        if (!chart4.Series.IsUniqueName("Rend. Global"))
                        {
                            chart4.Series["Rend. Global"].Points.AddXY(vazaoLpm, rendimentoGlobal);
                        }
                        if (!chart4.Series.IsUniqueName("Ef. Volumetrica"))
                        {
                            chart4.Series["Ef. Volumetrica"].Points.AddXY(vazaoLpm, eficienciaVolumetrica);
                        }
                    }
                    // --- Fim Lógica de cálculo e adição de dados para Chart 4 ---

                    // --- Lógica de adição de dados para Chart 5 (Rampa de Amaciamento) ---
                    // Obter valor de Temperatura (já atualizado neste Invoke)
                    if (double.TryParse(sensor_Temp_C.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double temperaturaC))
                    {
                        // Adiciona pontos para Temperatura, Pressão e Vazão vs Tempo
                        // CORRIGIDO: Usando os nomes corretos das séries para Chart 5
                        if (!chart5.Series.IsUniqueName("Temperatura"))
                        {
                            chart5.Series["Temperatura"].Points.AddXY(_timeCounterSeconds, temperaturaC);
                        }
                        if (!chart5.Series.IsUniqueName("Pressão Rampa") && double.TryParse(sensor_Press_BAR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double pressaoRampaBar))
                        {
                            chart5.Series["Pressão Rampa"].Points.AddXY(_timeCounterSeconds, pressaoRampaBar);
                        }
                        if (!chart5.Series.IsUniqueName("Vazão Rampa") && double.TryParse(sensor_Vazao_LPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double vazaoRampaLpm))
                        {
                            chart5.Series["Vazão Rampa"].Points.AddXY(_timeCounterSeconds, vazaoRampaLpm);
                        }
                    }
                    // --- End Lógica de adição de dados para Chart 5 ---

                    // --- Lógica de adição de dados para Chart 6 (Vazão Real vs Rotação) ---
                    // Obter valores de Rotação e Vazão (já atualizados neste Invoke)
                    if (double.TryParse(sensor_rotacao_RPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double rotacaoRPM) &&
                        double.TryParse(sensor_Vazao_LPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double vazaoRealLpm))
                    {
                        // Adiciona ponto para Vazão Real vs Rotação
                        // Usando !IsUniqueName para verificar se a série existe
                        if (!chart6.Series.IsUniqueName("Vazão Real"))
                        {
                            chart6.Series["Vazão Real"].Points.AddXY(rotacaoRPM, vazaoRealLpm);
                        }
                    }
                    // --- End Lógica de adição de dados para Chart 6 ---


                });
            }
            else
            {
                // Se já estiver no thread da UI (improvável para System.Timers.Timer, mas boa prática)
                sensor_Press_BAR.Text = e.PressureBar.ToString("F2", CultureInfo.InvariantCulture);
                sensor_Vazao_LPM.Text = e.FlowLpm.ToString("F2", CultureInfo.InvariantCulture);
                sensor_rotacao_RPM.Text = e.RotationRpm.ToString("F2", CultureInfo.InvariantCulture);
                sensor_Temp_C.Text = e.TemperatureC.ToString("F2", CultureInfo.InvariantCulture);

                // Incrementa o contador de tempo para Chart 5
                _timeCounterSeconds++;

                Datapoint_Bar_Lpm? dataPoint = _viewModel.GetChartDataIfRotationConstant(
                    sensor_Press_BAR.Text,
                    sensor_Vazao_LPM.Text,
                    sensor_rotacao_RPM.Text // Usa o valor atual do TextBox de Rotação
                );

                if (dataPoint.HasValue)
                {
                    // Adiciona o ponto ao gráfico 1
                    // Usando !IsUniqueName para verificar se a série existe
                    if (!chart1.Series.IsUniqueName("Pre.x Vaz."))
                    {
                        // Note que a ordem dos eixos X e Y aqui é Pressão (BAR) no X e Vazão (LPM) no Y,
                        // conforme a inicialização do chart1.
                        chart1.Series["Pre.x Vaz."].Points.AddXY(dataPoint.Value.PressureBar, dataPoint.Value.FlowLpm);
                    }
                }

                // --- Lógica de cálculo e adição de dados para Chart 4 (Eficiência) ---
                // Obter valores atuais dos TextBoxes
                if (double.TryParse(sensor_Vazao_LPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double vazaoLpm) &&
                    double.TryParse(sensor_Press_BAR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double pressaoBar) &&
                    double.TryParse(sensor_rotacao_RPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double rotacaoRpm) &&
                    double.TryParse(sensor_lpm_DR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double vazaoDrenoLpm))
                {
                    // Calcular Eficiência Volumetrica (simplificada)
                    double eficienciaVolumetrica = 0;
                    if (vazaoLpm + vazaoDrenoLpm > 0)
                    {
                        eficienciaVolumetrica = (vazaoLpm / (vazaoLpm + vazaoDrenoLpm)) * 100;
                    }
                    if (eficienciaVolumetrica > 100) eficienciaVolumetrica = 100;
                    if (eficienciaVolumetrica < 0) eficienciaVolumetrica = 0;

                    // Calcular Rendimento Global (simplificada)
                    double rendimentoGlobal = eficienciaVolumetrica * (1 - (pressaoBar / 400.0));
                    if (rendimentoGlobal < 0) rendimentoGlobal = 0;
                    if (rendimentoGlobal > eficienciaVolumetrica) rendimentoGlobal = eficienciaVolumetrica;


                    // Adiciona os pontos aos gráficos de eficiência (Chart 4)
                    // Usando !IsUniqueName para verificar se a série existe
                    if (!chart4.Series.IsUniqueName("Rend. Global"))
                    {
                        chart4.Series["Rend. Global"].Points.AddXY(vazaoLpm, rendimentoGlobal);
                    }
                    if (!chart4.Series.IsUniqueName("Ef. Volumetrica"))
                    {
                        chart4.Series["Ef. Volumetrica"].Points.AddXY(vazaoLpm, eficienciaVolumetrica);
                    }
                }
                // --- Fim Lógica de cálculo e adição de dados para Chart 4 ---

                // --- Lógica de adição de dados para Chart 5 (Rampa de Amaciamento) ---
                // Obter valor de Temperatura (já atualizado neste handler)
                if (double.TryParse(sensor_Temp_C.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double temperaturaC))
                {
                    // Adiciona pontos para Temperatura, Pressão e Vazão vs Tempo
                    // CORRIGIDO: Usando os nomes corretos das séries para Chart 5
                    if (!chart5.Series.IsUniqueName("Temperatura"))
                    {
                        chart5.Series["Temperatura"].Points.AddXY(_timeCounterSeconds, temperaturaC);
                    }
                    if (!chart5.Series.IsUniqueName("Pressão Rampa") && double.TryParse(sensor_Press_BAR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double pressaoRampaBar))
                    {
                        chart5.Series["Pressão Rampa"].Points.AddXY(_timeCounterSeconds, pressaoRampaBar);
                    }
                    if (!chart5.Series.IsUniqueName("Vazão Rampa") && double.TryParse(sensor_Vazao_LPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double vazaoRampaLpm))
                    {
                        chart5.Series["Vazão Rampa"].Points.AddXY(_timeCounterSeconds, vazaoRampaLpm);
                    }
                }
                // --- End Lógica de adição de dados para Chart 5 ---

                // --- Lógica de adição de dados para Chart 6 (Vazão Real vs Rotação) ---
                // Obter valores de Rotação e Vazão (já atualizados neste handler)
                if (double.TryParse(sensor_rotacao_RPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double rotacaoRPM) &&
                    double.TryParse(sensor_Vazao_LPM.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double vazaoRealLpm))
                {
                    // Adiciona ponto para Vazão Real vs Rotação
                    // Usando !IsUniqueName para verificar se a série existe
                    if (!chart6.Series.IsUniqueName("Vazão Real"))
                    {
                        chart6.Series["Vazão Real"].Points.AddXY(rotacaoRPM, vazaoRealLpm);
                    }
                }
                // --- End Lógica de adição de dados para Chart 6 ---

            }
        }

        // Handler para o evento DataUpdated do SimulatedDrainDataGenerator (Chart 2 e Chart 3)
        private void DrainDataGenerator_DataUpdated(object sender, DrainDataEventArgs e)
        {
            // Como este evento é disparado em um thread diferente da UI,
            // é necessário usar Invoke para atualizar os controles da UI de forma segura.
            if (sensor_lpm_DR.InvokeRequired)
            {
                sensor_lpm_DR.Invoke((MethodInvoker)delegate {
                    // Atualiza os TextBoxes com os dados simulados do segundo gerador
                    sensor_lpm_DR.Text = e.DrainFlowLpm.ToString("F2", CultureInfo.InvariantCulture); // Vazão de Dreno
                    // sensor_rotacao_RPM.Text é atualizado pelo DataGenerator_DataUpdated para consistência
                    // sensor_rotacao_RPM.Text = e.RotationRpm.ToString("F2", CultureInfo.InvariantCulture); // Rotação - Removido para evitar conflito


                    // --- Atualiza Chart 2 (Vazão de Dreno vs Rotação) ---
                    // Não precisamos de uma lógica de rotação constante específica aqui para adicionar pontos.
                    // Usando !IsUniqueName para verificar se a série existe
                    if (!chart2.Series.IsUniqueName("Vaz.In.X Rot"))
                    {
                        // Adiciona Rotação no eixo X e Vazão de Dreno no eixo Y
                        // Usando o valor de Rotação que veio neste evento (do DrainDataGenerator)
                        chart2.Series["Vaz.In.X Rot"].Points.AddXY(e.RotationRpm, e.DrainFlowLpm);
                    }


                    // --- Atualiza Chart 3 (Vazão de Dreno vs Pressão de Carga) ---
                    // Usando !IsUniqueName para verificar se a série existe
                    // Precisamos ler o valor atual da Pressão de Carga do TextBox sensor_Press_BAR.
                    if (!chart3.Series.IsUniqueName("Vaz. x Pres."))
                    {
                        // Tenta converter o texto da Pressão de Carga para double
                        if (double.TryParse(sensor_Press_BAR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double pressureBar))
                        {
                            // Adiciona Pressão de Carga (BAR) no eixo X e Vazão de Dreno (LPM) no eixo Y
                            chart3.Series["Vaz. x Pres."].Points.AddXY(pressureBar, e.DrainFlowLpm); // Usa Pressão do TextBox e Vazão de Dreno do EventArgs
                        }
                        // Se a conversão falhar, o ponto não é adicionado para evitar erros.
                    }

                    // --- Lógica de cálculo e adição de dados para Chart 4 (Eficiência) ---
                    // Esta lógica de cálculo e adição de pontos para chart4 foi movida para o DataGenerator_DataUpdated.
                    // --- Fim Lógica de cálculo e adição de dados para Chart 4 ---

                    // --- Lógica de adição de dados para Chart 5 (Rampa de Amaciamento) ---
                    // Esta lógica de adição de pontos para chart5 foi movida para o DataGenerator_DataUpdated.
                    // --- End Lógica de adição de dados para Chart 5 ---

                    // --- Lógica de adição de dados para Chart 6 (Vazão Real vs Rotação) ---
                    // Esta lógica de adição de pontos para chart6 foi movida para o DataGenerator_DataUpdated.
                    // --- End Lógica de adição de dados para Chart 6 ---


                });
            }
            else
            {
                // Se já estiver no thread da UI
                sensor_lpm_DR.Text = e.DrainFlowLpm.ToString("F2", CultureInfo.InvariantCulture);
                // sensor_rotacao_RPM.Text = e.RotationRpm.ToString("F2", CultureInfo.InvariantCulture); // Removido para evitar conflito

                // --- Atualiza Chart 2 (Vazão de Dreno vs Rotação) ---
                // Usando !IsUniqueName para verificar se a série existe
                if (!chart2.Series.IsUniqueName("Vaz.In.X Rot"))
                {
                    chart2.Series["Vaz.In.X Rot"].Points.AddXY(e.RotationRpm, e.DrainFlowLpm);
                }


                // --- Atualiza Chart 3 (Vazão de Dreno vs Pressão de Carga) ---
                // Usando !IsUniqueName para verificar se a série existe
                // Precisamos ler o valor atual da Pressão de Carga do TextBox sensor_Press_BAR.
                if (!chart3.Series.IsUniqueName("Vaz. x Pres."))
                {
                    // Tenta converter o texto da Pressão de Carga para double
                    if (double.TryParse(sensor_Press_BAR.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double pressureBar))
                    {
                        // Adiciona Pressão de Carga (BAR) no eixo X e Vazão de Dreno (LPM) no eixo Y
                        chart3.Series["Vaz. x Pres."].Points.AddXY(pressureBar, e.DrainFlowLpm); // Usa Pressão do TextBox e Vazão de Dreno do EventArgs
                    }
                    // Se a conversão falhar, o ponto não é adicionado para evitar erros.
                }

                // --- Lógica de cálculo e adição de dados para Chart 4 (Eficiência) ---
                // Mantida apenas no DataGenerator_DataUpdated.
                // --- Fim Lógica de cálculo e adição de dados para Chart 4 ---

                // --- Lógica de adição de dados para Chart 5 (Rampa de Amaciamento) ---
                // Mantida apenas no DataGenerator_DataUpdated.
                // --- End Lógica de adição de dados para Chart 5 ---

                // --- Lógica de adição de dados para Chart 6 (Vazão Real vs Rotação) ---
                // Mantida apenas no DataGenerator_DataUpdated.
                // --- End Lógica de adição de dados para Chart 6 ---
            }
        }


        #endregion

        #region FIM_TESTE
        private void btnParar_Click(object sender, EventArgs e)
        {
            timer.Stop(); // Para o timer do cronômetro
            cronometroIniciado = false;
            _isMonitoring = false;
            _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5, panel2, panel6, panel11, panel9);
            Fimteste = DateTime.Now.ToString();
            trackBar1.Enabled = true;
            PararMonitoramento(); // Stops monitoramentoTimer

            // --- Simulated Data Generators Stop ---
            // Ao parar, a atualização dos gráficos cessa automaticamente
            _dataGenerator.Stop(); // Para a geração de dados simulados para chart1, chart4, chart5, chart6
            _drainDataGenerator.Stop(); // Para a geração de dados simulados para chart2, chart3
            // Os pontos existentes nos gráficos permanecem visíveis
            // --- End Simulated Data Generators Stop ---

            _viewModel.FinalizarTesteBomba(Stage_box_bomba);
            btngravar.Enabled = false;
            bntFinalizar.Enabled = false;
            btnreset.Enabled = false;
            btnrelatoriobomba.Enabled = false;
            btniniciarteste.Enabled = true;


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
                // Parar todos os processos em andamento
                timer.Stop(); // Para o timer do cronômetro
                cronometroIniciado = false;
                _isMonitoring = false;

                // Parar monitoramento e testes
                PararMonitoramento(); // Stops monitoramentoTimer

                // Reinicia o contador de tempo para o Chart 5
                _timeCounterSeconds = 0;

                // --- Simulated Data Generators Stop and Reset ---
                _dataGenerator.Stop(); // Para a geração de dados simulados para chart1, chart4, chart5, chart6
                _dataGenerator.Reset(); // Reseta os valores simulados para chart1, chart4, chart5, chart6
                _drainDataGenerator.Stop(); // Para a geração de dados simulados para chart2, chart3
                _drainDataGenerator.Reset(); // Reseta os valores simulados para chart2, chart3

                // Limpa os TextBoxes dos sensores simulados
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
                // --- End Simulated Data Generators Stop and Reset ---


                _viewModel.FinalizarTesteBomba(Stage_box_bomba);

                // Limpar DataGridViews (adicione todos os seus DataGridViews aqui)
                dataGridView1.Rows.Clear();
                dataGridView1.DataSource = null;

                if (visualizador.DataSource != null)
                {
                    visualizador.DataSource = null;
                    visualizador.Rows.Clear();
                }
                else
                {
                    visualizador.Rows.Clear();
                }

                // Limpar listas de dados
                etapaAtual = 1;
                dadosSensores.Clear();
                _dadosColetados.Clear();
                // Resetar variáveis de controle
                Inicioteste = string.Empty;
                Fimteste = string.Empty;
                valorDefinidoManualmente = true;

                // Resetar interface gráfica
                circularProgressBar1.Value = 0;
                // _viewModel.LimparCamposEntrada(textBox6, textBox5, textBox4); // Adicione este método no ViewModel se necessário

                // --- Chart 1 Reset ---
                // Remove e recria a série para limpar o gráfico.
                string chart1SeriesName = "Pre.x Vaz.";
                if (chart1.Series.IsUniqueName(chart1SeriesName)) // Verifica se a série existe antes de tentar remover
                {
                    chart1.Series.Remove(chart1.Series[chart1SeriesName]);
                }
                if (chart1.Series.IsUniqueName(chart1SeriesName)) // Agora IsUniqueName deve ser true
                {
                    Series performanceSeries = new Series(chart1SeriesName);
                    performanceSeries.ChartType = SeriesChartType.FastLine;
                    performanceSeries.Color = Color.Blue;
                    performanceSeries.BorderWidth = 2;
                    chart1.Series.Add(performanceSeries);
                }
                _viewModel.ResetChartDataLogic(); // Reset the previous rotation in VM (para chart1)
                // --- End Chart 1 Reset ---

                // --- Chart 2 Reset ---
                // Remove e recria a série para limpar o gráfico.
                string chart2SeriesName = "Vaz.In.X Rot";
                if (chart2.Series.IsUniqueName(chart2SeriesName)) // Verifica se a série existe antes de tentar remover
                {
                    chart2.Series.Remove(chart2.Series[chart2SeriesName]);
                }
                if (chart2.Series.IsUniqueName(chart2SeriesName)) // Agora IsUniqueName deve ser true
                {
                    Series drainRotationSeries = new Series(chart2SeriesName);
                    drainRotationSeries.ChartType = SeriesChartType.FastLine;
                    drainRotationSeries.Color = Color.Red;
                    drainRotationSeries.BorderWidth = 2;
                    chart2.Series.Add(drainRotationSeries);
                }
                // Não há lógica de rotação constante específica para o chart2 no VM, então não há ResetChartDataLogic para ele.
                // --- End Chart 2 Reset ---

                // --- Chart 3 Reset ---
                // Remove e recria a série para limpar o gráfico.
                string chart3SeriesName = "Vaz. x Pres.";
                if (chart3.Series.IsUniqueName(chart3SeriesName)) // Verifica se a série existe antes de tentar remover
                {
                    chart3.Series.Remove(chart3.Series[chart3SeriesName]);
                }
                if (chart3.Series.IsUniqueName(chart3SeriesName)) // Agora IsUniqueName deve ser true
                {
                    Series drainPressureSeries = new Series(chart3SeriesName);
                    drainPressureSeries.ChartType = SeriesChartType.FastLine;
                    drainPressureSeries.Color = Color.Orange;
                    drainPressureSeries.BorderWidth = 2;
                    chart3.Series.Add(drainPressureSeries);
                }
                // --- End Chart 3 Reset ---

                // --- Chart 4 Reset ---
                // Remove e recria as séries para limpar o gráfico de eficiência.
                string chart4GlobalSeriesName = "Rend. Global";
                if (chart4.Series.IsUniqueName(chart4GlobalSeriesName)) // Verifica se a série existe antes de tentar remover
                {
                    chart4.Series.Remove(chart4.Series[chart4GlobalSeriesName]);
                }
                if (chart4.Series.IsUniqueName(chart4GlobalSeriesName)) // Agora IsUniqueName deve ser true
                {
                    Series globalEfficiencySeries = new Series(chart4GlobalSeriesName);
                    globalEfficiencySeries.ChartType = SeriesChartType.FastLine;
                    globalEfficiencySeries.Color = Color.Green;
                    globalEfficiencySeries.BorderWidth = 2;
                    chart4.Series.Add(globalEfficiencySeries);
                }

                string chart4VolumetricSeriesName = "Ef. Volumetrica";
                if (chart4.Series.IsUniqueName(chart4VolumetricSeriesName)) // Verifica se a série existe antes de tentar remover
                {
                    chart4.Series.Remove(chart4.Series[chart4VolumetricSeriesName]);
                }
                if (chart4.Series.IsUniqueName(chart4VolumetricSeriesName)) // Agora IsUniqueName deve ser true
                {
                    Series volumetricEfficiencySeries = new Series(chart4VolumetricSeriesName);
                    volumetricEfficiencySeries.ChartType = SeriesChartType.FastLine;
                    volumetricEfficiencySeries.Color = Color.Purple;
                    volumetricEfficiencySeries.BorderWidth = 2;
                    chart4.Series.Add(volumetricEfficiencySeries);
                }
                // Não há lógica de reset específica do VM para chart4 neste momento.
                // --- End Chart 4 Reset ---

                // --- Chart 5 Reset ---
                // Remove e recria as séries para limpar o gráfico de rampa.
                string chart5TemperatureSeriesName = "Temperatura";
                if (chart5.Series.IsUniqueName(chart5TemperatureSeriesName)) // Verifica se a série existe antes de tentar remover
                {
                    chart5.Series.Remove(chart5.Series[chart5TemperatureSeriesName]);
                }
                if (chart5.Series.IsUniqueName(chart5TemperatureSeriesName)) // Agora IsUniqueName deve ser true
                {
                    Series temperatureSeries = new Series(chart5TemperatureSeriesName);
                    temperatureSeries.ChartType = SeriesChartType.FastLine;
                    temperatureSeries.Color = Color.Blue;
                    temperatureSeries.BorderWidth = 2;
                    temperatureSeries.YAxisType = AxisType.Primary; // Mapeia para o Eixo Y Primário (Temperatura)
                    chart5.Series.Add(temperatureSeries);
                }

                string chart5PressureSeriesName = "Pressão Rampa";
                if (chart5.Series.IsUniqueName(chart5PressureSeriesName)) // Verifica se a série existe antes de tentar remover
                {
                    chart5.Series.Remove(chart5.Series[chart5PressureSeriesName]);
                }
                if (chart5.Series.IsUniqueName(chart5PressureSeriesName)) // Agora IsUniqueName deve ser true
                {
                    Series pressureRampSeries = new Series(chart5PressureSeriesName);
                    pressureRampSeries.ChartType = SeriesChartType.FastLine;
                    pressureRampSeries.Color = Color.Red;
                    pressureRampSeries.BorderWidth = 2;
                    pressureRampSeries.YAxisType = AxisType.Secondary; // Mapeia para o Eixo Y Secundário (Pressão/Vazão)
                    chart5.Series.Add(pressureRampSeries);
                }

                string chart5FlowSeriesName = "Vazão Rampa";
                if (chart5.Series.IsUniqueName(chart5FlowSeriesName)) // Verifica se a série existe antes de tentar remover
                {
                    chart5.Series.Remove(chart5.Series[chart5FlowSeriesName]);
                }
                if (chart5.Series.IsUniqueName(chart5FlowSeriesName)) // Agora IsUniqueName deve ser true
                {
                    Series flowRampSeries = new Series(chart5FlowSeriesName);
                    flowRampSeries.ChartType = SeriesChartType.FastLine;
                    flowRampSeries.Color = Color.Green;
                    flowRampSeries.BorderWidth = 2;
                    flowRampSeries.YAxisType = AxisType.Secondary; // Mapeia para o Eixo Y Secundário (Pressão/Vazão)
                    chart5.Series.Add(flowRampSeries);
                }
                // --- End Chart 5 Reset ---

                // --- Chart 6 Reset ---
                // Remove e recria a série para limpar o gráfico de vazão real.
                string chart6RealFlowSeriesName = "Vazão Real";
                if (chart6.Series.IsUniqueName(chart6RealFlowSeriesName)) // Verifica se a série existe antes de tentar remover
                {
                    chart6.Series.Remove(chart6.Series[chart6RealFlowSeriesName]);
                }
                if (chart6.Series.IsUniqueName(chart6RealFlowSeriesName)) // Agora IsUniqueName deve ser true
                {
                    Series realFlowSeries = new Series(chart6RealFlowSeriesName);
                    realFlowSeries.ChartType = SeriesChartType.FastLine;
                    realFlowSeries.Color = Color.Blue;
                    realFlowSeries.BorderWidth = 2;
                    chart6.Series.Add(realFlowSeries);
                }
                // --- End Chart 6 Reset ---


                // Reinicia o processo como se o botão Iniciar fosse clicado novamente (sem a validação inicial)
                _isMonitoring = true;
                _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5, panel2, panel6, panel11, panel9);
                Inicioteste = DateTime.Now.ToString();
                _viewModel.IniciarTesteBomba(Stage_box_bomba);
                //trackBar1.Enabled = false;
                InicializarMonitoramento(); // Starts monitoramentoTimer

                // --- Simulated Data Generators Start ---
                _dataGenerator.Start(); // Inicia a geração de dados simulados novamente para chart1, chart4, chart5, chart6
                _drainDataGenerator.Start(); // Inicia a geração de dados simulados novamente para chart2, chart3
                // --- End Simulated Data Generators Start ---

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

                MessageBox.Show("Processo reiniciado com sucesso!", "Reinício Completo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Recarregar a configuração inicial

            }
        }



        #endregion


        #region BOTÕES_MEDIDAS
        private void unidade_medidapilotagem1_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_psi_PL.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pilotagem",
                    Valor = valorAtual,
                    Medidas = "psi"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }


        private void unidade_medidapilotagem2_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_bar_PL.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pilotagem",
                    Valor = valorAtual,
                    Medidas = "bar"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidadreno1_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_lpm_DR.Text; // Nota: O original estava como gpm, mantive o nome do controle

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "dreno",
                    Valor = valorAtual,
                    Medidas = "gpm" // Medida definida como gpm
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidadreno2_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_lpm_DR.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "dreno",
                    Valor = valorAtual,
                    Medidas = "lpm"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidapressao1_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_Press_PSI.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pressão",
                    Valor = valorAtual,
                    Medidas = "psi"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidapressao2_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_Press_BAR.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pressão",
                    Valor = valorAtual,
                    Medidas = "bar"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidarota_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_rotacao_RPM.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "rotação",
                    Valor = valorAtual,
                    Medidas = "rpm"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidasvazao1_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_Vazao_GPM.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "vazão",
                    Valor = valorAtual,
                    Medidas = "gpm"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidasvazao2_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_Vazao_LPM.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "vazão",
                    Valor = valorAtual,
                    Medidas = "lpm"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidatemp_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_Temp_C.Text;
                string sendorname = sensor_Temp_C.Tag.ToString();

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = sendorname,
                    Valor = valorAtual,
                    Medidas = "celsus" // Pode querer corrigir para "celsius" se for o caso
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void btn_gravar_Click(object sender, EventArgs e)
        {
            if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
                return; // Sai do método se não estiver monitorando
            }


            if (etapaAtual <= _synchronizerNivel.maxControlEtapas)
            {
                dataGridViewLoad();
                AtualizarDataGridView();
                foreach (var item in dadosSensoresSelecionados)
                {
                    if (sensorMap.ContainsKey(item))
                    {
                        string valorAtual = sensorMap[item].Text;
                        string medida = sensorMapmedida[item];
                        string sendorname = sensorMap[item].Tag.ToString();

                        // Criar uma nova instância de EtapaData e inicializar a lista de leituras
                        var etapaData = new EtapaData
                        {
                            Etapa = etapaAtual,
                            leituras = new List<SensorData>()
                        };

                        // Adicionar os dados do sensor à lista de leituras
                        etapaData.leituras.Add(new SensorData
                        {
                            Sensor = sendorname,
                            Valor = valorAtual,
                            Medidas = medida
                        });

                        // Adicionar a etapa à lista de dados coletados
                        _dadosColetados.Add(etapaData);
                    }
                }

                // Incrementa a etapa atual
                etapaAtual++;
            }
            else
            {
                MessageBox.Show("Limite de etapas atingido.");
            }

        }

        private void btnDefinir_Click(object sender, EventArgs e)
        {
            if (!cronometroIniciado)
            {
                if (int.TryParse(textBox1.Text, out int valor))
                {
                    valorDefinido = valor;
                    valorDefinidoManualmente = true;
                    // Habilita os botões button4 e button6 após definir um valor
                    button4.Enabled = false;
                    button6.Enabled = true;
                    MessageBox.Show($"O valor {valorDefinido} foi definido.", "Sucesso");
                }
                else
                {
                    MessageBox.Show("Por favor, insira um valor numérico válido no TextBox.", "Erro");
                }
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            if (!cronometroIniciado)
            {
                textBox1.Text = "0";
                valorDefinidoManualmente = false;
                // Desabilita os botões button4 e button6 ao limpar o textbox
                button4.Enabled = true;
                button6.Enabled = true;
            }
        }
        #endregion


        #region VALIDAÇÕES
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
            // Tenta converter os valores, usando InvariantCulture para consistência (ponto decimal)
            bool minOk = decimal.TryParse(minTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMinimo);
            bool maxOk = decimal.TryParse(maxTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMaximo);

            string erroMsg = null; // Armazena a mensagem de erro específica

            // 1. Valida se são números
            if (!minOk || !maxOk)
            {
                erroMsg = $"Os valores de Mínimo e Máximo para {nomeUnidade} devem ser numéricos.";
            }
            // 2. Valida se não são negativos (só faz sentido se forem números)
            else if (valorMinimo < 0 || valorMaximo < 0)
            {
                erroMsg = $"Os valores de Mínimo e Máximo para {nomeUnidade} não podem ser menores que 0.";
            }
            // 3. Valida se min <= max (só faz sentido se não forem negativos)
            else if (valorMinimo > valorMaximo)
            {
                erroMsg = $"O valor Mínimo para {nomeUnidade} não pode ser maior que o valor Máximo .";
            }

            // Se houve algum erro (erroMsg foi definida)
            if (erroMsg != null)
            {
                // Desmarca o CheckBox que disparou a validação
                checkBox.Checked = false;

                // Mostra a mensagem de erro
                MessageBox.Show(erroMsg + $"\nA verificação {nomeUnidade} foi desativada.",
                                $"Erro de Validação - {nomeUnidade}",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                minTextBox.Clear(); // Equivalente a minTextBox.Text = "";
                maxTextBox.Clear(); // Equivalente a maxTextBox.Text = "";


                // Opcional: Foca no TextBox com problema
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
            // 'sender' é o CheckBox que disparou o evento
            CheckBox cb = sender as CheckBox;
            // Só valida se o checkbox FOI MARCADO
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para PSI
                ValidarMinMaxCheckBox(cb, textBox9, textBox8, "PSI");
            }
        }

        private void checkBox_gpm_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para GPM
                ValidarMinMaxCheckBox(cb, textBox14, textBox12, "GPM");
            }
        }

        private void checkBox_bar_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para BAR
                ValidarMinMaxCheckBox(cb, textBox11, textBox10, "BAR");
            }
        }

        private void checkBox_rotacao_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para Rotação
                ValidarMinMaxCheckBox(cb, textBox18, textBox17, "Rotação (RPM)");
            }
        }

        private void checkBox_lpm_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para LPM
                ValidarMinMaxCheckBox(cb, textBox16, textBox15, "LPM");
            }
        }

        private void checkBox_temperatura_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para Temperatura
                ValidarMinMaxCheckBox(cb, textBox20, textBox19, "Temperatura (°C)");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (circularProgressBar1.Value > 0)
            {
                circularProgressBar1.Value--;
            }
            else
            {
                timer.Stop();
                cronometroIniciado = false; // Permite definir e limpar novamente
                btnParar_Click(this, EventArgs.Empty);
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
            Realatoriobase relatorio = new Realatoriobase();
            //relatorio.GerarRelatorio(dadosSensores, _dadosColetados, Inicioteste, Fimteste);
            relatorio.Show();
            this.Hide();

        }

        public void dataGridViewLoad()
        {
            // Inicializa o DataGridView
            dataGridView1.Size = new Size(852, 174);
            dataGridView1.ColumnCount = 11; // Etapa + 10 sensores
            dataGridView1.Columns[0].HeaderText = "Etapa";
            dataGridView1.Columns[1].HeaderText = "Pilotagem PSI";
            dataGridView1.Columns[2].HeaderText = "Pilotagem BAR";
            dataGridView1.Columns[3].HeaderText = "Dreno GPM";
            dataGridView1.Columns[4].HeaderText = "Dreno LPM";
            dataGridView1.Columns[5].HeaderText = "Pressão PSI";
            dataGridView1.Columns[6].HeaderText = "Pressão BAR";
            dataGridView1.Columns[7].HeaderText = "Rotação RPM";
            dataGridView1.Columns[8].HeaderText = "Vazão GPM";
            dataGridView1.Columns[9].HeaderText = "Vazão LPM";
            dataGridView1.Columns[10].HeaderText = "Temperatura Celsius";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public void AtualizarDataGridView()
        {
            // Obtém os valores dos sensores
            string pilotagemPsi = sensor_psi_PL.Text;
            string pilotagemBar = sensor_bar_PL.Text;
            string drenoGpm = sensor_gpm_DR.Text;
            string drenoLpm = sensor_lpm_DR.Text;
            string pressaoPsi = sensor_Press_PSI.Text;
            string pressaoBar = sensor_Press_BAR.Text;
            string rotacaoRpm = sensor_rotacao_RPM.Text;
            string vazaoGpm = sensor_Vazao_GPM.Text;
            string vazaoLpm = sensor_Vazao_LPM.Text;
            string temperaturaCelsius = sensor_Temp_C.Text;

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

            // Adiciona a nova linha ao DataGridView
            if (dataGridView1.Rows.Count < _synchronizerNivel.maxControlEtapas)
            {
                dataGridView1.Rows.Add(novaLinha);

            }
            else
            {
                // Se já atingiu o máximo de linhas, você pode optar por:
                // 1. Remover a primeira linha e adicionar a nova (comportamento de histórico)
                // 2. Exibir uma mensagem informando que o limite foi atingido
                // 3. Desabilitar o botão de gravação

                dataGridView1.Rows.Add(novaLinha);
                // etapaAtual permanece o mesmo ou você pode decidir incrementá-lo
            }
        }

        // Método para o botão Retornar
        private void btnretornar_Click(object sender, EventArgs e)
        {
            // Para os geradores de dados simulados
            _dataGenerator?.Stop();
            _drainDataGenerator?.Stop();

            // Remove e recria a série para limpar o gráfico.
            string chart1SeriesName = "Pre.x Vaz.";
            if (chart1.Series.IsUniqueName(chart1SeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart1.Series.Remove(chart1.Series[chart1SeriesName]);
            }
            if (chart1.Series.IsUniqueName(chart1SeriesName)) // Agora IsUniqueName deve ser true
            {
                Series performanceSeries = new Series(chart1SeriesName);
                performanceSeries.ChartType = SeriesChartType.FastLine;
                performanceSeries.Color = Color.Blue;
                performanceSeries.BorderWidth = 2;
                chart1.Series.Add(performanceSeries);
            }

            string chart2SeriesName = "Vaz.In.X Rot";
            if (chart2.Series.IsUniqueName(chart2SeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart2.Series.Remove(chart2.Series[chart2SeriesName]);
            }
            if (chart2.Series.IsUniqueName(chart2SeriesName)) // Agora IsUniqueName deve ser true
            {
                Series drainRotationSeries = new Series(chart2SeriesName);
                drainRotationSeries.ChartType = SeriesChartType.FastLine;
                drainRotationSeries.Color = Color.Red;
                drainRotationSeries.BorderWidth = 2;
                chart2.Series.Add(drainRotationSeries);
            }

            string chart3SeriesName = "Vaz. x Pres.";
            if (chart3.Series.IsUniqueName(chart3SeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart3.Series.Remove(chart3.Series[chart3SeriesName]);
            }
            if (chart3.Series.IsUniqueName(chart3SeriesName)) // Agora IsUniqueName deve ser true
            {
                Series drainPressureSeries = new Series(chart3SeriesName);
                drainPressureSeries.ChartType = SeriesChartType.FastLine;
                drainPressureSeries.Color = Color.Orange;
                drainPressureSeries.BorderWidth = 2;
                chart3.Series.Add(drainPressureSeries);
            }

            // --- Chart 4 Return Logic ---
            // Remove e recria as séries para limpar o gráfico de eficiência ao retornar.
            string chart4GlobalSeriesName = "Rend. Global";
            if (chart4.Series.IsUniqueName(chart4GlobalSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart4.Series.Remove(chart4.Series[chart4GlobalSeriesName]);
            }
            if (chart4.Series.IsUniqueName(chart4GlobalSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series globalEfficiencySeries = new Series(chart4GlobalSeriesName);
                globalEfficiencySeries.ChartType = SeriesChartType.FastLine;
                globalEfficiencySeries.Color = Color.Green;
                globalEfficiencySeries.BorderWidth = 2;
                chart4.Series.Add(globalEfficiencySeries);
            }

            string chart4VolumetricSeriesName = "Ef. Volumetrica";
            if (chart4.Series.IsUniqueName(chart4VolumetricSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart4.Series.Remove(chart4.Series[chart4VolumetricSeriesName]);
            }
            if (chart4.Series.IsUniqueName(chart4VolumetricSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series volumetricEfficiencySeries = new Series(chart4VolumetricSeriesName);
                volumetricEfficiencySeries.ChartType = SeriesChartType.FastLine;
                volumetricEfficiencySeries.Color = Color.Purple;
                volumetricEfficiencySeries.BorderWidth = 2;
                chart4.Series.Add(volumetricEfficiencySeries);
            }
            // --- End Chart 4 Return Logic ---

            // --- Chart 5 Return Logic ---
            // Remove e recria as séries para limpar o gráfico de rampa.
            string chart5TemperatureSeriesName = "Temperatura";
            if (chart5.Series.IsUniqueName(chart5TemperatureSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart5.Series.Remove(chart5.Series[chart5TemperatureSeriesName]);
            }
            if (chart5.Series.IsUniqueName(chart5TemperatureSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series temperatureSeries = new Series(chart5TemperatureSeriesName);
                temperatureSeries.ChartType = SeriesChartType.FastLine;
                temperatureSeries.Color = Color.Blue;
                temperatureSeries.BorderWidth = 2;
                temperatureSeries.YAxisType = AxisType.Primary; // Mapeia para o Eixo Y Primário (Temperatura)
                chart5.Series.Add(temperatureSeries);
            }

            string chart5PressureSeriesName = "Pressão Rampa";
            if (chart5.Series.IsUniqueName(chart5PressureSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart5.Series.Remove(chart5.Series[chart5PressureSeriesName]);
            }
            if (chart5.Series.IsUniqueName(chart5PressureSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series pressureRampSeries = new Series(chart5PressureSeriesName);
                pressureRampSeries.ChartType = SeriesChartType.FastLine;
                pressureRampSeries.Color = Color.Red;
                pressureRampSeries.BorderWidth = 2;
                pressureRampSeries.YAxisType = AxisType.Secondary; // Mapeia para o Eixo Y Secundário (Pressão/Vazão)
                chart5.Series.Add(pressureRampSeries);
            }

            string chart5FlowSeriesName = "Vazão Rampa";
            if (chart5.Series.IsUniqueName(chart5FlowSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart5.Series.Remove(chart5.Series[chart5FlowSeriesName]);
            }
            if (chart5.Series.IsUniqueName(chart5FlowSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series flowRampSeries = new Series(chart5FlowSeriesName);
                flowRampSeries.ChartType = SeriesChartType.FastLine;
                flowRampSeries.Color = Color.Green;
                flowRampSeries.BorderWidth = 2;
                flowRampSeries.YAxisType = AxisType.Secondary; // Mapeia para o Eixo Y Secundário (Pressão/Vazão)
                chart5.Series.Add(flowRampSeries);
            }
            // --- End Chart 5 Return Logic ---

            // --- Chart 6 Return Logic ---
            // Remove e recria a série para limpar o gráfico de vazão real.
            string chart6RealFlowSeriesName = "Vazão Real";
            if (chart6.Series.IsUniqueName(chart6RealFlowSeriesName)) // Verifica se a série existe antes de tentar remover
            {
                chart6.Series.Remove(chart6.Series[chart6RealFlowSeriesName]);
            }
            if (chart6.Series.IsUniqueName(chart6RealFlowSeriesName)) // Agora IsUniqueName deve ser true
            {
                Series realFlowSeries = new Series(chart6RealFlowSeriesName);
                realFlowSeries.ChartType = SeriesChartType.FastLine;
                realFlowSeries.Color = Color.Blue;
                realFlowSeries.BorderWidth = 2;
                chart6.Series.Add(realFlowSeries);
            }
            // --- End Chart 6 Return Logic ---


            // Lógica para retornar ao menu principal ou fechar o formulário
            _fechamentoForcado = true; // Indica que é um fechamento controlado
            Menuapp.Instance.Show(); // Supondo que Menuapp é o formulário principal
            this.Close(); // Fecha o formulário atual
        }
    }
}
