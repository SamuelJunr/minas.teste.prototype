using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.ViewModel;

namespace minas.teste.prototype.MVVM.View // Certifique-se que o namespace está correto
{
    public partial class ConfigFormBomb : Form
    {
        private List<ReadingData> _allReadings;
        public ConfigurationResult CurrentConfiguration { get; private set; }

        // Estrutura para definir as categorias e como agrupar as leituras
        private class ReadingCategory
        {
            public string Title { get; set; }
            public Func<ReadingData, bool> BelongsToCategory { get; set; }
            // Opcional: para ordenar as checkboxes dentro da categoria, se necessário
            public Func<ReadingData, string> OrderKey { get; set; }
        }

        private List<ReadingCategory> _readingCategories;


        public ConfigFormBomb(string testTypeDescription, List<ReadingData> allReadings, ConfigurationResult previousConfiguration)
        {
            InitializeComponent();
            this.Text = $"Configurar Ensaio: {testTypeDescription}"; // Título do modal
            _allReadings = allReadings;
            CurrentConfiguration = previousConfiguration ?? new ConfigurationResult();

            DefineReadingCategories(); // Define as categorias
            InitializeModalControls();
        }
        private void DefineReadingCategories()
        {
            _readingCategories = new List<ReadingCategory>
            {
                new ReadingCategory { Title = "Pressão", BelongsToCategory = r => r.Name.StartsWith("P") && !r.Name.Contains("Piloto") && r.Type=="pressure", OrderKey = r => r.Name },
                new ReadingCategory { Title = "Pilotagem", BelongsToCategory = r => r.Name.Contains("Piloto"), OrderKey = r => r.Name },
                new ReadingCategory { Title = "Vazão", BelongsToCategory = r => r.Name.StartsWith("Vazão"), OrderKey = r => r.Name },
                new ReadingCategory { Title = "Dreno", BelongsToCategory = r => r.Name.StartsWith("Dreno"), OrderKey = r => r.Name },
                new ReadingCategory { Title = "Hidro A", BelongsToCategory = r => r.Name.StartsWith("Hidro A"), OrderKey = r => r.Name },
                new ReadingCategory { Title = "Hidro B", BelongsToCategory = r => r.Name.StartsWith("Hidro B"), OrderKey = r => r.Name },
                new ReadingCategory { Title = "Motor A", BelongsToCategory = r => r.Name.StartsWith("Motor A"), OrderKey = r => r.Name },
                new ReadingCategory { Title = "Motor B", BelongsToCategory = r => r.Name.StartsWith("Motor B"), OrderKey = r => r.Name },
                new ReadingCategory { Title = "Temperatura", BelongsToCategory = r => r.Name == "Temperatura", OrderKey = r => r.Name },
                new ReadingCategory { Title = "Rotação", BelongsToCategory = r => r.Name == "Rotação", OrderKey = r => r.Name }
            };
        }


