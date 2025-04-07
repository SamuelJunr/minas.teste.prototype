using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.View;

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


            Tela_Bombas form3 = new Tela_Bombas();
            form3.Show();
            this.Hide();

        }

        private void Menuapp_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            fechar_box.apresentacao_FormClosing(sender, e);
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            configuracao form4 = new configuracao();
            form4.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            conexao form5 = new conexao();
            form5.Show();
            this.Hide();
            

        }
    }
}
