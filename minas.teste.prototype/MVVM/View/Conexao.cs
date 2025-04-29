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
            _serialManager = new SerialManager();
            _serialManager.DataReceived += SerialManager_DataReceived;
            _serialManager.ConnectionStatusChanged += SerialManager_ConnectionStatusChanged;
            //_serialManager.ErrorOccurred += SerialManager_ErrorOccurred;
        }
        private void ConfigureUI()
        {
            // Configuração inicial dos controles
            comboBox2.DataSource = SerialManager.CommonBaudRates;
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
 
            comboBox1.DataSource = null;
            _serialManager.RefreshPorts();
            comboBox1.DataSource = _serialManager.AvailablePorts;
 
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
 
            label3.Text = isConnected ? "Desconectar" : "Conectar" ;
            label3.ForeColor = isConnected ? System.Drawing.Color.Red : System.Drawing.Color.Green;
 
           
 
            comboBox1.Enabled = !isConnected;
            comboBox2.Enabled = !isConnected;
        }

        private void SerialManager_ErrorOccurred(object sender, string errorMessage)
        {
            // Garantir exibição mesmo em threads diferentes
            BeginInvoke((MethodInvoker)delegate {
                MessageBox.Show(errorMessage, "Erro na Comunicação",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            });
        }

        private void btnConnect_Click(object sender, EventArgs e)
        
        {
            if (comboBox1.SelectedItem == null) return;
 
            // Validate selections
            if (comboBox1.SelectedItem == null)
 

            if (_serialManager.IsConnected)
 
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
                _serialManager.Connect(port, baudRate);
           
        }

        private void btnDesconnect_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;
            
            _serialManager.Disconnect();


        }

        private void conexao_FormClosing(object sender, FormClosingEventArgs e)
        {
          
            // Só encerra a aplicação se não for um fechamento controlado
            if (!_fechamentoForcado)
            {
                fechar_box.apresentacao_FormClosing(sender, e);
            }
            else
 
                Menuapp.Instance.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
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
                    Menuapp.Instance.Show();
                    this.Close();
                }
                return;
                
                
            }
            else
            {
                _fechamentoForcado = true; // Indica que é um fechamento controlado
                Menuapp.Instance.Show();
                this.Close();
            }
                
        }

        private void cuiButton3_Click(object sender, EventArgs e)
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
                ConnectionSettingsApplication.UpdateConnection(
                     comboBox1.SelectedItem.ToString(),
                        (int)comboBox2.SelectedItem);
                MessageBox.Show("Configurações salvas com sucesso!");
            }
        }

        private void conexao_Load(object sender, EventArgs e)
        {
            if (ConnectionSettingsApplication.TryAutoConnect())
            {
                MessageBox.Show($"Conectado automaticamente na porta {ConnectionSettingsApplication.PortName}");
                // Atualiza UI com os valores persistentes
                comboBox1.SelectedItem = ConnectionSettingsApplication.PortName;
                comboBox2.SelectedItem = ConnectionSettingsApplication.BaudRate;
                SerialManager_ConnectionStatusChanged(null, ConnectionSettingsApplication.PersistentSerialManager.IsConnected);
            }
            else
            {
                MessageBox.Show("Arduino não encontrado. Selecione manualmente.");
            }

        }
    }
}