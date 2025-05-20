namespace minas.teste.prototype.MVVM.View
{
    partial class ConfigFormBomb
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblPressureUnit;
        private System.Windows.Forms.RadioButton radioBar;
        private System.Windows.Forms.RadioButton radioPsi;
        private System.Windows.Forms.GroupBox groupPressureUnit;
        private System.Windows.Forms.GroupBox groupFlowUnit;
        private System.Windows.Forms.RadioButton radioGpm;
        private System.Windows.Forms.RadioButton radioLpm;
        private System.Windows.Forms.Label lblFlowUnit;
        private System.Windows.Forms.Label lblSelectReadings;
        private System.Windows.Forms.Panel panelReadingsCheckboxes; // Este painel será o contêiner principal rolável
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnCancelConfig;
        private System.Windows.Forms.Button btnClearSelections;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.groupPressureUnit = new System.Windows.Forms.GroupBox();
            this.radioPsi = new System.Windows.Forms.RadioButton();
            this.radioBar = new System.Windows.Forms.RadioButton();
            this.lblPressureUnit = new System.Windows.Forms.Label();
            this.groupFlowUnit = new System.Windows.Forms.GroupBox();
            this.radioGpm = new System.Windows.Forms.RadioButton();
            this.radioLpm = new System.Windows.Forms.RadioButton();
            this.lblFlowUnit = new System.Windows.Forms.Label();
            this.lblSelectReadings = new System.Windows.Forms.Label();
            this.panelReadingsCheckboxes = new System.Windows.Forms.Panel();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.btnCancelConfig = new System.Windows.Forms.Button();
            this.btnClearSelections = new System.Windows.Forms.Button();
            this.groupPressureUnit.SuspendLayout();
            this.groupFlowUnit.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupPressureUnit
            // 
            this.groupPressureUnit.Controls.Add(this.radioPsi);
            this.groupPressureUnit.Controls.Add(this.radioBar);
            this.groupPressureUnit.Controls.Add(this.lblPressureUnit);
            this.groupPressureUnit.Location = new System.Drawing.Point(241, 12);
            this.groupPressureUnit.Name = "groupPressureUnit";
            this.groupPressureUnit.Size = new System.Drawing.Size(460, 70);
            this.groupPressureUnit.TabIndex = 0;
            this.groupPressureUnit.TabStop = false;
            this.groupPressureUnit.Text = "Unidades de Medida";
            // 
            // radioPsi
            // 
            this.radioPsi.AutoSize = true;
            this.radioPsi.Location = new System.Drawing.Point(230, 30);
            this.radioPsi.Name = "radioPsi";
            this.radioPsi.Size = new System.Drawing.Size(46, 20);
            this.radioPsi.TabIndex = 2;
            this.radioPsi.Text = "psi";
            this.radioPsi.UseVisualStyleBackColor = true;
            // 
            // radioBar
            // 
            this.radioBar.AutoSize = true;
            this.radioBar.Checked = true;
            this.radioBar.Location = new System.Drawing.Point(140, 30);
            this.radioBar.Name = "radioBar";
            this.radioBar.Size = new System.Drawing.Size(48, 20);
            this.radioBar.TabIndex = 1;
            this.radioBar.TabStop = true;
            this.radioBar.Text = "bar";
            this.radioBar.UseVisualStyleBackColor = true;
            // 
            // lblPressureUnit
            // 
            this.lblPressureUnit.AutoSize = true;
            this.lblPressureUnit.Location = new System.Drawing.Point(6, 32);
            this.lblPressureUnit.Name = "lblPressureUnit";
            this.lblPressureUnit.Size = new System.Drawing.Size(116, 16);
            this.lblPressureUnit.TabIndex = 0;
            this.lblPressureUnit.Text = "Unidade Pressão:";
            // 
            // groupFlowUnit
            // 
            this.groupFlowUnit.Controls.Add(this.radioGpm);
            this.groupFlowUnit.Controls.Add(this.radioLpm);
            this.groupFlowUnit.Controls.Add(this.lblFlowUnit);
            this.groupFlowUnit.Location = new System.Drawing.Point(241, 88);
            this.groupFlowUnit.Name = "groupFlowUnit";
            this.groupFlowUnit.Size = new System.Drawing.Size(460, 70);
            this.groupFlowUnit.TabIndex = 1;
            this.groupFlowUnit.TabStop = false;
            // 
            // radioGpm
            // 
            this.radioGpm.AutoSize = true;
            this.radioGpm.Location = new System.Drawing.Point(230, 30);
            this.radioGpm.Name = "radioGpm";
            this.radioGpm.Size = new System.Drawing.Size(55, 20);
            this.radioGpm.TabIndex = 2;
            this.radioGpm.Text = "gpm";
            this.radioGpm.UseVisualStyleBackColor = true;
            // 
            // radioLpm
            // 
            this.radioLpm.AutoSize = true;
            this.radioLpm.Checked = true;
            this.radioLpm.Location = new System.Drawing.Point(140, 30);
            this.radioLpm.Name = "radioLpm";
            this.radioLpm.Size = new System.Drawing.Size(50, 20);
            this.radioLpm.TabIndex = 1;
            this.radioLpm.TabStop = true;
            this.radioLpm.Text = "lpm";
            this.radioLpm.UseVisualStyleBackColor = true;
            // 
            // lblFlowUnit
            // 
            this.lblFlowUnit.AutoSize = true;
            this.lblFlowUnit.Location = new System.Drawing.Point(6, 32);
            this.lblFlowUnit.Name = "lblFlowUnit";
            this.lblFlowUnit.Size = new System.Drawing.Size(104, 16);
            this.lblFlowUnit.TabIndex = 0;
            this.lblFlowUnit.Text = "Unidade Vazão:";
            // 
            // lblSelectReadings
            // 
            this.lblSelectReadings.AutoSize = true;
            this.lblSelectReadings.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectReadings.Location = new System.Drawing.Point(392, 161);
            this.lblSelectReadings.Name = "lblSelectReadings";
            this.lblSelectReadings.Size = new System.Drawing.Size(145, 16);
            this.lblSelectReadings.TabIndex = 2;
            this.lblSelectReadings.Text = "Selecionar Leituras:";
            // 
            // panelReadingsCheckboxes
            // 
            this.panelReadingsCheckboxes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelReadingsCheckboxes.AutoScroll = true;
            this.panelReadingsCheckboxes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelReadingsCheckboxes.Location = new System.Drawing.Point(12, 180);
            this.panelReadingsCheckboxes.Name = "panelReadingsCheckboxes";
            this.panelReadingsCheckboxes.Size = new System.Drawing.Size(901, 245);
            this.panelReadingsCheckboxes.TabIndex = 3;
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveConfig.Location = new System.Drawing.Point(818, 440);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(95, 30);
            this.btnSaveConfig.TabIndex = 4;
            this.btnSaveConfig.Text = "Salvar";
            this.btnSaveConfig.UseVisualStyleBackColor = true;
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);
            // 
            // btnCancelConfig
            // 
            this.btnCancelConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelConfig.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelConfig.Location = new System.Drawing.Point(717, 440);
            this.btnCancelConfig.Name = "btnCancelConfig";
            this.btnCancelConfig.Size = new System.Drawing.Size(95, 30);
            this.btnCancelConfig.TabIndex = 5;
            this.btnCancelConfig.Text = "Cancelar";
            this.btnCancelConfig.UseVisualStyleBackColor = true;
            this.btnCancelConfig.Click += new System.EventHandler(this.btnCancelConfig_Click);
            // 
            // btnClearSelections
            // 
            this.btnClearSelections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearSelections.Location = new System.Drawing.Point(12, 440);
            this.btnClearSelections.Name = "btnClearSelections";
            this.btnClearSelections.Size = new System.Drawing.Size(130, 30);
            this.btnClearSelections.TabIndex = 6;
            this.btnClearSelections.Text = "Limpar Seleções";
            this.btnClearSelections.UseVisualStyleBackColor = true;
            this.btnClearSelections.Click += new System.EventHandler(this.btnClearSelections_Click);
            // 
            // ConfigFormBomb
            // 
            this.AcceptButton = this.btnSaveConfig;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancelConfig;
            this.ClientSize = new System.Drawing.Size(925, 481);
            this.Controls.Add(this.btnClearSelections);
            this.Controls.Add(this.btnCancelConfig);
            this.Controls.Add(this.btnSaveConfig);
            this.Controls.Add(this.panelReadingsCheckboxes);
            this.Controls.Add(this.lblSelectReadings);
            this.Controls.Add(this.groupFlowUnit);
            this.Controls.Add(this.groupPressureUnit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigFormBomb";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configurar Leituras";
            this.groupPressureUnit.ResumeLayout(false);
            this.groupPressureUnit.PerformLayout();
            this.groupFlowUnit.ResumeLayout(false);
            this.groupFlowUnit.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
