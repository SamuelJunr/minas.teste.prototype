using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq; // Adicionado para .Contains
using System.Threading;
using System.Diagnostics; // Para Debug.WriteLine

namespace minas.teste.prototype.Service
{
    public class SerialManager : IDisposable
    {
        public List<string> AvailablePorts { get; private set; } = new List<string>();
        public static List<int> CommonBaudRates { get; } = new List<int>
        {
            1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200 
        };
        private bool _isConnected;
        public bool IsConnected => _isConnected;

        public string PortName { get; internal set; }
        public int BaudRate { get; internal set; }

        public event EventHandler<string> DataReceived;
        public event EventHandler<bool> ConnectionStatusChanged;
        public event EventHandler<string> ErrorOccurred; // Alterado para EventHandler<string> para passar a mensagem de erro

        private SerialPort _serialPort;
        private readonly SynchronizationContext _context; // Tornar readonly

        // Buffer para leitura mais eficiente
        private byte[] _readBuffer; // Buffer de bytes para leitura
        private const int DefaultReadBufferSize = 4096; // Pode ser ajustado

        public SerialManager()
        {
            _context = SynchronizationContext.Current ?? new SynchronizationContext();
            RefreshPorts();
        }

        public void RefreshPorts()
        {
            try
            {
                AvailablePorts.Clear();
                AvailablePorts.AddRange(SerialPort.GetPortNames());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SerialManager: Erro ao listar portas: {ex.Message}");
                OnErrorOccurred($"Erro ao listar portas disponíveis: {ex.Message}");
                AvailablePorts.Clear(); // Garante que a lista esteja vazia em caso de erro
            }
        }

        public void Connect(string portName, int baudRate)
        {
            if (string.IsNullOrEmpty(portName))
            {
                OnErrorOccurred("Nome da porta não pode ser vazio.");
                return;
            }

            try
            {
                if (_serialPort?.IsOpen == true) Disconnect();

                RefreshPorts(); // Atualiza a lista de portas antes de verificar
                if (!AvailablePorts.Contains(portName)) // Usar Linq.Contains
                {
                    _isConnected = false; // Garantir que esteja falso
                    OnConnectionStatusChanged(false); // Notificar status
                    OnErrorOccurred($"A porta {portName} não está disponível ou foi desconectada.");
                    return;
                }

                _serialPort = new SerialPort(portName, baudRate)
                {
                    Parity = Parity.None,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Handshake = Handshake.None,
                    ReadTimeout = 500,  // Aumentado ligeiramente para flexibilidade, mas ReadExisting não bloqueia por muito tempo
                    WriteTimeout = 500,
                    ReadBufferSize = DefaultReadBufferSize * 2, // Aumenta o buffer interno do SerialPort (Sugestão 1)
                    ReceivedBytesThreshold = 1 // Dispara evento o mais rápido possível (Sugestão 1)
                };

                _readBuffer = new byte[_serialPort.ReadBufferSize]; // Inicializa o buffer de bytes

                _serialPort.DataReceived += HandleDataReceived;
                _serialPort.ErrorReceived += HandleErrorReceived; // Adiciona handler para erros da porta
                _serialPort.Open();
                _isConnected = true;
                OnConnectionStatusChanged(true);
            }
            catch (UnauthorizedAccessException ex)
            {
                _isConnected = false;
                OnConnectionStatusChanged(false);
                OnErrorOccurred($"Acesso negado à porta {portName}. Verifique se ela está em uso ou se há permissões. Detalhes: {ex.Message}");
            }
            catch (System.IO.IOException ex)
            {
                _isConnected = false;
                OnConnectionStatusChanged(false);
                OnErrorOccurred($"Erro de IO na porta {portName}. A porta pode não existir ou o dispositivo foi removido. Detalhes: {ex.Message}");
            }
            catch (Exception ex)
            {
                _isConnected = false;
                OnConnectionStatusChanged(false);
                OnErrorOccurred($"Erro desconhecido ao conectar à porta {portName}: {ex.Message}");
            }
        }

        public void Disconnect()
        {
            if (_serialPort == null) return;

            try
            {
                if (_serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= HandleDataReceived;
                    _serialPort.ErrorReceived -= HandleErrorReceived;

                    // Em algumas situações, fechar a porta pode bloquear ou demorar.
                    // Considerar executar em uma Task se isso se tornar um problema de responsividade da UI ao desconectar.
                    _serialPort.Close();
                }
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Erro ao fechar a porta em Disconnect: {ex.Message}");
            }
            finally // Garante que Dispose seja chamado e o status atualizado
            {
                // _serialPort.Dispose() também chama Close() internamente se a porta estiver aberta.
                _serialPort.Dispose();
                _serialPort = null;
                _readBuffer = null; // Limpar buffer
                if (_isConnected) // Só altera e notifica se o estado realmente mudou
                {
                    _isConnected = false;
                    OnConnectionStatusChanged(false);
                }
            }
        }

