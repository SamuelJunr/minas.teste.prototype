using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Extensions.Logging;
using minas.teste.prototype.MVVM.Model.Abstract;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.Service;
using minas.teste.prototype.Teste.Tela_Bomba;
//using MultiSensorMonitor;

namespace minas.teste.prototype
{

    public partial class Tela_Bombas : Form
    {
        private apresentacao fechar_box;
        private bool _fechamentoForcado = false;
        private string _serialDataIn;
        private HoraDia _Tempo;
        private object txtLogEventos;
        private const int MAX_LOG_LINES_DISPLAY = 500;
        private Dictionary<string, double> _currentValues = new Dictionary<string, double>();
        private System.Timers.Timer _updateTimer;
        private bool _isMonitoring = false;
        private Dreno_bomba _dreno;
        private Pilotagem_bomba _pilotagem;
        private Pressao_bomba _pressao;
        private Vazao_bomba _vazao;
        private Rotacao_bomba _rotacao;
        private Temperatura_bomba _temperatura;
        private cronometroSK4 _cronometro;
        private System.Windows.Forms.Timer _timerAtualizacao;
        private const string LABEL_CRONOMETRO = "labelCronometro";
        private TestDataGenerator _testDataGenerator;


        // Replace the obsolete InitializeComponent() method call with a custom implementation.  
        // Ensure that all necessary UI initialization logic is included in the new method.  

        public Tela_Bombas()
        {
            InitializeComponent();  
            
            fechar_box = new apresentacao();
            _Tempo = new HoraDia(label13);
            LoggerTelas.LogMessageAdded += Logger_LogMessageAdded;
            InitializeChart();
            InitializeDataGridView();
            InitializeObjects();
            SetupUpdateTimer();
            _cronometro = new cronometroSK4();
            SetupCronometroTimer();
            VincularCronometroLabel();
        }

                 

        private void Tela_Bombas_Load(object sender, EventArgs e)
        {
            Text = Properties.Resources.ResourceManager.GetString("MainFormTitle");
            pictureBox1.BackgroundImage = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("off");    

        }
        private void Logger_LogMessageAdded(object sender, LogEventArgs e)
        {
            // IMPORTANTE: Verificar se a atualização precisa ser feita na thread da UI
            if (HistoricalEvents.InvokeRequired)
            {
                // Chama a si mesmo (ou um método dedicado) na thread da UI de forma assíncrona
                HistoricalEvents.BeginInvoke(new Action(() => UpdateLogDisplay(e.Message)));
            }
            else
            {
                // Já está na thread da UI, pode atualizar diretamente
                UpdateLogDisplay(e.Message);
            }
        }
        private void UpdateLogDisplay(string logEntry)
        {
            try
            {
                // Adiciona a entrada (já formatada pelo Logger) e a quebra de linha
                HistoricalEvents.AppendText(logEntry + Environment.NewLine);

                // Gerenciamento do tamanho do *TextBox*
                if (HistoricalEvents.Lines.Length > MAX_LOG_LINES_DISPLAY)
                {
                    var limitedLines = HistoricalEvents.Lines
                                        .Skip(HistoricalEvents.Lines.Length - MAX_LOG_LINES_DISPLAY)
                                        .ToArray();
                    HistoricalEvents.Lines = limitedLines;
                }

                // Rolagem automática
                HistoricalEvents.SelectionStart = HistoricalEvents.Text.Length;
                HistoricalEvents.ScrollToCaret();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar display do log: {ex.Message}");
            }
        }

        private void InitializeChart()
        {
            chart1.Series.Clear();

            // Série 1: Vazão LPM
            var series1 = new Series("Vazão (LPM)")
            {
                ChartType = SeriesChartType.FastLine,
                Color = System.Drawing.Color.Blue
            };
            chart1.Series.Add(series1);

            // Série 6: Pressão BAR
            var series6 = new Series("Pressão (BAR)")
            {
                ChartType = SeriesChartType.FastLine,
                Color = System.Drawing.Color.Red
            };
            chart1.Series.Add(series6);

            // Configuração dos eixos
            chart1.ChartAreas[0].AxisY.Maximum = 300;
            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Interval = 50;

            chart1.ChartAreas[0].AxisX.Maximum = 300;
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Interval = 50;
        }

