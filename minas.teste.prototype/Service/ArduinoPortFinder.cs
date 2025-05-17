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
    // Timeout aumentado para permitir a recepção e acumulação de dados
    private int connectionTimeout = 3000;
    private string receivedDataString = null; // Para acumular os dados recebidos

    // Define as chaves esperadas no formato de dados
    private readonly List<string> expectedKeys = new List<string>
    {
        "P1", "fluxo1", "Piloto1", "dreno1", "RPM", "temp"
        // Adicione outras chaves esperadas conforme necessário
    };

    /// <summary>
    /// Encontra uma porta serial que possa conectar e receber dados no formato esperado (KEY:VALUE|...),
    /// testando os baud rates comuns.
    /// </summary>
    /// <returns>True se uma porta for encontrada e dados no formato esperado forem recebidos, False caso contrário.</returns>
    public bool FindArduinoAndConfirmData()
    {
        string[] ports = SerialPort.GetPortNames();
        List<int> baudRatesToTest = SerialManager.CommonBaudRates; // Acessando a lista de baud rates comuns

        foreach (string port in ports)
        {
            foreach (int currentBaudRate in baudRatesToTest)
            {
                receivedDataString = null; // Reseta para cada nova tentativa de porta/baud rate

                using (SerialPort sp = new SerialPort(port, currentBaudRate))
                {
                    // Timeout de leitura e escrita
                    sp.ReadTimeout = connectionTimeout;
                    sp.WriteTimeout = connectionTimeout;

                    // Usando um ManualResetEvent para sinalizar a recepção de dados
                    using (var dataReceivedEvent = new ManualResetEvent(false))
                    {
                        // Handler do evento DataReceived
                        SerialDataReceivedEventHandler dataHandler = (sender, e) =>
                        {
                            try
                            {
                                // Lê todos os dados disponíveis no buffer
                                string data = sp.ReadExisting();
                                if (!string.IsNullOrEmpty(data))
                                {
                                    // Acumula os dados recebidos. Pode chegar em pedaços.
                                    receivedDataString += data;
                                    // Sinaliza o evento indicando que algum dado foi recebido.
                                    dataReceivedEvent.Set();
                                }
                            }
                            catch (Exception ex)
                            {
                                // Logar erro no handler, mas não abortar o teste da porta/baud rate
                                Console.WriteLine($"Erro no handler DataReceived para {port}@{currentBaudRate}: {ex.Message}");
                            }
                        };

                        try
                        {
                            sp.DataReceived += dataHandler; // Assina antes de abrir a porta
                            sp.Open();

                            // Limpa buffers para garantir comunicação limpa
                            sp.DiscardInBuffer();
                            sp.DiscardOutBuffer();

                            // Opcional: Enviar um comando para tentar provocar o envio de dados, se o Arduino precisar.
                            // Adapte o comando "REQUEST_DATA" conforme o protocolo do seu Arduino.
                            try
                            {
                                sp.WriteLine("REQUEST_DATA\n"); // Envia um comando e uma nova linha
                            }
                            catch (TimeoutException)
                            {
                                // Ignorar timeout de escrita durante o teste inicial
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Erro ao enviar comando de teste para {port}@{currentBaudRate}: {ex.Message}");
                            }


                            // Espera pelo evento DataReceived ser sinalizado ou pelo timeout.
                            // Esperamos pelo timeout completo para tentar acumular uma mensagem completa ou parcial válida.
                            bool signaled = dataReceivedEvent.WaitOne(connectionTimeout);

                            // Desinscreve o handler imediatamente após a espera para evitar chamadas futuras
                            sp.DataReceived -= dataHandler;

                            // Após a espera, verifica se acumulamos dados e se eles contêm o formato esperado
                            if (!string.IsNullOrEmpty(receivedDataString))
                            {
                                if (IsValidDataFormat(receivedDataString))
                                {
                                    // Dados no formato esperado (pelo menos um par KEY:VALUE com chave conhecida) foram recebidos.
                                    ConnectedPort = port;
                                    BaudRate = currentBaudRate;

                                    // Fecha a porta de teste antes de retornar
                                    if (sp.IsOpen) sp.Close();

                                    return true; // Encontrou uma conexão funcional com dados no formato válido
                                }
                            }
                            // Se não recebeu dados ou o formato não é válido, continua.
                        }
                        catch (UnauthorizedAccessException)
                        {
                            // A porta provavelmente está em uso. Pular para a próxima porta.
                            Console.WriteLine($"Porta {port} está em uso. Pulando.");
                            sp.DataReceived -= dataHandler; // Garante desinscrição
                            continue; // Ir para a próxima porta
                        }
                        catch (Exception ex)
                        {
                            // Logar outros erros, mas continuar testando
                            Console.WriteLine($"Erro ao testar a porta {port} com baud rate {currentBaudRate}: {ex.Message}");
                            sp.DataReceived -= dataHandler; // Garante desinscrição
                        }
                        finally
                        {
                            // Garantir que a porta seja fechada
                            if (sp.IsOpen)
                            {
                                sp.Close();
                            }
                        }
                    } // ManualResetEvent é descartado
                } // SerialPort é descartada
            }
        }
        return false; // Nenhuma conexão funcional encontrada com o formato de dados esperado
    }

    /// <summary>
    /// Valida se a string recebida contém pelo menos um par KEY:VALUE onde KEY é uma das chaves esperadas.
    /// Ignora partes da string que não se encaixam nesse formato.
    /// </summary>
    /// <param name="data">A string de dados recebida.</param>
    /// <returns>True se pelo menos um par KEY:VALUE válido com chave esperada for encontrado, False caso contrário.</returns>
    private bool IsValidDataFormat(string data)
    {
        if (string.IsNullOrEmpty(data))
        {
            return false;
        }

        // Remove caracteres de fim de linha e espaços em branco extras do início/fim
        string cleanedData = data.Trim();

        // Verifica se a string não está vazia após limpar
        if (string.IsNullOrEmpty(cleanedData))
        {
            return false;
        }

        // Divide a string em partes usando o caractere '|'.
        // RemoveEmptyEntries lida com separadores múltiplos ou no início/fim.
        string[] pairs = cleanedData.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

        if (pairs.Length == 0)
        {
            // Nenhuma parte encontrada após a divisão
            return false;
        }

        // Verifica se pelo menos uma parte corresponde ao formato KEY:VALUE e tem uma chave esperada
        foreach (string pair in pairs)
        {
            string[] parts = pair.Split(':');
            // Verifica se a parte tem exatamente duas subpartes (chave e valor)
            if (parts.Length == 2)
            {
                string key = parts[0].Trim();
                // Não precisamos usar o valor para esta validação, apenas verificar a chave
                // string valueString = parts[1].Trim();

                // Verifica se a chave extraída (sem espaços) está na nossa lista de chaves esperadas
                if (expectedKeys.Contains(key))
                {
                    // Encontrou pelo menos um par KEY:VALUE com uma chave esperada. Sucesso!
                    Console.WriteLine($"Chave esperada '{key}' encontrada em um par chave-valor válido.");
                    return true;
                }
            }
            // Se parts.Length não for 2, esta parte não é um par KEY:VALUE válido, simplesmente ignoramos.
            // Isso aborda a necessidade de ignorar caracteres repetidos ou estruturas fora do formato.
        }

        // Se o loop terminou e nenhum par KEY:VALUE com uma chave esperada foi encontrado
        Console.WriteLine("Nenhum par KEY:VALUE com uma chave esperada encontrado nos dados recebidos.");
        return false;
    }
}