namespace minas.teste.prototype.MVVM.View
{
    partial class RelatorioTestes
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RelatorioTestes));
            this.cuiGroupBox1 = new CuoreUI.Controls.cuiGroupBox();
            this.cuiGroupBox2 = new CuoreUI.Controls.cuiGroupBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // cuiGroupBox1
            // 
            this.cuiGroupBox1.BackColor = System.Drawing.Color.Silver;
            this.cuiGroupBox1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cuiGroupBox1.Content = "Group Box";
            this.cuiGroupBox1.Location = new System.Drawing.Point(0, 27);
            this.cuiGroupBox1.Name = "cuiGroupBox1";
            this.cuiGroupBox1.Padding = new System.Windows.Forms.Padding(4, 17, 4, 4);
            this.cuiGroupBox1.Rounding = new System.Windows.Forms.Padding(4);
            this.cuiGroupBox1.Size = new System.Drawing.Size(1193, 78);
            this.cuiGroupBox1.TabIndex = 0;
            // 
            // cuiGroupBox2
            // 
            this.cuiGroupBox2.BackColor = System.Drawing.Color.DarkGray;
            this.cuiGroupBox2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cuiGroupBox2.Content = "Group Box";
            this.cuiGroupBox2.Location = new System.Drawing.Point(0, 21);
            this.cuiGroupBox2.Name = "cuiGroupBox2";
            this.cuiGroupBox2.Padding = new System.Windows.Forms.Padding(4, 17, 4, 4);
            this.cuiGroupBox2.Rounding = new System.Windows.Forms.Padding(4);
            this.cuiGroupBox2.Size = new System.Drawing.Size(1193, 156);
            this.cuiGroupBox2.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(205, 407);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1192, 379);
            this.dataGridView1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panel1.Location = new System.Drawing.Point(12, 72);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(176, 489);
            this.panel1.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panel2.Controls.Add(this.cuiGroupBox1);
            this.panel2.Location = new System.Drawing.Point(205, 72);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1193, 132);
            this.panel2.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Black;
            this.panel3.Controls.Add(this.cuiGroupBox2);
            this.panel3.Location = new System.Drawing.Point(205, 210);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1193, 191);
            this.panel3.TabIndex = 5;
            // 
            // RelatorioTestes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1424, 843);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RelatorioTestes";
            this.Opacity = 0.25D;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private CuoreUI.Controls.cuiGroupBox cuiGroupBox1;
        private CuoreUI.Controls.cuiGroupBox cuiGroupBox2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
    }
}