        private void InitializeDataGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Parametro", "Parâmetro");
            dataGridView1.Columns.Add("Valor", "Valor");
            dataGridView1.Columns.Add("Unidade", "Unidade");
            dataGridView1.Size = new System.Drawing.Size(460, 284);
        }

        private void InitializeObjects()
        {
            // Update the constructor call for Dreno_bomba to include the required 'isInGpm' parameter.
            _dreno = new Dreno_bomba(0, false); // Assuming 'false' as the default value for 'isInGpm'. Adjust as needed.
            _pilotagem = new Pilotagem_bomba(0, 0);
            _pressao = new Pressao_bomba(0);
            _vazao = new Vazao_bomba(0);
            _rotacao = new Rotacao_bomba(0);
            _temperatura = new Temperatura_bomba(0);
        }

        private void SetupUpdateTimer()
        {
            _updateTimer = new System.Timers.Timer(1000);
            _updateTimer.Elapsed += (s, e) => UpdateDisplay();
        }

        //private void btnIniciar_Click(object sender, EventArgs e)
        //{
        //    _isMonitoring = true;
        //    ConnectionSettingsApplication.PersistentSerialManager.DataReceived += HandleSerialData;
        //    _updateTimer.Start();
        //    pictureBox1.BackgroundImage = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("on");
        //    // Atualiza estado dos painéis
        //    AlterarEstadoPaineis(true);
        //}

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            _isMonitoring = true;
            _testDataGenerator = new TestDataGenerator();
            _testDataGenerator.OnDataGenerated += HandleTestData; // Substitui o evento serial
            _testDataGenerator.StartGeneration();

