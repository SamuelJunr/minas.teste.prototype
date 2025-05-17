using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.Service;

namespace minas.teste.prototype.MVVM.View
{
    public partial class conexao : Form
    {
        public SerialManager _serialManager;

        private StringBuilder _dataBuffer = new StringBuilder();
        private System.Windows.Forms.Timer _updateTimer;
        private object _bufferLock = new object();
        private apresentacao fechar_box;
        private bool _fechamentoForcado = false;


        public conexao()
        {
            InitializeComponent();
            InitializeSerialManager();
            ConfigureUI();

            InitializeTimer();
            ClearOldData();
            fechar_box = new apresentacao();


        }

        private void InitializeSerialManager()
        {
            // Usa a instância persistente do SerialManager
            _serialManager = ConnectionSettingsApplication.PersistentSerialManager;
            _serialManager.DataReceived += SerialManager_DataReceived;
            _serialManager.ConnectionStatusChanged += SerialManager_ConnectionStatusChanged;
            //_serialManager.ErrorOccurred += SerialManager_ErrorOccurred; // Considere subscrever este evento para feedback de erro
        }
        private void ConfigureUI()
        {
            // Configuração inicial dos controles
            comboBox2.DataSource = SerialManager.CommonBaudRates;
            // Seleciona o Baud Rate salvo ou um valor padrão
            comboBox2.SelectedItem = ConnectionSettingsApplication.BaudRate != 0
                ? ConnectionSettingsApplication.BaudRate
                : 9600; // Valor padrão

            RefreshPortsList();

            // Seleciona a porta salva se estiver disponível
            if (!string.IsNullOrEmpty(ConnectionSettingsApplication.PortName))
            {
                if (comboBox1.Items.Contains(ConnectionSettingsApplication.PortName))
                {
                    comboBox1.SelectedItem = ConnectionSettingsApplication.PortName;
                }
            }
            // Atualiza o status da conexão na UI ao carregar o formulário
            SerialManager_ConnectionStatusChanged(null, _serialManager.IsConnected);
        }
        private void ClearOldData()
        {
            if (textBox1.TextLength > 20000) // Mantém máximo de 10.000 caracteres
            {
                textBox1.Clear();
            }
        }

        private void RefreshPortsList()
        {

            try
            {
                comboBox1.BeginUpdate();
                var previousSelection = comboBox1.SelectedItem?.ToString();
                comboBox1.DataSource = null;
                _serialManager.RefreshPorts();
                comboBox1.DataSource = _serialManager.AvailablePorts;

                if (!string.IsNullOrEmpty(previousSelection) &&
                    comboBox1.Items.Contains(previousSelection))
                {
                    comboBox1.SelectedItem = previousSelection;
                }
                comboBox1.EndUpdate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar portas: {ex.Message}");
            }
        }
        private void InitializeTimer()
        {
            _updateTimer = new System.Windows.Forms.Timer();
            _updateTimer.Interval = 1000; // 1 segundos
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();

            // Esta parte parece redundante aqui, pois RefreshPortsList já é chamado em ConfigureUI
            // comboBox1.DataSource = null;
            // _serialManager.RefreshPorts();
            // comboBox1.DataSource = _serialManager.AvailablePorts;

        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            string dataToAppend = null; // Variável para guardar os dados a serem anexados

            // Trava para acessar e limpar o buffer com segurança
            lock (_bufferLock)
            {
                // Se houver dados no buffer, captura-os e limpa o buffer
                if (_dataBuffer.Length > 0)
                {
                    dataToAppend = _dataBuffer.ToString();
                    _dataBuffer.Clear();
                }
            } // Fim do lock

            // Se capturamos dados, tenta atualizar o TextBox
            if (dataToAppend != null)
            {
                // Verifica se o textBox ainda é válido ANTES de tentar InvokeRequired ou acessá-lo
                // Adiciona verificação 'Disposing' também, pois IsDisposed pode ser falso durante o descarte
                if (textBox1 != null && !textBox1.IsDisposed && !textBox1.Disposing)
                {
                    try
                    {
                        if (textBox1.InvokeRequired)
                        {
                            // Usa BeginInvoke para não bloquear a thread atual (se vier de fora da UI)
                            textBox1.BeginInvoke((MethodInvoker)delegate
                            {
                                // Verifica novamente DENTRO do delegado, pois o estado pode ter mudado
                                if (textBox1 != null && !textBox1.IsDisposed && !textBox1.Disposing)
                                {
                                    textBox1.AppendText(dataToAppend);
                                    textBox1.ScrollToCaret(); // Rola para o final
                                }
                            });
                        }
                        else // Já estamos na thread da UI
                        {
                            textBox1.AppendText(dataToAppend);
                            textBox1.ScrollToCaret(); // Rola para o final
                        }
                    }
                    catch (ObjectDisposedException)
                    {
                        // O controle foi descartado entre a verificação e o uso, ignora o erro.
                        // O Timer deve ser parado em FormClosing para evitar isso recorrentemente.
                    }
                    catch (InvalidOperationException)
                    {
                        // Pode ocorrer se o handle da janela não foi criado ou foi destruído.
                        // Ignorar, pois o Timer será parado em FormClosing.
                    }
                }
                // Se o textBox não for válido (null, IsDisposed, Disposing), simplesmente não fazemos nada
                // com dataToAppend nesta iteração. O Timer será parado em FormClosing.
            }
        }
        private void SerialManager_DataReceived(object sender, string data)
        {

            lock (_bufferLock)
            {
                _dataBuffer.AppendLine($"{data}");
            }


        }

