namespace minas.teste.prototype.Service
{
    public class SerialManager
    {
        private SerialPort _serialPort;

        public SerialManager()
        {
            _serialPort = new SerialPort();
            _serialPort.DataReceived += HandleDataReceived; // Correct placement of the event subscription
        }

        private void HandleDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // Implement the logic to handle data received from the serial port.
        }
    }
}
