using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using minas.teste.prototype.MVVM.Model.Abstract;
using minas.teste.prototype.Service;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    class ConnectionSettingsApplication : ConnectionSettingsBase
    {
        // Properties and methods specific to the application connection settings
        public static string PortName { get; set; }
        public static int BaudRate { get; set; }

        // A flag IsReceivingData e seu handler foram removidos, pois a validação do recebimento
        // de dados no formato esperado é feita no ArduinoPortFinder durante a detecção automática.

        private static SerialManager _persistentSerialManager;

        // Propriedade estática para acesso global.
        // A subscrição do evento DataReceived deve ser feita pela classe UI (Tela_Bombas)
        // que é responsável por processar e exibir os dados recebidos continuamente.
        public static SerialManager PersistentSerialManager
        {
            get
            {
                if (_persistentSerialManager == null)
                {
                    _persistentSerialManager = new SerialManager();
                    // **Não subscreva o DataReceived aqui**; faça-o na classe que lida com a UI e processamento contínuo, como Tela_Bombas.
                }
                return _persistentSerialManager;
            }
            private set => _persistentSerialManager = value;
        }

        // O handler PersistentSerialManager_DataReceived foi removido.

        public static void UpdateConnection(string port, int baudRate)
        {
            // Desconecta a conexão anterior, se existir
            if (_persistentSerialManager != null)
            {
                _persistentSerialManager.Disconnect();
            }

            // Garante que PersistentSerialManager é inicializado antes de conectar
            var managerToConnect = PersistentSerialManager;

            managerToConnect.Connect(port, baudRate);
            PortName = port;
            BaudRate = baudRate;
        }

        /// <summary>
        /// Tenta realizar a conexão automática encontrando uma porta que envia dados no formato esperado.
        /// Utiliza o finder para testar portas e baud rates.
        /// </summary>
        /// <param name="finder">A instância do ArduinoPortFinder a ser usada.</param>
        /// <returns>True se uma porta for encontrada e dados no formato esperado forem recebidos, False caso contrário.</returns>
        public static bool TryAutoConnect(ArduinoPortFinder finder)
        {
            // O finder agora testará todas as portas e baud rates e confirmará
            // a recepção de dados no formato específico.
            if (finder.FindArduinoAndConfirmData()) // Chama o método aprimorado
            {
                // Se o finder foi bem-sucedido, atualiza a conexão
                UpdateConnection(finder.ConnectedPort, finder.BaudRate); // Usa as propriedades do finder
                return true; // Conexão estabelecida e dados no formato esperado recebidos
            }
            // Se o finder falhou, nenhuma conexão funcional com o formato de dados esperado foi encontrada.
            return false;
        }

        public static void CloseAllConnections()
        {
            if (_persistentSerialManager != null)
            {
                _persistentSerialManager.Disconnect();
                _persistentSerialManager = null; // Permite a re-inicialização via getter se necessário
            }
        }
    }
}