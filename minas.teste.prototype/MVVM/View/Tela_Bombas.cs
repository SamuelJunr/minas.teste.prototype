using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using minas.teste.prototype.Service;
//using MultiSensorMonitor;

namespace minas.teste.prototype
{
    
    public partial class Tela_Bombas : Form
    {
        private apresentacao fechar_box;

        private bool _fechamentoForcado = false;

        private string _serialDataIn;

        private HoraDia _Tempo;
        public string DataSensorA { get; private set; }
        public string DataSensorB { get; private set; }
        public string DataSensorC { get; private set; }

        public Tela_Bombas()
        {
            InitializeComponent();
            fechar_box = new apresentacao();
            _Tempo = new HoraDia(label13);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = "COM3";
                serialPort1.BaudRate = 9600;
                serialPort1.ReadTimeout = 100;
                serialPort1.WriteTimeout = 100;
                serialPort1.Open();
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }

        private void Tela_Bombas_Load(object sender, EventArgs e)
        {
            Text = Properties.Resources.ResourceManager.GetString("MainFormTitle");
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _fechamentoForcado = true; // Indica que é um fechamento controlado
            Menuapp.Instance.Show();
            this.Close();
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _serialDataIn = serialPort1.ReadLine();
            Invoke(new Action(() => ProcessSerialData(_serialDataIn)));
        }

        public void ProcessSerialData(string serialData)
        {
            _serialDataIn = serialData;
            ResetValues();
            ParseData();
            UpdateUI();
            // UpdateProgressBarDataSensorA(); // Atualiza a ProgressBar
        }

        private void ResetValues()
        {
            DataSensorA = string.Empty;
            DataSensorB = string.Empty;
            DataSensorC = string.Empty;
        }

        private void ParseData()
        {
            var segments = _serialDataIn.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var segment in segments)
            {
                if (segment.Length < 1) continue;

                switch (segment[0])
                {
                    case 'A' when segment.Length > 1:
                        DataSensorA = segment.Substring(1);
                        break;
                    case 'B' when segment.Length > 1:
                        DataSensorB = segment.Substring(1);
                        break;
                    case 'C' when segment.Length > 1:
                        DataSensorC = segment.Substring(1);
                        break;
                    default:
                        LogError($"Formato inválido: {segment}");
                        break;
                }
            }
        }

        private void UpdateUI()
        {
            UpdateTextBoxSafe(textBox1, DataSensorA);
            //UpdateTextBoxSafe(textBox2, DataSensorB);
            //UpdateTextBoxSafe(textBox3, DataSensorC);
        }

        private void UpdateTextBoxSafe(TextBox textBox, string value)
        {
            if (textBox.InvokeRequired)
            {
                textBox.Invoke(new Action(() => textBox.Text = value));
            }
            else
            {
                textBox.Text = value;
            }
        }

        private void LogError(string message)
        {
            Debug.WriteLine($"Erro: {message}");
        }

        //private void UpdateProgressBarDataSensorA()
        //{
        //    if (int.TryParse(DataSensorA, out int value))
        //    {
        //        if (progressBar1.InvokeRequired)
        //        {
        //            progressBar1.Invoke(new Action(() =>
        //            {
        //                progressBar1.Value = Math.Max(progressBar1.Minimum, Math.Min(progressBar1.Maximum, value));
        //            })); 
        //        }
        //        else
        //        {
        //            progressBar1.Value = Math.Max(progressBar1.Minimum, Math.Min(progressBar1.Maximum, value));
        //        }
        //    }
        //    else
        //    {
        //        // Lida com casos em que DataSensorA não é um inteiro válido.
        //        Debug.WriteLine($"Aviso: DataSensorA '{DataSensorA}' não é um inteiro válido.");
        //    }
        //}

        private void aGauge1_ValueInRangeChanged(object sender, ValueInRangeChangedEventArgs e)
        {

        }

        private void aGauge2_ValueInRangeChanged(object sender, ValueInRangeChangedEventArgs e)
        {

        }

        private void Tela_Bombas_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
                serialPort1.Close();

            // Só encerra a aplicação se não for um fechamento controlado
            if (!_fechamentoForcado)
            {
                fechar_box.apresentacao_FormClosing(sender, e);
            }
            else
                Menuapp.Instance.Show();

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        



    }
}