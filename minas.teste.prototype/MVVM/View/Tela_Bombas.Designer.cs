using System.IO;
using System.Drawing;
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
        [System.Obsolete]
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            CodeArtEng.Gauge.Themes.ThemeColors themeColors1 = new CodeArtEng.Gauge.Themes.ThemeColors();
            CodeArtEng.Gauge.Themes.ThemeColors themeColors2 = new CodeArtEng.Gauge.Themes.ThemeColors();
            CodeArtEng.Gauge.Themes.ThemeColors themeColors3 = new CodeArtEng.Gauge.Themes.ThemeColors();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Title title1 = new System.Windows.Forms.DataVisualization.Charting.Title();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tela_Bombas));
            this.button4 = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel12 = new System.Windows.Forms.TableLayoutPanel();
            this.Btn_Grava_Temperatura = new CodeArtEng.Gauge.InputGauge.BitButton();
            this.label23 = new System.Windows.Forms.Label();
            this.sensor_Temp_C = new CodeArtEng.Gauge.PanelGauges.SegmentedDisplay();
            this.panel9 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel11 = new System.Windows.Forms.TableLayoutPanel();
            this.Btn_Grava_rotacao = new CodeArtEng.Gauge.InputGauge.BitButton();
            this.label21 = new System.Windows.Forms.Label();
            this.sensor_rotacao_RPM = new CodeArtEng.Gauge.PanelGauges.SegmentedDisplay();
            this.panel6 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel10 = new System.Windows.Forms.TableLayoutPanel();
            this.Btn_Grava_vazao = new CodeArtEng.Gauge.InputGauge.BitButton();
            this.label22 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.sensor_Vazao_BAR = new CodeArtEng.Gauge.PanelGauges.SegmentedDisplay();
            this.sensor_Vazao_PSI = new CodeArtEng.Gauge.PanelGauges.SegmentedDisplay();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tableLayoutPanel9 = new System.Windows.Forms.TableLayoutPanel();
            this.label20 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.sensor_Press_BAR = new CodeArtEng.Gauge.PanelGauges.SegmentedDisplay();
            this.sensor_Press_PSI = new CodeArtEng.Gauge.PanelGauges.SegmentedDisplay();
            this.panel7 = new System.Windows.Forms.Panel();
            this.Btn_Grava_pressao = new CodeArtEng.Gauge.InputGauge.BitButton();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.Btn_Grava_dreno = new CodeArtEng.Gauge.InputGauge.BitButton();
            this.label18 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.sensor_gpm_DR = new CodeArtEng.Gauge.PanelGauges.SegmentedDisplay();
            this.sensor_lpm_DR = new CodeArtEng.Gauge.PanelGauges.SegmentedDisplay();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cuiButton2 = new CuoreUI.Controls.cuiButton();
            this.cuiButton1 = new CuoreUI.Controls.cuiButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.cuiButton3 = new CuoreUI.Controls.cuiButton();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.Btn_Grava_pilotagem = new CodeArtEng.Gauge.InputGauge.BitButton();
            this.sensor_lpm_PL = new CodeArtEng.Gauge.PanelGauges.SegmentedDisplay();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.sensor_gpm_PL = new CodeArtEng.Gauge.PanelGauges.SegmentedDisplay();
            this.label14 = new System.Windows.Forms.Label();
            this.panel12 = new System.Windows.Forms.Panel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.labelCronometro = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.HistoricalEvents = new System.Windows.Forms.TextBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel12.SuspendLayout();
            this.tableLayoutPanel11.SuspendLayout();
            this.tableLayoutPanel10.SuspendLayout();
            this.tableLayoutPanel9.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel12.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.AutoSize = true;
            this.button4.Location = new System.Drawing.Point(7, 7);
            this.button4.Margin = new System.Windows.Forms.Padding(4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(112, 86);
            this.button4.TabIndex = 40;
            this.button4.Text = "Retornar";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.74074F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.25926F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 667F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 500F));
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel4, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel5, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.panel12, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(8, 145);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 588F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 588F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1405, 591);
            this.tableLayoutPanel2.TabIndex = 58;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel12, 1, 3);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel11, 0, 3);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel10, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.label9, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel9, 0, 1);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(737, 5);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90.9396F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 9.060403F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 233F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(646, 558);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // tableLayoutPanel12
            // 
            this.tableLayoutPanel12.ColumnCount = 3;
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48.10126F));
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 51.89874F));
            this.tableLayoutPanel12.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tableLayoutPanel12.Controls.Add(this.Btn_Grava_Temperatura, 0, 1);
            this.tableLayoutPanel12.Controls.Add(this.label23, 2, 0);
            this.tableLayoutPanel12.Controls.Add(this.sensor_Temp_C, 1, 0);
            this.tableLayoutPanel12.Controls.Add(this.panel9, 0, 0);
            this.tableLayoutPanel12.Location = new System.Drawing.Point(327, 324);
            this.tableLayoutPanel12.Name = "tableLayoutPanel12";
            this.tableLayoutPanel12.RowCount = 2;
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.98238F));
            this.tableLayoutPanel12.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.01762F));
            this.tableLayoutPanel12.Size = new System.Drawing.Size(299, 227);
            this.tableLayoutPanel12.TabIndex = 9;
            // 
            // Btn_Grava_Temperatura
            // 
            this.Btn_Grava_Temperatura.ButtonStyle = CodeArtEng.Gauge.InputGauge.BitButtonStyle.TextOnly;
            this.Btn_Grava_Temperatura.Image = null;
            this.Btn_Grava_Temperatura.Location = new System.Drawing.Point(3, 121);
            this.Btn_Grava_Temperatura.Maximum = 2147483647D;
            this.Btn_Grava_Temperatura.Name = "Btn_Grava_Temperatura";
            this.Btn_Grava_Temperatura.ResetValue = 0D;
            this.Btn_Grava_Temperatura.ScaleFactor = 1D;
            this.Btn_Grava_Temperatura.Size = new System.Drawing.Size(107, 90);
            this.Btn_Grava_Temperatura.TabIndex = 15;
            this.Btn_Grava_Temperatura.Title = "Gravar";
            this.Btn_Grava_Temperatura.Unit = "";
            this.Btn_Grava_Temperatura.UserDefinedColors.Base = themeColors1;
            this.Btn_Grava_Temperatura.UserDefinedColors.Error = themeColors2;
            this.Btn_Grava_Temperatura.UserDefinedColors.Warning = themeColors3;
            this.Btn_Grava_Temperatura.Load += new System.EventHandler(this.Btn_Grava_Temperatura_Load);
            // 
            // label23
            // 
            this.label23.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(256, 102);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(21, 16);
            this.label23.TabIndex = 14;
            this.label23.Text = "Cº";
            // 
            // sensor_Temp_C
            // 
            this.sensor_Temp_C.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensor_Temp_C.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.sensor_Temp_C.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sensor_Temp_C.Location = new System.Drawing.Point(116, 25);
            this.sensor_Temp_C.Name = "sensor_Temp_C";
            this.sensor_Temp_C.ResetValue = 0D;
            this.sensor_Temp_C.ScaleFactor = 1D;
            this.sensor_Temp_C.Size = new System.Drawing.Size(104, 68);
            this.sensor_Temp_C.TabIndex = 4;
            this.sensor_Temp_C.Theme = CodeArtEng.Gauge.GaugeTheme.Dark;
            this.sensor_Temp_C.Title = "";
            this.sensor_Temp_C.Unit = "PSI";
            this.sensor_Temp_C.Value = 0D;
            this.sensor_Temp_C.Load += new System.EventHandler(this.sensor_Temp_C_Load);
            // 
            // panel9
            // 
            this.panel9.BackgroundImage = global::minas.teste.prototype.Properties.Resources.termometro;
            this.panel9.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel9.Location = new System.Drawing.Point(3, 3);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(107, 112);
            this.panel9.TabIndex = 9;
            // 
            // tableLayoutPanel11
            // 
            this.tableLayoutPanel11.ColumnCount = 3;
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.67901F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 54.32099F));
            this.tableLayoutPanel11.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tableLayoutPanel11.Controls.Add(this.Btn_Grava_rotacao, 0, 1);
            this.tableLayoutPanel11.Controls.Add(this.label21, 2, 0);
            this.tableLayoutPanel11.Controls.Add(this.sensor_rotacao_RPM, 1, 0);
            this.tableLayoutPanel11.Controls.Add(this.panel6, 0, 0);
            this.tableLayoutPanel11.Location = new System.Drawing.Point(6, 324);
            this.tableLayoutPanel11.Name = "tableLayoutPanel11";
            this.tableLayoutPanel11.RowCount = 2;
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.19048F));
            this.tableLayoutPanel11.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.80952F));
            this.tableLayoutPanel11.Size = new System.Drawing.Size(298, 228);
            this.tableLayoutPanel11.TabIndex = 9;
            // 
            // Btn_Grava_rotacao
            // 
            this.Btn_Grava_rotacao.ButtonStyle = CodeArtEng.Gauge.InputGauge.BitButtonStyle.TextOnly;
            this.Btn_Grava_rotacao.Image = null;
            this.Btn_Grava_rotacao.Location = new System.Drawing.Point(3, 119);
            this.Btn_Grava_rotacao.Maximum = 2147483647D;
            this.Btn_Grava_rotacao.Name = "Btn_Grava_rotacao";
            this.Btn_Grava_rotacao.ResetValue = 0D;
            this.Btn_Grava_rotacao.ScaleFactor = 1D;
            this.Btn_Grava_rotacao.Size = new System.Drawing.Size(101, 92);
            this.Btn_Grava_rotacao.TabIndex = 15;
            this.Btn_Grava_rotacao.Title = "Gravar";
            this.Btn_Grava_rotacao.Unit = "";
            this.Btn_Grava_rotacao.Load += new System.EventHandler(this.Btn_Grava_rotacao_Load);
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(247, 100);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(37, 16);
            this.label21.TabIndex = 14;
            this.label21.Text = "RPM";
            // 
            // sensor_rotacao_RPM
            // 
            this.sensor_rotacao_RPM.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensor_rotacao_RPM.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.sensor_rotacao_RPM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sensor_rotacao_RPM.Location = new System.Drawing.Point(110, 25);
            this.sensor_rotacao_RPM.Name = "sensor_rotacao_RPM";
            this.sensor_rotacao_RPM.ResetValue = 0D;
            this.sensor_rotacao_RPM.ScaleFactor = 1D;
            this.sensor_rotacao_RPM.Size = new System.Drawing.Size(97, 65);
            this.sensor_rotacao_RPM.TabIndex = 4;
            this.sensor_rotacao_RPM.Theme = CodeArtEng.Gauge.GaugeTheme.Dark;
            this.sensor_rotacao_RPM.Title = "";
            this.sensor_rotacao_RPM.Unit = "PSI";
            this.sensor_rotacao_RPM.Value = 0D;
            this.sensor_rotacao_RPM.Load += new System.EventHandler(this.sensor_rotacao_RPM_Load);
            // 
            // panel6
            // 
            this.panel6.BackgroundImage = global::minas.teste.prototype.Properties.Resources._14837433_de_contorno_do_icone_da_bomba_da_industria_sistema_motor_vetor_removebg_preview;
            this.panel6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel6.Location = new System.Drawing.Point(3, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(101, 110);
            this.panel6.TabIndex = 4;
            // 
            // tableLayoutPanel10
            // 
            this.tableLayoutPanel10.ColumnCount = 3;
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.85597F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.14403F));
            this.tableLayoutPanel10.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayoutPanel10.Controls.Add(this.Btn_Grava_vazao, 0, 1);
            this.tableLayoutPanel10.Controls.Add(this.label22, 2, 1);
            this.tableLayoutPanel10.Controls.Add(this.label19, 2, 0);
            this.tableLayoutPanel10.Controls.Add(this.sensor_Vazao_BAR, 1, 1);
            this.tableLayoutPanel10.Controls.Add(this.sensor_Vazao_PSI, 1, 0);
            this.tableLayoutPanel10.Controls.Add(this.panel8, 0, 0);
            this.tableLayoutPanel10.Location = new System.Drawing.Point(327, 39);
            this.tableLayoutPanel10.Name = "tableLayoutPanel10";
            this.tableLayoutPanel10.RowCount = 3;
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.70968F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35.48387F));
            this.tableLayoutPanel10.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.40322F));
            this.tableLayoutPanel10.Size = new System.Drawing.Size(299, 248);
            this.tableLayoutPanel10.TabIndex = 11;
            // 
            // Btn_Grava_vazao
            // 
            this.Btn_Grava_vazao.ButtonStyle = CodeArtEng.Gauge.InputGauge.BitButtonStyle.TextOnly;
            this.Btn_Grava_vazao.Image = null;
            this.Btn_Grava_vazao.Location = new System.Drawing.Point(3, 99);
            this.Btn_Grava_vazao.Maximum = 2147483647D;
            this.Btn_Grava_vazao.Name = "Btn_Grava_vazao";
            this.Btn_Grava_vazao.ResetValue = 0D;
            this.Btn_Grava_vazao.ScaleFactor = 1D;
            this.Btn_Grava_vazao.Size = new System.Drawing.Size(103, 77);
            this.Btn_Grava_vazao.TabIndex = 16;
            this.Btn_Grava_vazao.Title = "Gravar";
            this.Btn_Grava_vazao.Unit = "";
            this.Btn_Grava_vazao.Load += new System.EventHandler(this.Btn_Grava_vazao_Load);
            // 
            // label22
            // 
            this.label22.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(254, 168);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(34, 16);
            this.label22.TabIndex = 15;
            this.label22.Text = "LPM";
            // 
            // label19
            // 
            this.label19.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(252, 80);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(37, 16);
            this.label19.TabIndex = 12;
            this.label19.Text = "GPM";
            // 
            // sensor_Vazao_BAR
            // 
            this.sensor_Vazao_BAR.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensor_Vazao_BAR.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.sensor_Vazao_BAR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sensor_Vazao_BAR.Location = new System.Drawing.Point(112, 106);
            this.sensor_Vazao_BAR.Name = "sensor_Vazao_BAR";
            this.sensor_Vazao_BAR.ResetValue = 0D;
            this.sensor_Vazao_BAR.ScaleFactor = 1D;
            this.sensor_Vazao_BAR.Size = new System.Drawing.Size(112, 68);
            this.sensor_Vazao_BAR.TabIndex = 11;
            this.sensor_Vazao_BAR.Theme = CodeArtEng.Gauge.GaugeTheme.Dark;
            this.sensor_Vazao_BAR.Title = "";
            this.sensor_Vazao_BAR.Unit = "PSI";
            this.sensor_Vazao_BAR.Value = 0D;
            this.sensor_Vazao_BAR.Load += new System.EventHandler(this.sensor_Vazao_BAR_Load);
            // 
            // sensor_Vazao_PSI
            // 
            this.sensor_Vazao_PSI.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensor_Vazao_PSI.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.sensor_Vazao_PSI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sensor_Vazao_PSI.Location = new System.Drawing.Point(112, 13);
            this.sensor_Vazao_PSI.Name = "sensor_Vazao_PSI";
            this.sensor_Vazao_PSI.ResetValue = 0D;
            this.sensor_Vazao_PSI.ScaleFactor = 1D;
            this.sensor_Vazao_PSI.Size = new System.Drawing.Size(112, 70);
            this.sensor_Vazao_PSI.TabIndex = 9;
            this.sensor_Vazao_PSI.Theme = CodeArtEng.Gauge.GaugeTheme.Dark;
            this.sensor_Vazao_PSI.Title = "";
            this.sensor_Vazao_PSI.Unit = "PSI";
            this.sensor_Vazao_PSI.Value = 0D;
            this.sensor_Vazao_PSI.Load += new System.EventHandler(this.sensor_Vazao_PSI_Load);
            // 
            // panel8
            // 
            this.panel8.BackgroundImage = global::minas.teste.prototype.Properties.Resources.vazãobomba_removebg_preview__1_;
            this.panel8.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel8.Location = new System.Drawing.Point(3, 3);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(103, 90);
            this.panel8.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.6F);
            this.label2.Location = new System.Drawing.Point(327, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 27);
            this.label2.TabIndex = 6;
            this.label2.Text = "Vazão";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.6F);
            this.label3.Location = new System.Drawing.Point(6, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 27);
            this.label3.TabIndex = 5;
            this.label3.Text = "Pressão";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.6F);
            this.label9.Location = new System.Drawing.Point(327, 293);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(126, 25);
            this.label9.TabIndex = 3;
            this.label9.Text = "Temperatura";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.6F);
            this.label4.Location = new System.Drawing.Point(6, 293);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 25);
            this.label4.TabIndex = 2;
            this.label4.Text = "Rotação";
            // 
            // tableLayoutPanel9
            // 
            this.tableLayoutPanel9.ColumnCount = 3;
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.71545F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.28455F));
            this.tableLayoutPanel9.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 62F));
            this.tableLayoutPanel9.Controls.Add(this.label20, 2, 1);
            this.tableLayoutPanel9.Controls.Add(this.label17, 2, 0);
            this.tableLayoutPanel9.Controls.Add(this.sensor_Press_BAR, 1, 1);
            this.tableLayoutPanel9.Controls.Add(this.sensor_Press_PSI, 1, 0);
            this.tableLayoutPanel9.Controls.Add(this.panel7, 0, 0);
            this.tableLayoutPanel9.Controls.Add(this.Btn_Grava_pressao, 0, 1);
            this.tableLayoutPanel9.Location = new System.Drawing.Point(6, 39);
            this.tableLayoutPanel9.Name = "tableLayoutPanel9";
            this.tableLayoutPanel9.RowCount = 3;
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.70968F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 36.69355F));
            this.tableLayoutPanel9.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel9.Size = new System.Drawing.Size(298, 248);
            this.tableLayoutPanel9.TabIndex = 10;
            // 
            // label20
            // 
            this.label20.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(249, 169);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(35, 16);
            this.label20.TabIndex = 13;
            this.label20.Text = "BAR";
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(252, 79);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(28, 16);
            this.label17.TabIndex = 11;
            this.label17.Text = "PSI";
            // 
            // sensor_Press_BAR
            // 
            this.sensor_Press_BAR.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensor_Press_BAR.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.sensor_Press_BAR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sensor_Press_BAR.Location = new System.Drawing.Point(108, 108);
            this.sensor_Press_BAR.Name = "sensor_Press_BAR";
            this.sensor_Press_BAR.ResetValue = 0D;
            this.sensor_Press_BAR.ScaleFactor = 1D;
            this.sensor_Press_BAR.Size = new System.Drawing.Size(106, 63);
            this.sensor_Press_BAR.TabIndex = 10;
            this.sensor_Press_BAR.Theme = CodeArtEng.Gauge.GaugeTheme.Dark;
            this.sensor_Press_BAR.Title = "";
            this.sensor_Press_BAR.Unit = "PSI";
            this.sensor_Press_BAR.Value = 0D;
            this.sensor_Press_BAR.Load += new System.EventHandler(this.sensor_Press_BAR_Load);
            // 
            // sensor_Press_PSI
            // 
            this.sensor_Press_PSI.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensor_Press_PSI.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.sensor_Press_PSI.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sensor_Press_PSI.Location = new System.Drawing.Point(108, 14);
            this.sensor_Press_PSI.Name = "sensor_Press_PSI";
            this.sensor_Press_PSI.ResetValue = 0D;
            this.sensor_Press_PSI.ScaleFactor = 1D;
            this.sensor_Press_PSI.Size = new System.Drawing.Size(106, 67);
            this.sensor_Press_PSI.TabIndex = 8;
            this.sensor_Press_PSI.Theme = CodeArtEng.Gauge.GaugeTheme.Dark;
            this.sensor_Press_PSI.Title = "";
            this.sensor_Press_PSI.Unit = "PSI";
            this.sensor_Press_PSI.Value = 0D;
            this.sensor_Press_PSI.Load += new System.EventHandler(this.sensor_Press_PSI_Load);
            // 
            // panel7
            // 
            this.panel7.BackgroundImage = global::minas.teste.prototype.Properties.Resources._14988722_de_contorno_do_icone_do_manometro_de_pressao_manometro_vetor_removebg_preview;
            this.panel7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel7.Location = new System.Drawing.Point(3, 3);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(99, 89);
            this.panel7.TabIndex = 7;
            // 
            // Btn_Grava_pressao
            // 
            this.Btn_Grava_pressao.ButtonStyle = CodeArtEng.Gauge.InputGauge.BitButtonStyle.TextOnly;
            this.Btn_Grava_pressao.Image = null;
            this.Btn_Grava_pressao.Location = new System.Drawing.Point(3, 98);
            this.Btn_Grava_pressao.Maximum = 2147483647D;
            this.Btn_Grava_pressao.Name = "Btn_Grava_pressao";
            this.Btn_Grava_pressao.ResetValue = 0D;
            this.Btn_Grava_pressao.ScaleFactor = 1D;
            this.Btn_Grava_pressao.Size = new System.Drawing.Size(99, 77);
            this.Btn_Grava_pressao.TabIndex = 14;
            this.Btn_Grava_pressao.Title = "Gravar";
            this.Btn_Grava_pressao.Unit = "";
            this.Btn_Grava_pressao.Load += new System.EventHandler(this.Btn_Grava_pressao_Load);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel8, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.label10, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.label11, 0, 2);
            this.tableLayoutPanel5.Controls.Add(this.label12, 0, 4);
            this.tableLayoutPanel5.Controls.Add(this.panel2, 0, 5);
            this.tableLayoutPanel5.Controls.Add(this.panel3, 0, 6);
            this.tableLayoutPanel5.Controls.Add(this.tableLayoutPanel7, 0, 1);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(6, 5);
            this.tableLayoutPanel5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 8;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 24F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 82.82832F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 21F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 168F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 17.17167F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 72F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 57F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 11F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(260, 558);
            this.tableLayoutPanel5.TabIndex = 1;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 3;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 42.51208F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 57.48792F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.tableLayoutPanel8.Controls.Add(this.Btn_Grava_dreno, 0, 1);
            this.tableLayoutPanel8.Controls.Add(this.label18, 2, 1);
            this.tableLayoutPanel8.Controls.Add(this.label16, 2, 0);
            this.tableLayoutPanel8.Controls.Add(this.sensor_gpm_DR, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.sensor_lpm_DR, 1, 1);
            this.tableLayoutPanel8.Controls.Add(this.panel5, 0, 0);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(6, 207);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 2;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.19048F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.80952F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(248, 162);
            this.tableLayoutPanel8.TabIndex = 8;
            // 
            // Btn_Grava_dreno
            // 
            this.Btn_Grava_dreno.ButtonStyle = CodeArtEng.Gauge.InputGauge.BitButtonStyle.TextOnly;
            this.Btn_Grava_dreno.Image = null;
            this.Btn_Grava_dreno.Location = new System.Drawing.Point(3, 85);
            this.Btn_Grava_dreno.Maximum = 2147483647D;
            this.Btn_Grava_dreno.Name = "Btn_Grava_dreno";
            this.Btn_Grava_dreno.ResetValue = 0D;
            this.Btn_Grava_dreno.ScaleFactor = 1D;
            this.Btn_Grava_dreno.Size = new System.Drawing.Size(67, 70);
            this.Btn_Grava_dreno.TabIndex = 11;
            this.Btn_Grava_dreno.Title = "Gravar";
            this.Btn_Grava_dreno.Unit = "";
            this.Btn_Grava_dreno.Load += new System.EventHandler(this.Btn_Grava_dreno_Load);
            // 
            // label18
            // 
            this.label18.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(193, 146);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(34, 16);
            this.label18.TabIndex = 9;
            this.label18.Text = "LPM";
            // 
            // label16
            // 
            this.label16.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(191, 66);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(37, 16);
            this.label16.TabIndex = 5;
            this.label16.Text = "GPM";
            // 
            // sensor_gpm_DR
            // 
            this.sensor_gpm_DR.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensor_gpm_DR.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.sensor_gpm_DR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sensor_gpm_DR.Location = new System.Drawing.Point(76, 8);
            this.sensor_gpm_DR.Name = "sensor_gpm_DR";
            this.sensor_gpm_DR.ResetValue = 0D;
            this.sensor_gpm_DR.ScaleFactor = 1D;
            this.sensor_gpm_DR.Size = new System.Drawing.Size(93, 65);
            this.sensor_gpm_DR.TabIndex = 4;
            this.sensor_gpm_DR.Theme = CodeArtEng.Gauge.GaugeTheme.Dark;
            this.sensor_gpm_DR.Title = "";
            this.sensor_gpm_DR.Unit = "PSI";
            this.sensor_gpm_DR.Value = 0D;
            this.sensor_gpm_DR.Load += new System.EventHandler(this.sensor_gpm_DR_Load);
            // 
            // sensor_lpm_DR
            // 
            this.sensor_lpm_DR.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensor_lpm_DR.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.sensor_lpm_DR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sensor_lpm_DR.Location = new System.Drawing.Point(76, 90);
            this.sensor_lpm_DR.Name = "sensor_lpm_DR";
            this.sensor_lpm_DR.ResetValue = 0D;
            this.sensor_lpm_DR.ScaleFactor = 1D;
            this.sensor_lpm_DR.Size = new System.Drawing.Size(93, 64);
            this.sensor_lpm_DR.TabIndex = 3;
            this.sensor_lpm_DR.Theme = CodeArtEng.Gauge.GaugeTheme.Dark;
            this.sensor_lpm_DR.Title = "";
            this.sensor_lpm_DR.Unit = "BAR";
            this.sensor_lpm_DR.Value = 0D;
            this.sensor_lpm_DR.Load += new System.EventHandler(this.sensor_lpm_DR_Load);
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = global::minas.teste.prototype.Properties.Resources._10311715_ilustracao_contorno_icone_icone_de_agua_bomba_de_agua_vetor_removebg_preview;
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel5.Controls.Add(this.panel11);
            this.panel5.Location = new System.Drawing.Point(3, 3);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(67, 76);
            this.panel5.TabIndex = 2;
            // 
            // panel11
            // 
            this.panel11.Location = new System.Drawing.Point(98, 69);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(284, 154);
            this.panel11.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.6F);
            this.label10.Location = new System.Drawing.Point(81, 3);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(98, 24);
            this.label10.TabIndex = 7;
            this.label10.Text = "Pilotagem";
            // 
            // label11
            // 
            this.label11.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.6F);
            this.label11.Location = new System.Drawing.Point(97, 180);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(65, 21);
            this.label11.TabIndex = 4;
            this.label11.Text = "Dreno";
            // 
            // label12
            // 
            this.label12.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.6F);
            this.label12.Location = new System.Drawing.Point(82, 375);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(96, 25);
            this.label12.TabIndex = 3;
            this.label12.Text = "Controles";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cuiButton2);
            this.panel2.Controls.Add(this.cuiButton1);
            this.panel2.Location = new System.Drawing.Point(7, 412);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(246, 62);
            this.panel2.TabIndex = 2;
            // 
            // cuiButton2
            // 
            this.cuiButton2.CheckButton = false;
            this.cuiButton2.Checked = false;
            this.cuiButton2.CheckedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton2.CheckedForeColor = System.Drawing.Color.White;
            this.cuiButton2.CheckedImageTint = System.Drawing.Color.White;
            this.cuiButton2.CheckedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton2.Content = "Finalizar";
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
            this.cuiButton2.Location = new System.Drawing.Point(131, 4);
            this.cuiButton2.Margin = new System.Windows.Forms.Padding(4);
            this.cuiButton2.Name = "cuiButton2";
            this.cuiButton2.NormalBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton2.NormalOutline = System.Drawing.Color.Empty;
            this.cuiButton2.OutlineThickness = 1.6F;
            this.cuiButton2.PressedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton2.PressedForeColor = System.Drawing.Color.White;
            this.cuiButton2.PressedImageTint = System.Drawing.Color.White;
            this.cuiButton2.PressedOutline = System.Drawing.Color.Empty;
            this.cuiButton2.Rounding = new System.Windows.Forms.Padding(8);
            this.cuiButton2.Size = new System.Drawing.Size(112, 48);
            this.cuiButton2.TabIndex = 3;
            this.cuiButton2.TextOffset = new System.Drawing.Point(0, 0);
            this.cuiButton2.Click += new System.EventHandler(this.btnParar_Click);
            // 
            // cuiButton1
            // 
            this.cuiButton1.CheckButton = false;
            this.cuiButton1.Checked = false;
            this.cuiButton1.CheckedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton1.CheckedForeColor = System.Drawing.Color.White;
            this.cuiButton1.CheckedImageTint = System.Drawing.Color.White;
            this.cuiButton1.CheckedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton1.Content = "Iniciar";
            this.cuiButton1.DialogResult = System.Windows.Forms.DialogResult.None;
            this.cuiButton1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cuiButton1.ForeColor = System.Drawing.Color.White;
            this.cuiButton1.HoverBackground = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton1.HoveredImageTint = System.Drawing.Color.White;
            this.cuiButton1.HoverForeColor = System.Drawing.Color.White;
            this.cuiButton1.HoverOutline = System.Drawing.Color.Empty;
            this.cuiButton1.Image = null;
            this.cuiButton1.ImageAutoCenter = true;
            this.cuiButton1.ImageExpand = new System.Drawing.Point(0, 0);
            this.cuiButton1.ImageOffset = new System.Drawing.Point(0, 0);
            this.cuiButton1.ImageTint = System.Drawing.Color.White;
            this.cuiButton1.Location = new System.Drawing.Point(4, 4);
            this.cuiButton1.Margin = new System.Windows.Forms.Padding(4);
            this.cuiButton1.Name = "cuiButton1";
            this.cuiButton1.NormalBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton1.NormalOutline = System.Drawing.Color.Empty;
            this.cuiButton1.OutlineThickness = 1.6F;
            this.cuiButton1.PressedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton1.PressedForeColor = System.Drawing.Color.White;
            this.cuiButton1.PressedImageTint = System.Drawing.Color.White;
            this.cuiButton1.PressedOutline = System.Drawing.Color.Empty;
            this.cuiButton1.Rounding = new System.Windows.Forms.Padding(8);
            this.cuiButton1.Size = new System.Drawing.Size(112, 45);
            this.cuiButton1.TabIndex = 2;
            this.cuiButton1.TextOffset = new System.Drawing.Point(0, 0);
            this.cuiButton1.Click += new System.EventHandler(this.btnIniciar_Click);
            // 
            // panel3
            // 
            this.panel3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.panel3.Controls.Add(this.cuiButton3);
            this.panel3.Location = new System.Drawing.Point(49, 487);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(161, 49);
            this.panel3.TabIndex = 2;
            // 
            // cuiButton3
            // 
            this.cuiButton3.CheckButton = false;
            this.cuiButton3.Checked = false;
            this.cuiButton3.CheckedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton3.CheckedForeColor = System.Drawing.Color.White;
            this.cuiButton3.CheckedImageTint = System.Drawing.Color.White;
            this.cuiButton3.CheckedOutline = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton3.Content = "Relátorio";
            this.cuiButton3.DialogResult = System.Windows.Forms.DialogResult.None;
            this.cuiButton3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.cuiButton3.ForeColor = System.Drawing.Color.White;
            this.cuiButton3.HoverBackground = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton3.HoveredImageTint = System.Drawing.Color.White;
            this.cuiButton3.HoverForeColor = System.Drawing.Color.White;
            this.cuiButton3.HoverOutline = System.Drawing.Color.Empty;
            this.cuiButton3.Image = null;
            this.cuiButton3.ImageAutoCenter = true;
            this.cuiButton3.ImageExpand = new System.Drawing.Point(0, 0);
            this.cuiButton3.ImageOffset = new System.Drawing.Point(0, 0);
            this.cuiButton3.ImageTint = System.Drawing.Color.White;
            this.cuiButton3.Location = new System.Drawing.Point(4, 2);
            this.cuiButton3.Margin = new System.Windows.Forms.Padding(4);
            this.cuiButton3.Name = "cuiButton3";
            this.cuiButton3.NormalBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton3.NormalOutline = System.Drawing.Color.Empty;
            this.cuiButton3.OutlineThickness = 1.6F;
            this.cuiButton3.PressedBackground = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(106)))), ((int)(((byte)(0)))));
            this.cuiButton3.PressedForeColor = System.Drawing.Color.White;
            this.cuiButton3.PressedImageTint = System.Drawing.Color.White;
            this.cuiButton3.PressedOutline = System.Drawing.Color.Empty;
            this.cuiButton3.Rounding = new System.Windows.Forms.Padding(8);
            this.cuiButton3.Size = new System.Drawing.Size(153, 43);
            this.cuiButton3.TabIndex = 4;
            this.cuiButton3.TextOffset = new System.Drawing.Point(0, 0);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 3;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 41.19718F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 58.80282F));
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.tableLayoutPanel7.Controls.Add(this.Btn_Grava_pilotagem, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.sensor_lpm_PL, 1, 1);
            this.tableLayoutPanel7.Controls.Add(this.panel4, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.label15, 2, 1);
            this.tableLayoutPanel7.Controls.Add(this.sensor_gpm_PL, 1, 0);
            this.tableLayoutPanel7.Controls.Add(this.label14, 2, 0);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(6, 33);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 2;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 51.2987F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 48.7013F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(248, 141);
            this.tableLayoutPanel7.TabIndex = 6;
            // 
            // Btn_Grava_pilotagem
            // 
            this.Btn_Grava_pilotagem.ButtonStyle = CodeArtEng.Gauge.InputGauge.BitButtonStyle.TextOnly;
            this.Btn_Grava_pilotagem.Image = null;
            this.Btn_Grava_pilotagem.Location = new System.Drawing.Point(3, 75);
            this.Btn_Grava_pilotagem.Maximum = 2147483647D;
            this.Btn_Grava_pilotagem.Name = "Btn_Grava_pilotagem";
            this.Btn_Grava_pilotagem.ResetValue = 0D;
            this.Btn_Grava_pilotagem.ScaleFactor = 1D;
            this.Btn_Grava_pilotagem.Size = new System.Drawing.Size(64, 63);
            this.Btn_Grava_pilotagem.TabIndex = 12;
            this.Btn_Grava_pilotagem.Title = "Gravar";
            this.Btn_Grava_pilotagem.Unit = "";
            this.Btn_Grava_pilotagem.Load += new System.EventHandler(this.Btn_Grava_pilotagem_Load);
            // 
            // sensor_lpm_PL
            // 
            this.sensor_lpm_PL.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensor_lpm_PL.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.sensor_lpm_PL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sensor_lpm_PL.Location = new System.Drawing.Point(73, 75);
            this.sensor_lpm_PL.Name = "sensor_lpm_PL";
            this.sensor_lpm_PL.ResetValue = 0D;
            this.sensor_lpm_PL.ScaleFactor = 1D;
            this.sensor_lpm_PL.Size = new System.Drawing.Size(94, 63);
            this.sensor_lpm_PL.TabIndex = 5;
            this.sensor_lpm_PL.Theme = CodeArtEng.Gauge.GaugeTheme.Dark;
            this.sensor_lpm_PL.Title = "";
            this.sensor_lpm_PL.Unit = "BAR";
            this.sensor_lpm_PL.Value = 0D;
            this.sensor_lpm_PL.Load += new System.EventHandler(this.sensor_lpm_PL_Load);
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = global::minas.teste.prototype.Properties.Resources.pilotagembomba_removebg_preview__1_;
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Controls.Add(this.panel10);
            this.panel4.Location = new System.Drawing.Point(3, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(64, 66);
            this.panel4.TabIndex = 2;
            // 
            // panel10
            // 
            this.panel10.Location = new System.Drawing.Point(98, 69);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(284, 154);
            this.panel10.TabIndex = 0;
            // 
            // label15
            // 
            this.label15.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(191, 125);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 16);
            this.label15.TabIndex = 6;
            this.label15.Text = "BAR";
            // 
            // sensor_gpm_PL
            // 
            this.sensor_gpm_PL.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.sensor_gpm_PL.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.sensor_gpm_PL.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.sensor_gpm_PL.Location = new System.Drawing.Point(73, 3);
            this.sensor_gpm_PL.Name = "sensor_gpm_PL";
            this.sensor_gpm_PL.ResetValue = 0D;
            this.sensor_gpm_PL.ScaleFactor = 1D;
            this.sensor_gpm_PL.Size = new System.Drawing.Size(94, 66);
            this.sensor_gpm_PL.TabIndex = 2;
            this.sensor_gpm_PL.Theme = CodeArtEng.Gauge.GaugeTheme.Dark;
            this.sensor_gpm_PL.Title = "";
            this.sensor_gpm_PL.Unit = "PSI";
            this.sensor_gpm_PL.Value = 0D;
            this.sensor_gpm_PL.Load += new System.EventHandler(this.sensor_gpm_PL_Load);
            // 
            // label14
            // 
            this.label14.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(195, 56);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(28, 16);
            this.label14.TabIndex = 0;
            this.label14.Text = "PSI";
            // 
            // panel12
            // 
            this.panel12.Controls.Add(this.flowLayoutPanel1);
            this.panel12.Controls.Add(this.chart1);
            this.panel12.Location = new System.Drawing.Point(275, 6);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(453, 556);
            this.panel12.TabIndex = 2;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.dataGridView1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(5, 266);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(460, 284);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(467, 0);
            this.dataGridView1.TabIndex = 0;
            // 
            // chart1
            // 
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Location = new System.Drawing.Point(5, 3);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series1.Legend = "Legend1";
            series1.LegendText = "vazão";
            series1.Name = "Series1";
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Spline;
            series2.Legend = "Legend1";
            series2.LegendText = "Pressão ";
            series2.Name = "Series6";
            this.chart1.Series.Add(series1);
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(478, 257);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            title1.Name = "Vazão X Pressão";
            this.chart1.Titles.Add(title1);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 128F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 486F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 309F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 344F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.labelCronometro, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.button4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label13, 3, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(8, 38);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 97F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1405, 100);
            this.tableLayoutPanel1.TabIndex = 59;
            // 
            // labelCronometro
            // 
            this.labelCronometro.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelCronometro.AutoSize = true;
            this.labelCronometro.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.6F);
            this.labelCronometro.Location = new System.Drawing.Point(1230, 34);
            this.labelCronometro.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelCronometro.Name = "labelCronometro";
            this.labelCronometro.Size = new System.Drawing.Size(0, 32);
            this.labelCronometro.TabIndex = 43;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.BackgroundImage = global::minas.teste.prototype.Properties.Resources.off;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(145, 8);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(90, 84);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox1.Location = new System.Drawing.Point(260, 7);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.textBox1.Size = new System.Drawing.Size(479, 86);
            this.textBox1.TabIndex = 41;
            // 
            // label13
            // 
            this.label13.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.6F);
            this.label13.Location = new System.Drawing.Point(900, 34);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(0, 32);
            this.label13.TabIndex = 42;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel6.Location = new System.Drawing.Point(845, 3);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 2;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.45045F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 49.54955F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(206, 92);
            this.tableLayoutPanel6.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.label1.AutoSize = true;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(51, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 34);
            this.label1.TabIndex = 43;
            this.label1.Text = "lStatus";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 5;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 134F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 486F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 346F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 155F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 485F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 299F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 283F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Controls.Add(this.label5, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.label6, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.label7, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.label8, 1, 0);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(8, 14);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1405, 20);
            this.tableLayoutPanel3.TabIndex = 60;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Top;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(1062, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(340, 15);
            this.label5.TabIndex = 44;
            this.label5.Text = "TESTE";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Top;
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(748, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(308, 15);
            this.label6.TabIndex = 43;
            this.label6.Text = "DATA e HORA";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Top;
            this.label7.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(262, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(480, 15);
            this.label7.TabIndex = 42;
            this.label7.Text = "NOME  TESTE";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Top;
            this.label8.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label8.Font = new System.Drawing.Font("Times New Roman", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(128, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(128, 15);
            this.label8.TabIndex = 41;
            this.label8.Text = "STATUS GERAL";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.HistoricalEvents);
            this.panel1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.panel1.Location = new System.Drawing.Point(8, 741);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1405, 88);
            this.panel1.TabIndex = 61;
            // 
            // HistoricalEvents
            // 
            this.HistoricalEvents.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.HistoricalEvents.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.HistoricalEvents.BackColor = System.Drawing.SystemColors.InfoText;
            this.HistoricalEvents.Location = new System.Drawing.Point(6, 3);
            this.HistoricalEvents.Multiline = true;
            this.HistoricalEvents.Name = "HistoricalEvents";
            this.HistoricalEvents.Size = new System.Drawing.Size(1389, 80);
            this.HistoricalEvents.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 46);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(172, 122);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 46);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(172, 122);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // Tela_Bombas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.DarkGray;
            this.ClientSize = new System.Drawing.Size(1424, 843);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Tela_Bombas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Tela_Bombas_FormClosing);
            this.Load += new System.EventHandler(this.Tela_Bombas_Load);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            this.tableLayoutPanel12.ResumeLayout(false);
            this.tableLayoutPanel12.PerformLayout();
            this.tableLayoutPanel11.ResumeLayout(false);
            this.tableLayoutPanel11.PerformLayout();
            this.tableLayoutPanel10.ResumeLayout(false);
            this.tableLayoutPanel10.PerformLayout();
            this.tableLayoutPanel9.ResumeLayout(false);
            this.tableLayoutPanel9.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel8.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel12.ResumeLayout(false);
            this.flowLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button button4;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox1;

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
       
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label11;
        private CuoreUI.Controls.cuiButton cuiButton2;
        private CuoreUI.Controls.cuiButton cuiButton1;
        private System.Windows.Forms.Panel panel3;
        private CuoreUI.Controls.cuiButton cuiButton3;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel7;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Label label10;
        private CodeArtEng.Gauge.PanelGauges.SegmentedDisplay sensor_gpm_PL;
        private CodeArtEng.Gauge.PanelGauges.SegmentedDisplay sensor_lpm_DR;
        private CodeArtEng.Gauge.PanelGauges.SegmentedDisplay sensor_gpm_DR;
        private CodeArtEng.Gauge.PanelGauges.SegmentedDisplay sensor_lpm_PL;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel10;
        private CodeArtEng.Gauge.PanelGauges.SegmentedDisplay sensor_Vazao_BAR;
        private CodeArtEng.Gauge.PanelGauges.SegmentedDisplay sensor_Vazao_PSI;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel9;
        private CodeArtEng.Gauge.PanelGauges.SegmentedDisplay sensor_Press_BAR;
        private CodeArtEng.Gauge.PanelGauges.SegmentedDisplay sensor_Press_PSI;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel12;
        private CodeArtEng.Gauge.PanelGauges.SegmentedDisplay sensor_Temp_C;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel11;
        private CodeArtEng.Gauge.PanelGauges.SegmentedDisplay sensor_rotacao_RPM;
        private System.Windows.Forms.Panel panel12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private CodeArtEng.Gauge.InputGauge.BitButton Btn_Grava_Temperatura;
        private CodeArtEng.Gauge.InputGauge.BitButton Btn_Grava_rotacao;
        private CodeArtEng.Gauge.InputGauge.BitButton Btn_Grava_vazao;
        private CodeArtEng.Gauge.InputGauge.BitButton Btn_Grava_pressao;
        private CodeArtEng.Gauge.InputGauge.BitButton Btn_Grava_dreno;
        private CodeArtEng.Gauge.InputGauge.BitButton Btn_Grava_pilotagem;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox HistoricalEvents;
        private System.Windows.Forms.Label labelCronometro;
    }
}