        private void SerialManager_ConnectionStatusChanged(object sender, bool isConnected)
        {
            // Verifica se o controle label3 ainda é válido antes de tentar acessá-lo
            if (label3 != null && !label3.IsDisposed && !label3.Disposing)
            {
                // Usa Invoke para garantir que a atualização ocorra na thread da UI
                if (label3.InvokeRequired)
                {
                    label3.Invoke((MethodInvoker)delegate
                    {
                        label3.Text = isConnected ? "Desconectar" : "Conectar";
                        label3.ForeColor = isConnected ? System.Drawing.Color.Red : System.Drawing.Color.Green;
                    });
                }
                else
                {
                    label3.Text = isConnected ? "Desconectar" : "Conectar";
                    label3.ForeColor = isConnected ? System.Drawing.Color.Red : System.Drawing.Color.Green;
                }
            }

            // Verifica se os controles comboBox1 e comboBox2 ainda são válidos
            if (comboBox1 != null && !comboBox1.IsDisposed && !comboBox1.Disposing)
            {
                if (comboBox1.InvokeRequired)
                {
                    comboBox1.Invoke((MethodInvoker)delegate
                    {
                        comboBox1.Enabled = !isConnected;
                    });
                }
                else
                {
                    comboBox1.Enabled = !isConnected;
                }
            }

            if (comboBox2 != null && !comboBox2.IsDisposed && !comboBox2.Disposing)
            {
                if (comboBox2.InvokeRequired)
                {
                    comboBox2.Invoke((MethodInvoker)delegate
                    {
                        comboBox2.Enabled = !isConnected;
                    });
                }
                else
                {
                    comboBox2.Enabled = !isConnected;
                }
            }
        }


