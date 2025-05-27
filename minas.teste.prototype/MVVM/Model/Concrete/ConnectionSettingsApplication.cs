using System;
using System.Diagnostics;
using minas.teste.prototype.Service; // Certifique-se que este using está presente e correto

// Remova qualquer using desnecessário que possa causar ambiguidade, como:
// using minas.teste.prototype.MVVM.Model.Concrete.ArduinoPortFinder; // Se algo assim existir por engano

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public static class ConnectionSettingsApplication
    {
        public static string CurrentPortName { get; private set; }
        public static int CurrentBaudRate { get; private set; }
        public static bool IsCurrentlyConnected => _persistentSerialManager?.IsConnected ?? false;

        private static SerialManager _persistentSerialManager;

        public static SerialManager PersistentSerialManager
        {
            get
            {
                if (_persistentSerialManager == null)
                {
                    Debug.WriteLine("ConnectionSettingsApplication: Criando nova instância de PersistentSerialManager.");
                    _persistentSerialManager = new SerialManager();
                }
                return _persistentSerialManager;
            }
        }

        public static bool UpdateConnection(string port, int baudRate)
        {
            Debug.WriteLine($"ConnectionSettingsApplication: Tentando UpdateConnection para Porta: {port}, Baud: {baudRate}");
            var manager = PersistentSerialManager;

            if (manager.IsConnected)
            {
                Debug.WriteLine($"ConnectionSettingsApplication: Desconectando conexão existente em {CurrentPortName}@{CurrentBaudRate} antes de tentar nova conexão.");
                manager.Disconnect();
            }

            manager.Connect(port, baudRate);

            if (manager.IsConnected)
            {
                CurrentPortName = port;
                CurrentBaudRate = baudRate;
                Debug.WriteLine($"ConnectionSettingsApplication: Conexão bem-sucedida em {CurrentPortName}@{CurrentBaudRate}.");
                return true;
            }
            else
            {
                Debug.WriteLine($"ConnectionSettingsApplication: Falha ao conectar em {port}@{baudRate}.");
                return false;
            }
        }

        /// <summary>
        /// Tenta realizar a conexão automática.
        /// Utiliza o ArduinoPortFinder para encontrar uma porta e baud rate válidos,
        /// e então tenta estabelecer a conexão no PersistentSerialManager.
        /// </summary>
        /// <param name="finder">A instância do ArduinoPortFinder do namespace minas.teste.prototype.Service.</param>
        /// <returns>True se a autoconexão e a configuração do PersistentSerialManager forem bem-sucedidas.</returns>
        public static bool TryAutoConnect(ArduinoPortFinder finder) // <--- CORREÇÃO APLICADA AQUI
        {
            if (finder == null)
            {
                Debug.WriteLine("ConnectionSettingsApplication.TryAutoConnect: ArduinoPortFinder não pode ser nulo.");
                return false;
            }

            Debug.WriteLine("ConnectionSettingsApplication.TryAutoConnect: Chamando finder.FindArduinoAndConfirmData()...");
            if (finder.TryFindArduinoAndConfirmData()) // finder executa a busca e validação
            {
                Debug.WriteLine($"ConnectionSettingsApplication.TryAutoConnect: Finder encontrou porta candidata: {finder.ConnectedPort}@{finder.BaudRate}. Tentando conexão persistente...");
                // Se o finder foi bem-sucedido, usa UpdateConnection para estabelecer
                // a conexão no PersistentSerialManager.
                if (UpdateConnection(finder.ConnectedPort, finder.BaudRate))
                {
                    Debug.WriteLine("ConnectionSettingsApplication.TryAutoConnect: Conexão persistente estabelecida com sucesso.");
                    return true;
                }
                else
                {
                    Debug.WriteLine("ConnectionSettingsApplication.TryAutoConnect: Finder encontrou uma porta, mas falha ao estabelecer conexão persistente.");
                    return false;
                }
            }

            Debug.WriteLine("ConnectionSettingsApplication.TryAutoConnect: Finder não encontrou nenhuma porta Arduino válida ou não confirmou os dados.");
            return false;
        }

        public static void UpdateCurrentSettings(string portName, int baudRate)
        {
            CurrentPortName = portName;
            CurrentBaudRate = baudRate;
        }

        public static void CloseAllConnections()
        {
            if (_persistentSerialManager != null)
            {
                Debug.WriteLine("ConnectionSettingsApplication: Iniciando CloseAllConnections.");
                _persistentSerialManager.Disconnect();
                _persistentSerialManager.Dispose();
                _persistentSerialManager = null;
                Debug.WriteLine("ConnectionSettingsApplication: Instância PersistentSerialManager desconectada e descartada.");
            }
            else
            {
                Debug.WriteLine("ConnectionSettingsApplication: Nenhuma instância PersistentSerialManager para fechar.");
            }

            CurrentPortName = null;
            CurrentBaudRate = 0;
            Debug.WriteLine("ConnectionSettingsApplication: Configurações de porta e baud rate resetadas.");
        }
    }
}
