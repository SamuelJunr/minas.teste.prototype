using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using minas.teste.prototype.Service;

namespace minas.teste.prototype.MVVM.ViewModel
{
    public class BombasVM : INotifyPropertyChanged
    {
        private readonly ArduinoChartDataProvider _dataProvider;
        private string _psi;
        private string _bar;
        private string _rpm;

        public string Psi
        {
            get => _psi;
            set
            {
                if (_psi != value)
                {
                    _psi = value;
                    OnPropertyChanged(nameof(Psi));
                }
            }
        }

        public string Bar
        {
            get => _bar;
            set
            {
                if (_bar != value)
                {
                    _bar = value;
                    OnPropertyChanged(nameof(Bar));
                }
            }
        }

        public string Rpm
        {
            get => _rpm;
            set
            {
                if (_rpm != value)
                {
                    _rpm = value;
                    OnPropertyChanged(nameof(Rpm));
                }
            }
        }

        public BombasVM(Chart chart, Label lblPsi, Label lblBar, Label lblRpm)
        {
            _dataProvider = new ArduinoChartDataProvider(chart, lblPsi, lblBar, lblRpm);
            _dataProvider.PropertyChanged += DataProvider_PropertyChanged;
            _dataProvider.StartMonitoring();
            UpdateDisplayValues(); // Inicializa os valores na UI
        }

        private void DataProvider_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateDisplayValues();
        }

        private void UpdateDisplayValues()
        {
            Psi = $"{_dataProvider.PsiValue:F2} PSI";
            Bar = $"{_dataProvider.BarValue:D3} BAR";
            Rpm = $"{_dataProvider.RpmValue:D} RPM";
        }

        public void StopMonitoring()
        {
            _dataProvider.StopReading();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

public class ArduinoChartDataProvider : INotifyPropertyChanged
{
    private SerialPort _activePort;
    private bool _readingData;
    private readonly Chart _targetChart;
    private readonly SynchronizationContext _syncContext;
    private const string LogFile = "serial_log.json";
    private readonly object _logLock;
    public double PsiValue { get; private set; }
    public int BarValue { get; private set; }
    public int RpmValue { get; private set; }
    private readonly Label _lblPsi;
    private readonly Label _lblBar;
    private readonly Label _lblRpm;
    private void InitializeLogFile();
    private void InitializeChart();
    private void CreateSeries(string name, Color color, SeriesChartType type);
    private void WriteLog(ArduinoChartDataProvider.LogEntry entry);
    public void StartMonitoring();
    private void CheckAndReadFromArduino();
    private bool ValidateAndConnect(string portName);
    private void StartReadingData();
    private void ReadDataFromPort();
    private void ProcessData(string rawData);
    private bool ValidatePsi(string value);
    private bool ValidateBar(string value);
    private bool ValidateRpm(string value);
    private void HandleValuePsi(string value);
    private void HandleValueBar(string value);
    private void HandleValueRpm(string value);
    private void LogInvalidSegment(string segment, string rawData);
    private void LogInvalidValue(string type, string value);
    private void LogValue<T>(string type, T value);
    private void UpdateUIValue(Label label, string text);
    private void AddDataPointToChart(string seriesName, double value);
    private void UpdateStatus(string message, bool isError);
    public void StopReading();
    public event PropertyChangedEventHandler PropertyChanged;
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
