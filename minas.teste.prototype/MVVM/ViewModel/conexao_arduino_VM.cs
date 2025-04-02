using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace minas.teste.prototype.MVVM.ViewModel
{
    class conexao_arduino_VM
    {
        private SerialPort _serialPort;
        private bool _isReceiving;
        private TextBox _outputTextBox;
        private readonly object _lock = new object();

        public conexao_arduino_VM(TextBox textBox)
        {
            _outputTextBox = textBox;
            _isReceiving = false;
        }

        public string DetectarPortaArduino()
        {
            string[] portasDisponiveis = SerialPort.GetPortNames();
            if (portasDisponiveis.Length == 0)
            {
                MessageBox.Show("Nenhuma porta serial disponível.");
                return null;
            }

            foreach (string porta in portasDisponiveis.OrderBy(p => p))
            {
                SerialPort port = null;
                try
                {
                    port = new SerialPort(porta, 9600)
                    {
                        Encoding = Encoding.UTF8,
                        WriteTimeout = 500, // Adicionado timeout para escrita
                        ReadTimeout = 1000 // Timeout de leitura de 1 segundo
                    };
                    port.Open();
                    port.WriteLine("AT"); 
                    port.ReadTimeout = 1000;// Envia comando de teste
                    port.BaseStream.Flush();

                    _serialPort = port;
                    string response = port.ReadLine().Trim();
                    if (response != null) return porta; // retorno do arduino não é um ok , não esta programado para receber o comenado AT
                }
                catch (TimeoutException)
                {
                    MessageBox.Show($"Porta {porta}: Nenhuma resposta recebida.");
                    if (port != null || port.IsOpen)
                    {
                        port.Close();
                        port.Dispose();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show($"Acesso negado à porta {porta}.");
                    if (port != null || port.IsOpen)
                    {
                        port.Close();
                        port.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro na porta {porta}: {ex.Message}");
                    if (port != null || port.IsOpen)
                    {
                        port.Close();
                        port.Dispose();
                    }
                }
                
            }
            return null;
        }

        

        // Conecta à porta detectada
        public bool Conectar(string portaDetectada)
        {
            lock (_lock)
            {
                if (portaDetectada == null) return false;

                if (_serialPort != null && _serialPort.IsOpen) return true;

                try
                {
                    _serialPort = new SerialPort(portaDetectada, 9600)
                    {
                        Encoding = Encoding.UTF8,
                        ReadTimeout = 500
                    };

                    _serialPort.Open();
                    _isReceiving = false; // Inicia com recebimento desligado
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        // Evento de recebimento de dados
        private void LerDadosRecebidos(object sender, SerialDataReceivedEventArgs e)
        {
            lock (_lock) // Sincronização
            {
                try
                {
                    if (!_isReceiving) return; // Não processa se recebimento estiver desligado

                    string dados = _serialPort.ReadLine().Trim();
                    if (ValidarFormatoString(dados))
                    {
                        AtualizarUI(dados);
                    }
                }
                catch
                {
                    _isReceiving = false;
                }
            }
        }

        // Validação de dados
        private bool ValidarFormatoString(string dados)
        {
            return !string.IsNullOrEmpty(dados) && !dados.Any(c => char.IsControl(c));
        }

        // Atualização thread-safe da UI
        private void AtualizarUI(string texto)
        {
            if (_outputTextBox.InvokeRequired)
            {
                _outputTextBox.Invoke(new Action(() =>
                    _outputTextBox.AppendText(texto + Environment.NewLine)));
            }
            else
            {
                _outputTextBox.AppendText(texto + Environment.NewLine);
            }
        }

        // Liga/desliga o recebimento
        public void ToggleRecebimento(bool ligar)
        {
            lock (_lock)
            {
                if (_serialPort.IsOpen)
                {
                    // Passo 1: Detectar a porta do Arduino
                    

                    // Passo 2: Verificar se a porta é válida
                    bool portaValida = portaDetectada != null;

                    if (portaValida)
                    {
                        // Passo 3: Conectar à porta detectada
                        bool conectado = Conectar(portaDetectada);

                        if (conectado)
                        {
                            // Passo 4: Ativar/desativar recebimento conforme o parâmetro "ligar"
                            if (ligar)
                            {
                                if (!_isReceiving)
                                {
                                    _serialPort.DataReceived += LerDadosRecebidos;
                                    _isReceiving = true;
                                }
                            }
                            else
                            {
                                if (_isReceiving)
                                {
                                    _serialPort.DataReceived -= LerDadosRecebidos;
                                    _isReceiving = false;
                                    _serialPort.DiscardInBuffer();
                                }
                            }
                        }
                    }
                    else
                    {
                        // Passo 5: Se não houver porta válida, desligar recebimento
                        if (_isReceiving)
                        {
                            _serialPort.DataReceived -= LerDadosRecebidos;
                            _isReceiving = false;
                            if (_serialPort != null && _serialPort.IsOpen)
                            {
                                _serialPort.DiscardInBuffer();
                            }
                        }
                    }
                }
                else
                {
                    string portaDetectada = DetectarPortaArduino();
                    ToggleRecebimento(ligar);
                }
            }
        }

        // Verifica se dados estão sendo exibidos
        public bool DadosEstaoSendoExibidos()
        {
            return _serialPort != null &&
                   _serialPort.IsOpen &&
                   _isReceiving &&
                   !string.IsNullOrEmpty(_outputTextBox.Text);
        }

        // Desconecta a porta
        public void Desconectar()
        {
            lock (_lock) // Sincronização
            {
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                    _isReceiving = false;
                }
            }
        }
    }
}