namespace minas.teste.prototype.MVVM.View
{
    partial class configuracao
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(configuracao));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.comboBoxPortaCOM = new System.Windows.Forms.ComboBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxSensorTipo = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnTestarConexao = new MetroFramework.Controls.MetroButton();
            this.BtnLimpar = new MetroFramework.Controls.MetroButton();
            this.BtnSalvar = new MetroFramework.Controls.MetroButton();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.StatusID = new System.Windows.Forms.Label();
            this.lblStatusConexao = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.comboBoxBaudRate);
            this.panel1.Controls.Add(this.comboBoxPortaCOM);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comboBoxSensorTipo);
            this.panel1.Location = new System.Drawing.Point(16, 98);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(329, 231);
            this.panel1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 169);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "Baute Rate";
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.FormattingEnabled = true;
            this.comboBoxBaudRate.Location = new System.Drawing.Point(19, 189);
            this.comboBoxBaudRate.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(205, 24);
            this.comboBoxBaudRate.TabIndex = 6;
            // 
            // comboBoxPortaCOM
            // 
            this.comboBoxPortaCOM.FormattingEnabled = true;
            this.comboBoxPortaCOM.Location = new System.Drawing.Point(19, 125);
            this.comboBoxPortaCOM.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxPortaCOM.Name = "comboBoxPortaCOM";
            this.comboBoxPortaCOM.Size = new System.Drawing.Size(205, 24);
            this.comboBoxPortaCOM.TabIndex = 5;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackgroundImage = global::minas.teste.prototype.Properties.Resources.porta_vga;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(235, 97);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(85, 80);
            this.pictureBox2.TabIndex = 4;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::minas.teste.prototype.Properties.Resources.medidor;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(235, 4);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(85, 80);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 97);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Porta Serial";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 22);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Sensor";
            // 
            // comboBoxSensorTipo
            // 
            this.comboBoxSensorTipo.FormattingEnabled = true;
            this.comboBoxSensorTipo.Location = new System.Drawing.Point(20, 58);
            this.comboBoxSensorTipo.Margin = new System.Windows.Forms.Padding(4);
            this.comboBoxSensorTipo.Name = "comboBoxSensorTipo";
            this.comboBoxSensorTipo.Size = new System.Drawing.Size(205, 24);
            this.comboBoxSensorTipo.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnTestarConexao);
            this.groupBox1.Controls.Add(this.BtnLimpar);
            this.groupBox1.Controls.Add(this.BtnSalvar);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(353, 98);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(319, 328);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Coeficiente de Calibração";
            // 
            // btnTestarConexao
            // 
            this.btnTestarConexao.Highlight = false;
            this.btnTestarConexao.Location = new System.Drawing.Point(29, 87);
            this.btnTestarConexao.Margin = new System.Windows.Forms.Padding(4);
            this.btnTestarConexao.Name = "btnTestarConexao";
            this.btnTestarConexao.Size = new System.Drawing.Size(253, 52);
            this.btnTestarConexao.Style = MetroFramework.MetroColorStyle.Green;
            this.btnTestarConexao.StyleManager = null;
            this.btnTestarConexao.TabIndex = 3;
            this.btnTestarConexao.Text = "Testar";
            this.btnTestarConexao.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.btnTestarConexao.Click += new System.EventHandler(this.btnTestarConexao_Click);
            // 
            // BtnLimpar
            // 
            this.BtnLimpar.Highlight = false;
            this.BtnLimpar.Location = new System.Drawing.Point(29, 242);
            this.BtnLimpar.Margin = new System.Windows.Forms.Padding(4);
            this.BtnLimpar.Name = "BtnLimpar";
            this.BtnLimpar.Size = new System.Drawing.Size(253, 52);
            this.BtnLimpar.Style = MetroFramework.MetroColorStyle.Blue;
            this.BtnLimpar.StyleManager = null;
            this.BtnLimpar.TabIndex = 2;
            this.BtnLimpar.Text = "Limpar";
            this.BtnLimpar.Theme = MetroFramework.MetroThemeStyle.Light;
            this.BtnLimpar.Click += new System.EventHandler(this.btnLimparConfigGeral_Click);
            // 
            // BtnSalvar
            // 
            this.BtnSalvar.Highlight = false;
            this.BtnSalvar.Location = new System.Drawing.Point(29, 162);
            this.BtnSalvar.Margin = new System.Windows.Forms.Padding(4);
            this.BtnSalvar.Name = "BtnSalvar";
            this.BtnSalvar.Size = new System.Drawing.Size(253, 52);
            this.BtnSalvar.Style = MetroFramework.MetroColorStyle.Green;
            this.BtnSalvar.StyleManager = null;
            this.BtnSalvar.TabIndex = 1;
            this.BtnSalvar.Text = "Salvar";
            this.BtnSalvar.Theme = MetroFramework.MetroThemeStyle.Dark;
            this.BtnSalvar.Click += new System.EventHandler(this.btnSalvarAplicarConfiguracoes_Click);
            // 
            // textBox2
            // 
            this.textBox2.ImeMode = System.Windows.Forms.ImeMode.On;
            this.textBox2.Location = new System.Drawing.Point(29, 37);
            this.textBox2.Margin = new System.Windows.Forms.Padding(4);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(252, 32);
            this.textBox2.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Location = new System.Drawing.Point(16, 334);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(329, 92);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(71, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(173, 52);
            this.button1.TabIndex = 4;
            this.button1.Text = "Retornar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnVoltar_Click);
            // 
            // StatusID
            // 
            this.StatusID.AutoSize = true;
            this.StatusID.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusID.Location = new System.Drawing.Point(199, 9);
            this.StatusID.Name = "StatusID";
            this.StatusID.Size = new System.Drawing.Size(230, 32);
            this.StatusID.TabIndex = 4;
            this.StatusID.Text = "Status Conexão";
            // 
            // lblStatusConexao
            // 
            this.lblStatusConexao.AutoSize = true;
            this.lblStatusConexao.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblStatusConexao.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatusConexao.Location = new System.Drawing.Point(199, 52);
            this.lblStatusConexao.Name = "lblStatusConexao";
            this.lblStatusConexao.Size = new System.Drawing.Size(2, 34);
            this.lblStatusConexao.TabIndex = 5;
            // 
            // configuracao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(699, 451);
            this.Controls.Add(this.lblStatusConexao);
            this.Controls.Add(this.StatusID);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "configuracao";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Configuracao_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBoxSensorTipo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.TextBox textBox2;
        private MetroFramework.Controls.MetroButton BtnLimpar;
        private MetroFramework.Controls.MetroButton BtnSalvar;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.Windows.Forms.ComboBox comboBoxPortaCOM;
        private MetroFramework.Controls.MetroButton btnTestarConexao;
        private System.Windows.Forms.Label StatusID;
        private System.Windows.Forms.Label lblStatusConexao;
    }
}