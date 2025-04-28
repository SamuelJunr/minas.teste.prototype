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
using minas.teste.prototype.MVVM.ViewModel;

namespace minas.teste.prototype.MVVM.View
{
    public partial class Tela_Cilindros: Form
    {
        //-------------------------------------//
        //          objetos GLOBAIS            //
        //-------------------------------------//

        private apresentacao _fechar_box;
        

        //-------------------------------------//
        //          variaveis GLOBAIS          //
        //-------------------------------------//

        private bool _fechamentoForcado;
        
        public Tela_Cilindros()
        {
            InitializeComponent();
        }

        #region EVENTOS_FECHAMANETO  
        private void CloseWindows_Click(object sender, EventArgs e)
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
                _fechar_box.apresentacao_FormClosing(sender, e);
            }
            else
                Menuapp.Instance.Show();

        }
        #endregion

    }
}
