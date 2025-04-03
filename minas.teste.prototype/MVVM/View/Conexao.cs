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
    public partial class conexao: Form
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
            cmbBaudRate.DataSource = SerialManager.CommonBaudRates;
            cmbBaudRate.SelectedItem = 9600;
            RefreshPortsList();
        }

        private void RefreshPortsList()
        {
            cmbPorts.DataSource = null;
            _serialManager.RefreshPorts();
            cmbPorts.DataSource = _serialManager.AvailablePorts;
        }

        private void SerialManager_DataReceived(object sender, string data)
        {
            txtReceivedData.AppendText($"[{DateTime.Now:T}] Recebido: {data}{Environment.NewLine}");
        }

        private void SerialManager_ConnectionStatusChanged(object sender, bool isConnected)
        {
            btnConnect.Text = isConnected ? "Desconectar" : "Conectar";
            cmbPorts.Enabled = !isConnected;
            cmbBaudRate.Enabled = !isConnected;
        }

        private void SerialManager_ErrorOccurred(object sender, string errorMessage)
        {
            MessageBox.Show(errorMessage, "Erro na Comunicação", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (cmbPorts.SelectedItem == null) return;

            if (_serialManager.IsConnected)
            {
                _serialManager.Disconnect();
            }
            else
            {
                var port = cmbPorts.SelectedItem.ToString();
                var baudRate = (int)cmbBaudRate.SelectedItem;
                _serialManager.Connect(port, baudRate);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtSendData.Text))
            {
                _serialManager.SendData(txtSendData.Text);
                txtReceivedData.AppendText($"[{DateTime.Now:T}] Enviado: {txtSendData.Text}{Environment.NewLine}");
                txtSendData.Clear();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshPortsList();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _serialManager.Dispose();
            base.OnFormClosing(e);
        }

    }
}
