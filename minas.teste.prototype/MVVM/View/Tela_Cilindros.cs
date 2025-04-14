using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.Estilo;
using minas.teste.prototype.MVVM.Model.Concrete;

namespace minas.teste.prototype.MVVM.View
{
    public partial class Tela_Cilindros: Form
    {
        private bool _fechamentoForcado;
        private apresentacao fechar_box;

        public Tela_Cilindros()
        {
            InitializeComponent();
            EstiloFormulario.AplicarEstiloBasico(this);
        }

        private void Retornar_Click(object sender, EventArgs e)
        {
            _fechamentoForcado = true; // Indica que é um fechamento controlado
            Menuapp.Instance.Show();
            this.Close();
        }

        private void Tela_Cilindros_FormClosing(object sender, FormClosingEventArgs e)
        {
           
            // Só encerra a aplicação se não for um fechamento controlado
            if (!_fechamentoForcado)
            {
                
                fechar_box.apresentacao_FormClosing(sender, e);
            }
            else
                Menuapp.Instance.Show();

        }
    }
}
