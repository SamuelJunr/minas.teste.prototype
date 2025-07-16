namespace minas.teste.prototype.MVVM.View
{
    partial class FormVisualizarRelatorio
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private System.Windows.Forms.WebBrowser webBrowser1; // Declaração do controle

        private System.Windows.Forms.Button btnGerarPdf;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "FormVisualizarRelatorio";

            this.webBrowser1 = new System.Windows.Forms.WebBrowser(); // Instanciação
            this.btnGerarPdf = new System.Windows.Forms.Button(); // Instanciação do botão


            //
            // Propriedades e localização do webBrowser1
            this.webBrowser1.Location = new System.Drawing.Point(12, 12); // Exemplo de localização
            this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser1.Name = "webBrowser1"; // O nome do controle deve ser este
            this.webBrowser1.Size = new System.Drawing.Size(776, 350); // Exemplo de tamanho
            this.webBrowser1.TabIndex = 0;
            // Adiciona o webBrowser1 e o botão ao formulário
            this.Controls.Add(this.webBrowser1);
            this.Controls.Add(this.btnGerarPdf);

            
        }

        #endregion
    }
}