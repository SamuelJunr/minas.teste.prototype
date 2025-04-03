namespace minas.teste.prototype
{
    partial class Tela_Bombas
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
            System.Windows.Forms.DataVisualization.Charting.LineAnnotation lineAnnotation1 = new System.Windows.Forms.DataVisualization.Charting.LineAnnotation();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tela_Bombas));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.gaugeData1 = new CodeArtEng.Gauge.GaugeData();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.aGauge1 = new System.Windows.Forms.AGauge();
            this.aGauge2 = new System.Windows.Forms.AGauge();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox12 = new System.Windows.Forms.TextBox();
            this.textBox13 = new System.Windows.Forms.TextBox();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.LightSlateGray;
            this.tableLayoutPanel1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.chart1, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 251);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 386F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(522, 386);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // chart1
            // 
            lineAnnotation1.ClipToChartArea = "ChartArea1";
            lineAnnotation1.Name = "LineAnnotation1";
            this.chart1.Annotations.Add(lineAnnotation1);
            this.chart1.BackColor = System.Drawing.Color.SlateGray;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(3, 3);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Legend = "Legend1";
            series2.Name = "Series2";
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Legend = "Legend1";
            series3.Name = "Series3";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Series.Add(series3);
            this.chart1.Size = new System.Drawing.Size(516, 380);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // textBox1
            // 
            this.textBox1.AutoCompleteCustomSource.AddRange(new string[] {
            "Psi\'s"});
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(72, 23);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(69, 20);
            this.textBox1.TabIndex = 20;
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Location = new System.Drawing.Point(279, 23);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(69, 20);
            this.textBox2.TabIndex = 21;
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Location = new System.Drawing.Point(159, 19);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(69, 20);
            this.textBox3.TabIndex = 23;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(71, 19);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(69, 20);
            this.textBox5.TabIndex = 25;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(278, 19);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(69, 20);
            this.textBox6.TabIndex = 26;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(70, 19);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(69, 20);
            this.textBox7.TabIndex = 27;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(277, 19);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(69, 20);
            this.textBox8.TabIndex = 28;
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(70, 14);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(69, 20);
            this.textBox9.TabIndex = 29;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(277, 14);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(69, 20);
            this.textBox10.TabIndex = 30;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(127, 401);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 44);
            this.button1.TabIndex = 31;
            this.button1.Text = "Gravar";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(238, 401);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 44);
            this.button2.TabIndex = 32;
            this.button2.Text = "Reset";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(345, 401);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(101, 44);
            this.button3.TabIndex = 33;
            this.button3.Text = "Relátorio";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Location = new System.Drawing.Point(42, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(379, 69);
            this.groupBox1.TabIndex = 34;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pressão";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBox3);
            this.groupBox2.Location = new System.Drawing.Point(43, 89);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(377, 48);
            this.groupBox2.TabIndex = 35;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Rotação";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox11);
            this.groupBox3.Location = new System.Drawing.Point(44, 147);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(377, 48);
            this.groupBox3.TabIndex = 36;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Temperatura";
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(158, 19);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(69, 20);
            this.textBox11.TabIndex = 23;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textBox6);
            this.groupBox4.Controls.Add(this.textBox5);
            this.groupBox4.Location = new System.Drawing.Point(43, 204);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(378, 58);
            this.groupBox4.TabIndex = 37;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Pilotagem";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBox8);
            this.groupBox5.Controls.Add(this.textBox7);
            this.groupBox5.Location = new System.Drawing.Point(44, 269);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(376, 59);
            this.groupBox5.TabIndex = 38;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Dreno";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.textBox9);
            this.groupBox6.Controls.Add(this.textBox10);
            this.groupBox6.Location = new System.Drawing.Point(44, 334);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(377, 49);
            this.groupBox6.TabIndex = 39;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Vazão";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 12);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(108, 44);
            this.button4.TabIndex = 40;
            this.button4.Text = "Retornar";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(20, 401);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(101, 44);
            this.button5.TabIndex = 41;
            this.button5.Text = "Teste";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.groupBox6);
            this.panel1.Controls.Add(this.groupBox5);
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Location = new System.Drawing.Point(566, 193);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(453, 452);
            this.panel1.TabIndex = 43;
            // 
            // gaugeData1
            // 
            this.gaugeData1.ErrorLimit = double.NaN;
            this.gaugeData1.InvertLimit = false;
            this.gaugeData1.Maximum = 100D;
            this.gaugeData1.Minimum = 0D;
            this.gaugeData1.Name = "gaugeData1";
            this.gaugeData1.Unit = "";
            this.gaugeData1.Value = 0D;
            this.gaugeData1.WarningLimit = double.NaN;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(663, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 15);
            this.label1.TabIndex = 48;
            this.label1.Text = "PSI";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(927, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 15);
            this.label2.TabIndex = 49;
            this.label2.Text = "BAR";
            // 
            // aGauge1
            // 
            this.aGauge1.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge1.BaseArcRadius = 80;
            this.aGauge1.BaseArcStart = 135;
            this.aGauge1.BaseArcSweep = 250;
            this.aGauge1.BaseArcWidth = 3;
            this.aGauge1.Center = new System.Drawing.Point(150, 120);
            this.aGauge1.Location = new System.Drawing.Point(519, 1);
            this.aGauge1.MaxValue = 700F;
            this.aGauge1.MinValue = 0F;
            this.aGauge1.Name = "aGauge1";
            this.aGauge1.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Red;
            this.aGauge1.NeedleColor2 = System.Drawing.Color.Black;
            this.aGauge1.NeedleRadius = 80;
            this.aGauge1.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.aGauge1.NeedleWidth = 3;
            this.aGauge1.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.aGauge1.ScaleLinesInterInnerRadius = 70;
            this.aGauge1.ScaleLinesInterOuterRadius = 80;
            this.aGauge1.ScaleLinesInterWidth = 1;
            this.aGauge1.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.aGauge1.ScaleLinesMajorInnerRadius = 70;
            this.aGauge1.ScaleLinesMajorOuterRadius = 80;
            this.aGauge1.ScaleLinesMajorStepValue = 50F;
            this.aGauge1.ScaleLinesMajorWidth = 2;
            this.aGauge1.ScaleLinesMinorColor = System.Drawing.Color.IndianRed;
            this.aGauge1.ScaleLinesMinorInnerRadius = 75;
            this.aGauge1.ScaleLinesMinorOuterRadius = 80;
            this.aGauge1.ScaleLinesMinorTicks = 9;
            this.aGauge1.ScaleLinesMinorWidth = 2;
            this.aGauge1.ScaleNumbersColor = System.Drawing.Color.Black;
            this.aGauge1.ScaleNumbersFormat = null;
            this.aGauge1.ScaleNumbersRadius = 100;
            this.aGauge1.ScaleNumbersRotation = 0;
            this.aGauge1.ScaleNumbersStartScaleLine = 0;
            this.aGauge1.ScaleNumbersStepScaleLines = 1;
            this.aGauge1.Size = new System.Drawing.Size(277, 186);
            this.aGauge1.TabIndex = 47;
            this.aGauge1.Text = "aGauge1";
            this.aGauge1.Value = 0F;
            // 
            // aGauge2
            // 
            this.aGauge2.BaseArcColor = System.Drawing.Color.Gray;
            this.aGauge2.BaseArcRadius = 80;
            this.aGauge2.BaseArcStart = 135;
            this.aGauge2.BaseArcSweep = 250;
            this.aGauge2.BaseArcWidth = 3;
            this.aGauge2.Center = new System.Drawing.Point(150, 120);
            this.aGauge2.Location = new System.Drawing.Point(783, 1);
            this.aGauge2.MaxValue = 700F;
            this.aGauge2.MinValue = 0F;
            this.aGauge2.Name = "aGauge2";
            this.aGauge2.NeedleColor1 = System.Windows.Forms.AGaugeNeedleColor.Red;
            this.aGauge2.NeedleColor2 = System.Drawing.Color.Black;
            this.aGauge2.NeedleRadius = 80;
            this.aGauge2.NeedleType = System.Windows.Forms.NeedleType.Advance;
            this.aGauge2.NeedleWidth = 3;
            this.aGauge2.ScaleLinesInterColor = System.Drawing.Color.Black;
            this.aGauge2.ScaleLinesInterInnerRadius = 70;
            this.aGauge2.ScaleLinesInterOuterRadius = 80;
            this.aGauge2.ScaleLinesInterWidth = 1;
            this.aGauge2.ScaleLinesMajorColor = System.Drawing.Color.Black;
            this.aGauge2.ScaleLinesMajorInnerRadius = 70;
            this.aGauge2.ScaleLinesMajorOuterRadius = 80;
            this.aGauge2.ScaleLinesMajorStepValue = 50F;
            this.aGauge2.ScaleLinesMajorWidth = 2;
            this.aGauge2.ScaleLinesMinorColor = System.Drawing.Color.IndianRed;
            this.aGauge2.ScaleLinesMinorInnerRadius = 75;
            this.aGauge2.ScaleLinesMinorOuterRadius = 80;
            this.aGauge2.ScaleLinesMinorTicks = 9;
            this.aGauge2.ScaleLinesMinorWidth = 2;
            this.aGauge2.ScaleNumbersColor = System.Drawing.Color.Black;
            this.aGauge2.ScaleNumbersFormat = null;
            this.aGauge2.ScaleNumbersRadius = 100;
            this.aGauge2.ScaleNumbersRotation = 0;
            this.aGauge2.ScaleNumbersStartScaleLine = 0;
            this.aGauge2.ScaleNumbersStepScaleLines = 1;
            this.aGauge2.Size = new System.Drawing.Size(277, 186);
            this.aGauge2.TabIndex = 46;
            this.aGauge2.Text = "aGauge2";
            this.aGauge2.Value = 1F;
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // textBox4
            // 
            this.textBox4.ImeMode = System.Windows.Forms.ImeMode.On;
            this.textBox4.Location = new System.Drawing.Point(15, 102);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(196, 20);
            this.textBox4.TabIndex = 50;
            // 
            // textBox12
            // 
            this.textBox12.ImeMode = System.Windows.Forms.ImeMode.On;
            this.textBox12.Location = new System.Drawing.Point(15, 167);
            this.textBox12.Name = "textBox12";
            this.textBox12.Size = new System.Drawing.Size(196, 20);
            this.textBox12.TabIndex = 51;
            // 
            // textBox13
            // 
            this.textBox13.ImeMode = System.Windows.Forms.ImeMode.On;
            this.textBox13.Location = new System.Drawing.Point(270, 102);
            this.textBox13.Name = "textBox13";
            this.textBox13.Size = new System.Drawing.Size(196, 20);
            this.textBox13.TabIndex = 52;
            // 
            // textBox14
            // 
            this.textBox14.ImeMode = System.Windows.Forms.ImeMode.On;
            this.textBox14.Location = new System.Drawing.Point(270, 167);
            this.textBox14.Name = "textBox14";
            this.textBox14.Size = new System.Drawing.Size(196, 20);
            this.textBox14.TabIndex = 53;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(267, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 15);
            this.label3.TabIndex = 54;
            this.label3.Text = "Ordem de Serviço";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(267, 138);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(68, 15);
            this.label4.TabIndex = 55;
            this.label4.Text = "Nota Fiscal";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 138);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 15);
            this.label5.TabIndex = 56;
            this.label5.Text = "Responsável";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(57, 15);
            this.label6.TabIndex = 57;
            this.label6.Text = "Empresa";
            // 
            // Tela_Bombas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(1058, 649);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox14);
            this.Controls.Add(this.textBox13);
            this.Controls.Add(this.textBox12);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.aGauge1);
            this.Controls.Add(this.aGauge2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Tela_Bombas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Tela_Bombas_FormClosing);
            this.Load += new System.EventHandler(this.Tela_Bombas_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Panel panel1;
        private CodeArtEng.Gauge.GaugeData gaugeData1;
        private System.Windows.Forms.AGauge aGauge2;
        private System.Windows.Forms.AGauge aGauge1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox12;
        private System.Windows.Forms.TextBox textBox13;
        private System.Windows.Forms.TextBox textBox14;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}