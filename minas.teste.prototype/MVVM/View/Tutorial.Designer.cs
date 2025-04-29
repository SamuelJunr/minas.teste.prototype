namespace minas.teste.prototype.MVVM.View
{
    partial class Tutorial
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
            CodeArtEng.Gauge.Themes.ThemeColors themeColors1 = new CodeArtEng.Gauge.Themes.ThemeColors();
            CodeArtEng.Gauge.Themes.ThemeColors themeColors2 = new CodeArtEng.Gauge.Themes.ThemeColors();
            CodeArtEng.Gauge.Themes.ThemeColors themeColors3 = new CodeArtEng.Gauge.Themes.ThemeColors();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tutorial));
            this.gaugePanel1 = new CodeArtEng.Gauge.PanelGauges.GaugePanel();
            this.backgroundImagePictureBox = new System.Windows.Forms.PictureBox();
            this.guiabanca = new System.Windows.Forms.GroupBox();
            this.guiasoft = new System.Windows.Forms.GroupBox();
            this.cuiButton3 = new CuoreUI.Controls.cuiButton();
            this.gaugePanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundImagePictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // gaugePanel1
            // 
            this.gaugePanel1.Controls.Add(this.guiabanca);
            this.gaugePanel1.Controls.Add(this.cuiButton3);
            this.gaugePanel1.Controls.Add(this.guiasoft);
            this.gaugePanel1.Controls.Add(this.backgroundImagePictureBox);
            this.gaugePanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gaugePanel1.Location = new System.Drawing.Point(0, 0);
            this.gaugePanel1.Name = "gaugePanel1";
            this.gaugePanel1.Size = new System.Drawing.Size(1900, 1000);
            this.gaugePanel1.TabIndex = 0;
            this.gaugePanel1.UserDefinedColors.Base = themeColors1;
            this.gaugePanel1.UserDefinedColors.Error = themeColors2;
            this.gaugePanel1.UserDefinedColors.Warning = themeColors3;
            // 
            // backgroundImagePictureBox
            // 
            this.backgroundImagePictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundImagePictureBox.Location = new System.Drawing.Point(0, 0);
            this.backgroundImagePictureBox.Name = "backgroundImagePictureBox";
            this.backgroundImagePictureBox.Size = new System.Drawing.Size(1900, 1000);
            this.backgroundImagePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.backgroundImagePictureBox.TabIndex = 0;
            this.backgroundImagePictureBox.TabStop = false;
            // 
            // guiabanca
            // 
            this.guiabanca.BackColor = System.Drawing.Color.Transparent;
            this.guiabanca.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guiabanca.Location = new System.Drawing.Point(24, 12);
            this.guiabanca.Name = "guiabanca";
            this.guiabanca.Size = new System.Drawing.Size(552, 481);
            this.guiabanca.TabIndex = 1;
            this.guiabanca.TabStop = false;
            this.guiabanca.Text = "GUIA DE UTILIZAÇÃO DA BANCADA";
            // 
            // guiasoft
            // 
            this.guiasoft.BackColor = System.Drawing.Color.Transparent;
            this.guiasoft.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.guiasoft.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.guiasoft.Location = new System.Drawing.Point(24, 508);
            this.guiasoft.Name = "guiasoft";
            this.guiasoft.Size = new System.Drawing.Size(552, 270);
            this.guiasoft.TabIndex = 2;
            this.guiasoft.TabStop = false;
            this.guiasoft.Text = "GUIA DE UTILIZAÇÃO DO SOFTWARE";
            // 
            // cuiButton3
            // 
            this.cuiButton3.BackColor = System.Drawing.Color.Transparent;
            this.cuiButton3.CheckButton = false;
            this.cuiButton3.Checked = false;
            this.cuiButton3.CheckedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton3.CheckedForeColor = System.Drawing.Color.White;
            this.cuiButton3.CheckedImageTint = System.Drawing.Color.White;
            this.cuiButton3.CheckedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton3.Content = "Retornar";
            this.cuiButton3.DialogResult = System.Windows.Forms.DialogResult.None;
            this.cuiButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cuiButton3.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.cuiButton3.HoverBackground = System.Drawing.Color.DimGray;
            this.cuiButton3.HoveredImageTint = System.Drawing.Color.White;
            this.cuiButton3.HoverForeColor = System.Drawing.Color.White;
            this.cuiButton3.HoverOutline = System.Drawing.Color.Empty;
            this.cuiButton3.Image = null;
            this.cuiButton3.ImageAutoCenter = true;
            this.cuiButton3.ImageExpand = new System.Drawing.Point(0, 0);
            this.cuiButton3.ImageOffset = new System.Drawing.Point(0, 0);
            this.cuiButton3.ImageTint = System.Drawing.Color.White;
            this.cuiButton3.Location = new System.Drawing.Point(177, 822);
            this.cuiButton3.Margin = new System.Windows.Forms.Padding(4);
            this.cuiButton3.Name = "cuiButton3";
            this.cuiButton3.NormalBackground = System.Drawing.Color.DimGray;
            this.cuiButton3.NormalOutline = System.Drawing.Color.Empty;
            this.cuiButton3.OutlineThickness = 1.6F;
            this.cuiButton3.PressedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.cuiButton3.PressedForeColor = System.Drawing.Color.White;
            this.cuiButton3.PressedImageTint = System.Drawing.Color.White;
            this.cuiButton3.PressedOutline = System.Drawing.Color.Empty;
            this.cuiButton3.Rounding = new System.Windows.Forms.Padding(8);
            this.cuiButton3.Size = new System.Drawing.Size(192, 86);
            this.cuiButton3.TabIndex = 9;
            this.cuiButton3.TextOffset = new System.Drawing.Point(0, 0);
            this.cuiButton3.Click += new System.EventHandler(this.CloseWindows_Click);
            // 
            // Tutorial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1900, 1000);
            this.Controls.Add(this.gaugePanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Tutorial";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tutorial";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.gaugePanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.backgroundImagePictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CodeArtEng.Gauge.PanelGauges.GaugePanel gaugePanel1;
        private System.Windows.Forms.PictureBox backgroundImagePictureBox;
        private System.Windows.Forms.GroupBox guiasoft;
        private System.Windows.Forms.GroupBox guiabanca;
        private CuoreUI.Controls.cuiButton cuiButton3;
    }
}