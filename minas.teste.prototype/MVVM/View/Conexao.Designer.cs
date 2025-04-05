namespace minas.teste.prototype.MVVM.View
{
    partial class conexao
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
            CodeArtEng.Gauge.Themes.ThemeColors themeColors1 = new CodeArtEng.Gauge.Themes.ThemeColors();
            CodeArtEng.Gauge.Themes.ThemeColors themeColors2 = new CodeArtEng.Gauge.Themes.ThemeColors();
            CodeArtEng.Gauge.Themes.ThemeColors themeColors3 = new CodeArtEng.Gauge.Themes.ThemeColors();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(conexao));
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.cuiTextBox21 = new CuoreUI.Controls.cuiTextBox2();
            this.bitButton1 = new CodeArtEng.Gauge.InputGauge.BitButton();
            this.cuiButton1 = new CuoreUI.Controls.cuiButton();
            this.cuiButton2 = new CuoreUI.Controls.cuiButton();
            this.cuiLabel1 = new CuoreUI.Controls.cuiLabel();
            this.cuiLabel2 = new CuoreUI.Controls.cuiLabel();
            this.nn = new System.IO.Ports.SerialPort(this.components);
            this.metroPanel1.SuspendLayout();
            this.metroPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 46);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(611, 423);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 46);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(611, 423);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.label1);
            this.metroPanel1.Controls.Add(this.metroLabel5);
            this.metroPanel1.Controls.Add(this.metroLabel3);
            this.metroPanel1.Controls.Add(this.metroLabel2);
            this.metroPanel1.Controls.Add(this.comboBox3);
            this.metroPanel1.Controls.Add(this.comboBox2);
            this.metroPanel1.Controls.Add(this.comboBox1);
            this.metroPanel1.Controls.Add(this.metroLabel1);
            this.metroPanel1.CustomBackground = false;
            this.metroPanel1.HorizontalScrollbar = false;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 25;
            this.metroPanel1.Location = new System.Drawing.Point(14, 63);
            this.metroPanel1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(351, 409);
            this.metroPanel1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroPanel1.StyleManager = null;
            this.metroPanel1.TabIndex = 0;
            this.metroPanel1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroPanel1.VerticalScrollbar = false;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 26;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(52, 322);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 32);
            this.label1.TabIndex = 11;
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.CustomBackground = false;
            this.metroLabel5.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabel5.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel5.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel5.Location = new System.Drawing.Point(102, 36);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(41, 20);
            this.metroLabel5.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel5.StyleManager = null;
            this.metroLabel5.TabIndex = 10;
            this.metroLabel5.Text = "Porta";
            this.metroLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel5.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel5.UseStyleColors = false;
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.CustomBackground = false;
            this.metroLabel3.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabel3.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel3.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel3.Location = new System.Drawing.Point(96, 200);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(48, 20);
            this.metroLabel3.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel3.StyleManager = null;
            this.metroLabel3.TabIndex = 8;
            this.metroLabel3.Text = "Dados";
            this.metroLabel3.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel3.UseStyleColors = false;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.CustomBackground = false;
            this.metroLabel2.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabel2.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel2.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel2.Location = new System.Drawing.Point(96, 120);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(75, 20);
            this.metroLabel2.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel2.StyleManager = null;
            this.metroLabel2.TabIndex = 7;
            this.metroLabel2.Text = "BaudeRate";
            this.metroLabel2.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel2.UseStyleColors = false;
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(17, 241);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(255, 24);
            this.comboBox3.TabIndex = 5;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(17, 156);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(255, 24);
            this.comboBox2.TabIndex = 4;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(17, 70);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(255, 24);
            this.comboBox1.TabIndex = 3;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.CustomBackground = false;
            this.metroLabel1.FontSize = MetroFramework.MetroLabelSize.Medium;
            this.metroLabel1.FontWeight = MetroFramework.MetroLabelWeight.Light;
            this.metroLabel1.LabelMode = MetroFramework.Controls.MetroLabelMode.Default;
            this.metroLabel1.Location = new System.Drawing.Point(96, 36);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(0, 0);
            this.metroLabel1.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel1.StyleManager = null;
            this.metroLabel1.TabIndex = 2;
            this.metroLabel1.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroLabel1.UseStyleColors = false;
            // 
            // metroPanel2
            // 
            this.metroPanel2.Controls.Add(this.cuiTextBox21);
            this.metroPanel2.CustomBackground = false;
            this.metroPanel2.HorizontalScrollbar = false;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 25;
            this.metroPanel2.Location = new System.Drawing.Point(375, 63);
            this.metroPanel2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(400, 300);
            this.metroPanel2.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroPanel2.StyleManager = null;
            this.metroPanel2.TabIndex = 1;
            this.metroPanel2.Theme = MetroFramework.MetroThemeStyle.Light;
            this.metroPanel2.VerticalScrollbar = false;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 26;
            // 
            // cuiTextBox21
            // 
            this.cuiTextBox21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.cuiTextBox21.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cuiTextBox21.BorderFocusColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiTextBox21.BorderSize = 1;
            this.cuiTextBox21.Content = "";
            this.cuiTextBox21.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.cuiTextBox21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cuiTextBox21.ForeColor = System.Drawing.Color.DarkGreen;
            this.cuiTextBox21.Location = new System.Drawing.Point(18, 19);
            this.cuiTextBox21.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cuiTextBox21.Multiline = false;
            this.cuiTextBox21.Name = "cuiTextBox21";
            this.cuiTextBox21.Padding = new System.Windows.Forms.Padding(18, 121, 18, 0);
            this.cuiTextBox21.PasswordChar = false;
            this.cuiTextBox21.PlaceholderColor = System.Drawing.Color.DimGray;
            this.cuiTextBox21.PlaceholderText = "";
            this.cuiTextBox21.Rounding = 8;
            this.cuiTextBox21.Size = new System.Drawing.Size(357, 260);
            this.cuiTextBox21.TabIndex = 2;
            this.cuiTextBox21.TextOffset = new System.Drawing.Size(0, 0);
            this.cuiTextBox21.UnderlinedStyle = false;
            // 
            // bitButton1
            // 
            this.bitButton1.ButtonStyle = CodeArtEng.Gauge.InputGauge.BitButtonStyle.BottomIndicator;
            this.bitButton1.Image = null;
            this.bitButton1.Location = new System.Drawing.Point(375, 370);
            this.bitButton1.Maximum = 2147483647D;
            this.bitButton1.Name = "bitButton1";
            this.bitButton1.ResetValue = 0D;
            this.bitButton1.ScaleFactor = 1D;
            this.bitButton1.Size = new System.Drawing.Size(128, 102);
            this.bitButton1.TabIndex = 2;
            this.bitButton1.Title = "Testar";
            this.bitButton1.Unit = "";
            this.bitButton1.UserDefinedColors.Base = themeColors1;
            this.bitButton1.UserDefinedColors.Error = themeColors2;
            this.bitButton1.UserDefinedColors.Warning = themeColors3;
            // 
            // cuiButton1
            // 
            this.cuiButton1.CheckButton = false;
            this.cuiButton1.Checked = false;
            this.cuiButton1.CheckedBackground = System.Drawing.Color.Blue;
            this.cuiButton1.CheckedForeColor = System.Drawing.Color.White;
            this.cuiButton1.CheckedImageTint = System.Drawing.Color.White;
            this.cuiButton1.CheckedOutline = System.Drawing.Color.Blue;
            this.cuiButton1.Content = "Conectar";
            this.cuiButton1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.cuiButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cuiButton1.ForeColor = System.Drawing.Color.White;
            this.cuiButton1.HoverBackground = System.Drawing.Color.Silver;
            this.cuiButton1.HoveredImageTint = System.Drawing.Color.White;
            this.cuiButton1.HoverForeColor = System.Drawing.Color.White;
            this.cuiButton1.HoverOutline = System.Drawing.Color.Empty;
            this.cuiButton1.Image = null;
            this.cuiButton1.ImageAutoCenter = true;
            this.cuiButton1.ImageExpand = new System.Drawing.Point(0, 0);
            this.cuiButton1.ImageOffset = new System.Drawing.Point(0, 0);
            this.cuiButton1.ImageTint = System.Drawing.Color.White;
            this.cuiButton1.Location = new System.Drawing.Point(512, 370);
            this.cuiButton1.Name = "cuiButton1";
            this.cuiButton1.NormalBackground = System.Drawing.Color.DarkGreen;
            this.cuiButton1.NormalOutline = System.Drawing.Color.Empty;
            this.cuiButton1.OutlineThickness = 1.6F;
            this.cuiButton1.PressedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton1.PressedForeColor = System.Drawing.Color.White;
            this.cuiButton1.PressedImageTint = System.Drawing.Color.White;
            this.cuiButton1.PressedOutline = System.Drawing.Color.Empty;
            this.cuiButton1.Rounding = new System.Windows.Forms.Padding(8);
            this.cuiButton1.Size = new System.Drawing.Size(130, 102);
            this.cuiButton1.TabIndex = 3;
            this.cuiButton1.TextOffset = new System.Drawing.Point(0, 0);
            this.cuiButton1.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cuiButton2
            // 
            this.cuiButton2.CheckButton = false;
            this.cuiButton2.Checked = false;
            this.cuiButton2.CheckedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton2.CheckedForeColor = System.Drawing.Color.White;
            this.cuiButton2.CheckedImageTint = System.Drawing.Color.White;
            this.cuiButton2.CheckedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton2.Content = "Desconectar";
            this.cuiButton2.DialogResult = System.Windows.Forms.DialogResult.None;
            this.cuiButton2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cuiButton2.ForeColor = System.Drawing.Color.White;
            this.cuiButton2.HoverBackground = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton2.HoveredImageTint = System.Drawing.Color.White;
            this.cuiButton2.HoverForeColor = System.Drawing.Color.White;
            this.cuiButton2.HoverOutline = System.Drawing.Color.Empty;
            this.cuiButton2.Image = null;
            this.cuiButton2.ImageAutoCenter = true;
            this.cuiButton2.ImageExpand = new System.Drawing.Point(0, 0);
            this.cuiButton2.ImageOffset = new System.Drawing.Point(0, 0);
            this.cuiButton2.ImageTint = System.Drawing.Color.White;
            this.cuiButton2.Location = new System.Drawing.Point(648, 370);
            this.cuiButton2.Name = "cuiButton2";
            this.cuiButton2.NormalBackground = System.Drawing.Color.Crimson;
            this.cuiButton2.NormalOutline = System.Drawing.Color.Empty;
            this.cuiButton2.OutlineThickness = 1.6F;
            this.cuiButton2.PressedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton2.PressedForeColor = System.Drawing.Color.White;
            this.cuiButton2.PressedImageTint = System.Drawing.Color.White;
            this.cuiButton2.PressedOutline = System.Drawing.Color.Empty;
            this.cuiButton2.Rounding = new System.Windows.Forms.Padding(8);
            this.cuiButton2.Size = new System.Drawing.Size(130, 102);
            this.cuiButton2.TabIndex = 4;
            this.cuiButton2.TextOffset = new System.Drawing.Point(0, 0);
            // 
            // cuiLabel1
            // 
            this.cuiLabel1.Content = "Saida\\ Analógica";
            this.cuiLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cuiLabel1.HorizontalAlignment = CuoreUI.Controls.cuiLabel.HorizontalAlignments.Center;
            this.cuiLabel1.Location = new System.Drawing.Point(495, 17);
            this.cuiLabel1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cuiLabel1.Name = "cuiLabel1";
            this.cuiLabel1.Size = new System.Drawing.Size(190, 38);
            this.cuiLabel1.TabIndex = 3;
            // 
            // cuiLabel2
            // 
            this.cuiLabel2.Content = "Configuração\\ de\\ Controlador";
            this.cuiLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cuiLabel2.HorizontalAlignment = CuoreUI.Controls.cuiLabel.HorizontalAlignments.Center;
            this.cuiLabel2.Location = new System.Drawing.Point(14, 13);
            this.cuiLabel2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cuiLabel2.Name = "cuiLabel2";
            this.cuiLabel2.Size = new System.Drawing.Size(307, 38);
            this.cuiLabel2.TabIndex = 5;
            // 
            // conexao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.HotTrack;
            this.ClientSize = new System.Drawing.Size(835, 572);
            this.Controls.Add(this.cuiLabel2);
            this.Controls.Add(this.cuiLabel1);
            this.Controls.Add(this.cuiButton2);
            this.Controls.Add(this.cuiButton1);
            this.Controls.Add(this.bitButton1);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.metroPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "conexao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel1.PerformLayout();
            this.metroPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private CodeArtEng.Gauge.InputGauge.BitButton bitButton1;
        private CuoreUI.Controls.cuiButton cuiButton1;
        private CuoreUI.Controls.cuiButton cuiButton2;
        private CuoreUI.Controls.cuiLabel cuiLabel1;
        private CuoreUI.Controls.cuiTextBox2 cuiTextBox21;
        private CuoreUI.Controls.cuiLabel cuiLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private System.Windows.Forms.Label label1;
        private System.IO.Ports.SerialPort nn;
    }
}