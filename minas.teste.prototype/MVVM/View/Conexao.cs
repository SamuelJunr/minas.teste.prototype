using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        public conexao()
        {
            InitializeComponent();
            InitializeSerialManager();
            ConfigureUI();

        }

        private void InitializeSerialManager()
        {
            _serialManager = new SerialManager();
            _serialManager.DataReceived += SerialManager_DataReceived;
            _serialManager.ConnectionStatusChanged += SerialManager_ConnectionStatusChanged;
            _serialManager.ErrorOccurred += SerialManager_ErrorOccurred;
        }
        private void ConfigureUI()
        {
            // Configuração inicial dos controles
            comboBox2.DataSource = SerialManager.CommonBaudRates;
            comboBox2.SelectedItem = 9600;
            RefreshPortsList();
        }

        private void RefreshPortsList()
        {
            comboBox1.DataSource = null;
            _serialManager.RefreshPorts();
            comboBox1.DataSource = _serialManager.AvailablePorts;
        }

        private void SerialManager_DataReceived(object sender, string data)
        {
            cuiTextBox21.Content = ($"[{DateTime.Now:T}] Recebido: {data}{Environment.NewLine}");
        }

        private void SerialManager_ConnectionStatusChanged(object sender, bool isConnected)
        {
            label1.Text = isConnected ? "Desconectar" : "Conectar";
            comboBox1.Enabled = !isConnected;
            comboBox2.Enabled = !isConnected;
        }

        private void SerialManager_ErrorOccurred(object sender, string errorMessage)
        {
            MessageBox.Show(errorMessage, "Erro na Comunicação", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        
        {
            if (comboBox1.SelectedItem == null) return;

            if (_serialManager.IsConnected)
            {
                _serialManager.Disconnect();
            }
            else
            {
                var port = comboBox1.SelectedItem.ToString();
                var baudRate = (int)comboBox2.SelectedItem;
                _serialManager.Connect(port, baudRate);
            }
        }


    }
}