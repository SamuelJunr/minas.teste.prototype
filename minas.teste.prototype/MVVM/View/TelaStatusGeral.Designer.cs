namespace minas.teste.prototype.MVVM.View
{
    partial class TelaStatusGeral
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlTop = new System.Windows.Forms.Panel();
            this.tblTopLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblStatusGeral = new System.Windows.Forms.Label();
            this.lblNomeTexto = new System.Windows.Forms.Label();
            this.lblDataHora = new System.Windows.Forms.Label();
            this.lblTeste = new System.Windows.Forms.Label();
            this.btnRetornar = new System.Windows.Forms.Button();
            this.pnlPlaceholder = new System.Windows.Forms.Panel();
            this.txtNomeTexto = new System.Windows.Forms.TextBox();
            this.dtpDataHora = new System.Windows.Forms.DateTimePicker();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.tblMainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.grpPilotagem = new System.Windows.Forms.GroupBox();
            this.grpDreno = new System.Windows.Forms.GroupBox();
            this.grpControles = new System.Windows.Forms.GroupBox();
            this.grpVazao = new System.Windows.Forms.GroupBox();
            this.grpPressao = new System.Windows.Forms.GroupBox();
            this.grpRotacao = new System.Windows.Forms.GroupBox();
            this.grpTemperatura = new System.Windows.Forms.GroupBox();
            this.pnlTop.SuspendLayout();
            this.tblTopLayout.SuspendLayout();
            this.tblMainLayout.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlTop
            //
            this.pnlTop.BackColor = System.Drawing.SystemColors.ControlDark; // Cor de fundo cinza para o painel superior
            this.pnlTop.Controls.Add(this.tblTopLayout);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(984, 60); // Altura ajustável conforme necessário
            this.pnlTop.TabIndex = 0;
            //
            // tblTopLayout
            //
            this.tblTopLayout.ColumnCount = 4;
            this.tblTopLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F)); // Coluna Status Geral/Retornar
            this.tblTopLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F)); // Coluna Nome Texto
            this.tblTopLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F)); // Coluna Data Hora
            this.tblTopLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F)); // Coluna Teste
            this.tblTopLayout.Controls.Add(this.lblStatusGeral, 0, 0);
            this.tblTopLayout.Controls.Add(this.lblNomeTexto, 1, 0);
            this.tblTopLayout.Controls.Add(this.lblDataHora, 2, 0);
            this.tblTopLayout.Controls.Add(this.lblTeste, 3, 0);
            this.tblTopLayout.Controls.Add(this.btnRetornar, 0, 1);
            this.tblTopLayout.Controls.Add(this.pnlPlaceholder, 1, 1); // Placeholder na coluna Nome Texto
            this.tblTopLayout.Controls.Add(this.txtNomeTexto, 1, 1); // TextBox sobreposto ou ao lado do placeholder
            this.tblTopLayout.Controls.Add(this.dtpDataHora, 2, 1);
            // Adicionar controle para "TESTE" se houver (ex: um Label vazio ou outro controle)
            // this.tblTopLayout.Controls.Add(new System.Windows.Forms.Label() { Dock = System.Windows.Forms.DockStyle.Fill }, 3, 1);
            this.tblTopLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblTopLayout.Location = new System.Drawing.Point(0, 0);
            this.tblTopLayout.Name = "tblTopLayout";
            this.tblTopLayout.RowCount = 2;
            this.tblTopLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F)); // Linha para Labels
            this.tblTopLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F)); // Linha para Controles
            this.tblTopLayout.Size = new System.Drawing.Size(984, 60);
            this.tblTopLayout.TabIndex = 0;
            //
            // lblStatusGeral
            //
            this.lblStatusGeral.AutoSize = true;
            this.lblStatusGeral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatusGeral.Location = new System.Drawing.Point(3, 0);
            this.lblStatusGeral.Name = "lblStatusGeral";
            this.lblStatusGeral.Size = new System.Drawing.Size(190, 20);
            this.lblStatusGeral.TabIndex = 0;
            this.lblStatusGeral.Text = "STATUS GERAL";
            this.lblStatusGeral.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblNomeTexto
            //
            this.lblNomeTexto.AutoSize = true;
            this.lblNomeTexto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNomeTexto.Location = new System.Drawing.Point(199, 0);
            this.lblNomeTexto.Name = "lblNomeTexto";
            this.lblNomeTexto.Size = new System.Drawing.Size(338, 20);
            this.lblNomeTexto.TabIndex = 1;
            this.lblNomeTexto.Text = "NOME TEXTO";
            this.lblNomeTexto.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblDataHora
            //
            this.lblDataHora.AutoSize = true;
            this.lblDataHora.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataHora.Location = new System.Drawing.Point(543, 0);
            this.lblDataHora.Name = "lblDataHora";
            this.lblDataHora.Size = new System.Drawing.Size(240, 20);
            this.lblDataHora.TabIndex = 2;
            this.lblDataHora.Text = "DATA HORA";
            this.lblDataHora.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblTeste
            //
            this.lblTeste.AutoSize = true;
            this.lblTeste.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTeste.Location = new System.Drawing.Point(789, 0);
            this.lblTeste.Name = "lblTeste";
            this.lblTeste.Size = new System.Drawing.Size(192, 20);
            this.lblTeste.TabIndex = 3;
            this.lblTeste.Text = "TESTE";
            this.lblTeste.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // btnRetornar
            //
            this.btnRetornar.Anchor = System.Windows.Forms.AnchorStyles.None; // Centraliza na célula
            this.btnRetornar.Location = new System.Drawing.Point(61, 28); // Ajustar posição fina se necessário
            this.btnRetornar.Name = "btnRetornar";
            this.btnRetornar.Size = new System.Drawing.Size(75, 23);
            this.btnRetornar.TabIndex = 4;
            this.btnRetornar.Text = "Retornar";
            this.btnRetornar.UseVisualStyleBackColor = true;
            //
            // pnlPlaceholder (O quadrado cinza claro)
            //
            this.pnlPlaceholder.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right; // Estica horizontalmente
            this.pnlPlaceholder.BackColor = System.Drawing.SystemColors.ControlLight; // Cinza claro
            this.pnlPlaceholder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPlaceholder.Location = new System.Drawing.Point(202, 26); // Posição inicial (será sobreposto pelo TextBox)
            this.pnlPlaceholder.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6); // Margens verticais
            this.pnlPlaceholder.Name = "pnlPlaceholder";
            this.pnlPlaceholder.Size = new System.Drawing.Size(50, 28); // Tamanho do quadrado
            this.pnlPlaceholder.TabIndex = 5;
            // Nota: A imagem mostra o TextBox e o Placeholder na mesma área.
            // Você pode colocar o TextBox ao lado ou remover o placeholder se o TextBox ocupar o espaço.
            // Aqui, vamos colocar o TextBox para preencher a célula, potencialmente cobrindo o placeholder.
            //
            // txtNomeTexto
            //
            this.txtNomeTexto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right))); // Estica horizontalmente
            this.txtNomeTexto.Location = new System.Drawing.Point(258, 30); // Posição ao lado do placeholder
            this.txtNomeTexto.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.txtNomeTexto.Name = "txtNomeTexto";
            this.txtNomeTexto.Size = new System.Drawing.Size(279, 20); // Ajusta largura
            this.txtNomeTexto.TabIndex = 6;
            //
            // dtpDataHora
            //
            this.dtpDataHora.Anchor = System.Windows.Forms.AnchorStyles.None; // Centraliza
            this.dtpDataHora.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpDataHora.Location = new System.Drawing.Point(583, 30); // Ajustar posição
            this.dtpDataHora.Name = "dtpDataHora";
            this.dtpDataHora.ShowUpDown = true; // Estilo parecido com o da imagem
            this.dtpDataHora.Size = new System.Drawing.Size(160, 20); // Largura ajustável
            this.dtpDataHora.TabIndex = 7;
            this.dtpDataHora.Value = System.DateTime.Parse("19:03:27"); // Valor inicial da imagem
            //
            // pnlBottom
            //
            this.pnlBottom.BackColor = System.Drawing.SystemColors.ControlDark; // Cor cinza
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 521); // Posição baseada na altura total - altura do painel
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(984, 40); // Altura fixa
            this.pnlBottom.TabIndex = 1;
            //
            // tblMainLayout
            //
            this.tblMainLayout.BackColor = System.Drawing.SystemColors.ControlDarkDark; // Fundo cinza escuro para a área principal
            this.tblMainLayout.ColumnCount = 3;
            this.tblMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F)); // Coluna Esquerda (Pilotagem, Dreno, Controles)
            this.tblMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F)); // Coluna Meio (Vazão, Rotação)
            this.tblMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F)); // Coluna Direita (Pressão, Temperatura)
            this.tblMainLayout.Controls.Add(this.grpPilotagem, 0, 0);
            this.tblMainLayout.Controls.Add(this.grpDreno, 0, 1);
            this.tblMainLayout.Controls.Add(this.grpControles, 0, 2);
            this.tblMainLayout.Controls.Add(this.grpVazao, 1, 0);
            this.tblMainLayout.Controls.Add(this.grpPressao, 2, 0);
            this.tblMainLayout.Controls.Add(this.grpRotacao, 1, 2);
            this.tblMainLayout.Controls.Add(this.grpTemperatura, 2, 2);
            this.tblMainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMainLayout.Location = new System.Drawing.Point(0, 60); // Abaixo do pnlTop
            this.tblMainLayout.Name = "tblMainLayout";
            this.tblMainLayout.Padding = new System.Windows.Forms.Padding(5); // Espaçamento interno
            this.tblMainLayout.RowCount = 3;
            this.tblMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F)); // Linha Superior (Pilotagem, Vazão, Pressão)
            this.tblMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 20F)); // Linha Meio (Dreno)
            this.tblMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F)); // Linha Inferior (Controles, Rotação, Temperatura)
            this.tblMainLayout.Size = new System.Drawing.Size(984, 461); // Preenche espaço entre pnlTop e pnlBottom
            this.tblMainLayout.TabIndex = 2;
            //
            // grpPilotagem
            //
            this.grpPilotagem.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPilotagem.ForeColor = System.Drawing.SystemColors.ControlLightLight; // Texto branco para contraste
            this.grpPilotagem.Location = new System.Drawing.Point(8, 8); // Padding do TableLayoutPanel
            this.grpPilotagem.Name = "grpPilotagem";
            this.grpPilotagem.Size = new System.Drawing.Size(383, 172); // Tamanho calculado pelo TableLayoutPanel
            this.grpPilotagem.TabIndex = 0;
            this.grpPilotagem.TabStop = false;
            this.grpPilotagem.Text = "Pilotagem";
            // Adicione controles dentro deste GroupBox conforme necessário
            //
            // grpDreno
            //
            this.grpDreno.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpDreno.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.grpDreno.Location = new System.Drawing.Point(8, 186); // Linha 1
            this.grpDreno.Name = "grpDreno";
            this.grpDreno.Size = new System.Drawing.Size(383, 80); // Tamanho calculado
            this.grpDreno.TabIndex = 1;
            this.grpDreno.TabStop = false;
            this.grpDreno.Text = "Dreno";
            //
            // grpControles
            //
            this.grpControles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpControles.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.grpControles.Location = new System.Drawing.Point(8, 272); // Linha 2
            this.grpControles.Name = "grpControles";
            this.grpControles.Size = new System.Drawing.Size(383, 176); // Tamanho calculado
            this.grpControles.TabIndex = 2;
            this.grpControles.TabStop = false;
            this.grpControles.Text = "Controles";
            //
            // grpVazao
            //
            this.grpVazao.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpVazao.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.grpVazao.Location = new System.Drawing.Point(397, 8); // Coluna 1, Linha 0
            this.grpVazao.Name = "grpVazao";
            // Nota: Na imagem, parece ocupar visualmente 2 linhas.
            // Para simplificar, colocamos na linha 0 e coluna 1.
            // Se precisar que o GroupBox realmente ocupe o espaço da linha do Dreno:
            // this.tblMainLayout.SetRowSpan(this.grpVazao, 2); // Ocupa 2 linhas
            this.grpVazao.Size = new System.Drawing.Size(286, 172); // Tamanho calculado
            this.grpVazao.TabIndex = 3;
            this.grpVazao.TabStop = false;
            this.grpVazao.Text = "Vazão";
            //
            // grpPressao
            //
            this.grpPressao.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPressao.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.grpPressao.Location = new System.Drawing.Point(689, 8); // Coluna 2, Linha 0
            this.grpPressao.Name = "grpPressao";
            // this.tblMainLayout.SetRowSpan(this.grpPressao, 2); // Ocupa 2 linhas
            this.grpPressao.Size = new System.Drawing.Size(287, 172); // Tamanho calculado
            this.grpPressao.TabIndex = 4;
            this.grpPressao.TabStop = false;
            this.grpPressao.Text = "Pressão";
            //
            // grpRotacao
            //
            this.grpRotacao.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpRotacao.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.grpRotacao.Location = new System.Drawing.Point(397, 272); // Coluna 1, Linha 2
            this.grpRotacao.Name = "grpRotacao";
            this.grpRotacao.Size = new System.Drawing.Size(286, 176); // Tamanho calculado
            this.grpRotacao.TabIndex = 5;
            this.grpRotacao.TabStop = false;
            this.grpRotacao.Text = "Rotação";
            //
            // grpTemperatura
            //
            this.grpTemperatura.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTemperatura.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.grpTemperatura.Location = new System.Drawing.Point(689, 272); // Coluna 2, Linha 2
            this.grpTemperatura.Name = "grpTemperatura";
            this.grpTemperatura.Size = new System.Drawing.Size(287, 176); // Tamanho calculado
            this.grpTemperatura.TabIndex = 6;
            this.grpTemperatura.TabStop = false;
            this.grpTemperatura.Text = "Temperatura";
            //
            // TelaStatusGeral
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 561); // Tamanho inicial da janela
            this.Controls.Add(this.tblMainLayout);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Name = "TelaStatusGeral";
            this.Text = "Tela Status Geral"; // Título da Janela
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized; // Inicia maximizado como na imagem
            this.pnlTop.ResumeLayout(false);
            this.tblTopLayout.ResumeLayout(false);
            this.tblTopLayout.PerformLayout();
            this.tblMainLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.TableLayoutPanel tblMainLayout;
        private System.Windows.Forms.GroupBox grpPilotagem;
        private System.Windows.Forms.GroupBox grpDreno;
        private System.Windows.Forms.GroupBox grpControles;
        private System.Windows.Forms.GroupBox grpVazao;
        private System.Windows.Forms.GroupBox grpPressao;
        private System.Windows.Forms.GroupBox grpRotacao;
        private System.Windows.Forms.GroupBox grpTemperatura;
        private System.Windows.Forms.TableLayoutPanel tblTopLayout;
        private System.Windows.Forms.Label lblStatusGeral;
        private System.Windows.Forms.Label lblNomeTexto;
        private System.Windows.Forms.Label lblDataHora;
        private System.Windows.Forms.Label lblTeste;
        private System.Windows.Forms.Button btnRetornar;
        private System.Windows.Forms.Panel pnlPlaceholder;
        private System.Windows.Forms.TextBox txtNomeTexto;
        private System.Windows.Forms.DateTimePicker dtpDataHora;
    }
}