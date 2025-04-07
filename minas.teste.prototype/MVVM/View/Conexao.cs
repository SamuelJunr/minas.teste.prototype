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
using minas.teste.prototype.Service;

namespace minas.teste.prototype.MVVM.View
{
    public partial class conexao : Form
    {
        private SerialManager _serialManager;
 
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
            comboBox2.SelectedItem = 9600;
            RefreshPortsList();
        }
        private void ClearOldData()
        {
            if (textBox1.TextLength > 10000) // Mantém máximo de 10.000 caracteres
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
            _updateTimer.Interval = 2000; // 1 segundos
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
 
            comboBox1.DataSource = null;
            _serialManager.RefreshPorts();
            comboBox1.DataSource = _serialManager.AvailablePorts;
 
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            lock (_bufferLock)
            {
                if (_dataBuffer.Length > 0)
                {
                    if (textBox1.InvokeRequired)
                    {
                        textBox1.BeginInvoke((MethodInvoker)delegate
                        {
                            textBox1.AppendText(_dataBuffer.ToString());
                            textBox1.ScrollToCaret();
                        });
                    }
                    else
                    {
                        textBox1.AppendText(_dataBuffer.ToString());
                        textBox1.ScrollToCaret();
                    }
                    _dataBuffer.Clear();
                }
            }
        }
        private void SerialManager_DataReceived(object sender, string data)
        {
 
            lock (_bufferLock)
            {
                _dataBuffer.AppendLine($"Recebido: {data}");
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
            if (_serialManager.IsConnected)
                _serialManager.Disconnect(); 

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
            _fechamentoForcado = true; // Indica que é um fechamento controlado
            Menuapp.Instance.Show();
            this.Close();
        }
 
            
      


 
    }
}