using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;


namespace minas.teste.prototype.Service
{
    
    public class SerialManager : IDisposable
    {
        // Configurações
        public List<string> AvailablePorts { get; private set; } = new List<string>();
        public static List<int> CommonBaudRates { get; } = new List<int>
    {
        300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200
    };
        private bool _isConnected;
        public bool IsConnected => _isConnected;

        // Eventos
        public event EventHandler<string> DataReceived;
        public event EventHandler<bool> ConnectionStatusChanged;
        public event EventHandler<string> ErrorOccurred;

        private SerialPort _serialPort;
        private SynchronizationContext _context = SynchronizationContext.Current;

        public SerialManager()
        {
            RefreshPorts();
        }

        public void RefreshPorts()
        {
            AvailablePorts.Clear();
            AvailablePorts.AddRange(SerialPort.GetPortNames());
        }

        public void Connect(string portName, int baudRate)
        {
            try
            {
                if (_serialPort?.IsOpen == true) Disconnect();

                _serialPort = new SerialPort(portName, baudRate)
                {
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    ReadTimeout = 100,
                    WriteTimeout = 100
                };

                _serialPort.DataReceived += HandleDataReceived;
                _serialPort.Open();
                _isConnected = true;
                OnConnectionStatusChanged(true);
            }
            catch (Exception ex)
            {
                _isConnected = false;
                OnErrorOccurred($"Erro na conexão: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            try
            {
                if (_serialPort != null)
                {
                    _serialPort.Close();
                    _serialPort.DataReceived -= HandleDataReceived;
                    _serialPort.Dispose();
                    _serialPort = null;
                    _isConnected = false;
                    OnConnectionStatusChanged(false);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Erro ao desconectar: {ex.Message}");
            }
        }

        public void SendData(string data)
        {
            try
            {
                if (_serialPort?.IsOpen == true)
                {
                    _serialPort.WriteLine(data);
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Erro ao enviar dados: {ex.Message}");
            }
        }

        private void HandleDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var data = _serialPort.ReadLine();
                SafeInvoke(() => DataReceived?.Invoke(this, data));
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Erro na recepção: {ex.Message}");
            }
        }

        private void SafeInvoke(Action action)
        {
            if (_context != null)
                _context.Post(_ => action(), null);
            else
                action();
        }

        private void OnConnectionStatusChanged(bool isConnected)
        {
            SafeInvoke(() => ConnectionStatusChanged?.Invoke(this, isConnected));
        }

        private void OnErrorOccurred(string message)
        {
            SafeInvoke(() => ErrorOccurred?.Invoke(this, message));
        }

        public void Dispose()
        {
            Disconnect();
        }
    }
}
