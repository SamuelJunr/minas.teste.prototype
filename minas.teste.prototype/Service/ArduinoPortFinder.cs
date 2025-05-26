using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using minas.teste.prototype.Service; // Assumindo que SerialManager está aqui
using System.Linq;
// using System.Text.RegularExpressions; // Regex não é estritamente necessário com o novo parsing

public class ArduinoPortFinder
{
    public string ConnectedPort { get; private set; }
    public int BaudRate { get; private set; }
    private int connectionTimeout = 3000;
    private string receivedDataString = null;

    private readonly List<string> expectedKeys = new List<string>
    {
        "HA1","HA2","HB1","HB2","MA1","MA2","MB1","MB2","TEM","ROT","DR1","DR2","DR3","DR4","PL1","PL2","PL3","PL4","PR1","PR2","PR3","PR4","VZ1","VZ2","VZ3","VZ4"

    };

    /// <summary>
    /// Encontra uma porta serial que possa conectar e receber dados no formato esperado (KEY:VALUE|...),
    /// testando os baud rates comuns.
    /// </summary>
    /// <returns>True se uma porta for encontrada e dados no formato esperado forem recebidos, False caso contrário.</returns>
    public bool TryFindArduinoAndConfirmData()
    {
        string[] ports = SerialPort.GetPortNames();
        List<int> baudRatesToTest = SerialManager.CommonBaudRates;

        foreach (string port in ports)
        {
            foreach (int currentBaudRate in baudRatesToTest)
            {
                receivedDataString = null;

                using (SerialPort sp = new SerialPort(port, currentBaudRate))
                {
                    sp.ReadTimeout = connectionTimeout;
                    sp.WriteTimeout = connectionTimeout;

                    using (var dataReceivedEvent = new ManualResetEvent(false))
                    {
                        SerialDataReceivedEventHandler dataHandler = (sender, e) =>
                        {
                            try
                            {
                                string data = sp.ReadExisting();
                                if (!string.IsNullOrEmpty(data))
                                {
                                    receivedDataString += data;
                                    dataReceivedEvent.Set();
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Erro no handler DataReceived para {port}@{currentBaudRate}: {ex.Message}");
                            }
                        };

                        try
                        {
                            sp.DataReceived += dataHandler;
                            sp.Open();

                            sp.DiscardInBuffer();
                            sp.DiscardOutBuffer();

                            try
                            {
                                sp.WriteLine("REQUEST_DATA\n");
                            }
                            catch (TimeoutException) { }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Erro ao enviar comando de teste para {port}@{currentBaudRate}: {ex.Message}");
                            }

                            bool signaled = dataReceivedEvent.WaitOne(connectionTimeout);

                            sp.DataReceived -= dataHandler;

                            if (!string.IsNullOrEmpty(receivedDataString))
                            {
                                if (IsValidDataFormat(receivedDataString))
                                {
                                    ConnectedPort = port;
                                    BaudRate = currentBaudRate;

                                    if (sp.IsOpen) sp.Close();

                                    return true;
                                }
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            Console.WriteLine($"Porta {port} está em uso. Pulando.");
                            sp.DataReceived -= dataHandler;
                            continue;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro ao testar a porta {port} com baud rate {currentBaudRate}: {ex.Message}");
                            sp.DataReceived -= dataHandler;
                        }
                        finally
                        {
                            if (sp.IsOpen)
                            {
                                sp.Close();
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    private bool IsValidDataFormat(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return false;
        }

        string cleanedData = data.Trim();

        if (string.IsNullOrEmpty(cleanedData))
        {
            return false;
        }

        string[] pairs = cleanedData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        if (pairs.Length == 0)
        {
            return false;
        }

        foreach (string pair in pairs)
        {
            string[] parts = pair.Split(':');
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();

                if (expectedKeys.Contains(key))
                {
                    Console.WriteLine($"Chave esperada '{key}' encontrada em um par chave-valor válido.");
                    return true;
                }
            }
        }

        Console.WriteLine("Nenhum par KEY:VALUE com uma chave esperada encontrado nos dados recebidos.");
        return false;
    }
}
