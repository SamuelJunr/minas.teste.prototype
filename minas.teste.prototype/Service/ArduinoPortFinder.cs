using System;
using System.IO.Ports;
using System.Threading;

public class ArduinoPortFinder
{
    public string ConnectedPort { get; private set; }
    public int baudRate ,timeout = 500;

    public bool FindArduino(int baudRate = 115200, int timeout =500)
    {
        string[] ports = SerialPort.GetPortNames();
        foreach (string port in ports)
        {
            using (SerialPort sp = new SerialPort(port, baudRate))
            {
                sp.ReadTimeout = timeout;
                sp.WriteTimeout = timeout;

                try
                {
                    sp.Open();

                    // Limpa buffers para garantir comunicação limpa
                    sp.DiscardInBuffer();
                    sp.DiscardOutBuffer();

                    // Envia comando AT
                    sp.WriteLine("AT");

                    // Espera um curto período para resposta
                    Thread.Sleep(100);

                    // Lê a resposta
                    string response = sp.ReadExisting();

                    // Valida se a resposta é uma string não vazia
                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        ConnectedPort = port;
                        return true;
                    }
                }
                catch
                {
                    // Ignora erros e continua para a próxima porta
                    continue;
                }
            }
        }
        return false;
    }
}