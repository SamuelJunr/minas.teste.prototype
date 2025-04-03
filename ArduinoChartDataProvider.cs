using System.ComponentModel;

namespace minas.teste.prototype.Service
{
    public class ArduinoChartDataProvider : INotifyPropertyChanged
    {
        private SerialPort _activePort;
        private bool _readingData;
        private readonly Chart _targetChart;
        private readonly SynchronizationContext _syncContext;
        private const string LogFile = "serial_log.json";
        private readonly object _logLock = new object();

        // Variáveis para armazenamento dos valores
        private double _psiValue;
        public double PsiValue
        {
            get => _psiValue;
            private set
            {
                if (_psiValue != value)
                {
                    _psiValue = value;
                    OnPropertyChanged(nameof(PsiValue));
                }
            }
        }

        private int _barValue;
        public int BarValue
        {
            get => _barValue;
            private set
            {
                if (_barValue != value)
                {
                    _barValue = value;
                    OnPropertyChanged(nameof(BarValue));
                }
            }
        }

        private int _rpmValue;
        public int RpmValue
        {
            get => _rpmValue;
            private set
            {
                if (_rpmValue != value)
                {
                    _rpmValue = value;
                    OnPropertyChanged(nameof(RpmValue));
                }
            }
        }

        // Controles UI para exibição (renomeados para clareza)
        private readonly Label _lblPsi, _lblBar, _lblRpm;

        public ArduinoChartDataProvider(Chart chart, Label lblPsi, Label lblBar, Label lblRpm)
        {
            _targetChart = chart;
            _lblPsi = lblPsi;
            _lblBar = lblBar;
            _lblRpm = lblRpm;
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

                // Configuração das séries (mantendo para visualização histórica, se desejado)
                CreateSeries("Psi", Color.Blue, SeriesChartType.FastLine);
                CreateSeries("Bar", Color.Red, SeriesChartType.FastLine);
                CreateSeries("Rpm", Color.Green, SeriesChartType.FastLine);

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
                UpdateStatus($"Conectado à porta {portName}", false);
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
                    case 'A' when ValidatePsi(valuePart):
                        HandleValuePsi(valuePart);
                        break;

                    case 'B' when ValidateBar(valuePart):
                        HandleValueBar(valuePart);
                        break;

                    case 'C' when ValidateRpm(valuePart):
                        HandleValueRpm(valuePart);
                        break;

                    default:
                        LogInvalidSegment(segment, rawData);
                        break;
                }
            }
        }

        private bool ValidatePsi(string value) => value.Length >= 1 && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _);
        private bool ValidateBar(string value) => value.Length >= 1 && int.TryParse(value, out _);
        private bool ValidateRpm(string value) => value.Length >= 1 && int.TryParse(value, out _);

        private void HandleValuePsi(string value)
        {
            if (double.TryParse(value, CultureInfo.InvariantCulture, out double psi))
            {
                PsiValue = psi;
                UpdateUIValue(_lblPsi, $"PSI: {PsiValue:F2}");
                AddDataPointToChart("Psi", PsiValue);
                LogValue("PSI", PsiValue);
            }
            else
            {
                LogInvalidValue("PSI", value);
            }
        }

        private void HandleValueBar(string value)
        {
            if (int.TryParse(value, out int bar))
            {
                BarValue = bar;
                UpdateUIValue(_lblBar, $"BAR: {BarValue:D3}");
                AddDataPointToChart("Bar", BarValue);
                LogValue("BAR", BarValue);
            }
            else
            {
                LogInvalidValue("BAR", value);
            }
        }

        private void HandleValueRpm(string value)
        {
            if (int.TryParse(value, out int rpm))
            {
                RpmValue = rpm;
                UpdateUIValue(_lblRpm, $"RPM: {RpmValue:D}");
                AddDataPointToChart("Rpm", RpmValue);
                LogValue("RPM", RpmValue);
            }
            else
            {
                LogInvalidValue("RPM", value);
            }
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

        private void LogInvalidValue(string type, string value)
        {
            WriteLog(new LogEntry
            {
                Timestamp = DateTime.Now,
                Level = "Warning",
                Message = $"Valor inválido para {type}: {value}"
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

                chartArea.AxisX.Minimum = 0;
                chartArea.AxisX.Maximum = 1300;
                chartArea.AxisX.Interval = 100;
                chartArea.AxisY.IsStartedFromZero = false;
                chartArea.AxisX.ScaleView.Size = 1300;
                chartArea.AxisX.ScaleView.Position = Math.Max(0, series.Points.Count - 1300);

                series.Points.AddXY(series.Points.Count, value);

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
