﻿using System;
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
    public partial class Tela_Cilindros: Form
    {
        public Tela_Cilindros()
        {
            InitializeComponent();
            EstiloFormulario.AplicarEstiloBasico(this);
        }

        
    }
}
