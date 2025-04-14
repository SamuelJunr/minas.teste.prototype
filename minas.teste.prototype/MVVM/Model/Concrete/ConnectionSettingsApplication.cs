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
        public static int baudRate { get; set; }

        private static SerialManager _persistentSerialManager;

        // Propriedade estática para acesso global
        public static SerialManager PersistentSerialManager
        {
            get
            {
                if (_persistentSerialManager == null)
                {
                    _persistentSerialManager = new SerialManager();
                }
                return _persistentSerialManager;
            }
            private set => _persistentSerialManager = value;
        }

        public static void UpdateConnection(string port, int baudRate)
        {
            PersistentSerialManager.Disconnect();
            PersistentSerialManager.Connect(port, baudRate);
            PortName = port;
            BaudRate = baudRate;
        }

        public static bool TryAutoConnect()
        {
            var finder = new ArduinoPortFinder();
            if (finder.FindArduino(BaudRate == 0 ? 115200 : BaudRate))
            {
                UpdateConnection(finder.ConnectedPort, finder.baudRate);
                return true;
            }
            return false;
        }
        public static void CloseAllConnections()
        {
            if (_persistentSerialManager != null)
            {
                PersistentSerialManager.Disconnect();
            }
        }
    }
}
