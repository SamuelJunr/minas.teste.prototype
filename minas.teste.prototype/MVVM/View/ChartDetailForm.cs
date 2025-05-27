using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.ComponentModel;

// Ensure this namespace matches your project structure
namespace minas.teste.prototype.MVVM.View
{
    public partial class ChartDetailForm : Form
    {
        private Chart chartControlModal;
        private IContainer componentsField = null; // Required for icon loading via ComponentResourceManager

        public ChartDetailForm(SeriesCollection originalSeries, string title, ChartArea originalChartArea, Icon formIcon)
        {
            InitializeComponentLocal(); // Call the local InitializeComponent

            this.Text = title;
            if (formIcon != null)
            {
                this.Icon = formIcon;
            }
            else
            {
                // Fallback: Attempt to load a generic icon if available in resources
                ComponentResourceManager resources = new ComponentResourceManager(typeof(ChartDetailForm));
                // You might need to add an icon to your project resources and name it "$this.Icon"
                // or handle the case where no icon is found more gracefully.
                // For now, we'll try to load, but it might not find one.
                try
                {
                    this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
                }
                catch
                {
                    // No icon found or error loading, proceed without an icon
                }
            }

            // Configure the modal's chart control
            chartControlModal.Titles.Clear();
            chartControlModal.Titles.Add(new Title(title, Docking.Top, new Font("Arial", 14, FontStyle.Bold), Color.Black));

            chartControlModal.ChartAreas.Clear();
            ChartArea caModal = new ChartArea("DetailArea_" + (originalChartArea?.Name ?? "Default"));

            if (originalChartArea != null)
            {
                caModal.BackColor = originalChartArea.BackColor;

                // Axis X
                caModal.AxisX.Title = originalChartArea.AxisX.Title;
                caModal.AxisX.TitleFont = new Font(originalChartArea.AxisX.TitleFont?.FontFamily ?? new Font("Arial", 10f).FontFamily, originalChartArea.AxisX.TitleFont?.SizeInPoints ?? 10f, originalChartArea.AxisX.TitleFont?.Style ?? FontStyle.Bold);
                caModal.AxisX.Minimum = originalChartArea.AxisX.Minimum;
                caModal.AxisX.Maximum = originalChartArea.AxisX.Maximum;
                caModal.AxisX.LabelStyle.Format = originalChartArea.AxisX.LabelStyle.Format;
                caModal.AxisX.MajorGrid.LineColor = originalChartArea.AxisX.MajorGrid.LineColor;
                caModal.AxisX.MajorGrid.Enabled = originalChartArea.AxisX.MajorGrid.Enabled;
                caModal.AxisX.LineColor = originalChartArea.AxisX.LineColor;
                caModal.AxisX.LabelStyle.ForeColor = originalChartArea.AxisX.LabelStyle.ForeColor;


                // Axis Y (Primary)
                caModal.AxisY.Title = originalChartArea.AxisY.Title;
                caModal.AxisY.TitleFont = new Font(originalChartArea.AxisY.TitleFont?.FontFamily ?? new Font("Arial", 10f).FontFamily, originalChartArea.AxisY.TitleFont?.SizeInPoints ?? 10f, originalChartArea.AxisY.TitleFont?.Style ?? FontStyle.Bold);
                caModal.AxisY.Minimum = originalChartArea.AxisY.Minimum;
                caModal.AxisY.Maximum = originalChartArea.AxisY.Maximum;
                caModal.AxisY.LabelStyle.Format = originalChartArea.AxisY.LabelStyle.Format;
                caModal.AxisY.MajorGrid.LineColor = originalChartArea.AxisY.MajorGrid.LineColor;
                caModal.AxisY.MajorGrid.Enabled = originalChartArea.AxisY.MajorGrid.Enabled;
                caModal.AxisY.LineColor = originalChartArea.AxisY.LineColor;
                caModal.AxisY.LabelStyle.ForeColor = originalChartArea.AxisY.LabelStyle.ForeColor;
                caModal.AxisY.TitleForeColor = originalChartArea.AxisY.TitleForeColor;

                // Axis Y2 (Secondary)
                if (originalChartArea.AxisY2.Enabled == AxisEnabled.True)
                {
                    caModal.AxisY2.Enabled = AxisEnabled.True;
                    caModal.AxisY2.Title = originalChartArea.AxisY2.Title;
                    caModal.AxisY2.TitleFont = new Font(originalChartArea.AxisY2.TitleFont?.FontFamily ?? new Font("Arial", 10f).FontFamily, originalChartArea.AxisY2.TitleFont?.SizeInPoints ?? 10f, originalChartArea.AxisY2.TitleFont?.Style ?? FontStyle.Bold);
                    caModal.AxisY2.Minimum = originalChartArea.AxisY2.Minimum;
                    caModal.AxisY2.Maximum = originalChartArea.AxisY2.Maximum;
                    caModal.AxisY2.LabelStyle.Format = originalChartArea.AxisY2.LabelStyle.Format;
                    caModal.AxisY2.MajorGrid.Enabled = originalChartArea.AxisY2.MajorGrid.Enabled;
                    caModal.AxisY2.MajorGrid.LineColor = originalChartArea.AxisY2.MajorGrid.LineColor;
                    caModal.AxisY2.LineColor = originalChartArea.AxisY2.LineColor;
                    caModal.AxisY2.LabelStyle.ForeColor = originalChartArea.AxisY2.LabelStyle.ForeColor;
                    caModal.AxisY2.TitleForeColor = originalChartArea.AxisY2.TitleForeColor;
                    caModal.AxisY2.ScaleView.Zoomable = originalChartArea.AxisY2.ScaleView.Zoomable;
                }

                // Zoom and Pan capabilities
                caModal.CursorX.IsUserEnabled = originalChartArea.CursorX.IsUserEnabled;
                caModal.CursorX.IsUserSelectionEnabled = originalChartArea.CursorX.IsUserSelectionEnabled;
                caModal.CursorY.IsUserEnabled = originalChartArea.CursorY.IsUserEnabled;
                caModal.CursorY.IsUserSelectionEnabled = originalChartArea.CursorY.IsUserSelectionEnabled;
                caModal.AxisX.ScaleView.Zoomable = originalChartArea.AxisX.ScaleView.Zoomable;
                caModal.AxisY.ScaleView.Zoomable = originalChartArea.AxisY.ScaleView.Zoomable;
            }
            chartControlModal.ChartAreas.Add(caModal);

            chartControlModal.Series.Clear();
            foreach (Series sOriginal in originalSeries)
            {
                Series sNewModal = new Series(sOriginal.Name);
                sNewModal.ChartType = sOriginal.ChartType;
                sNewModal.Color = sOriginal.Color;
                sNewModal.BorderWidth = sOriginal.BorderWidth > 0 ? sOriginal.BorderWidth + 1 : 2; // Make lines slightly thicker
                sNewModal.XValueType = sOriginal.XValueType;
                sNewModal.YValueType = sOriginal.YValueType;
                sNewModal.YAxisType = sOriginal.YAxisType; // Preserve primary or secondary axis assignment

                // Add markers if there are few points, for better visibility
                sNewModal.MarkerStyle = (sOriginal.Points.Count > 0 && sOriginal.Points.Count < 75) ? MarkerStyle.Circle : MarkerStyle.None;
                sNewModal.MarkerSize = 6;
                sNewModal.MarkerColor = sOriginal.Color; // Match series color
                sNewModal.MarkerBorderColor = Color.Black;


                foreach (DataPoint dpOriginal in sOriginal.Points)
                {
                    sNewModal.Points.Add(dpOriginal.Clone()); // Clone data points
                }
                chartControlModal.Series.Add(sNewModal);
            }

            // Legend
            chartControlModal.Legends.Clear();
            if (originalSeries.Count > 0)
            {
                Legend modalLegend = new Legend("ModalLegend_" + (originalChartArea?.Name ?? "Default"));
                modalLegend.Docking = Docking.Bottom;
                modalLegend.Alignment = StringAlignment.Center;
                modalLegend.Font = new Font("Arial", 9);
                modalLegend.BackColor = Color.Transparent;
                chartControlModal.Legends.Add(modalLegend);
                foreach (Series s in chartControlModal.Series)
                {
                    s.Legend = modalLegend.Name;
                    s.IsVisibleInLegend = true; // Ensure it's visible
                }
            }
        }