        private void HandleErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            // Erros como Parity, Frame, Overrun podem ser tratados aqui.
            OnErrorOccurred($"Erro da porta serial: {e.EventType}");
            // Dependendo do erro, você pode querer tentar uma desconexão/reconexão
            // ou apenas logar e informar o usuário.
        }

        public void SendData(string data)
        {
            try
            {
                if (_serialPort?.IsOpen == true)
                {
                    // Adicionar um NewLine se o dispositivo esperar por isso com WriteLine
                    // ou usar _serialPort.Write(data) se a terminação já estiver na string 'data'.
                    _serialPort.WriteLine(data);
                }
                else
                {
                    OnErrorOccurred("Não é possível enviar dados. Porta não está aberta.");
                }
            }
            catch (TimeoutException ex)
            {
                OnErrorOccurred($"Timeout ao enviar dados: {ex.Message}");
            }
            catch (Exception ex)
            {
                OnErrorOccurred($"Erro ao enviar dados: {ex.Message}");
            }
        }

        private void HandleDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (_serialPort == null || !_serialPort.IsOpen) return;

            try
            {
                // Lê todos os bytes disponíveis para o buffer _readBuffer.
                // Isso é mais eficiente do que ReadExisting() que cria uma nova string a cada chamada
                // e pode envolver múltiplas leituras internas.
                int bytesToRead = _serialPort.BytesToRead;
                if (bytesToRead > 0)
                {
                    if (bytesToRead > _readBuffer.Length)
                    {
                        // Isso não deveria acontecer se ReadBufferSize for grande o suficiente
                        // e os dados forem lidos com frequência. Se acontecer, pode ser necessário
                        // um buffer maior ou uma estratégia de leitura em loop.
                        Debug.WriteLine($"SerialManager: BytesToRead ({bytesToRead}) excede o tamanho do _readBuffer ({_readBuffer.Length}). Lendo em partes ou truncando.");
                        // Para simplificar, vamos ler apenas o que cabe no buffer.
                        // Uma solução mais robusta leria em loop até esvaziar o buffer da porta.
                        bytesToRead = _readBuffer.Length;
                    }

                    int bytesRead = _serialPort.Read(_readBuffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        // Converte os bytes lidos para string usando a codificação da porta (geralmente ASCII ou UTF-8 para Arduino)
                        // SerialPort.Encoding padrão é ASCII. Se o Arduino envia UTF-8 ou outros caracteres, ajuste.
                        string data = _serialPort.Encoding.GetString(_readBuffer, 0, bytesRead);
                        SafeInvoke(() => DataReceived?.Invoke(this, data));
                    }
                }
            }
            catch (TimeoutException)
            {
                // ReadTimeout pode ocorrer se a porta for fechada enquanto uma leitura está pendente,
                // ou se o dispositivo parar de enviar dados inesperadamente.
                // Geralmente, com ReceivedBytesThreshold = 1 e leitura reativa, isso é menos comum.
                Debug.WriteLine("SerialManager: Timeout na leitura da porta serial.");
                // Não necessariamente um erro a ser propagado para o usuário como crítico,
                // a menos que se torne frequente.
            }
            catch (Exception ex)
            {
                // Erros inesperados durante a leitura
                OnErrorOccurred($"Erro crítico na recepção de dados: {ex.Message}");
                // Considerar uma desconexão controlada aqui se o erro for grave e repetitivo.
                // Ex: SafeInvoke(() => AttemptControlledDisconnectAndNotify("Erro crítico na leitura: " + ex.Message));
            }
        }

        private void SafeInvoke(Action action)
        {
            if (_context != SynchronizationContext.Current && _context != null) // Verifica se _context não é null
            {
                _context.Post(_ => action(), null);
            }
            else
            {
                action();
            }
        }

        private void OnConnectionStatusChanged(bool connected)
        {
            SafeInvoke(() => ConnectionStatusChanged?.Invoke(this, connected));
        }

        private void OnErrorOccurred(string message)
        {
            SafeInvoke(() => ErrorOccurred?.Invoke(this, message));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Liberar recursos gerenciados (como o SerialPort)
                Disconnect(); // Disconnect já lida com _serialPort?.Dispose()
            }
            // Liberar recursos não gerenciados (se houver)
        }

        
    }
}