        private void SerialManager_ErrorOccurred(object sender, string errorMessage)
        {
            // Garantir exibição mesmo em threads diferentes
            // Verifica se o formulário ainda é válido antes de tentar BeginInvoke
            if (!this.IsDisposed && !this.Disposing)
            {
                BeginInvoke((MethodInvoker)delegate
                {
                    // Verifica novamente DENTRO do delegado
                    if (!this.IsDisposed && !this.Disposing)
                    {
                        MessageBox.Show(errorMessage, "Erro na Comunicação",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                });
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)

        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione uma porta COM.", "Seleção Necessária", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione uma taxa de Baud Rate válida.", "Seleção Necessária", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var port = comboBox1.SelectedItem.ToString();
            var baudRate = (int)comboBox2.SelectedItem;

            if (_serialManager.IsConnected)
            {
                // Se já conectado, este botão agora funciona como Desconectar
                _serialManager.Disconnect();
            }
            else
            {
                // Tenta conectar
                _serialManager.Connect(port, baudRate);
                // Se a conexão for bem-sucedida, salva as configurações
                if (_serialManager.IsConnected)
                {
                    ConnectionSettingsApplication.PortName = port;
                    ConnectionSettingsApplication.BaudRate = baudRate;
                    MessageBox.Show($"Conectado e configurações salvas: Porta {port}, Baud Rate {baudRate}", "Conexão e Salvamento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnDesconnect_Click(object sender, EventArgs e)
        {
            // Este botão agora pode ser removido ou sua lógica integrada ao btnConnect_Click
            // Se mantido, apenas chama Disconnect
            _serialManager.Disconnect();
        }

        private void conexao_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Para o timer de atualização da UI ao fechar o formulário
            _updateTimer?.Stop();
            _updateTimer?.Dispose(); // Libera recursos do timer

            // Desconecta a porta serial ao fechar o formulário
            _serialManager?.Disconnect(); // Usa o SerialManager persistente

            // Só encerra a aplicação se não for um fechamento controlado
            if (!_fechamentoForcado)
            {
                // Assumindo que apresentacao lida com a lógica final de encerramento da aplicação
                if (fechar_box != null)
                {
                    // Se você quer que a aplicação feche completamente, pode precisar de Application.Exit()
                    // em vez de depender de fechar_box se ele apenas oculta a janela principal.
                    // fechar_box.apresentacao_FormClosing(sender, e); // Comentei ou remova esta linha se o comportamento padrão de fechamento for desejado
                    Application.Exit(); // Garante que a aplicação fecha
                }
                else
                {
                    // Se fechar_box não estiver disponível, garante que a aplicação saia
                    Application.Exit();
                }
            }
            else
            {
                // Se for um fechamento controlado (retornando ao menu), apenas garante que o menu principal seja mostrado
                if (Menuapp.Instance != null && !Menuapp.Instance.Visible)
                {
                    Menuapp.Instance.Show();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) // Este parece ser o botão "Retornar" ou "Sair sem Salvar"
        {
            // Para o timer de atualização da UI
            _updateTimer?.Stop();
            _updateTimer?.Dispose(); // Libera recursos do timer

            // Desconecta a porta serial
            _serialManager?.Disconnect(); // Usa o SerialManager persistente


            if (string.IsNullOrEmpty(ConnectionSettingsApplication.PortName) || ConnectionSettingsApplication.BaudRate <= 0)
            {
                var result = MessageBox.Show(
                    "Nenhuma configuração salva. Deseja sair sem Salvar as Configurações Atuais?", "salvar?",

                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (result == DialogResult.Yes)
                {
                    _fechamentoForcado = true; // Indica que é um fechamento controlado
                    // Assumindo que Menuapp é o formulário principal
                    if (Menuapp.Instance != null) Menuapp.Instance.Show();
                    this.Close(); // Fecha o formulário atual
                }
                // Se o resultado for No, o formulário permanece aberto e nada acontece.
            }
            else
            {
                _fechamentoForcado = true; // Indica que é um fechamento controlado
                // Assumindo que Menuapp é o formulário principal
                if (Menuapp.Instance != null) Menuapp.Instance.Show();
                this.Close(); // Fecha o formulário atual
            }

        }

        private void cuiButton3_Click(object sender, EventArgs e) // Este parece ser o botão "Salvar Configurações"
        {
            if (comboBox1.SelectedItem == null || comboBox2.SelectedItem == null)
            {
                MessageBox.Show("Selecione uma porta e baud rate válidos antes de salvar.");
                return;
            }

            var result = MessageBox.Show(
                "Deseja salvar as configurações atuais?",
                "Salvar Configurações",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                // Salva as configurações na classe estática ConnectionSettingsApplication
                ConnectionSettingsApplication.PortName = comboBox1.SelectedItem.ToString();
                ConnectionSettingsApplication.BaudRate = (int)comboBox2.SelectedItem;

                // TODO: Implementar salvamento persistente (em arquivo .config, JSON, etc.)
                // Para que as configurações persistam entre as execuções da aplicação.
                // Exemplo básico (usando Properties.Settings.Default - requer configuração no projeto):
                // Properties.Settings.Default.PortName = ConnectionSettingsApplication.PortName;
                // Properties.Settings.Default.BaudRate = ConnectionSettingsApplication.BaudRate;
                // Properties.Settings.Default.Save(); // Salva as alterações

                MessageBox.Show("Configurações salvas com sucesso!");
            }
        }

        private void conexao_Load(object sender, EventArgs e)
        {
            // A lógica de TryAutoConnect foi movida para Program.cs para ser executada no início da aplicação.
            // Ao carregar este formulário, apenas configuramos a UI com os valores salvos/conectados.
            ConfigureUI(); // Garante que os ComboBoxes reflitam as configurações atuais

            // Se PersistentSerialManager já estiver conectado (por TryAutoConnect no Program.cs)
            if (ConnectionSettingsApplication.PersistentSerialManager.IsConnected)
            {
                MessageBox.Show($"Conectado automaticamente na porta {ConnectionSettingsApplication.PortName}");
                // A UI já deve estar atualizada via SerialManager_ConnectionStatusChanged
            }
            else if (!string.IsNullOrEmpty(ConnectionSettingsApplication.PortName))
            {
                // Se houver configurações salvas mas não conectado automaticamente
                MessageBox.Show($"Configurações salvas: Porta {ConnectionSettingsApplication.PortName}, Baud Rate {ConnectionSettingsApplication.BaudRate}. Conecte manualmente se necessário.");
            }
            else
            {
                // Se não há configurações salvas nem conexão automática
                MessageBox.Show("Nenhuma configuração serial salva. Selecione a porta e baud rate manualmente.");
            }
        }
    }
}