        // Manually created InitializeComponent for ChartDetailForm
        private void InitializeComponentLocal()
        {
            this.chartControlModal = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chartControlModal)).BeginInit();
            this.SuspendLayout();
            //
            // chartControlModal
            //
            this.chartControlModal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControlModal.Location = new System.Drawing.Point(0, 0);
            this.chartControlModal.Name = "chartControlModal";
            this.chartControlModal.TabIndex = 0;
            // A default ChartArea for the designer - it will be cleared and reconfigured in the constructor.
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartAreaModalDefault = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            chartAreaModalDefault.Name = "ChartAreaModalDefault";
            this.chartControlModal.ChartAreas.Add(chartAreaModalDefault);
            // A default Legend for the designer.
            System.Windows.Forms.DataVisualization.Charting.Legend legendModalDefault = new System.Windows.Forms.DataVisualization.Charting.Legend();
            legendModalDefault.Name = "LegendModalDefault";
            this.chartControlModal.Legends.Add(legendModalDefault);
            //
            // ChartDetailForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561); // Max 800x600 - this is a good internal size
            this.Controls.Add(this.chartControlModal);
            this.MinimizeBox = false;
            this.MaximizeBox = true; // Allow user to maximize the detailed chart view
            this.Name = "ChartDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Detalhes do Gráfico"; // Default title, will be overridden
            ((System.ComponentModel.ISupportInitialize)(this.chartControlModal)).EndInit();
            this.ResumeLayout(false);
        }


        

    }
}
