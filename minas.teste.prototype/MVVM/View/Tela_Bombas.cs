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

namespace minas.teste.prototype
{
    public partial class Tela_Bombas : Form
    {
        private ArduinoChartDataProvider _dataProvider;
        private Label lblA;
        private Label lblB;
        private Label lblC;

        public Tela_Bombas()
        {
            InitializeComponent();
            lblA = new Label();
            lblB = new Label();
            lblC = new Label();
            _dataProvider = new ArduinoChartDataProvider(chart1, lblA, lblB, lblC);
        }


        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }


        private void Bombas_Load(object sender, EventArgs e)
        {
            Text = Properties.Resources.ResourceManager.GetString("MainFormTitle");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }



        private void button6_Click(object sender, EventArgs e)
        {
            _dataProvider.StartMonitoring();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            _dataProvider.StopReading();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            Menuapp menuForm = new Menuapp();
            menuForm.Show();
            this.Close();
        }
    }
}
