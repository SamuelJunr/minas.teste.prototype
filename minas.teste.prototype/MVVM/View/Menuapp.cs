using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.MVVM.View;
using minas.teste.prototype.MVVM.ViewModel;

namespace minas.teste.prototype
{
    public partial class Menuapp : Form
    {
        private static Menuapp _instance;
        private  apresentacao fechar_box; 

        public Menuapp()
        {
            InitializeComponent();
            fechar_box = new apresentacao();
            


        }

        public static Menuapp Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                    _instance = new Menuapp();
                return _instance;
            }
        }

        private void Menuapp_Load(object sender, EventArgs e)
        {
            Text = Properties.Resources.ResourceManager.GetString("MainFormTitle");
        }

        
        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {


            Tela_Bombas bombasvv = new Tela_Bombas();
            bombasvv.Show();
            this.Hide();

            //Tela_Bombas teste = new Tela_Bombas();
            //teste.Show();
            //this.Hide();

        }

        private void Menuapp_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            fechar_box.apresentacao_FormClosing(sender, e);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            configuracao ConfiguracaoVV = new configuracao();
            ConfiguracaoVV.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conexao ConexaoVV = new conexao();
            ConexaoVV.Show();
            this.Hide();
            

        }

        private void button10_Click(object sender, EventArgs e)
        {
            Tela_CilindrosCon CilindrosVV = new Tela_CilindrosCon();
            CilindrosVV.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Tela_Comandos ComandosVV = new Tela_Comandos();
            ComandosVV.Show();
            this.Hide();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Tela_Direcao DirecaoVV = new Tela_Direcao();
            DirecaoVV.Show();
            this.Hide();
        }
    }
}
