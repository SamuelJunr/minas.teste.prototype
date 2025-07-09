using System;
using System.IO.Ports;
using System.Threading;
using System.Collections.Generic;
using minas.teste.prototype.Service; // Assumindo que SerialManager está aqui
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
// using System.Text.RegularExpressions; // Regex não é estritamente necessário com o novo parsing

public class ArduinoPortFinder
{
    public string ConnectedPort { get; private set; }
    public int BaudRate { get; private set; }
    private int connectionTimeout = 3000; // 3 seconds timeout for each test
    private CancellationTokenSource cancellationTokenSource;

    // Use HashSet for O(1) average time complexity for lookups
    private readonly HashSet<string> expectedKeys = new HashSet<string>
    {
 
        "HA1","HA2","HB1","HB2","MA1","MA2","MB1","MB2","TEM","ROT","DR1","DR2","DR3","DR4","PL1","PL2","PL3","PL4","PR1","PR2","PR3","PR4","VZ1","VZ2","VZ3","VZ4"

 
        "HA1:", "HA2:", "HB1:", "HB2:", "MA1:", "MA2:", "MB1:", "MB2:", "TEM:", "ROT:",
        "DR1:", "DR2:", "DR3:", "DR4:", "PL1:", "PL2:", "PL3:", "PL4:", "PR1:", "PR2:",
        "PR3:", "PR4:", "VZ1:", "VZ2:", "VZ3:", "VZ4:"
 ( )
    };

    /// <summary>
    /// Encontra uma porta serial que possa conectar e receber dados no formato esperado (KEY:VALUE|...),
    /// testando os baud rates comuns de forma assíncrona.
    /// </summary>
    /// <returns>True se uma porta for encontrada e dados no formato esperado forem recebidos, False caso contrário.</returns>
    public async Task<bool> TryFindArduinoAndConfirmDataAsync()
    {
        string[] ports = SerialPort.GetPortNames();
        List<int> baudRatesToTest = SerialManager.CommonBaudRates;

        // Use a ConcurrentBag to collect successful connections, if multiple are found
        // However, since we want the first one, a simple lock or Interlocked can be used.
        // For this scenario, we'll just return true and cancel others as soon as one is found.
        cancellationTokenSource = new CancellationTokenSource();

        var tasks = new List<Task<bool>>();

        foreach (string port in ports)
        {
            foreach (int currentBaudRate in baudRatesToTest)
            {
                tasks.Add(Task.Run(() => TryConnectAndReadAsync(port, currentBaudRate, cancellationTokenSource.Token)));
            }
        }

        // Wait for any task to complete and return true if successful
        // Or wait for all tasks to complete if no success is found
        while (tasks.Any())
        {
            Task<bool> finishedTask = await Task.WhenAny(tasks);
            tasks.Remove(finishedTask);

            if (await finishedTask)
            {
                // A successful connection was made, cancel any remaining tasks
                cancellationTokenSource.Cancel();
                return true;
            }
        }

        return false;
    }

    private async Task<bool> TryConnectAndReadAsync(string port, int baudRate, CancellationToken cancellationToken)
    {
        // Use a StringBuilder for efficient string concatenation in the DataReceived event
        StringBuilder receivedDataStringBuilder = new StringBuilder();
        SerialPort sp = null; // Declare outside try-finally to ensure it's disposed
        ManualResetEvent dataReceivedEvent = new ManualResetEvent(false);

        SerialDataReceivedEventHandler dataHandler = (sender, e) =>
        {
            try
            {
                // Check for cancellation early
                if (cancellationToken.IsCancellationRequested) return;

                SerialPort currentSp = (SerialPort)sender;
                string data = currentSp.ReadExisting();
                if (!string.IsNullOrEmpty(data))
                {
                    receivedDataStringBuilder.Append(data);
                    // Signal that data has been received, but only once we think we have enough.
                    // A better approach would be to check for a full message terminator,
                    // but for now, we'll signal just on any data.
                    dataReceivedEvent.Set();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no handler DataReceived para {port}@{baudRate}: {ex.Message}");
            }
        };

        try
        {
            sp = new SerialPort(port, baudRate);
            sp.ReadTimeout = connectionTimeout;
            sp.WriteTimeout = connectionTimeout;

            sp.DataReceived += dataHandler;
            sp.Open();

            sp.DiscardInBuffer();
            sp.DiscardOutBuffer();

            // Send a request command if needed, but don't block
            try
            {
                // You might need to adjust the delay based on how quickly the Arduino responds
                await Task.Delay(100, cancellationToken); // Give a little time for the Arduino to get ready
                sp.WriteLine("REQUEST_DATA\n");
            }
            catch (TimeoutException) { /* This will be caught by the outer catch if the port isn't open */ }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar comando de teste para {port}@{baudRate}: {ex.Message}");
            }

            // Wait for data or timeout
            bool signaled = dataReceivedEvent.WaitOne(connectionTimeout);

            if (cancellationToken.IsCancellationRequested)
            {
                return false; // Another port was found
            }

            if (!string.IsNullOrEmpty(receivedDataStringBuilder.ToString()))
            {
                if (IsValidDataFormat(receivedDataStringBuilder.ToString()))
                {
                    ConnectedPort = port;
                    BaudRate = baudRate;
                    return true;
                }
            }
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"Porta {port} está em uso. Pulando.");
            return false;
        }
        catch (Exception ex)
        {
            // Log other exceptions
            Console.WriteLine($"Erro ao testar a porta {port} com baud rate {baudRate}: {ex.Message}");
            return false;
        }
        finally
        {
            // Always unsubscribe and dispose
            if (sp != null)
            {
                sp.DataReceived -= dataHandler;
                if (sp.IsOpen)
                {
                    sp.Close();
                }
                sp.Dispose();
            }
            dataReceivedEvent.Dispose();
        }
    }

    private bool IsValidDataFormat(string data)
    {
        if (string.IsNullOrWhiteSpace(data)) // Use IsNullOrWhiteSpace for better handling of whitespace-only strings
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

                // O(1) lookup thanks to HashSet
                if (expectedKeys.Contains(key))
                {
                    // Console.WriteLine($"Chave esperada '{key}' encontrada em um par chave-valor válido.");
                    return true;
                }
            }
        }

        // Console.WriteLine("Nenhum par KEY:VALUE com uma chave esperada encontrado nos dados recebidos.");
        return false;
    }
}