        private void InitializeModalControls()
        {
            // Configurar unidades de pressão e vazão
            if (CurrentConfiguration.SelectedPressureUnit.ToLower() == "psi")
                radioPsi.Checked = true;
            else
                radioBar.Checked = true;

            if (CurrentConfiguration.SelectedFlowUnit.ToLower() == "gpm")
                radioGpm.Checked = true;
            else
                radioLpm.Checked = true;

            // Limpar o painel principal de checkboxes
            panelReadingsCheckboxes.Controls.Clear();
            panelReadingsCheckboxes.SuspendLayout(); // Otimização para adicionar múltiplos controles

            int currentY = 5; // Posição Y inicial dentro do panelReadingsCheckboxes
            int categoryRowHeight = 30; // Altura aproximada para cada linha de categoria
            int titleLabelWidth = 100; // Largura para o título da categoria
            int checkboxFlowPanelMarginLeft = 5;

            foreach (var category in _readingCategories)
            {
                var readingsInCategory = _allReadings.Where(category.BelongsToCategory)
                                                     .OrderBy(category.OrderKey ?? (r => r.Name))
                                                     .ToList();

                if (!readingsInCategory.Any()) continue; // Pular categoria se não houver leituras

                // Painel para a linha da categoria (Título + FlowLayoutPanel para checkboxes)
                Panel categoryRowPanel = new Panel
                {
                    Width = panelReadingsCheckboxes.ClientSize.Width - 10, // Ajustar à largura do contêiner pai
                    Height = categoryRowHeight, // Ajustar altura conforme necessário, pode ser AutoSize
                    Location = new Point(5, currentY),
                    // Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right // Para responsividade
                };

                // Label para o título da categoria
                Label titleLabel = new Label
                {
                    Text = category.Title,
                    AutoSize = false, // Para definir a largura manualmente
                    Width = titleLabelWidth,
                    Height = categoryRowHeight - 5, // Um pouco menos que a altura da linha para alinhar
                    TextAlign = ContentAlignment.MiddleLeft,
                    Location = new Point(0, (categoryRowHeight - new Label().Height) / 2), // Centraliza verticalmente
                    Font = new Font(this.Font, FontStyle.Bold)
                };
                categoryRowPanel.Controls.Add(titleLabel);

                // FlowLayoutPanel para os checkboxes da categoria
                FlowLayoutPanel checkboxesFlowPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.LeftToRight,
                    Location = new Point(titleLabel.Right + checkboxFlowPanelMarginLeft, 0),
                    // Largura para preencher o restante do categoryRowPanel
                    Width = categoryRowPanel.Width - titleLabel.Width - checkboxFlowPanelMarginLeft - 5,
                    Height = categoryRowHeight, // Mesma altura da linha
                    WrapContents = true, // Permitir quebra de linha se muitos checkboxes
                    // AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, // Alternativa para altura dinâmica
                };
                categoryRowPanel.Controls.Add(checkboxesFlowPanel);

                foreach (var reading in readingsInCategory)
                {
                    CheckBox chk = new CheckBox
                    {
                        Text = reading.Name, // Nome da leitura específica
                        Tag = reading.Id,
                        AutoSize = true, // Para o texto caber
                        Checked = CurrentConfiguration.SelectedReadingIds.Contains(reading.Id),
                        Margin = new Padding(3, 3, 10, 3) // Espaçamento entre checkboxes
                    };
                    checkboxesFlowPanel.Controls.Add(chk);
                }

                // Ajusta a altura do categoryRowPanel se o FlowLayoutPanel precisar de mais espaço (devido ao WrapContents)
                // Isso é um pouco mais complexo e pode exigir cálculo da altura real do FlowLayoutPanel.
                // Por simplicidade, manteremos uma altura fixa por enquanto. Se WrapContents for importante,
                // a altura do categoryRowPanel e o incremento de currentY precisarão ser dinâmicos.
                // Exemplo simples para altura dinâmica (pode precisar de refinamento):
                // checkboxesFlowPanel.ResumeLayout(false); // Força cálculo do layout
                // categoryRowPanel.Height = Math.Max(categoryRowHeight, checkboxesFlowPanel.DisplayRectangle.Height);


                panelReadingsCheckboxes.Controls.Add(categoryRowPanel);
                currentY += categoryRowPanel.Height + 5; // Espaçamento entre as linhas de categoria
            }
            panelReadingsCheckboxes.ResumeLayout(true);
        }


        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            CurrentConfiguration.SelectedPressureUnit = radioBar.Checked ? "bar" : "psi";
            CurrentConfiguration.SelectedFlowUnit = radioLpm.Checked ? "lpm" : "gpm";

            CurrentConfiguration.SelectedReadingIds.Clear();
            // Itera sobre os painéis de categoria, depois sobre os FlowLayoutPanels, depois sobre os CheckBoxes
            foreach (Panel categoryRowPanel in panelReadingsCheckboxes.Controls.OfType<Panel>())
            {
                FlowLayoutPanel flowPanel = categoryRowPanel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
                if (flowPanel != null)
                {
                    foreach (CheckBox chk in flowPanel.Controls.OfType<CheckBox>())
                    {
                        if (chk.Checked && chk.Tag != null)
                        {
                            CurrentConfiguration.SelectedReadingIds.Add(chk.Tag.ToString());
                        }
                    }
                }
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelConfig_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnClearSelections_Click(object sender, EventArgs e)
        {
            // Itera para desmarcar todos os checkboxes
            foreach (Panel categoryRowPanel in panelReadingsCheckboxes.Controls.OfType<Panel>())
            {
                FlowLayoutPanel flowPanel = categoryRowPanel.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
                if (flowPanel != null)
                {
                    foreach (CheckBox chk in flowPanel.Controls.OfType<CheckBox>())
                    {
                        chk.Checked = false;
                    }
                }
            }

            radioBar.Checked = true; // Padrão
            radioLpm.Checked = true; // Padrão

            if (CurrentConfiguration != null)
            {
                CurrentConfiguration.SelectedReadingIds.Clear();
                CurrentConfiguration.SelectedPressureUnit = "bar";
                CurrentConfiguration.SelectedFlowUnit = "lpm";
            }
        }
    }
}
