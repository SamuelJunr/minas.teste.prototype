using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.Service;


namespace minas.teste.prototype
{
    public partial class TelaInicial: Form
    {
       
        public TelaInicial()
        {
            
            InitializeComponent();
            var appSession = ApplicationSession.Instance;
        }

        

        private void rjButton1_Click(object sender, EventArgs e)
        {
            apresentacao.ExibirMensagemBoasVindas();
            

            // Cria uma instância do Menu
            Menuapp form2 = new Menuapp();

            // Exibe o Menu
            form2.Show();

            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TelaInicial_Load(object sender, EventArgs e)
        {
            //exibe tela de apresentação 
            Text = Properties.Resources.ResourceManager.GetString("MainFormTitle");
           
        }

        


    }
}
