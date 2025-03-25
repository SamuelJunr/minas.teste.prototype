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
    public partial class Menuapp: Form
    {
        public Menuapp()
        {
            InitializeComponent();
        }

        private void Menuapp_Load(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Bombas form3 = new Bombas();

            // Exibe o Meu
            form3.Show();

            this.Close();
        }
    }
}
