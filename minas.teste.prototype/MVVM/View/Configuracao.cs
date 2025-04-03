using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.Estilo;

namespace minas.teste.prototype.MVVM.View
{
    public partial class configuracao : Form
    {
        public configuracao()
        {
            InitializeComponent();
            EstiloFormulario.AplicarEstiloconfiguracao(this);
        }

        private void configuracao_Load(object sender, EventArgs e)
        {
            Text = Properties.Resources.ResourceManager.GetString("ConfigureTitle");
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Configuração salva com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Configuração Foram apagadas, entre com novos parametros!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
