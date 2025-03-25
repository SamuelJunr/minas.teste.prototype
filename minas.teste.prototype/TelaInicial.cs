using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace minas.teste.prototype
{
    public partial class TelaInicial: Form
    {
        public TelaInicial()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void rjButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bem-vindo à aplicação!");
            // Aqui você pode adicionar a lógica para abrir outra parte da aplicação

            // Cria uma instância do Menu
            Menuapp form2 = new Menuapp();

            // Exibe o Meu
            form2.Show();

            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
