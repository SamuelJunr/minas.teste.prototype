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
        private  apresentacao fechar_box; 

        public Menuapp()
        {
            InitializeComponent();
            fechar_box = new apresentacao();
            


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

            this.FormClosing -= Menuapp_FormClosing;
            this.Close();

        }

        private void Menuapp_FormClosing(object sender, FormClosingEventArgs e)
        {
            fechar_box.apresentacao_FormClosing(sender, e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Configuracao form4 = new Configuracao();
            form4.Show();

            this.FormClosing -= Menuapp_FormClosing;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Conexao form5 = new Conexao();
            form5.Show();

            this.FormClosing -= Menuapp_FormClosing;
            this.Close();
        }
    }
}