            _updateTimer.Start();
            using (var ms = new MemoryStream((byte[])Properties.Resources.ResourceManager.GetObject("on")))
            {
                pictureBox1.BackgroundImage = Image.FromStream(ms);
            }
            AlterarEstadoPaineis(true);
        }
        private void HandleTestData(string data)
        {
            // Remove os ':' para compatibilidade com ParseData
            string formattedData = data.Replace(":", "");
            ParseDataFromTestData(formattedData); // Renomeado para evitar ambiguidade
            UpdateObjects();
        }

        private void ParseDataFromTestData(string rawData) // Renomeado para evitar conflito
        {
            var segments = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            lock (_currentValues)
            {
                foreach (var segment in segments)
                {
                    if (segment.Length < 2) continue;

                    var key = segment.Substring(0, 1); // Primeiro caractere é a chave (A-J)
                    var value = segment.Substring(1); // Restante é o valor

                    if (double.TryParse(value, out double numericValue))
                    {
                        _currentValues[key] = numericValue;
                    }
                }
            }
        }

        //private void ParseData(string rawData)
        //{
        //    var segments = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        //    lock (_currentValues)
        //    {
        //        foreach (var segment in segments)
        //        {
        //            if (segment.Length < 2) continue;

        //            var key = segment.Substring(0, 1); // Primeiro caractere é a chave (A-J)
        //            var value = segment.Substring(1); // Restante é o valor

        //            if (double.TryParse(value, out double numericValue))
        //            {
        //                _currentValues[key] = numericValue;
        //            }
        //        }
        //    }
        //}

        private void AlterarEstadoPaineis(bool ativo)
        {
            // Implemente a mudança visual dos painéis aqui
           if(_isMonitoring)
            {
                // Exibe os painéis
                var imageBytes = (byte[])Properties.Resources.ResourceManager.GetObject("pilotagem_on");
                using (var ms = new MemoryStream(imageBytes))
                {
                    panel4.BackgroundImage = Image.FromStream(ms);
                }
                var imageBytes1 = (byte[])Properties.Resources.ResourceManager.GetObject("dreno_on");
                using (var ms = new MemoryStream(imageBytes))
                {
                    panel4.BackgroundImage = Image.FromStream(ms);
                }
                var imageBytes2 = (byte[])Properties.Resources.ResourceManager.GetObject("pressao_on");
                using (var ms = new MemoryStream(imageBytes))
                {
                    panel4.BackgroundImage = Image.FromStream(ms);
                }
                var imageBytes3 = (byte[])Properties.Resources.ResourceManager.GetObject("rotacao_on");
                using (var ms = new MemoryStream(imageBytes))
                {
                    panel4.BackgroundImage = Image.FromStream(ms);
                }
                var imageBytes4 = (byte[])Properties.Resources.ResourceManager.GetObject("vazao_on");
                using (var ms = new MemoryStream(imageBytes))
                {
                    panel4.BackgroundImage = Image.FromStream(ms);
                }
                var imageBytes5 = (byte[])Properties.Resources.ResourceManager.GetObject("temperatura_on");
                using (var ms = new MemoryStream(imageBytes))
                {
                    panel4.BackgroundImage = Image.FromStream(ms);
                }
               
           }
           
        }

        //private void HandleSerialData(object sender, string data)
        //{
        //    ParseData(data);
        //    UpdateObjects();
        //}

        private void ParseData(string rawData)
        {
            var segments = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            lock (_currentValues)
            {
                foreach (var segment in segments)
                {
                    if (segment.Length < 2) continue;

                    var key = segment.Substring(0, 1);
                    var value = segment.Substring(1);

                    if (double.TryParse(value, out double numericValue))
                    {
                        _currentValues[key] = numericValue;
                    }
                }
            }
        }

        private void UpdateObjects()
        {
            lock (_currentValues)
            {
                // Dreno
                if (_currentValues.TryGetValue("A", out double aVal)) _dreno.UpdateFlow(aVal);
                if (_currentValues.TryGetValue("B", out double bVal)) _dreno.UpdateFlow(bVal);

                // Pilotagem
                if (_currentValues.TryGetValue("C", out double cVal)) _pilotagem.UpdateFromArduino(cVal, 0);
                if (_currentValues.TryGetValue("D", out double dVal)) _pilotagem.UpdateFromArduino(dVal, 0);

                // Pressão
                if (_currentValues.TryGetValue("E", out double eVal)) _pressao.UpdatePressure(eVal);
                if (_currentValues.TryGetValue("F", out double fVal)) _pressao.UpdatePressure(fVal, true);

                // Vazão
                if (_currentValues.TryGetValue("G", out double gVal)) _vazao.UpdateFromArduino(gVal, true);
                if (_currentValues.TryGetValue("H", out double hVal)) _vazao.UpdateFromArduino(hVal);

                // Rotação
                if (_currentValues.TryGetValue("I", out double iVal)) _rotacao.UpdateRotation(iVal);

                // Temperatura
                if (_currentValues.TryGetValue("J", out double jVal)) _temperatura.UpdateTemperature(jVal);
            }
        }

        private void UpdateDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateDisplay));
                return;
            }

            // Atualiza gráfico
            chart1.Series[0].Points.AddY(_vazao.Lpm);
            chart1.Series[1].Points.AddY(_pressao.Bar);

            // Atualiza DataGridView
            dataGridView1.Rows.Clear();

            AddRow("Vazão LPM", _vazao.Lpm.ToString("F2"), "LPM");
            AddRow("Pressão BAR", _pressao.Bar.ToString("F2"), "BAR");
            AddRow("Rotação", _rotacao.RPM.ToString("F0"), "RPM");
            AddRow("Temperatura", _temperatura.Celsius.ToString("F1"), "°C");

            lock (_currentValues)
            {
                if (_currentValues.TryGetValue("A", out double aVal))
                    sensor_psi_PL.Text = aVal.ToString("F2"); // Formato com 2 casas decimais
                if (_currentValues.TryGetValue("B", out double bVal))
                    sensor_bar_PL.Text = bVal.ToString("F2");
            }

            // Mantém máximo de 300 pontos
            if (chart1.Series[0].Points.Count > 300)
            {
                chart1.Series[0].Points.RemoveAt(0);
                chart1.Series[1].Points.RemoveAt(0);
            }
        }
        private void SetupCronometroTimer()
        {
            _timerAtualizacao = new System.Windows.Forms.Timer();
            _timerAtualizacao.Interval = 100; // Atualiza a cada 100ms
            _timerAtualizacao.Tick += (s, e) => AtualizarDisplayCronometro();
        }

        private void VincularCronometroLabel()
        {
            _cronometro.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_cronometro.FormattedElapsed))
                {
                    AtualizarLabelSeguro(_cronometro.FormattedElapsed);
                }
            };
        }
        public void IniciarCronometro()
        {
            if (_cronometro.IsRunning) return;

            // Reset se já tiver dados
            if (_cronometro.GetCurrentTotalElapsedTime() > TimeSpan.Zero)
            {
                _cronometro.AccumulatedElapsed = TimeSpan.Zero;
            }

            _cronometro.StartTime = DateTime.Now;
            _cronometro.IsRunning = true;
            _timerAtualizacao.Start();
        }

        public void PararCronometro()
        {
            if (!_cronometro.IsRunning) return;

            _cronometro.AccumulatedElapsed = _cronometro.GetCurrentTotalElapsedTime();
            _cronometro.IsRunning = false;
            _timerAtualizacao.Stop();
            AtualizarDisplayCronometro();
        }
        private void AtualizarDisplayCronometro()
        {
            _cronometro.UpdateFormattedString();
            AtualizarLabelSeguro(_cronometro.FormattedElapsed);
        }

        private void AtualizarLabelSeguro(string valor)
        {
            var label = Controls.Find(LABEL_CRONOMETRO, true).FirstOrDefault() as Label;

            if (label?.InvokeRequired == true)
            {
                label.BeginInvoke((Action)(() => label.Text = valor));
            }
            else
            {
                label.Text = valor;
            }
        }

        private void AddRow(string parametro, string valor, string unidade)
        {
            dataGridView1.Rows.Add(parametro, valor, unidade);
        }

        //private void btnParar_Click(object sender, EventArgs e)
        //{
        //    _isMonitoring = false;
        //    _updateTimer.Stop();
        //    ConnectionSettingsApplication.PersistentSerialManager.DataReceived -= HandleSerialData;

        //    // Restaura tamanho do DataGridView
        //    dataGridView1.Size = new System.Drawing.Size(460, 284);
        //    AlterarEstadoPaineis(false);
        //}
        private void btnParar_Click(object sender, EventArgs e)
        {
            _isMonitoring = false;
            _updateTimer.Stop();
            _testDataGenerator?.StopGeneration(); // Para a geração de dados

            dataGridView1.Size = new System.Drawing.Size(460, 284);
            AlterarEstadoPaineis(false);
        }
        private void Retornar_Click(object sender, EventArgs e)
        {
            _fechamentoForcado = true; // Indica que é um fechamento controlado
            Menuapp.Instance.Show();
            this.Close();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _serialDataIn = serialPort1.ReadLine();
            Invoke(new Action(() => ProcessSerialData(_serialDataIn)));
        }

        public void ProcessSerialData(string serialData)
        {
            _serialDataIn = serialData;
            //ResetValues();
            //ParseData();
            //UpdateUI();
            // UpdateProgressBarDataSensorA(); // Atualiza a ProgressBar
        }

        //private void ResetValues()
        //{
        //    DataSensorA = string.Empty;
        //    DataSensorB = string.Empty;
        //    DataSensorC = string.Empty;
        //}

        //private void ParseData()
        //{
        //    var segments = _serialDataIn.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        //    foreach (var segment in segments)
        //    {
        //        if (segment.Length < 1) continue;

        //        switch (segment[0])
        //        {
        //            case 'A' when segment.Length > 1:
        //                DataSensorA = segment.Substring(1);
        //                break;
        //            case 'B' when segment.Length > 1:
        //                DataSensorB = segment.Substring(1);
        //                break;
        //            case 'C' when segment.Length > 1:
        //                DataSensorC = segment.Substring(1);
        //                break;
        //            default:
        //                LogError($"Formato inválido: {segment}");
        //                break;
        //        }
        //    }
        //}

        //private void UpdateUI()
        //{
        //    UpdateTextBoxSafe(textBox1, DataSensorA);
        //    //UpdateTextBoxSafe(textBox2, DataSensorB);
        //    //UpdateTextBoxSafe(textBox3, DataSensorC);
        //}

        private void UpdateTextBoxSafe(TextBox textBox, string value)
        {
            if (textBox.InvokeRequired)
            {
                textBox.Invoke(new Action(() => textBox.Text = value));
            }
            else
            {
                textBox.Text = value;
            }
        }

        private void LogError(string message)
        {
            Debug.WriteLine($"Erro: {message}");
        }

        //private void UpdateProgressBarDataSensorA()
        //{
        //    if (int.TryParse(DataSensorA, out int value))
        //    {
        //        if (progressBar1.InvokeRequired)
        //        {
        //            progressBar1.Invoke(new Action(() =>
        //            {
        //                progressBar1.Value = Math.Max(progressBar1.Minimum, Math.Min(progressBar1.Maximum, value));
        //            })); 
        //        }
        //        else
        //        {
        //            progressBar1.Value = Math.Max(progressBar1.Minimum, Math.Min(progressBar1.Maximum, value));
        //        }
        //    }
        //    else
        //    {
        //        // Lida com casos em que DataSensorA não é um inteiro válido.
        //        Debug.WriteLine($"Aviso: DataSensorA '{DataSensorA}' não é um inteiro válido.");
        //    }
        //}

        

        private void Tela_Bombas_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            // Só encerra a aplicação se não for um fechamento controlado
            if (!_fechamentoForcado)
            {
                
                fechar_box.apresentacao_FormClosing(sender, e);
            }
            else
                Menuapp.Instance.Show();

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void sensor_gpm_PL_Load(object sender, EventArgs e)
        {
            
        }

        private void sensor_lpm_PL_Load(object sender, EventArgs e)
        {

        }

        private void sensor_gpm_DR_Load(object sender, EventArgs e)
        {

        }

        private void sensor_lpm_DR_Load(object sender, EventArgs e)
        {

        }

        private void sensor_Press_PSI_Load(object sender, EventArgs e)
        {

        }

        private void sensor_Press_BAR_Load(object sender, EventArgs e)
        {

        }

        private void sensor_rotacao_RPM_Load(object sender, EventArgs e)
        {

        }

        private void sensor_Vazao_PSI_Load(object sender, EventArgs e)
        {

        }

        private void sensor_Vazao_BAR_Load(object sender, EventArgs e)
        {

        }

        private void sensor_Temp_C_Load(object sender, EventArgs e)
        {

        }

        private void Btn_Grava_pilotagem_Load(object sender, EventArgs e)
        {

        }

        private void Btn_Grava_dreno_Load(object sender, EventArgs e)
        {

        }

        private void Btn_Grava_pressao_Load(object sender, EventArgs e)
        {

        }

        private void Btn_Grava_vazao_Load(object sender, EventArgs e)
        {

        }

        private void Btn_Grava_rotacao_Load(object sender, EventArgs e)
        {

        }

        private void Btn_Grava_Temperatura_Load(object sender, EventArgs e)
        {

        }

        //botão iniciar

    }
}