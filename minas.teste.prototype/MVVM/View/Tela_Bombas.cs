using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Extensions.Logging;
using minas.teste.prototype.MVVM.Model.Abstract;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.Service;
using minas.teste.prototype.MVVM.ViewModel;

namespace minas.teste.prototype.MVVM.View
{
    public partial class Tela_Bombas : Form
    {
        //-------------------------------------//
        //          objetos GLOBAIS          //
        //-------------------------------------//
        private Tela_BombasVM _viewModel;
        private apresentacao fechar_box;


        //-------------------------------------//
        //          variaveis GLOBAIS          //
        //-------------------------------------//

        private bool _fechamentoForcado;
        private bool _isMonitoring = false;
        public string Inicioteste;
        public string Fimteste;


        public Tela_Bombas()
        {
            InitializeComponent();
            _viewModel = new Tela_BombasVM();
            fechar_box = new apresentacao();
        }

        #region LOADS_JANELA    
        private void Tela_Bombas_Load(object sender, EventArgs e)
        {
            _viewModel.Carregar_configuracao(this); // Carrega o estilo do formulário  
            _viewModel.Stage_signal(Stage_box_bomba);
            _viewModel.VincularRelogioLabel(LabelHorariotela);// configura a imagem de teste ligado ou desligado  

        }
        #endregion

        #region EVENTOS_FECHAMANETO  
        private void CloseWindows_Click(object sender, EventArgs e)
        {
            _fechamentoForcado = true; // Indica que é um fechamento controlado  
            Menuapp.Instance.Show();
            this.Close();
        }

        private void Tela_Bombas_FormClosing(object sender, FormClosingEventArgs e)
        {

            // Só encerra a aplicação se não for um fechamento controlado  
            if (!_fechamentoForcado)
            {  
                   fechar_box.apresentacao_FormClosing(sender, e);
            }
            else
                Menuapp.Instance.Show();

        }
        #endregion

        #region INCIO_TESTE
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            _isMonitoring = true;
            _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5,panel7, panel6, panel8, panel9);
            Inicioteste = DateTime.Now.ToString();
            _viewModel.IniciarTesteBomba(Stage_box_bomba);

        }

        private void btnParar_Click(object sender, EventArgs e)
        {
            _isMonitoring = false;
            Fimteste = DateTime.Now.ToString();
            _viewModel.FinalizarTesteBomba(Stage_box_bomba);

        }


        #endregion

    }
}
