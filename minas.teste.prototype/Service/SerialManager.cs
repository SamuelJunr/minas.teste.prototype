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
        private SynchronizationContext _context;
        public SerialManager()
        {
            // Garantir que o contexto seja capturado corretamente
            _context = SynchronizationContext.Current ?? new SynchronizationContext();
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

                // Verifica se a porta está disponível
                if (!AvailablePorts.Contains(portName))
                {
                    _isConnected = false;
                    OnErrorOccurred($"A porta {portName} não está disponível.");
                    return;
                }

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
            catch (UnauthorizedAccessException)
            {
                _isConnected = false;
                OnErrorOccurred($"Acesso negado à porta {portName}. Verifique se ela está em uso por outro programa ou execute o aplicativo como administrador.");
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
                    if (_serialPort.IsOpen)
                    {
                        // Desinscreve o manipulador de eventos DataReceived antes de fechar a porta
                        // para evitar chamadas após a porta ser fechada.
                        _serialPort.DataReceived -= HandleDataReceived;
                        _serialPort.Close();
                    }
                    // Libera os recursos nativos associados ao SerialPort
                    _serialPort.Dispose();
                    _serialPort = null; // Define como null para indicar que não há porta ativa
                    _isConnected = false;
                    OnConnectionStatusChanged(false);
                }
            }
            catch (Exception ex)
            {
                // Em caso de erro ao fechar, loga o erro mas continua para tentar liberar recursos
                OnErrorOccurred($"Erro ao desconectar: {ex.Message}");
                // Tenta descartar mesmo se fechar falhou, para liberar o handle do SO
                if (_serialPort != null)
                {
                    try { _serialPort.Dispose(); } catch { /* Ignora erros no dispose final */ }
                    _serialPort = null; // Garante que a referência seja nula
                }
                _isConnected = false;
                OnConnectionStatusChanged(false); // Garante que o status seja atualizado
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
            // Este método roda em uma thread secundária.
            // Qualquer acesso a controles de UI deve ser feito via Invoke/BeginInvoke.
            // O SafeInvoke já lida com isso.
            try
            {
                // Lê todos os dados disponíveis no buffer da porta serial
                var data = _serialPort.ReadExisting();
                if (!string.IsNullOrEmpty(data))
                {
                    // Dispara o evento DataReceived na thread do contexto de sincronização (UI thread se capturado)
                    SafeInvoke(() => DataReceived?.Invoke(this, data));
                }
            }
            catch (Exception ex)
            {
                // Lida com erros durante a recepção de dados
                OnErrorOccurred($"Erro na recepção: {ex.Message}");
                // Em caso de erro grave na recepção, pode ser necessário desconectar
                // para evitar um loop de erros. No entanto, desconectar aqui diretamente
                // pode causar deadlocks ou outros problemas de thread.
                // Uma abordagem mais segura seria sinalizar a thread da UI
                // para iniciar o processo de desconexão.
                // Por enquanto, apenas logamos o erro.
            }
        }

        /// <summary>
        /// Invoca uma ação na thread do contexto de sincronização capturado (geralmente a thread da UI).
        /// Se nenhum contexto foi capturado ou já estamos na thread correta, executa a ação diretamente.
        /// </summary>
        /// <param name="action">A ação a ser executada.</param>
        private void SafeInvoke(Action action)
        {
            // Verifica se há um contexto de sincronização e se não estamos já nele
            if (_context != null && _context != SynchronizationContext.Current)
            {
                // Posta a ação para ser executada na thread do contexto
                _context.Post(_ => action(), null);
            }
            else
            {
                // Executa a ação diretamente se não houver contexto ou já estiver na thread correta
                action();
            }
        }

        /// <summary>
        /// Dispara o evento ConnectionStatusChanged de forma segura para a thread.
        /// </summary>
        /// <param name="isConnected">O novo status da conexão.</param>
        private void OnConnectionStatusChanged(bool isConnected)
        {
            SafeInvoke(() => ConnectionStatusChanged?.Invoke(this, isConnected));
        }

        /// <summary>
        /// Dispara o evento ErrorOccurred de forma segura para a thread.
        /// </summary>
        /// <param name="message">A mensagem de erro.</param>
        private void OnErrorOccurred(string message)
        {
            SafeInvoke(() => ErrorOccurred?.Invoke(this, message));
        }

        /// <summary>
        /// Implementação do padrão IDisposable. Garante que a conexão serial seja fechada
        /// e os recursos liberados quando o objeto SerialManager for descartado.
        /// </summary>
        public void Dispose()
        {
            // Chama Disconnect para fechar a porta e liberar recursos.
            Disconnect();
            // Indica que o objeto já foi descartado para o coletor de lixo.
            GC.SuppressFinalize(this);
        }

        // Opcional: Finalizador (destrutor) caso Dispose não seja chamado explicitamente.
        // ~SerialManager()
        // {
        //     // Limpeza de recursos não gerenciados (a porta serial é um recurso não gerenciado)
        //     // Isso só deve ser feito se Dispose não foi chamado.
        //     // Disconnect(false); // Passa false para evitar disparar eventos ou logar novamente
        // }
    }
}
