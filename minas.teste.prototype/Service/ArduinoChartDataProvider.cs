using System;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace minas.teste.prototype.Service
{
    public class ArduinoChartDataProvider
    {
        private SerialPort _activePort;
        private bool _readingData;
        private readonly Chart _targetChart;
        private readonly SynchronizationContext _syncContext;
        private const string LogFile = "serial_log.json";
        private readonly object _logLock = new object();

        // Variáveis para armazenamento dos valores
        public double AValue { get; private set; }
        public int BValue { get; private set; }
        public bool CValue { get; private set; }

        // Controles UI para exibição
        private readonly Label _lblA, _lblB, _lblC;

        public ArduinoChartDataProvider(Chart chart, Label lblA, Label lblB, Label lblC)
        {
            _targetChart = chart;
            _lblA = lblA;
            _lblB = lblB;
            _lblC = lblC;
            _syncContext = SynchronizationContext.Current;
            InitializeLogFile();
            InitializeChart();
        }

        private void InitializeLogFile()
        {
            lock (_logLock)
            {
                if (!File.Exists(LogFile))
                {
                    File.WriteAllText(LogFile, "[]");
                }
            }
        }

        private void InitializeChart()
        {
            _syncContext.Send(state =>
            {
                _targetChart.Series.Clear();
                _targetChart.ChartAreas.Clear();

                var chartArea = _targetChart.ChartAreas.Add("MainArea");
                chartArea.AxisX.LabelStyle.Format = DateTime.Now.ToString();
                chartArea.AxisX.Title = "Tempo";
                chartArea.AxisY.Title = "Valores";

                // Configuração das séries
                CreateSeries("SerieA", Color.Blue, SeriesChartType.FastLine);
                CreateSeries("SerieB", Color.Red, SeriesChartType.FastLine);
                CreateSeries("SerieC", Color.Green, SeriesChartType.FastLine);

            }, null);
        }

        private void CreateSeries(string name, Color color, SeriesChartType type)
        {
            var series = _targetChart.Series.Add(name);
            series.ChartType = type;
            series.Color = color;
            series.BorderWidth = 2;
        }

        private void WriteLog(LogEntry entry)
        {
            lock (_logLock)
            {
                var logEntry = JsonConvert.SerializeObject(entry, Formatting.Indented);
                File.AppendAllText(LogFile, logEntry + Environment.NewLine);
            }
        }

        public void StartMonitoring()
        {
            new Thread(() =>
            {
                while (!_readingData)
                {
                    CheckAndReadFromArduino();
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        private void CheckAndReadFromArduino()
        {
            foreach (var portName in SerialPort.GetPortNames())
            {
                if (_readingData) return;

                if (ValidateAndConnect(portName))
                {
                    StartReadingData();
                }
            }
        }

        private bool ValidateAndConnect(string portName)
        {
            try
            {
                _activePort = new SerialPort(portName, 9600)
                {
                    ReadTimeout = 100,
                    WriteTimeout = 100,
                    NewLine = "\r\n"
                };

                _activePort.Open();
                WriteLog(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = "Information",
                    Message = $"Conectado à porta {portName}"
                });
                return true;
            }
            catch (Exception ex)
            {
                WriteLog(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = "Error",
                    Message = "Falha na conexão serial",
                    Exception = ex.Message
                });
                UpdateStatus($"Erro na conexão: {ex.Message}", true);
                return false;
            }
        }

        private void StartReadingData()
        {
            _readingData = true;
            new Thread(ReadDataFromPort).Start();
        }

        private void ReadDataFromPort()
        {
            try
            {
                WriteLog(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = "Information",
                    Message = "Leitura de dados iniciada"
                });
                UpdateStatus("Leitura de dados iniciada...", false);

                while (_readingData && _activePort.IsOpen)
                {
                    try
                    {
                        if (_activePort.BytesToRead > 0)
                        {
                            string data = _activePort.ReadLine();
                            ProcessData(data);
                        }
                    }
                    catch (TimeoutException) { /* Ignorar timeouts */ }
                }
            }
            catch (Exception ex)
            {
                WriteLog(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = "Error",
                    Message = "Erro na leitura serial",
                    Exception = ex.Message
                });
                UpdateStatus($"Erro na leitura: {ex.Message}", true);
            }
            finally
            {
                StopReading();
            }
        }

        private void ProcessData(string rawData)
        {
            var segments = rawData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in segments)
            {
                if (segment.Length < 2) continue;

                var identifier = segment[0];
                var valuePart = segment.Substring(1);

                switch (char.ToUpper(identifier))
                {
                    case 'A' when ValidateA(valuePart):
                        HandleValueA(valuePart);
                        break;

                    case 'B' when ValidateB(valuePart):
                        HandleValueB(valuePart);
                        break;

                    case 'C' when ValidateC(valuePart):
                        HandleValueC(valuePart);
                        break;

                    default:
                        LogInvalidSegment(segment, rawData);
                        break;
                }
            }
        }

        private bool ValidateA(string value) => value.Length == 4 && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        private bool ValidateB(string value) => value.Length == 3 && int.TryParse(value, out _);
        private bool ValidateC(string value) => value.Length == 1 && (value == "0" || value == "1");

        private void HandleValueA(string value)
        {
            AValue = double.Parse(value, CultureInfo.InvariantCulture);
            UpdateUIValue(_lblA, $"A: {AValue:F2}");
            AddDataPointToChart("SerieA", AValue);
            LogValue("A", AValue);
        }

        private void HandleValueB(string value)
        {
            BValue = int.Parse(value);
            UpdateUIValue(_lblB, $"B: {BValue:D3}");
            AddDataPointToChart("SerieB", BValue);
            LogValue("B", BValue);
        }

        private void HandleValueC(string value)
        {
            CValue = value == "1";
            UpdateUIValue(_lblC, $"C: {(CValue ? "ATIVO" : "INATIVO")}");
            LogValue("C", CValue);
        }

        private void LogInvalidSegment(string segment, string rawData)
        {
            WriteLog(new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = "Warning",
                Message = $"Formato inválido: {segment}",
                RawData = rawData
            });
        }

        private void LogValue<T>(string type, T value)
        {
            WriteLog(new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = "Information",
                Message = $"{type} processado",
                Value = value?.ToString()
            });
        }

        private void UpdateUIValue(Label label, string text)
        {
            _syncContext.Post(state =>
            {
                label.Text = $"[{DateTime.Now:HH:mm:ss}] {text}";
            }, null);
        }

        private void AddDataPointToChart(string seriesName, double value)
        {
            _syncContext.Post(state =>
            {
                var series = _targetChart.Series[seriesName];
                var chartArea = _targetChart.ChartAreas[0];

                // 1. Configura eixo X fixo
                chartArea.AxisX.Minimum = 0;
                chartArea.AxisX.Maximum = 1300;
                chartArea.AxisX.Interval = 100; // Intervalo de 100 unidades

                // 2. Mantém escala Y automática
                chartArea.AxisY.IsStartedFromZero = false;

                // 3. Calcula posição X sequencial
                int xPosition = series.Points.Count > 0
                ? (int)(series.Points.Last().XValue + 1)
                : 0;

                // 4. Limita a quantidade de pontos visíveis
                chartArea.AxisX.ScaleView.Size = 1300;
                chartArea.AxisX.ScaleView.Position = Math.Max(0, xPosition - 1300);

                // 5. Adiciona novo ponto
                series.Points.AddXY(xPosition, value);

                // 6. Mantém dados na faixa visível
                if (series.Points.Count > 1300)
                {
                    series.Points.RemoveAt(0);
                }

            }, null);
        }

        private void UpdateStatus(string message, bool isError)
        {
            _syncContext.Post(state =>
            {
                if (_targetChart.Parent is Form form && form.Controls.Find("statusLabel", true).FirstOrDefault() is Label label)
                {
                    label.Text = $"[{DateTime.Now:HH:mm:ss}] {message}";
                    label.ForeColor = isError ? Color.Red : Color.Black;
                }
            }, null);
        }

        public void StopReading()
        {
            _readingData = false;
            if (_activePort?.IsOpen == true)
            {
                _activePort.Close();
                WriteLog(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = "Information",
                    Message = "Conexão serial encerrada"
                });
                UpdateStatus("Conexão encerrada", false);
            }
            _activePort?.Dispose();
        }

        private class LogEntry
        {
            public DateTime Timestamp { get; set; }
            public string Level { get; set; }
            public string Message { get; set; }
            public string Exception { get; set; }
            public string Value { get; set; }
            public string RawData { get; set; }
        }
    }
}