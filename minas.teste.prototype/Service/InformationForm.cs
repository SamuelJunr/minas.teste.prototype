using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minas.teste.prototype.Service
{
    public partial class InformationForm : Form
    {

        public InformationForm(bool encontrado, string porta)
        {
            InitializeComponents(encontrado, porta);
        }

        private void InitializeComponents(bool encontrado, string porta)
        {
            this.Text = "Status do Arduino";
            this.Size = new System.Drawing.Size(300, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Label lblMensagem = new Label();
            lblMensagem.Text = encontrado
                ? $"Controlador encontrado na porta: {porta}"
                : "Controlador não encontrado! Conecte dispositivo a uma porta USB! \n Acesse Conexão para Configurar Controlador. ";

            lblMensagem.Dock = DockStyle.Fill;
            lblMensagem.TextAlign = ContentAlignment.MiddleCenter;

            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Dock = DockStyle.Bottom;
            btnOk.Click += (sender, e) => this.Close();

            this.Controls.Add(lblMensagem);
            this.Controls.Add(btnOk);
        }

    }  
}
