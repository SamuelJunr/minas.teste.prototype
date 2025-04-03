using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace minas.teste.prototype.Estilo
{
    public static class EstiloFormulario
    {
        public static void AplicarEstiloBasico(Form form)
        {
            form.BackColor = Color.DarkGray;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.Size = new Size(1076, 696);
            form.StartPosition = FormStartPosition.CenterParent;
            form.WindowState = FormWindowState.Normal;
            form.Text = Properties.Resources.ResourceManager.GetString("MainFormTitle");
        }

        public static void AplicarEstiloconfiguracao(Form form)
        {
            form.BackColor = System.Drawing.SystemColors.ActiveCaption;
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.Size = new Size(556, 418);
            form.StartPosition = FormStartPosition.CenterParent;
            form.WindowState = FormWindowState.Normal;
            form.Text = Properties.Resources.ResourceManager.GetString("ConfigureTitle");
        }
    }
}