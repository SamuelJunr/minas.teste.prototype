using System;
using System.IO;
using System.IO.Ports;
using System.ServiceProcess;
using System.Threading;
using Newtonsoft.Json;
using TimeoutException = System.TimeoutException;

namespace arduidomonitory
{
    public partial class Service1 : ServiceBase
    {
        private SerialPort _serialPort;
        private Thread _monitoringThread;
        private bool _isRunning;
        private const string LogPath = "C:\\ArduidoMonitory\\logs.json";
        private const string ConfigPath = "C:\\ArduidoMonitory\\config.json";

        public Service1()
        {
            InitializeComponent();
            this.ServiceName = "ArduidoMonitoryService";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {
            EnsureDirectories();
            _isRunning = true;
            _monitoringThread = new Thread(MonitorLoop)
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            _monitoringThread.Start();
        }

        private void EnsureDirectories()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
            if (!File.Exists(ConfigPath))
            {
                File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(new ServiceConfig
                {
                    BaudRate = 9600,
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One
                }));
            }
        }

        private void MonitorLoop()
        {
            while (_isRunning)
            {
                try
                {
                    if (_serialPort == null || !_serialPort.IsOpen)
                    {
                        InitializeSerialPort();
                    }

                    if (_serialPort != null && _serialPort.IsOpen)
                    {
                        ProcessIncomingData();
                    }
                }
                catch (Exception ex)
                {
                    LogError("Critical system error", ex);
                    Thread.Sleep(5000);
                }
            }
        }

        private void InitializeSerialPort()
        {
            var config = LoadConfig();
            foreach (var portName in SerialPort.GetPortNames())
            {
                try
                {
                    _serialPort = new SerialPort(
                        portName,
                        config.BaudRate,
                        config.Parity,
                        config.DataBits,
                        config.StopBits)
                    {
                        ReadTimeout = 500,
                        WriteTimeout = 500
                    };

                    _serialPort.Open();
                    LogMessage($"Connected to {portName}");
                    return;
                }
                catch (Exception ex)
                {
                    LogError($"Failed to connect to {portName}", ex);
                    _serialPort?.Dispose();
                    _serialPort = null;
                }
            }
        }

        private ServiceConfig LoadConfig()
        {
            return JsonConvert.DeserializeObject<ServiceConfig>(File.ReadAllText(ConfigPath));
        }

        private void ProcessIncomingData()
        {
            try
            {
                while (_isRunning && _serialPort.IsOpen)
                {
                    if (_serialPort.BytesToRead > 0)
                    {
                        var data = _serialPort.ReadLine();
                        ParseAndLogData(data.Trim());
                    }
                    Thread.Sleep(100);
                }
            }
            catch (TimeoutException) { }
            catch (Exception ex)
            {
                LogError("Data processing error", ex);
            }
        }

        private void ParseAndLogData(string rawData)
        {
            try
            {
                var segments = rawData.Split('|');
                var sensorData = new SensorData();

                foreach (var segment in segments)
                {
                    if (segment.Length < 2) continue;

                    var type = segment[0];
                    var value = segment.Substring(1);

                    switch (char.ToUpper(type))
                    {
                        case 'A' when value.Length == 4:
                            sensorData.AnalogValue = double.Parse(value);
                            break;

                        case 'B' when value.Length == 3:
                            sensorData.DigitalValue = int.Parse(value);
                            break;

                        case 'C' when value == "0" || value == "1":
                            sensorData.Status = value == "1";
                            break;
                    }
                }

                LogData(sensorData);
            }
            catch (Exception ex)
            {
                LogError("Data parsing error", ex);
            }
        }

        private void LogData(SensorData data)
        {
            var logEntry = new
            {
                Timestamp = DateTime.UtcNow,
                Source = "Arduino",
                Data = data
            };

            File.AppendAllText(LogPath,
                JsonConvert.SerializeObject(logEntry) + Environment.NewLine);
        }

        private void LogMessage(string message)
        {
            File.AppendAllText(LogPath,
                JsonConvert.SerializeObject(new
                {
                    Timestamp = DateTime.UtcNow,
                    Level = "INFO",
                    Message = message
                }) + Environment.NewLine);
        }

        private void LogError(string message, Exception ex)
        {
            File.AppendAllText(LogPath,
                JsonConvert.SerializeObject(new
                {
                    Timestamp = DateTime.UtcNow,
                    Level = "ERROR",
                    Message = message,
                    Exception = ex.ToString()
                }) + Environment.NewLine);
        }

        protected override void OnStop()
        {
            _isRunning = false;
            _monitoringThread?.Join(2000);

            if (_serialPort?.IsOpen == true)
            {
                _serialPort.Close();
                LogMessage("Service stopped gracefully");
            }
            _serialPort?.Dispose();
        }

        // Classes de suporte
        private class ServiceConfig
        {
            public int BaudRate { get; set; }
            public Parity Parity { get; set; }
            public int DataBits { get; set; }
            public StopBits StopBits { get; set; }
        }

        private class SensorData
        {
            public double? AnalogValue { get; set; }
            public int? DigitalValue { get; set; }
            public bool? Status { get; set; }
        }
    }
}