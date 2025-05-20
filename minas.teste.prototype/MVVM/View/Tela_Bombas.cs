using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
// Removido: using System.Web.UI.WebControls; pois TextBox é System.Windows.Forms.TextBox
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using minas.teste.prototype.MVVM.Model.Concrete; // Assumindo que esta e as próximas são necessárias para o restante da classe
using minas.teste.prototype.MVVM.ViewModel;
using minas.teste.prototype.Service;
using TextBox = System.Windows.Forms.TextBox; // Especifica o TextBox do Windows Forms

// Certifique-se que Tela_BombasVM.Datapoint_Bar_Lpm está acessível.
// Se estiver dentro de Tela_BombasVM, e Tela_BombasVM for uma classe,
// você precisará de uma instância ou torná-la estática, ou mover a struct.
// Exemplo: public struct Datapoint_Bar_Lpm { public double FlowLpm; public double PressureBar; }
// pode ser definida dentro deste namespace ou em um arquivo comum.

namespace minas.teste.prototype.MVVM.View
{
    public partial class Tela_Bombas : Form
    {
        // Estrutura para mapear parâmetros do DataGridView estático
        private struct ParameterRowInfo
        {
            public string DisplayName { get; } // Nome a ser exibido na primeira coluna
            public string SourceTextBoxName { get; } // Nome do TextBox de onde o dado virá
            public string FormattingType { get; } // "float", "temp", "rpm"

            public ParameterRowInfo(string displayName, string sourceTextBoxName, string formattingType)
            {
                DisplayName = displayName;
                SourceTextBoxName = sourceTextBoxName;
                FormattingType = formattingType;
            }
        }

        private List<ParameterRowInfo> _staticDataGridViewParameters;

        private Tela_BombasVM _viewModel;
        private List<EtapaData> _dadosColetados = new List<EtapaData>();
        public Dictionary<string, System.Windows.Forms.TextBox> sensorMap;
        public Dictionary<string, string> sensorMapmedida;
        private List<string> dadosSensoresSelecionados = new List<string>();
        public List<SensorData> dadosSensores;
        private Timer monitoramentoTimer;
        private Timer timerCronometro;
        private TextBoxTrackBarSynchronizer _synchronizerNivel;

        private SerialManager _serialManager;
        private Timer _updateUiTimer;
        private StringBuilder serialDataBuffer = new StringBuilder();
        private readonly object serialBufferLock = new object();
        private readonly object readingsLock = new object();

        private Dictionary<string, string> _latestRawSensorData = new Dictionary<string, string>();
        private Dictionary<string, double> _currentNumericSensorReadings = new Dictionary<string, double>();


        private const double BAR_TO_PSI_CONVERSION = 14.5;
        private const double LPM_TO_GPM_USER_CONVERSION = 3.98; // Considerar se este é o fator correto para GPM (US) ou GPM (Imperial)
        private const double BAR_CONVERSION_PILOT = 1.705;

        private int _timeCounterSecondsRampa = 0;
        private bool _fechamentoControladoPeloUsuario = false;
        private bool _isMonitoring = false;
        public string Inicioteste;
        public string Fimteste;
        private int etapaAtual = 1; // Controla a coluna da etapa atual (1 a 7) para o dataGridView1
        public string TbNomeCliente { get; set; }
        public string TbNomeBomba { get; set; }
        public string TbOrdemServico { get; set; }
        private int valorDefinidoCronometro;
        private bool cronometroIniciado = false;
        private bool tempoCronometroDefinidoManualmente = false;
        private Dictionary<string, string> sensorControlMap;
        private List<ReadingData> allReadingsData;
        private ConfigurationResult currentConfiguration;
        private Button currentlyActiveTestButton = null;
        private Color defaultButtonColor; // Para restaurar a cor original do botão



        // Mapeamento de IDs de ReadingData para os controles na Tela_Bombas
        private Dictionary<string, (TextBox valueTextBox, Label unitLabel)> sensorControlsMap;
        private ToolTip sensorToolTip;
        // --- FIM: Variáveis para o sistema de configuração de ensaio ---


        public Tela_Bombas()
        {
            InitializeComponent();
            InitializeStaticDataGridViewParameters(); // Deve ser chamado antes de SetupStaticDataGridView
            SetupStaticDataGridView(); // Chamada para o método atualizado

            _viewModel = new Tela_BombasVM();

            // Verifique se os TextBoxes e TrackBar existem no seu designer
            Control textBox2Control = this.Controls.Find("textBox2", true).FirstOrDefault();
            Control trackBar1Control = this.Controls.Find("trackBar1", true).FirstOrDefault();

            if (textBox2Control is System.Windows.Forms.TextBox tb2 && trackBar1Control is TrackBar tb1)
            {
                _synchronizerNivel = new TextBoxTrackBarSynchronizer(tb2, tb1, 1, 7);
            }
            else
            {
                Debug.WriteLine("Aviso: textBox2 ou trackBar1 não encontrados no formulário para TextBoxTrackBarSynchronizer.");
            }

            dadosSensores = new List<SensorData>();

            timerCronometro = new Timer();
            timerCronometro.Interval = 1000;
            timerCronometro.Tick += TimerCronometro_Tick;

            _serialManager = ConnectionSettingsApplication.PersistentSerialManager;

            _updateUiTimer = new Timer();
            _updateUiTimer.Interval = 150; // Intervalo de atualização da UI
           // _updateUiTimer.Tick += UpdateUiTimer_Tick;

            InitializeAllCharts();

            SetButtonState(btniniciarteste, true);
            SetButtonState(btngravar, false);
            SetButtonState(bntFinalizar, false);
            SetButtonState(btnreset, false);
            SetButtonState(btnrelatoriobomba, false);
            InitializeSensorConfigurationSystem();
           
        }

        // --- INÍCIO: Métodos para o sistema de configuração de ensaio ---
        private void InitializeSensorConfigurationSystem()
        {
            sensorToolTip = new ToolTip(); // Inicializa o ToolTip
            InitializeAllReadingsDataFromSpec();
            InitializeSensorControlsMap();
            currentConfiguration = new ConfigurationResult();

            if (btnConfigCircuitoAberto != null)
            {
                defaultButtonColor = btnConfigCircuitoAberto.BackColor;
            }
            else
            {
                defaultButtonColor = SystemColors.Control; // Padrão
            }

            // Os manipuladores de evento já estão em português em termos de suas descrições de string
            if (btnConfigCircuitoAberto != null)
                btnConfigCircuitoAberto.Click += (sender, e) => HandleConfigButtonClick(sender as Button, "Bombas Axiais de Circuito Aberto");
            if (btnConfigCircuitoFechado != null)
                btnConfigCircuitoFechado.Click += (sender, e) => HandleConfigButtonClick(sender as Button, "Bombas e Motores Axiais de Circuito Fechado");
            if (btnConfigEngrenagem != null)
                btnConfigEngrenagem.Click += (sender, e) => HandleConfigButtonClick(sender as Button, "Bombas de Engrenagem, Palheta e Cartucho");

            SetInitialButtonStates();
            UpdateTelaBombasDisplay();
        }

        private void InitializeAllReadingsDataFromSpec()
        {
            // Parâmetros do construtor ReadingData: id, name (exibir no modal e ToolTip), value (exemplo), type, originalUnit, valueTextBoxName, unitLabelName, nameLabelText (para ToolTip se diferente de Name)
            // Todas as strings aqui já estão em português ou são identificadores.
            allReadingsData = new List<ReadingData>
            {
                new ReadingData("sensor_MA1_press", "P1", 0, "pressure", "bar", "sensor_P1", "LbPressao1", "P1"),
                new ReadingData("sensor_MA2_press", "P2", 0, "pressure", "bar", "sensor_P2", "LbPressao2", "P2"),
                new ReadingData("sensor_MB1_press", "P3", 0, "pressure", "bar", "sensor_P3", "LbPressao3", "P3"),
                new ReadingData("sensor_MB2_press", "P4", 0, "pressure", "bar", "sensor_P4", "LbPressao4", "P4"),

                new ReadingData("sensor_P1_piloto", "Pressão Piloto 1", 0, "pressure", "bar", "sensor_PR1", "LbPilotagem1", "Pressão Piloto 1"),
                new ReadingData("sensor_P2_piloto", "Pressão Piloto 2", 0, "pressure", "bar", "sensor_PR2", "LbPilotagem2", "Pressão Piloto 2"),
                new ReadingData("sensor_P3_piloto", "Pressão Piloto 3", 0, "pressure", "bar", "sensor_PR3", "LbPilotagem3", "Pressão Piloto 3"),
                new ReadingData("sensor_P4_piloto", "Pressão Piloto 4", 0, "pressure", "bar", "sensor_PR4", "LbPilotagem4", "Pressão Piloto 4"),

                new ReadingData("sensor_V1_vazao", "Vazão 1", 0, "flow", "lpm", "sensor_V1", "LbVazao1", "Vazão 1"),
                new ReadingData("sensor_V2_vazao", "Vazão 2", 0, "flow", "lpm", "sensor_V2", "LbVazao2", "Vazão 2"),
                new ReadingData("sensor_V3_vazao", "Vazão 3", 0, "flow", "lpm", "sensor_V3", "LbVazao3", "Vazão 3"),
                new ReadingData("sensor_V4_vazao", "Vazão 4", 0, "flow", "lpm", "sensor_V4", "LbVazao4", "Vazão 4"),

                new ReadingData("sensor_DR1_dreno", "Dreno 1", 0, "flow", "lpm", "sensor_DR1", "LbDreno1", "Dreno 1"),
                new ReadingData("sensor_DR2_dreno", "Dreno 2", 0, "flow", "lpm", "sensor_DR2", "LbDreno2", "Dreno 2"),

                new ReadingData("sensor_HA1_hidro", "Hidro A1", 0, "pressure", "bar", "sensor_HA1", "LbHidroA1", "Hidro A1"),
                new ReadingData("sensor_HA2_hidro", "Hidro A2", 0, "pressure", "bar", "sensor_HA2", "LbHidroA2", "Hidro A2"),
                new ReadingData("sensor_HB1_hidro", "Hidro B1", 0, "pressure", "bar", "sensor_HB1", "LbHidroB1", "Hidro B1"),
                new ReadingData("sensor_HB2_hidro", "Hidro B2", 0, "pressure", "bar", "sensor_HB2", "LbHidroB2", "Hidro B2"),

                new ReadingData("sensor_MA1_motor", "Motor A1", 0, "pressure", "bar", "sensor_MA1", "LbMotorA1", "Motor A1"),
                new ReadingData("sensor_MA2_motor", "Motor A2", 0, "pressure", "bar", "sensor_MA2", "LbMotorA2", "Motor A2"),
                new ReadingData("sensor_MB1_motor", "Motor B1", 0, "pressure", "bar", "sensor_MB1", "LbMotorB1", "Motor B1"),
                new ReadingData("sensor_MB2_motor", "Motor B2", 0, "pressure", "bar", "sensor_MB2", "LbMotorB2", "Motor B2"),

                new ReadingData("sensor_CELSUS_temp", "Temperatura", 0, "fixed", "°C", "sensor_CELSUS", "LbTemp", "Temperatura"),
                new ReadingData("sensor_RPM_rot", "Rotação", 0, "fixed", "rpm", "sensor_RPM", "LbRota", "Rotação")
            };
        }

        private void InitializeSensorControlsMap()
        {
            // O dicionário agora armazena apenas TextBox e Label de unidade.
            sensorControlsMap = new Dictionary<string, (TextBox valueTextBox, Label unitLabel)>();

            foreach (ReadingData rd in allReadingsData)
            {
                Control[] tbControls = this.Controls.Find(rd.ValueTextBoxName, true);
                TextBox valueTb = tbControls.Length > 0 ? tbControls[0] as TextBox : null;

                Control[] ulControls = this.Controls.Find(rd.UnitLabelName, true);
                Label unitL = ulControls.Length > 0 ? ulControls[0] as Label : null;

                if (valueTb != null && unitL != null)
                {
                    // Configura o ToolTip para o TextBox de valor
                    sensorToolTip.SetToolTip(valueTb, rd.NameLabelText); // Usa NameLabelText para o ToolTip

                    sensorControlsMap[rd.Id] = (valueTb, unitL);
                }
                else
                {
                    Debug.WriteLine($"Controles não encontrados para o ID da Leitura: {rd.Id} (TextBox: {rd.ValueTextBoxName}, RótuloUnidade: {rd.UnitLabelName})");
                }
            }
        }

        private void HandleConfigButtonClick(Button clickedButton, string testTypeDescription)
        {
            if (clickedButton == null) return;

            if (currentlyActiveTestButton == null) // Nenhum teste ativo
            {
                OpenConfigModal(testTypeDescription, clickedButton);
            }
            else if (clickedButton == currentlyActiveTestButton) // Clicou no botão do teste ativo (verde)
            {
                DialogResult dr = MessageBox.Show("Deseja resetar a configuração atual e selecionar um novo tipo de ensaio?", // Já em Português
                                                 "Resetar Ensaio", MessageBoxButtons.YesNo, MessageBoxIcon.Question); // Já em Português
                if (dr == DialogResult.Yes)
                {
                    ResetActiveTestConfiguration();
                }
            }
            else // Outro teste já está ativo (outro botão está verde)
            {
                MessageBox.Show($"O ensaio '{currentlyActiveTestButton.Tag?.ToString() ?? "ativo"}' já está configurado.\n" + // Já em Português
                                "Para mudar o tipo de ensaio, clique no botão do teste ativo (verde) e confirme o reset.", // Já em Português
                                "Ensaio Ativo", MessageBoxButtons.OK, MessageBoxIcon.Information); // Já em Português
            }
        }

        private void OpenConfigModal(string testTypeDescription, Button activatingButton)
        {
            // Criar uma cópia da configuração para o modal, para não alterar a principal se cancelado
            var configForModal = new ConfigurationResult
            {
                SelectedPressureUnit = currentConfiguration.SelectedPressureUnit,
                SelectedFlowUnit = currentConfiguration.SelectedFlowUnit,
                SelectedReadingIds = new List<string>(currentConfiguration.SelectedReadingIds)
            };

            using (ConfigFormBomb configDialog = new ConfigFormBomb(testTypeDescription, allReadingsData, configForModal))
            {
                if (configDialog.ShowDialog(this) == DialogResult.OK)
                {
                    currentConfiguration = configDialog.CurrentConfiguration; // Obter a configuração salva do modal
                    UpdateTelaBombasDisplay();

                    if (currentlyActiveTestButton != null && currentlyActiveTestButton != activatingButton)
                    {
                        currentlyActiveTestButton.BackColor = defaultButtonColor; // Restaurar cor do anterior
                        currentlyActiveTestButton.ForeColor = SystemColors.ControlText;
                        currentlyActiveTestButton.Enabled = true;
                    }

                    currentlyActiveTestButton = activatingButton;
                    currentlyActiveTestButton.BackColor = Color.DarkGreen; // Verde escuro para melhor contraste com texto branco
                    currentlyActiveTestButton.ForeColor = Color.White;
                    currentlyActiveTestButton.Tag = testTypeDescription; // Armazenar a descrição do teste na Tag

                    // Desabilitar os outros botões de configuração
                    var configButtons = new[] { btnConfigCircuitoAberto, btnConfigCircuitoFechado, btnConfigEngrenagem };
                    foreach (Button btn in configButtons)
                    {
                        if (btn != null && btn != currentlyActiveTestButton)
                        {
                            btn.Enabled = false;
                            btn.BackColor = defaultButtonColor; // Garantir cor padrão se não estiver ativo
                            btn.ForeColor = SystemColors.ControlText; // Cor padrão do texto
                        }
                    }
                    if (btniniciarteste != null) btniniciarteste.Enabled = true; // Habilitar o botão de iniciar teste
                }
            }
        }

        private void UpdateTelaBombasDisplay()
        {
            if (allReadingsData == null || sensorControlsMap == null) return;

            // Agrupa os controles (TextBox de valor, Label de unidade) por GroupBox
            Dictionary<GroupBox, List<(TextBox valueTb, Label unitL)>> groupBoxReadingsMap =
                new Dictionary<GroupBox, List<(TextBox valueTb, Label unitL)>>();

            // Primeiro, define a visibilidade e atualiza unidades dos controles mapeados
            foreach (ReadingData rd in allReadingsData)
            {
                if (sensorControlsMap.TryGetValue(rd.Id, out var controls))
                {
                    bool isSelected = currentConfiguration.SelectedReadingIds.Contains(rd.Id);

                    controls.valueTextBox.Visible = isSelected;
                    controls.unitLabel.Visible = isSelected;

                    if (isSelected)
                    {
                        string unitText = "";
                        if (rd.Type == "pressure") unitText = currentConfiguration.SelectedPressureUnit.ToUpper();
                        else if (rd.Type == "flow") unitText = currentConfiguration.SelectedFlowUnit.ToUpper();
                        else unitText = rd.OriginalUnit.ToUpper();

                        controls.unitLabel.Text = unitText;
                        controls.unitLabel.Font = new Font(controls.unitLabel.Font, FontStyle.Bold);

                        GroupBox parentGroup = FindParentGroupBox(controls.valueTextBox);
                        if (parentGroup != null)
                        {
                            if (!groupBoxReadingsMap.ContainsKey(parentGroup))
                            {
                                groupBoxReadingsMap[parentGroup] = new List<(TextBox valueTb, Label unitL)>();
                            }
                            groupBoxReadingsMap[parentGroup].Add(controls); // Adiciona o par (TextBox, Label Unidade)
                        }
                    }
                }
            }

            // Aplica o layout para cada GroupBox que tem itens selecionados
            foreach (var kvp in groupBoxReadingsMap)
            {
                LayoutControlsInGroup(kvp.Key, kvp.Value);
            }

            // Garante que GroupBoxes que não têm mais itens selecionados tenham seus controles internos (mapeados) ocultos
            foreach (var groupControl in this.Controls.OfType<Panel>().SelectMany(p => p.Controls.OfType<GroupBox>()) // Assumindo que GroupBoxes estão em panel1
                                          .Concat(this.Controls.OfType<GroupBox>())) // Inclui GroupBoxes diretamente no Form
            {
                if (!groupBoxReadingsMap.ContainsKey(groupControl)) // Se este GroupBox não está no mapa de itens a serem exibidos
                {
                    // Oculta todos os controles de sensor mapeados dentro deste GroupBox
                    foreach (var rd in allReadingsData)
                    {
                        if (sensorControlsMap.TryGetValue(rd.Id, out var sensorPair))
                        {
                            if (FindParentGroupBox(sensorPair.valueTextBox) == groupControl)
                            {
                                sensorPair.valueTextBox.Visible = false;
                                sensorPair.unitLabel.Visible = false;
                            }
                        }
                    }
                }
            }
        }

        private GroupBox FindParentGroupBox(Control control)
        {
            Control current = control;
            while (current != null)
            {
                if (current is GroupBox gb)
                {
                    return gb;
                }
                current = current.Parent;
            }
            return null;
        }

        // MÉTODO LayoutControlsInGroup REATORADO PARA NOVO LAYOUT
        private void LayoutControlsInGroup(GroupBox groupBox, List<(TextBox valueTb, Label unitL)> selectedItemsInGroup)
        {
            if (groupBox == null) return;
            groupBox.SuspendLayout();

            // Ocultar todos os controles de sensor mapeados neste grupo que NÃO estão na lista `selectedItemsInGroup`
            foreach (var readingEntry in sensorControlsMap)
            {
                if (FindParentGroupBox(readingEntry.Value.valueTextBox) == groupBox)
                {
                    bool isCurrentlySelectedForThisGroup = selectedItemsInGroup.Any(item => item.valueTb == readingEntry.Value.valueTextBox);
                    readingEntry.Value.valueTextBox.Visible = isCurrentlySelectedForThisGroup;
                    readingEntry.Value.unitLabel.Visible = isCurrentlySelectedForThisGroup;
                }
            }

            if (selectedItemsInGroup == null || !selectedItemsInGroup.Any())
            {
                groupBox.ResumeLayout(true);
                return;
            }

            int yPos = 25; // Posição Y inicial dentro do GroupBox (abaixo do título do GroupBox)
            const int itemVerticalHeight = 35; // Altura de uma linha de (TextBox + Label) - Ajuste conforme o tamanho dos seus TextBoxes
            const int verticalSpacingBetweenRows = 5;
            const int horizontalPaddingInGroupBox = 10;
            const int spacingBetweenSensorItemsOnSameRow = 10; // Espaçamento entre "(TextBox)Label" na mesma linha
            const int spacingWithinSensorItem = 2; // Espaçamento entre o TextBox e seu Label de unidade

            // Larguras preferenciais (os TextBoxes já têm suas larguras do Designer)
            // A largura do Label de unidade pode ser AutoSize ou uma largura fixa pequena.
            // int valueTextBoxWidth = 75; // Exemplo, mas usaremos a largura real do TextBox
            const int unitLabelPreferredWidth = 40; // Ex: "BAR", "PSI"

            int currentItemGlobalIndex = 0;
            while (currentItemGlobalIndex < selectedItemsInGroup.Count)
            {
                int itemsToPlaceInThisVisualRow = 0;
                int remainingItemsForGroup = selectedItemsInGroup.Count - currentItemGlobalIndex;
                int availableWidthForItems = groupBox.ClientSize.Width - (2 * horizontalPaddingInGroupBox);

                // Determina quantos itens cabem na linha atual
                for (int attemptCount = Math.Min(4, remainingItemsForGroup); attemptCount >= 1; attemptCount--)
                {
                    // Calcula a largura necessária para 'attemptCount' itens
                    int widthNeededForAttempt = 0;
                    for (int k = 0; k < attemptCount; k++)
                    {
                        if (currentItemGlobalIndex + k < selectedItemsInGroup.Count)
                        {
                            var itemTuple = selectedItemsInGroup[currentItemGlobalIndex + k];
                            widthNeededForAttempt += itemTuple.valueTb.Width + spacingWithinSensorItem + unitLabelPreferredWidth;
                            if (k < attemptCount - 1) // Adiciona espaçamento entre itens
                            {
                                widthNeededForAttempt += spacingBetweenSensorItemsOnSameRow;
                            }
                        }
                    }

                    if (widthNeededForAttempt <= availableWidthForItems)
                    {
                        itemsToPlaceInThisVisualRow = attemptCount;
                        break;
                    }
                }

                if (itemsToPlaceInThisVisualRow == 0 && remainingItemsForGroup > 0)
                {
                    itemsToPlaceInThisVisualRow = 1; // Força ao menos um item se houver algum restante
                }

                if (itemsToPlaceInThisVisualRow == 0) break;

                // Calcula a largura total real dos itens que serão colocados nesta linha
                int actualRowWidth = 0;
                for (int i = 0; i < itemsToPlaceInThisVisualRow; i++)
                {
                    if (currentItemGlobalIndex + i < selectedItemsInGroup.Count)
                    {
                        var itemTuple = selectedItemsInGroup[currentItemGlobalIndex + i];
                        actualRowWidth += itemTuple.valueTb.Width + spacingWithinSensorItem + unitLabelPreferredWidth;
                    }
                }
                actualRowWidth += Math.Max(0, itemsToPlaceInThisVisualRow - 1) * spacingBetweenSensorItemsOnSameRow;

                int startX = horizontalPaddingInGroupBox + (availableWidthForItems - actualRowWidth) / 2;
                if (startX < horizontalPaddingInGroupBox) startX = horizontalPaddingInGroupBox;

                for (int i = 0; i < itemsToPlaceInThisVisualRow; i++)
                {
                    if (currentItemGlobalIndex >= selectedItemsInGroup.Count) break;

                    var currentItemTuple = selectedItemsInGroup[currentItemGlobalIndex];
                    TextBox valueTextBox = currentItemTuple.valueTb;
                    Label unitLabel = currentItemTuple.unitL;

                    // Garante que são visíveis
                    valueTextBox.Visible = true;
                    unitLabel.Visible = true;

                    // Define a largura do Label da unidade (pode ser AutoSize ou fixa)
                    unitLabel.AutoSize = true; // Deixa o Label definir sua largura baseada no texto
                    // unitLabel.Width = unitLabelPreferredWidth; // Ou define uma largura fixa

                    int currentItemStartX = startX + i * ((actualRowWidth - ((itemsToPlaceInThisVisualRow - 1) * spacingBetweenSensorItemsOnSameRow)) / itemsToPlaceInThisVisualRow + spacingBetweenSensorItemsOnSameRow);
                    if (i > 0) // Adiciona espaçamento entre itens, exceto para o primeiro
                    {
                        // A lógica de startX já considera o espaçamento distribuído.
                        // No entanto, se for um espaçamento fixo entre blocos:
                        // currentItemStartX = (valueTextBox anterior).Right + spacingBetweenSensorItemsOnSameRow;
                        // Para simplificar, o startX calculado para a linha inteira já tenta centralizar o conjunto.
                        // Vamos posicionar relativo ao startX da linha e ao acumulado dos itens anteriores.
                        Control previousControlSet_TB = selectedItemsInGroup[currentItemGlobalIndex - 1].valueTb;
                        Control previousControlSet_LBL = selectedItemsInGroup[currentItemGlobalIndex - 1].unitL;
                        currentItemStartX = previousControlSet_LBL.Right + spacingBetweenSensorItemsOnSameRow;

                    }
                    else
                    {
                        currentItemStartX = startX; // Posição X do primeiro item na linha
                    }


                    valueTextBox.Location = new Point(currentItemStartX, yPos + (itemVerticalHeight - valueTextBox.Height) / 2);
                    unitLabel.Location = new Point(valueTextBox.Right + spacingWithinSensorItem, yPos + (itemVerticalHeight - unitLabel.Height) / 2);

                    currentItemGlobalIndex++;
                }
                yPos += itemVerticalHeight + verticalSpacingBetweenRows;
            }
            groupBox.ResumeLayout(true);
        }


        private void ResetActiveTestConfiguration()
        {
            currentConfiguration = new ConfigurationResult();
            UpdateTelaBombasDisplay();

            if (currentlyActiveTestButton != null)
            {
                currentlyActiveTestButton.BackColor = defaultButtonColor;
                currentlyActiveTestButton.ForeColor = SystemColors.ControlText; // Restaurar cor do texto
                currentlyActiveTestButton.Tag = null; // Limpar a descrição do teste
            }
            currentlyActiveTestButton = null;

            // Habilitar todos os botões de configuração
            var configButtons = new[] { btnConfigCircuitoAberto, btnConfigCircuitoFechado, btnConfigEngrenagem };
            foreach (Button btn in configButtons)
            {
                if (btn != null)
                {
                    btn.Enabled = true;
                    btn.BackColor = defaultButtonColor; // Garantir cor padrão
                    btn.ForeColor = SystemColors.ControlText;
                }
            }
            if (btniniciarteste != null) btniniciarteste.Enabled = false; // Desabilitar botão de iniciar teste
        }

        private void SetInitialButtonStates()
        {
            // No início, nenhum teste está ativo, todos os botões de configuração habilitados
            // e o botão de iniciar teste desabilitado.
            var configButtons = new[] { btnConfigCircuitoAberto, btnConfigCircuitoFechado, btnConfigEngrenagem };
            foreach (Button btn in configButtons)
            {
                if (btn != null)
                {
                    btn.Enabled = true;
                    btn.BackColor = defaultButtonColor;
                    btn.ForeColor = SystemColors.ControlText;
                }
            }
            if (btniniciarteste != null) btniniciarteste.Enabled = false;
        }

        public string GetConfiguredSensorValue(string sensorId)
        {
            if (currentConfiguration.SelectedReadingIds.Contains(sensorId) &&
                sensorControlsMap.TryGetValue(sensorId, out var controls))
            {
                return controls.valueTextBox.Text; // Ou o valor numérico real se você o armazenar
            }
            return "N/D"; // Não Disponível
        }

        public string GetConfiguredSensorUnit(string sensorId)
        {
            if (currentConfiguration.SelectedReadingIds.Contains(sensorId) &&
               sensorControlsMap.TryGetValue(sensorId, out var controls))
            {
                return controls.unitLabel.Text;
            }
            return "";
        }

        private void btnLimparConfigTelaBombas_Click(object sender, EventArgs e)
        {
            ResetActiveTestConfiguration();
        }


        private void InitializeStaticDataGridViewParameters()
        {
            // ATENÇÃO: Os nomes em 'SourceTextBoxName' DEVEM corresponder
            // exatamente à propriedade 'Name' dos seus controles TextBox no formulário.
            // Se o TextBox não for encontrado, o valor será "N/D" na tabela.
            _staticDataGridViewParameters = new List<ParameterRowInfo>
            {
                new ParameterRowInfo("P1", "sensor_MA1", "float"),
                new ParameterRowInfo("P2", "sensor_MA2", "float"),
                new ParameterRowInfo("P3", "sensor_MB1", "float"),
                new ParameterRowInfo("P4", "sensor_MB2", "float"),
                new ParameterRowInfo("Pressão Piloto 1", "sensor_P1", "float"),
                new ParameterRowInfo("Pressão Piloto 2", "sensor_P2", "float"),
                new ParameterRowInfo("Pressão Piloto 3", "sensor_P3", "float"),
                new ParameterRowInfo("Pressão Piloto 4", "sensor_P4", "float"),
                new ParameterRowInfo("Vazão 1", "sensor_V1", "float"),
                new ParameterRowInfo("Vazão 2", "sensor_V2", "float"),
                new ParameterRowInfo("Vazão 3", "sensor_V3", "float"),
                new ParameterRowInfo("Vazão 4", "sensor_V4", "float"),
                new ParameterRowInfo("Dreno 1", "sensor_DR1", "float"),
                new ParameterRowInfo("Dreno 2", "sensor_DR2", "float"),
                new ParameterRowInfo("Hidro A1", "sensor_HA1", "float"),
                new ParameterRowInfo("Hidro A2", "sensor_HA2", "float"),
                new ParameterRowInfo("Hidro B1", "sensor_HB1", "float"),
                new ParameterRowInfo("Hidro B2", "sensor_HB2", "float"),
                new ParameterRowInfo("Motor A1", "sensor_MA1_dup", "float"), // Nome duplicado, verificar se é intencional ou usar outro SourceTextBoxName
                new ParameterRowInfo("Motor A2", "sensor_MA2_dup", "float"), // Nome duplicado
                new ParameterRowInfo("Motor B1", "sensor_MB1_dup", "float"), // Nome duplicado
                new ParameterRowInfo("Motor B2", "sensor_MB2_dup", "float"), // Nome duplicado
                new ParameterRowInfo("Temperatura", "sensor_CELSUS", "temp"),
                new ParameterRowInfo("Rotação", "sensor_RPM", "rpm")
            };
        }

        // ### MÉTODO ATUALIZADO ###
        private void SetupStaticDataGridView()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed) return;

            dataGridView1.SuspendLayout(); // Otimização para múltiplas alterações

            // Configurações básicas do DataGridView
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.None; // Essencial: desabilita barras de rolagem
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None; // Garante que o dimensionamento manual funcione
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; // Garante que o dimensionamento manual funcione

            // Limpa conteúdo e colunas anteriores
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // Estilos (ajuste a fonte se necessário para caber mais conteúdo)
            // Exemplo: dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 8f);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // Opcional: Fonte em negrito para cabeçalhos
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font(dataGridView1.Font ?? SystemFonts.DefaultFont, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Centraliza dados por padrão

            // Adiciona Colunas
            DataGridViewTextBoxColumn parametroCol = new DataGridViewTextBoxColumn
            {
                Name = "Parametro",
                HeaderText = "Parâmetro",
                // Alinha texto à esquerda e adiciona padding horizontal para respiro
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleLeft,
                    Padding = new Padding(5, 2, 5, 2) // 5px left/right, 2px top/bottom
                }
            };
            dataGridView1.Columns.Add(parametroCol);

            int numberOfEtapaColumns = 7; // Número de colunas "Etapa"
            for (int i = 1; i <= numberOfEtapaColumns; i++)
            {
                dataGridView1.Columns.Add($"Etapa{i}", $"Etapa {i}");
            }

            // Adiciona Linhas
            if (_staticDataGridViewParameters != null)
            {
                foreach (var paramInfo in _staticDataGridViewParameters)
                {
                    dataGridView1.Rows.Add(paramInfo.DisplayName);
                }
            }

            // Se não há linhas ou colunas, não há o que dimensionar
            if (dataGridView1.Rows.Count == 0 || dataGridView1.Columns.Count == 0)
            {
                dataGridView1.ResumeLayout();
                return;
            }

            // --- LÓGICA DE DIMENSIONAMENTO ---
            // Use ClientSize para obter a área útil interna do DataGridView
            // Assuma que dataGridView1.Size (ex: 808x601) já foi definido externamente (Designer ou Load).
            int availableClientWidth = dataGridView1.ClientSize.Width;
            int availableClientHeight = dataGridView1.ClientSize.Height;

            // 1. Calcula a largura da coluna "Parâmetro"
            int maxParametroTextWidth = 0;
            using (Graphics g = dataGridView1.CreateGraphics())
            {
                Font paramCellFont = parametroCol.DefaultCellStyle.Font ?? dataGridView1.DefaultCellStyle.Font ?? dataGridView1.Font ?? SystemFonts.DefaultFont;
                Font headerFont = dataGridView1.ColumnHeadersDefaultCellStyle.Font ?? dataGridView1.Font ?? SystemFonts.DefaultFont;

                // Mede o texto do cabeçalho da coluna "Parâmetro"
                int headerTextWidth = TextRenderer.MeasureText(g, parametroCol.HeaderText, headerFont).Width;
                maxParametroTextWidth = headerTextWidth;

                // Mede o texto de cada item na coluna "Parâmetro"
                if (_staticDataGridViewParameters != null)
                {
                    foreach (var paramInfo in _staticDataGridViewParameters)
                    {
                        int itemTextWidth = TextRenderer.MeasureText(g, paramInfo.DisplayName, paramCellFont).Width;
                        if (itemTextWidth > maxParametroTextWidth)
                        {
                            maxParametroTextWidth = itemTextWidth;
                        }
                    }
                }
            }
            // Adiciona o padding horizontal da célula e uma margem extra
            int parametroColHorizontalPadding = parametroCol.DefaultCellStyle.Padding.Horizontal;
            int calculatedParametroColWidth = maxParametroTextWidth + parametroColHorizontalPadding + 10; // 10px de margem visual

            int minParametroColWidth = 100; // Largura mínima para a coluna "Parâmetro"
            parametroCol.Width = Math.Max(minParametroColWidth, calculatedParametroColWidth);

            // Garante que a coluna "Parâmetro" não ocupe toda a largura disponível, deixando espaço para etapas
            int minWidthPerEtapaCol = 30; // Largura mínima para cada coluna "Etapa"
            if (parametroCol.Width > availableClientWidth - (numberOfEtapaColumns * minWidthPerEtapaCol))
            {
                parametroCol.Width = Math.Max(minParametroColWidth, availableClientWidth - (numberOfEtapaColumns * minWidthPerEtapaCol));
                if (parametroCol.Width < minParametroColWidth) parametroCol.Width = minParametroColWidth; // Respeita o mínimo absoluto
                if (parametroCol.Width < 0) parametroCol.Width = 0; // Evita largura negativa
            }


            // 2. Distribui a largura restante para as colunas de "Etapa"
            int remainingWidthForEtapas = availableClientWidth - parametroCol.Width;
            if (remainingWidthForEtapas < 0) remainingWidthForEtapas = 0; // Evita largura negativa para etapas

            if (numberOfEtapaColumns > 0 && remainingWidthForEtapas > 0)
            {
                int baseEtapaWidth = remainingWidthForEtapas / numberOfEtapaColumns;
                int remainderEtapaWidth = remainingWidthForEtapas % numberOfEtapaColumns;

                for (int i = 0; i < numberOfEtapaColumns; i++)
                {
                    // As colunas "Etapa" começam no índice 1 (0 é "Parametro")
                    DataGridViewColumn etapaCol = dataGridView1.Columns[i + 1];
                    etapaCol.Width = baseEtapaWidth + (i < remainderEtapaWidth ? 1 : 0);
                    // Garante uma largura mínima para colunas de etapa, se possível.
                    if (etapaCol.Width < minWidthPerEtapaCol && baseEtapaWidth >= minWidthPerEtapaCol) etapaCol.Width = minWidthPerEtapaCol;
                    else if (etapaCol.Width < 0) etapaCol.Width = 0;
                }
            }
            else if (numberOfEtapaColumns > 0) // Não há espaço suficiente ou é negativo
            {
                for (int i = 0; i < numberOfEtapaColumns; i++)
                {
                    dataGridView1.Columns[i + 1].Width = Math.Max(0, Math.Min(minWidthPerEtapaCol, availableClientWidth / (numberOfEtapaColumns + 1))); // Tenta dar um mínimo, ou 0
                }
            }

            // Ajuste final para garantir que a soma das larguras das colunas não exceda ClientSize.Width
            int currentTotalColWidth = 0;
            foreach (DataGridViewColumn col in dataGridView1.Columns)
            {
                if (col.Visible) currentTotalColWidth += col.Width;
            }

            if (currentTotalColWidth < availableClientWidth && dataGridView1.Columns.Count > 1)
            {
                int slack = availableClientWidth - currentTotalColWidth;
                dataGridView1.Columns[dataGridView1.Columns.Count - 1].Width += slack; // Adiciona à última coluna de etapa
            }
            else if (currentTotalColWidth > availableClientWidth && dataGridView1.Columns.Count > 1)
            {
                int overflow = currentTotalColWidth - availableClientWidth;
                // Tenta reduzir das colunas de etapa, respeitando um mínimo
                for (int i = dataGridView1.Columns.Count - 1; i > 0 && overflow > 0; i--) // Começa da última etapa
                {
                    DataGridViewColumn etapaCol = dataGridView1.Columns[i];
                    int reduction = Math.Min(overflow, etapaCol.Width - minWidthPerEtapaCol);
                    if (reduction > 0)
                    {
                        etapaCol.Width -= reduction;
                        overflow -= reduction;
                    }
                }
            }


            // 3. Calcula e Define Alturas das Linhas
            int columnHeadersHeight = dataGridView1.ColumnHeadersVisible ? dataGridView1.ColumnHeadersHeight : 0;
            int availableHeightForRows = availableClientHeight - columnHeadersHeight;
            if (availableHeightForRows < 0) availableHeightForRows = 0;

            int numberOfRows = dataGridView1.Rows.Count;

            if (numberOfRows > 0 && availableHeightForRows > 0)
            {
                Font rowFont = dataGridView1.DefaultCellStyle.Font ?? dataGridView1.Font ?? SystemFonts.DefaultFont;
                // Altura mínima: altura da fonte + padding vertical da célula + pequena margem
                int cellVerticalPadding = dataGridView1.DefaultCellStyle.Padding.Vertical; // Soma do Padding.Top e Padding.Bottom
                int minPracticalRowHeight = rowFont.Height + cellVerticalPadding + 4; // 4px de margem interna adicional

                int baseRowHeight = availableHeightForRows / numberOfRows;
                int remainderRowHeight = availableHeightForRows % numberOfRows;

                int actualRowHeight = Math.Max(minPracticalRowHeight, baseRowHeight);

                int totalAppliedRowHeight = 0;
                for (int i = 0; i < numberOfRows; i++)
                {
                    if (!dataGridView1.Rows[i].IsNewRow)
                    {
                        int heightForRow = actualRowHeight;
                        // Distribui o resto apenas se a altura base já respeita o mínimo prático
                        if (baseRowHeight >= minPracticalRowHeight && i < remainderRowHeight)
                        {
                            heightForRow++;
                        }
                        dataGridView1.Rows[i].Height = heightForRow;
                        totalAppliedRowHeight += heightForRow;
                    }
                }

                // Ajuste final para altura das linhas se a soma não bater com availableHeightForRows
                if (totalAppliedRowHeight < availableHeightForRows && numberOfRows > 0)
                {
                    int diff = availableHeightForRows - totalAppliedRowHeight;
                    for (int i = 0; i < Math.Min(diff, numberOfRows); i++)
                    { // Distribui a diferença
                        dataGridView1.Rows[i].Height++;
                    }
                }
                else if (totalAppliedRowHeight > availableHeightForRows && numberOfRows > 0)
                {
                    int diff = totalAppliedRowHeight - availableHeightForRows;
                    for (int i = 0; i < Math.Min(diff, numberOfRows); i++)
                    { // Tenta reduzir
                        if (dataGridView1.Rows[i].Height > minPracticalRowHeight)
                        {
                            dataGridView1.Rows[i].Height--;
                        }
                    }
                }

                dataGridView1.RowTemplate.Height = (numberOfRows > 0 && availableHeightForRows > 0) ?
                                                   Math.Max(minPracticalRowHeight, availableHeightForRows / numberOfRows) :
                                                   minPracticalRowHeight;
            }
            else if (numberOfRows > 0) // Sem altura disponível para linhas (cabeçalho consome tudo?)
            {
                int minHeight = dataGridView1.Font?.Height ?? 10; // Um mínimo absoluto pequeno
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow) row.Height = Math.Max(1, minHeight / 2); // Torna as linhas efetivamente minúsculas ou 1
                }
                dataGridView1.RowTemplate.Height = Math.Max(1, minHeight / 2);
            }

            dataGridView1.ResumeLayout(true); // Aplica todas as alterações de layout
        }
        // ### FIM DO MÉTODO ATUALIZADO ###


        // Update the SetButtonState method to handle the CuoreUI.Controls.cuiButton type
        private void SetButtonState(Control button, bool enabled)
        {
            if (button != null && !button.IsDisposed)
            {
                if (button.InvokeRequired)
                {
                    button.BeginInvoke((MethodInvoker)delegate { button.Enabled = enabled; });
                }
                else
                {
                    button.Enabled = enabled;
                }

                // Additional handling for cuiButton type
                if (button is CuoreUI.Controls.cuiButton cuiButton)
                {
                    // Se 'Checked' deve refletir 'Enabled', então:
                    // cuiButton.Checked = enabled; 
                    // No entanto, 'Checked' geralmente tem outro propósito (botão de alternância).
                    // Ajuste conforme a necessidade semântica do seu cuiButton.
                    // Por exemplo, você pode querer mudar a aparência de outra forma:
                    // cuiButton.Style = enabled ? CuoreUI.Controls. cuiButton.ButtonStyles. normale : CuoreUI.Controls.cuiButton.ButtonStyles.disable;
                }
            }
        }



        private void InitializeAllCharts()
        {
            InitializeChart(chart1, "Vazão (LPM)", "Pressão (BAR)", "Curva de Desempenho Principal",
                new List<SeriesConfig> { new SeriesConfig("Pre.x Vaz.", SeriesChartType.FastLine, Color.Blue) },
                0, 100, 0, 300); // Exemplo de limites, ajuste conforme necessário
            InitializeChart(chart2, "Rotação (RPM)", "Dreno (LPM)", "Vazamento Interno / Dreno",
                new List<SeriesConfig> { new SeriesConfig("Vaz.In.X Rot", SeriesChartType.FastLine, Color.Red) },
                0, 3000, 0, 100); // Exemplo de limites
            InitializeChart(chart3, "Pressão de Carga (BAR)", "Dreno (LPM)", "Curva de Pressão", // Título Y parece ser Dreno, mas o X é Pressão Carga
                new List<SeriesConfig> { new SeriesConfig("Vaz. x Pres.", SeriesChartType.FastLine, Color.Orange) },
                0, 300, 0, 10); // Exemplo de limites

        }

        private void InitializeChart(Chart chart, string xAxisTitle, string yAxisTitle, string chartTitle, List<SeriesConfig> seriesConfigs, double xMin, double xMax, double yMin, double yMax)
        {
            if (chart == null || chart.IsDisposed) return;
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            ChartArea chartArea = new ChartArea(chartTitle.Replace(" ", "") + "Area");

            chartArea.AxisX.Title = xAxisTitle;
            chartArea.AxisX.Minimum = xMin;
            if (xMax > xMin) chartArea.AxisX.Maximum = xMax; else chartArea.AxisX.Maximum = xMin + 1; // Evitar max <= min
            chartArea.AxisX.LabelStyle.Format = "F0"; // Formato para números inteiros ou com poucas casas decimais

            chartArea.AxisY.Title = yAxisTitle;
            chartArea.AxisY.Minimum = yMin;
            if (yMax > yMin) chartArea.AxisY.Maximum = yMax; else chartArea.AxisY.Maximum = yMin + 1; // Evitar max <= min
            chartArea.AxisY.LabelStyle.Format = "F0"; // Ajuste conforme os dados esperados

            chartArea.CursorX.IsUserEnabled = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.CursorY.IsUserEnabled = true;
            chartArea.CursorY.IsUserSelectionEnabled = true;
            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisY.ScaleView.Zoomable = true;

            chart.ChartAreas.Add(chartArea);

            foreach (var sc in seriesConfigs)
            {
                Series series = new Series(sc.Name)
                {
                    ChartType = sc.Type,
                    Color = sc.Color,
                    BorderWidth = 2
                };
                chart.Series.Add(series);
            }
            chart.Titles.Clear();
            chart.Titles.Add(new Title(chartTitle, Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black));
        }
        private void InitializeChartWithSecondaryAxis(Chart chart, string xAxisTitle, string yAxisTitlePrimary, string yAxisTitleSecondary, string chartTitle, List<SeriesConfig> seriesConfigs, double xMin, double xMax, double yMinPrimary, double yMaxPrimary, double yMinSecondary, double yMaxSecondary)
        {
            if (chart == null || chart.IsDisposed) return;
            chart.Series.Clear();
            chart.ChartAreas.Clear();
            ChartArea chartArea = new ChartArea(chartTitle.Replace(" ", "") + "Area");

            chartArea.AxisX.Title = xAxisTitle;
            chartArea.AxisX.Minimum = xMin;
            if (xMax > xMin) chartArea.AxisX.Maximum = xMax; else chartArea.AxisX.Maximum = xMin + 1;
            chartArea.AxisX.LabelStyle.Format = "F0";


            chartArea.AxisY.Title = yAxisTitlePrimary;
            chartArea.AxisY.Minimum = yMinPrimary;
            if (yMaxPrimary > yMinPrimary) chartArea.AxisY.Maximum = yMaxPrimary; else chartArea.AxisY.Maximum = yMinPrimary + 1;
            chartArea.AxisY.LineColor = Color.Black; // Cor do eixo primário
            chartArea.AxisY.LabelStyle.ForeColor = Color.Black;
            chartArea.AxisY.TitleForeColor = Color.Black;
            chartArea.AxisY.LabelStyle.Format = "F0";


            chartArea.AxisY2.Enabled = AxisEnabled.True;
            chartArea.AxisY2.Title = yAxisTitleSecondary;
            chartArea.AxisY2.Minimum = yMinSecondary;
            if (yMaxSecondary > yMinSecondary) chartArea.AxisY2.Maximum = yMaxSecondary; else chartArea.AxisY2.Maximum = yMinSecondary + 1;
            chartArea.AxisY2.LineColor = Color.DarkRed; // Cor do eixo secundário
            chartArea.AxisY2.LabelStyle.ForeColor = Color.DarkRed;
            chartArea.AxisY2.TitleForeColor = Color.DarkRed;
            chartArea.AxisY2.MajorGrid.Enabled = false; // Desabilita grid para o eixo Y secundário para clareza
            chartArea.AxisY2.LabelStyle.Format = "F0";


            chartArea.CursorX.IsUserEnabled = true;
            chartArea.CursorX.IsUserSelectionEnabled = true;
            chartArea.CursorY.IsUserEnabled = true; // Habilita cursor para ambos os eixos Y
            chartArea.CursorY.IsUserSelectionEnabled = true; // Habilita seleção para ambos os eixos Y

            chartArea.AxisX.ScaleView.Zoomable = true;
            chartArea.AxisY.ScaleView.Zoomable = true;
            chartArea.AxisY2.ScaleView.Zoomable = true;

            chart.ChartAreas.Add(chartArea);

            foreach (var sc in seriesConfigs)
            {
                Series series = new Series(sc.Name)
                {
                    ChartType = sc.Type,
                    Color = sc.Color,
                    BorderWidth = 2,
                    YAxisType = sc.Axis // Define se a série usa o eixo primário ou secundário
                };
                chart.Series.Add(series);
            }
            chart.Titles.Clear();
            chart.Titles.Add(new Title(chartTitle, Docking.Top, new Font("Arial", 12, FontStyle.Bold), Color.Black));
        }

        #region LOADS_JANELA
        private void Tela_Bombas_Load(object sender, EventArgs e)
        {
            _viewModel.Carregar_configuracao(this);
            if (Stage_box_bomba != null) _viewModel.Stage_signal(Stage_box_bomba);
            if (LabelHorariotela != null) _viewModel.VincularRelogioLabel(LabelHorariotela);
            LogHistoricalEvent("AGUARDANDO INÍCIO DO ENSAIO...", Color.DarkGreen);
        }
        #endregion

        #region EVENTOS_FECHAMANETO
        private void CloseWindows_Click(object sender, EventArgs e)
        {
            // Este método chama btnretornar_Click que já tem lógica de confirmação
            btnretornar_Click(sender, e);
        }

        private void Tela_Bombas_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_fechamentoControladoPeloUsuario) // Se o fechamento não foi iniciado por btnretornar_Click
            {
                DialogResult dr = MessageBox.Show("Deseja fechar toda a aplicação?", "Sair", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == DialogResult.Yes)
                {
                    StopAllOperationsAndQuit(true); // Fecha a aplicação inteira
                }
                else
                {
                    e.Cancel = true; // Cancela o fechamento do formulário
                }
            }
            else // Se _fechamentoControladoPeloUsuario é true, significa que veio de btnretornar_Click
            {
                StopAllOperationsAndQuit(false); // Apenas para este formulário, não sai da aplicação
            }
        }

        private void StopAllOperationsAndQuit(bool exitApplication = true)
        {
            StopTimers();
            StopSerialConnection(); // Garante que a conexão serial seja interrompida

            // Dispose de timers para liberar recursos
            _updateUiTimer?.Dispose();
            _updateUiTimer = null;
            monitoramentoTimer?.Dispose();
            monitoramentoTimer = null;
            timerCronometro?.Dispose();
            timerCronometro = null;

            if (exitApplication)
            {
                ConnectionSettingsApplication.CloseAllConnections(); // Fecha conexões globais se houver
                Environment.Exit(Environment.ExitCode); // Termina a aplicação
            }
        }
        #endregion

        #region INCIO_TESTE
        public void btnIniciar_Click(object sender, EventArgs e)
        {
            // Explicitly specify the namespace to resolve ambiguity
            System.Windows.Forms.TextBox tb6 = this.Controls.Find("textBox6", true).FirstOrDefault() as System.Windows.Forms.TextBox;
            System.Windows.Forms.TextBox tb5 = this.Controls.Find("textBox5", true).FirstOrDefault() as System.Windows.Forms.TextBox;
            System.Windows.Forms.TextBox tb4 = this.Controls.Find("textBox4", true).FirstOrDefault() as System.Windows.Forms.TextBox;

            if (tb6 == null || tb5 == null || tb4 == null || !_viewModel.cabecalhoinicial(tb6, tb5, tb4))
            {
                MessageBox.Show("Favor preencher os campos obrigatórios em DADOS DE ENSAIO.", "Campos Obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _isMonitoring = true;
            Inicioteste = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            if (Stage_box_bomba != null) _viewModel.IniciarTesteBomba(Stage_box_bomba);

            InicializarMonitoramento(); // Inicializa o timer de monitoramento (se houver lógica nele)
            _timeCounterSecondsRampa = 0; // Contador para os gráficos (se usado)
            etapaAtual = 1; // Reseta a etapa atual para o DataGridView estático

            if (tempoCronometroDefinidoManualmente && valorDefinidoCronometro > 0)
            {
                cronometroIniciado = true;
                int tempoTotal = valorDefinidoCronometro * 60; // Convertido para segundos
                if (circularProgressBar1 != null)
                {
                    circularProgressBar1.Maximum = tempoTotal > 0 ? tempoTotal : 1; // Garante que Maximum > 0
                    circularProgressBar1.Minimum = 0;
                    circularProgressBar1.Value = tempoTotal;
                    circularProgressBar1.Invalidate(); // Força redesenho
                }
                timerCronometro.Start();
            }
            else
            {
                // Se o cronômetro não foi definido, não inicia e avisa (ou não, dependendo do requisito)
                // MessageBox.Show("O cronômetro não foi definido. O teste não será finalizado automaticamente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                // Ou simplesmente não inicia o cronômetro.
                if (circularProgressBar1 != null)
                { // Reseta a barra de progresso se não houver tempo definido
                    circularProgressBar1.Value = 0;
                    circularProgressBar1.Maximum = 100; // Um valor padrão
                }
            }

            SetButtonState(btngravar, true);
            SetButtonState(bntFinalizar, true);
            SetButtonState(btnreset, true);
            SetButtonState(btnrelatoriobomba, false); // Habilita relatório apenas após finalizar
            SetButtonState(btniniciarteste, false);
            LogHistoricalEvent("INICIADO ENSAIO DE BOMBAS", Color.Blue);

            ClearCharts();
            _viewModel.ResetChartDataLogic(); // Lógica de reset de dados do ViewModel para gráficos
            ClearStaticDataGridViewCells(); // Limpa as células de dados do DataGridView
            StartSerialConnection(); // Inicia a comunicação serial
        }

        private void InicializarMonitoramento()
        {
            if (monitoramentoTimer == null)
            {
                monitoramentoTimer = new System.Windows.Forms.Timer();
                monitoramentoTimer.Interval = 1000; // Intervalo do timer
                // monitoramentoTimer.Tick += MonitoramentoTimer_Tick; // Descomente se tiver lógica de monitoramento aqui
            }
            // monitoramentoTimer.Start(); // Inicia o timer se houver lógica nele
        }

        private void PararMonitoramento()
        {
            monitoramentoTimer?.Stop();
        }

        //private void MonitoramentoTimer_Tick(object sender, EventArgs e)
        //{
        //    // Se houver lógica de validação de limites ou alarmes periódicos, ela iria aqui.
        //    // O código original está comentado, então mantendo assim.
        //}
        #endregion

        #region Serial Communication and Data Processing

        private void StartSerialConnection()
        {
            try
            {
                string portToConnect = ConnectionSettingsApplication.CurrentPortName;
                int baudRateToConnect = ConnectionSettingsApplication.CurrentBaudRate;

                if (string.IsNullOrEmpty(portToConnect) || baudRateToConnect <= 0)
                {
                    string errorMessage = "Configurações de porta serial (Nome da Porta e/ou Baud Rate) não definidas.";
                    MessageBox.Show(this, errorMessage, "Configuração Serial Ausente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LogHistoricalEvent($"ERRO - {errorMessage}", Color.Red);
                    // Considerar reverter estado dos botões se a conexão falhar no início.
                    btnParar_Click(this, EventArgs.Empty); // Chama o método de parada para resetar estado
                    return;
                }

                if (_serialManager == null)
                {
                    // Isso não deveria acontecer se _serialManager é inicializado no construtor ou globalmente.
                    LogHistoricalEvent("Erro crítico: SerialManager não inicializado antes de StartSerialConnection.", Color.Red);
                    btnParar_Click(this, EventArgs.Empty);
                    return;
                }

                // Remove handlers antigos antes de adicionar novos para evitar duplicação
                _serialManager.DataReceived -= SerialManager_DataReceived;
                _serialManager.DataReceived += SerialManager_DataReceived;
                _serialManager.ErrorOccurred -= SerialManager_ErrorOccurred;
                _serialManager.ErrorOccurred += SerialManager_ErrorOccurred;

                // Se já conectado com configurações diferentes, desconecta primeiro
                if (_serialManager.IsConnected &&
                    (_serialManager.PortName != portToConnect || _serialManager.BaudRate != baudRateToConnect))
                {
                    _serialManager.Disconnect();
                }

                if (!_serialManager.IsConnected) // Tenta conectar apenas se não estiver conectado
                {
                    _serialManager.Connect(portToConnect, baudRateToConnect);
                }


                if (_serialManager.IsConnected)
                {
                    LogHistoricalEvent($"Conectado à porta serial {portToConnect} ({baudRateToConnect} baud).", Color.Green);
                    StartUpdateUiTimer(); // Inicia o timer para atualizar a UI com dados seriais
                }
                else
                {
                    string errorMessage = $"ERRO ao conectar à porta serial {portToConnect}. Verifique as configurações ou se a porta está disponível.";
                    LogHistoricalEvent(errorMessage, Color.Red);
                    MessageBox.Show(this, errorMessage, "Erro de Conexão Serial", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnParar_Click(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                string attemptedPortName = ConnectionSettingsApplication.CurrentPortName;
                string errorMessage = $"Erro durante a tentativa de conexão serial {(string.IsNullOrEmpty(attemptedPortName) ? "" : $"à porta {attemptedPortName}")}: {ex.Message}";
                MessageBox.Show(this, errorMessage, "Erro de Conexão Serial Exceção", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogHistoricalEvent(errorMessage, Color.Red);
                btnParar_Click(this, EventArgs.Empty);
            }
        }

        private void StopSerialConnection()
        {
            StopUpdateUiTimer(); // Para o timer que lê os dados para a UI
            if (_serialManager != null)
            {
                // Remove handlers para evitar chamadas após desconexão ou em instâncias futuras
                _serialManager.DataReceived -= SerialManager_DataReceived;
                _serialManager.ErrorOccurred -= SerialManager_ErrorOccurred;
                if (_serialManager.IsConnected)
                {
                    try
                    {
                        _serialManager.Disconnect();
                        LogHistoricalEvent($"Desconectado da porta serial.", Color.Orange);
                    }
                    catch (Exception ex)
                    {
                        LogHistoricalEvent($"ERRO ao desconectar da porta serial: {ex.Message}", Color.Red);
                    }
                }
            }
        }

        private void SerialManager_ErrorOccurred(object sender, string errorMessage)
        {
            // Garante que a UI seja atualizada na thread correta e apenas se o form não estiver descartado
            if (this.IsDisposed || this.Disposing) return;
            BeginInvoke((MethodInvoker)delegate
            {
                if (!this.IsDisposed && !this.Disposing) // Verifica novamente dentro do delegate
                {
                    LogHistoricalEvent($"ERRO SERIAL: {errorMessage}", Color.Red);
                }
            });
        }

        private void SerialManager_DataReceived(object sender, string data)
        {
            if (string.IsNullOrEmpty(data)) return;
            // Filtra caracteres não imprimíveis ANTES de adicionar ao buffer
            string cleanData = FilterNonPrintableChars(data);
            if (string.IsNullOrEmpty(cleanData)) return;

            Debug.WriteLine($"[RAW_CLEAN] Dados: {cleanData.Replace("\n", "\\n").Replace("\r", "\\r")}");

            lock (serialBufferLock)
            {
                serialDataBuffer.Append(cleanData);
                ProcessSerialBuffer(); // Processa o buffer imediatamente após adicionar novos dados
            }
        }

        private void ProcessSerialBuffer()
        {
            string bufferContent;
            int newlineIndex;

            // Não precisa de lock aqui se ProcessSerialBuffer SÓ é chamado de dentro de um lock em serialBufferLock
            // No entanto, se puder ser chamado de outros lugares, o lock é necessário.
            // Para segurança, mantemos o lock se houver dúvida.
            // Mas como está sendo chamado de SerialManager_DataReceived que já tem lock, podemos otimizar.
            // Vou remover o lock interno para evitar deadlocks se chamado recursivamente (improvável aqui).

            // bufferContent = serialDataBuffer.ToString(); // Leitura fora do loop para evitar múltiplas conversões

            while (true) // Loop para processar todas as mensagens completas no buffer
            {
                lock (serialBufferLock) // Lock para ler e modificar o buffer
                {
                    bufferContent = serialDataBuffer.ToString();
                    newlineIndex = bufferContent.IndexOf('\n');
                }

                if (newlineIndex >= 0)
                {
                    string completeMessage = bufferContent.Substring(0, newlineIndex + 1).Trim(); // Trim para remover espaços/CR antes/depois

                    lock (serialBufferLock) // Lock para modificar o buffer
                    {
                        serialDataBuffer.Remove(0, newlineIndex + 1);
                    }

                    if (!string.IsNullOrWhiteSpace(completeMessage))
                    {
                        Debug.WriteLine($"[PROCESS] Linha completa: {completeMessage}");
                        // ParseAndStoreSensorData(completeMessage); // Descomente e implemente esta função
                    }
                }
                else
                {
                    break; // Nenhuma nova linha completa encontrada, sai do loop
                }
            }
        }


        private string FilterNonPrintableChars(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            StringBuilder sb = new StringBuilder(input.Length);
            foreach (char c in input)
            {
                // Mantém LF, CR, TAB. Remove outros caracteres de controle.
                if (char.IsControl(c) && c != '\n' && c != '\r' && c != '\t')
                    continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

        // Implemente ParseAndStoreSensorData para processar a 'completeMessage'
        // Exemplo (precisa ser adaptado à sua string de dados):
        // private void ParseAndStoreSensorData(string data)
        // {
        //     // Ex: "P1:123.4|fluxo1:56.7|Piloto1:8.9|dreno1:1.2|RPM:1500|temp:25.5"
        //     string[] pairs = data.Split('|');
        //     Dictionary<string, double> currentReadings = new Dictionary<string, double>();
        //     bool success = true;

        //     foreach (string pair in pairs)
        //     {
        //         string[] keyValue = pair.Split(':');
        //         if (keyValue.Length == 2)
        //         {
        //             string key = keyValue[0].Trim();
        //             if (double.TryParse(keyValue[1].Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
        //             {
        //                 currentReadings[key] = value;
        //             }
        //             else
        //             {
        //                 Debug.WriteLine($"[PARSE_ERROR] Valor inválido para {key}: {keyValue[1]}");
        //                 success = false; break;
        //             }
        //         }
        //         else
        //         {
        //              Debug.WriteLine($"[PARSE_ERROR] Par chave-valor malformado: {pair}");
        //              success = false; break;
        //         }
        //     }

        //     if (success && currentReadings.Count > 0) // Verifique se tem o número esperado de chaves
        //     {
        //         lock (readingsLock)
        //         {
        //             // Atualiza _currentNumericSensorReadings com os novos valores
        //             foreach(var entry in currentReadings)
        //             {
        //                _currentNumericSensorReadings[entry.Key] = entry.Value;
        //             }
        //         }
        //     }
        // }


        private void StartUpdateUiTimer()
        {
            if (_updateUiTimer == null) // Recria se for nulo (após um Dispose, por exemplo)
            {
                _updateUiTimer = new Timer();
                _updateUiTimer.Interval = 150; // Intervalo de atualização da UI
              //  _updateUiTimer.Tick += UpdateUiTimer_Tick;
            }
            if (!_updateUiTimer.Enabled)
            {
                _updateUiTimer.Start();
            }
        }

        private void StopUpdateUiTimer()
        {
            _updateUiTimer?.Stop();
            // Não faz Dispose aqui, pois pode ser reiniciado. Dispose é feito em StopAllOperationsAndQuit.
        }

        //private void UpdateUiTimer_Tick(object sender, EventArgs e)
        //{
        //    // Este timer é responsável por pegar os dados processados e atualizar a UI
        //    UpdateDisplay(); // Atualiza TextBoxes
        //    if (_isMonitoring)
        //    {
        //        _timeCounterSecondsRampa++; // Usado para eixos X baseados em tempo, se aplicável
        //        AddDataPointsToCharts(); // Adiciona dados aos gráficos
        //    }
        //}

        //private void UpdateDisplay()
        //{
        //    if (this.IsDisposed || this.Disposing) return;
        //    // Usa BeginInvoke para marshaling para a thread da UI de forma assíncrona
        //    if (this.InvokeRequired)
        //    {
        //        try { this.BeginInvoke((MethodInvoker)UpdateTextBoxes); }
        //        catch (ObjectDisposedException) { } // Ignora se o form for descartado durante o BeginInvoke
        //        catch (InvalidOperationException) { } // Ignora se o handle do form não foi criado ou foi invalidado
        //    }
        //    else
        //    {
        //        UpdateTextBoxes();
        //    }
        //}

        //private void UpdateTextBoxes()
        //{
        //    if (this.IsDisposed || this.Disposing) return; // Checagem final

        //    Dictionary<string, double> readingsSnapshot;
        //    lock (readingsLock) // Acessa o dicionário de leituras de forma segura
        //    {
        //        // Cria uma cópia para evitar manter o lock durante as atualizações da UI
        //        readingsSnapshot = new Dictionary<string, double>(_currentNumericSensorReadings);
        //    }

        //    // Mapeamento de chaves seriais para TextBoxes e conversões
        //    // IMPORTANTE: As chaves ("P1", "fluxo1", etc.) devem corresponder às chaves usadas em ParseAndStoreSensorData
        //    // E os nomes dos TextBoxes (sensor_bar_PL, etc.) devem existir no seu Form.

        //    // Exemplo de atualização para sensor_bar_PL e sensor_psi_PL (Pressão Pilotagem)
        //    if (readingsSnapshot.TryGetValue("Piloto1", out double pilotoVal)) // "Piloto1" é um exemplo de chave
        //    {
        //        UpdateTextBoxIfAvailable(sensor_bar_PL, pilotoVal.ToString("F2", CultureInfo.InvariantCulture));
        //        UpdateTextBoxIfAvailable(sensor_psi_PL, (pilotoVal * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture));
        //    }
        //    else
        //    {
        //        UpdateTextBoxIfAvailable(sensor_bar_PL, "N/A");
        //        UpdateTextBoxIfAvailable(sensor_psi_PL, "N/A");
        //    }

        //    // Exemplo para Dreno (dreno1)
        //    if (readingsSnapshot.TryGetValue("dreno1", out double drenoVal))
        //    {
        //        UpdateTextBoxIfAvailable(sensor_lpm_DR, drenoVal.ToString("F2", CultureInfo.InvariantCulture));
        //        UpdateTextBoxIfAvailable(sensor_gpm_DR, (drenoVal * LPM_TO_GPM_USER_CONVERSION).ToString("F2", CultureInfo.InvariantCulture));
        //    }
        //    else
        //    {
        //        UpdateTextBoxIfAvailable(sensor_lpm_DR, "N/A");
        //        UpdateTextBoxIfAvailable(sensor_gpm_DR, "N/A");
        //    }

        //    // Exemplo para Pressão Principal (P1)
        //    if (readingsSnapshot.TryGetValue("P1", out double p1Val))
        //    {
        //        UpdateTextBoxIfAvailable(sensor_Press_BAR, p1Val.ToString("F2", CultureInfo.InvariantCulture));
        //        UpdateTextBoxIfAvailable(sensor_Press_PSI, (p1Val * BAR_TO_PSI_CONVERSION).ToString("F2", CultureInfo.InvariantCulture));
        //    }
        //    else
        //    {
        //        UpdateTextBoxIfAvailable(sensor_Press_BAR, "N/A");
        //        UpdateTextBoxIfAvailable(sensor_Press_PSI, "N/A");
        //    }

        //    // Exemplo para Rotação (RPM)
        //    if (readingsSnapshot.TryGetValue("RPM", out double rpmVal))
        //    {
        //        UpdateTextBoxIfAvailable(sensor_rotacao_RPM, rpmVal.ToString("F0", CultureInfo.InvariantCulture)); // Sem casas decimais
        //    }
        //    else
        //    {
        //        UpdateTextBoxIfAvailable(sensor_rotacao_RPM, "N/A");
        //    }

        //    // Exemplo para Vazão Principal (fluxo1)
        //    if (readingsSnapshot.TryGetValue("fluxo1", out double fluxoVal))
        //    {
        //        UpdateTextBoxIfAvailable(sensor_Vazao_LPM, fluxoVal.ToString("F2", CultureInfo.InvariantCulture));
        //        UpdateTextBoxIfAvailable(sensor_Vazao_GPM, (fluxoVal * LPM_TO_GPM_USER_CONVERSION).ToString("F2", CultureInfo.InvariantCulture));
        //    }
        //    else
        //    {
        //        UpdateTextBoxIfAvailable(sensor_Vazao_LPM, "N/A");
        //        UpdateTextBoxIfAvailable(sensor_Vazao_GPM, "N/A");
        //    }

        //    // Exemplo para Temperatura (temp)
        //    if (readingsSnapshot.TryGetValue("temp", out double tempVal))
        //    {
        //        UpdateTextBoxIfAvailable(sensor_Temp_C, tempVal.ToString("F1", CultureInfo.InvariantCulture)); // Uma casa decimal
        //    }
        //    else
        //    {
        //        UpdateTextBoxIfAvailable(sensor_Temp_C, "N/A");
        //    }

        //    // Adicione aqui a lógica para os demais TextBoxes (sensor_MA1, sensor_MA2, etc.)
        //    // que são referenciados em _staticDataGridViewParameters, se eles devem ser atualizados em tempo real.
        //    // Ex: se "sensor_MA1" corresponde a uma chave serial "S_MA1":
        //    // if (readingsSnapshot.TryGetValue("S_MA1", out double ma1Val)) { UpdateTextBoxIfAvailable(sensor_MA1, ma1Val.ToString("F2")); }
        //    // else { UpdateTextBoxIfAvailable(sensor_MA1, "N/A"); }
        //    // Repita para sensor_MA2, sensor_MB1, sensor_MB2, sensor_P1, sensor_P2, etc.
        //    // Se esses TextBoxes são preenchidos por outra lógica ou diretamente pelo usuário, não precisa mexer.
        //}


        private void UpdateTextBoxIfAvailable(TextBox textBox, string value)
        {
            if (textBox != null && !textBox.IsDisposed)
            {
                textBox.Text = value;
            }
        }

        //private void AddDataPointsToCharts()
        //{
        //    if (this.IsDisposed || this.Disposing) return;
        //    // Usa BeginInvoke para marshaling para a thread da UI de forma assíncrona
        //    if (this.InvokeRequired)
        //    {
        //        try { this.BeginInvoke((MethodInvoker)AddDataPointsToChartsInternal); }
        //        catch (ObjectDisposedException) { }
        //        catch (InvalidOperationException) { }
        //    }
        //    else
        //    {
        //        AddDataPointsToChartsInternal();
        //    }
        //}

        //private void AddDataPointsToChartsInternal()
        //{
        //    if (this.IsDisposed || this.Disposing) return; // Checagem final
        //    Dictionary<string, double> readingsSnapshot;
        //    lock (readingsLock) // Acessa o dicionário de leituras de forma segura
        //    {
        //        readingsSnapshot = new Dictionary<string, double>(_currentNumericSensorReadings);
        //    }

        //    // Verifica se todas as chaves necessárias para os gráficos existem no snapshot
        //    bool chart1DataOk = readingsSnapshot.TryGetValue("P1", out double pressaoBar) &&
        //                        readingsSnapshot.TryGetValue("fluxo1", out double vazaoLpm);
        //    bool chart2DataOk = readingsSnapshot.TryGetValue("RPM", out double rotacaoRpm) &&
        //                        readingsSnapshot.TryGetValue("dreno1", out double drenoLpm); // dreno1 para chart2
        //    bool chart3DataOk = readingsSnapshot.TryGetValue("Piloto1", out double pilotagemRaw) &&
        //                        readingsSnapshot.ContainsKey("dreno1"); // Reusa drenoLpm de chart2DataOk se disponível

        //    if (chart1DataOk)
        //    {
        //        // Lógica do ViewModel para Chart 1 (se houver condição de rotação constante)
        //        Tela_BombasVM.Datapoint_Bar_Lpm? dataPoint = _viewModel.GetChartDataIfRotationConstant(
        //              pressaoBar.ToString(CultureInfo.InvariantCulture),
        //              vazaoLpm.ToString(CultureInfo.InvariantCulture),
        //              readingsSnapshot.TryGetValue("RPM", out double rpmForChart1) ? rpmForChart1.ToString(CultureInfo.InvariantCulture) : "0" // Passa RPM se disponível
        //           );

        //        if (chart1 != null && !chart1.IsDisposed && chart1.Series.Count > 0 && chart1.Series.IndexOf("Pre.x Vaz.") != -1)
        //        {
        //            if (dataPoint.HasValue) // Adiciona ponto apenas se o ViewModel retornar
        //            {
        //                chart1.Series["Pre.x Vaz."].Points.AddXY(dataPoint.Value.FlowLpm, dataPoint.Value.PressureBar);
        //            }
        //            // Se não houver lógica no ViewModel para filtrar, adiciona diretamente:
        //            // chart1.Series["Pre.x Vaz."].Points.AddXY(vazaoLpm, pressaoBar);
        //        }
        //    }

        //    if (chart2DataOk)
        //    {
        //        if (chart2 != null && !chart2.IsDisposed && chart2.Series.Count > 0 && chart2.Series.IndexOf("Vaz.In.X Rot") != -1)
        //        {
        //            chart2.Series["Vaz.In.X Rot"].Points.AddXY(rotacaoRpm, drenoLpm);
        //        }
        //    }

        //    if (chart3DataOk) // Reutiliza drenoLpm se chart2DataOk for true, senão pega novamente
        //    {
        //        double drenoForChart3 = chart2DataOk ? drenoLpm : (readingsSnapshot.TryGetValue("dreno1", out double drn3) ? drn3 : 0.0);
        //        double pilotagemBarConvertida = pilotagemRaw * BAR_CONVERSION_PILOT; // Conversão da pressão de pilotagem

        //        if (chart3 != null && !chart3.IsDisposed && chart3.Series.Count > 0 && chart3.Series.IndexOf("Vaz. x Pres.") != -1)
        //        {
        //            chart3.Series["Vaz. x Pres."].Points.AddXY(pilotagemBarConvertida, drenoForChart3);
        //        }
        //    }
        //}


        private void ClearCharts()
        {
            if (this.IsDisposed || this.Disposing) return;

            // Verifica se algum chart existe e se Invoke é necessário (se chamado de outra thread)
            var chartToCheck = chart1 ?? chart2 ?? chart3; // Pega o primeiro não nulo
            if (chartToCheck != null && !chartToCheck.IsDisposed && chartToCheck.InvokeRequired)
            {
                try { this.BeginInvoke((MethodInvoker)ClearChartsInternal); }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
            }
            else
            {
                ClearChartsInternal();
            }
        }

        private void ClearChartsInternal()
        {
            if (this.IsDisposed || this.Disposing) return;
            var charts = new List<Chart> { chart1, chart2, chart3 }; // Adicione outros charts se houver
            foreach (Chart chart in charts)
            {
                if (chart != null && !chart.IsDisposed)
                {
                    foreach (var series in chart.Series)
                    {
                        series.Points.Clear();
                    }
                }
            }
        }

        private void StopTimers()
        {
            timerCronometro?.Stop();
            monitoramentoTimer?.Stop(); // Se estiver usando para algo
            StopUpdateUiTimer(); // Para o timer da UI
        }

        private void LogHistoricalEvent(string message, Color? color = null)
        {
            if (HistoricalEvents == null || HistoricalEvents.IsDisposed || this.IsDisposed || this.Disposing) return;

            // Marshaling para a thread da UI de forma segura
            if (HistoricalEvents.InvokeRequired)
            {
                try
                {
                    HistoricalEvents.BeginInvoke((MethodInvoker)delegate {
                        AppendLogMessage(message, color); // Chama o método que realmente adiciona o texto
                    });
                }
                catch (ObjectDisposedException) { }
                catch (InvalidOperationException) { }
            }
            else
            {
                AppendLogMessage(message, color);
            }
        }
        private void AppendLogMessage(string message, Color? color = null) // Método auxiliar
        {
            if (HistoricalEvents == null || HistoricalEvents.IsDisposed) return; // Checagem dupla

            // Limita o tamanho do log para evitar consumo excessivo de memória
            if (HistoricalEvents.TextLength > 15000) // Aumentado um pouco o limite
            {
                HistoricalEvents.Text = HistoricalEvents.Text.Substring(HistoricalEvents.TextLength - 5000); // Mantém os últimos 5000 chars
            }

            HistoricalEvents.SelectionStart = HistoricalEvents.TextLength;
            HistoricalEvents.SelectionLength = 0;

            // Aplica cor se especificada (não implementado no RichTextBox padrão sem mais código)
            // Para simplicidade, apenas adiciona o texto. Para cor, precisaria de RichTextBox e SelectionColor.
            // if (color.HasValue) HistoricalEvents.SelectionColor = color.Value;

            HistoricalEvents.AppendText($"{DateTime.Now:G}: {message}" + Environment.NewLine);
            // if (color.HasValue) HistoricalEvents.SelectionColor = HistoricalEvents.ForeColor; // Reseta a cor

            HistoricalEvents.ScrollToCaret(); // Garante que o último log seja visível
        }
        #endregion

        #region FIM_TESTE
        private void btnParar_Click(object sender, EventArgs e)
        {
            StopTimers(); // Para todos os timers ativos
            StopSerialConnection(); // Para a comunicação serial

            cronometroIniciado = false; // Reseta flag do cronômetro
            _isMonitoring = false; // Reseta flag de monitoramento
            Fimteste = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); // Registra fim do teste

            // Configura o estado dos botões
            SetButtonState(btngravar, false); // Desabilita gravar após finalizar
            SetButtonState(bntFinalizar, false); // Desabilita finalizar (já finalizado)
            SetButtonState(btnreset, true); // Habilita reset
            SetButtonState(btnrelatoriobomba, true); // Habilita gerar relatório
            SetButtonState(btniniciarteste, true); // Habilita iniciar novo teste

            LogHistoricalEvent("ENSAIO DE BOMBAS FINALIZADO", Color.Red);
            if (Stage_box_bomba != null) _viewModel.FinalizarTesteBomba(Stage_box_bomba); // Sinaliza ViewModel
        }
        #endregion

        #region RESET
        private void btnretornar_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Tem certeza que deseja reiniciar o processo e retornar ao menu?\nTodos os dados não salvos em relatório serão perdidos!",
                "Confirmação de Reinício",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.No)
            {
                return; // Usuário cancelou, não faz nada
            }

            _fechamentoControladoPeloUsuario = true; // Sinaliza que o fechamento é intencional

            StopTimers();
            StopSerialConnection();
            _isMonitoring = false;
            cronometroIniciado = false;

            _timeCounterSecondsRampa = 0;
            etapaAtual = 1; // Reseta contador de etapas
            if (dadosSensores != null) dadosSensores.Clear(); // Limpa lista de dados de sensores coletados
            if (_dadosColetados != null) _dadosColetados.Clear(); // Limpa lista de dados de etapas


            Inicioteste = string.Empty; // Reseta timestamps
            Fimteste = string.Empty;

            lock (serialBufferLock) // Limpa buffer serial
            {
                serialDataBuffer.Clear();
            }
            lock (readingsLock) // Limpa leituras atuais
            {
                _currentNumericSensorReadings.Clear();
            }

           // UpdateTextBoxes(); // Atualiza TextBoxes para mostrar "N/A" ou valores zerados
            ClearStaticDataGridViewCells(); // Limpa os dados das etapas no DataGridView

            // Reseta CircularProgressBar
            if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
            {
                circularProgressBar1.Value = 0;
                // Define um máximo padrão se não houver tempo definido, ou o tempo definido se houver
                circularProgressBar1.Maximum = (tempoCronometroDefinidoManualmente && valorDefinidoCronometro > 0) ? (valorDefinidoCronometro * 60) : 100;
                circularProgressBar1.Invalidate();
            }

            ClearCharts(); // Limpa todos os gráficos
            _viewModel.ResetChartDataLogic(); // Pede ao ViewModel para resetar sua lógica de dados de gráfico

            if (Stage_box_bomba != null) _viewModel.FinalizarTesteBomba(Stage_box_bomba); // Sinaliza ViewModel
            LogHistoricalEvent("AGUARDANDO INÍCIO DO ENSAIO...", Color.DarkGreen);

            // Restaura estado inicial dos botões
            SetButtonState(btniniciarteste, true);
            SetButtonState(btngravar, false);
            SetButtonState(bntFinalizar, false);
            SetButtonState(btnreset, false); // Reset fica desabilitado até novo início
            SetButtonState(btnrelatoriobomba, false); // Relatório desabilitado

            try
            {
                // Tenta mostrar o MenuApp
                var menuAppInstance = Menuapp.Instance; // Supondo que Menuapp.Instance é a forma de acesso
                if (menuAppInstance != null && !menuAppInstance.IsDisposed)
                {
                    menuAppInstance.Show();
                    menuAppInstance.BringToFront(); // Traz para frente
                }
                else
                {
                    // Se Menuapp.Instance é nulo ou descartado, pode ser necessário recriá-lo
                    // ou logar que não foi possível retornar ao menu.
                    Debug.WriteLine("btnretornar_Click: Menuapp.Instance não disponível ou descartado.");
                    // Exemplo: new Menuapp().Show(); // Se for o caso de recriar
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"btnretornar_Click: Exceção ao tentar mostrar Menuapp: {ex.Message}");
                // Pode ser útil mostrar uma mensagem ao usuário se o menu não puder ser aberto.
            }
            this.Close(); // Fecha o formulário Tela_Bombas
        }
        #endregion

        private void ClearStaticDataGridViewCells()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed) return;

            Action action = () => {
                // Itera pelas colunas de "Etapa" (começando do índice 1)
                for (int colIndex = 1; colIndex < dataGridView1.Columns.Count; colIndex++)
                {
                    for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count; rowIndex++)
                    {
                        // Verifica se a célula existe antes de tentar acessá-la
                        if (dataGridView1.Rows[rowIndex].Cells.Count > colIndex &&
                            dataGridView1.Rows[rowIndex].Cells[colIndex] != null)
                        {
                            dataGridView1.Rows[rowIndex].Cells[colIndex].Value = string.Empty; // Ou "-", ou "0.00" conforme preferência
                        }
                    }
                }
            };

            if (dataGridView1.InvokeRequired)
            {
                try { dataGridView1.Invoke(action); } catch (ObjectDisposedException) { }
            }
            else
            {
                action();
            }
        }


        public struct SeriesConfig // Estrutura para configurar séries dos gráficos
        {
            public string Name;
            public SeriesChartType Type;
            public Color Color;
            public AxisType Axis; // Primário ou Secundário

            public SeriesConfig(string name, SeriesChartType type, Color color, AxisType axis = AxisType.Primary)
            {
                Name = name;
                Type = type;
                Color = color;
                Axis = axis;
            }
        }

        private void GravarDadoSensor(string nomeSensor, string valor, string unidade)
        {
            if (string.IsNullOrEmpty(valor) || valor == "N/A")
            {
                MessageBox.Show($"Valor do sensor '{nomeSensor}' não disponível para gravação.", "Dados Ausentes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isMonitoring) // Grava apenas se o teste estiver em andamento
            {
                if (dadosSensores == null) dadosSensores = new List<SensorData>();
                dadosSensores.Add(new SensorData { Sensor = nomeSensor, Valor = valor, Medidas = unidade });
                // Esta lista 'dadosSensores' parece ser uma lista geral. 
                // A gravação no DataGridView e em _dadosColetados (por etapa) é feita em btn_gravar_Click.
            }
            else
            {
                MessageBox.Show("Teste não iniciado. Favor iniciar o teste para capturar os dados.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private bool ValidarMinMaxCheckBoxLocal(System.Windows.Forms.CheckBox checkBox, System.Windows.Forms.TextBox minTextBox, System.Windows.Forms.TextBox maxTextBox, string nomeUnidade)
        {
            if (checkBox == null || minTextBox == null || maxTextBox == null) return false;

            if (!checkBox.Checked) return true; // Se não está checado, não valida os campos

            bool minOk = decimal.TryParse(minTextBox.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMinimo);
            bool maxOk = decimal.TryParse(maxTextBox.Text.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMaximo);
            string erroMsg = null;

            if (!minOk || !maxOk) erroMsg = $"Valores de Mínimo e Máximo para {nomeUnidade} devem ser numéricos.";
            else if (valorMinimo < 0 || valorMaximo < 0) erroMsg = $"Valores de Mínimo e Máximo para {nomeUnidade} não podem ser negativos.";
            else if (valorMinimo > valorMaximo) erroMsg = $"Valor Mínimo para {nomeUnidade} não pode ser maior que o Valor Máximo.";

            if (erroMsg != null)
            {
                MessageBox.Show(this, erroMsg + $"\nA verificação de limites para {nomeUnidade} foi desativada.", $"Erro de Validação - {nomeUnidade}", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                checkBox.Checked = false; // Desativa o checkbox se a validação falhar
                // Opcional: Limpar TextBoxes ou focar no problemático
                // minTextBox.Clear(); maxTextBox.Clear();
                // if (!minOk || (minOk && valorMinimo < 0) || (minOk && maxOk && valorMinimo > valorMaximo)) minTextBox.Focus(); else maxTextBox.Focus();
                return false;
            }
            return true; // Validação OK
        }
        // Event Handlers para CheckBoxes de validação de Mín/Máx
        private void checkBox_psi_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox9, textBox8, "PSI"); }
        private void checkBox_gpm_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox14, textBox12, "GPM"); }
        private void checkBox_bar_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox11, textBox10, "BAR"); }
        private void checkBox_rotacao_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox18, textBox17, "Rotação (RPM)"); }
        private void checkBox_lpm_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox16, textBox15, "LPM"); }
        private void checkBox_temperatura_CheckedChanged(object sender, EventArgs e) { ValidarMinMaxCheckBoxLocal(sender as System.Windows.Forms.CheckBox, textBox20, textBox19, "Temperatura (°C)"); }

        private void TimerCronometro_Tick(object sender, EventArgs e)
        {
            if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed && circularProgressBar1.Value > 0)
            {
                circularProgressBar1.Value--; // Decrementa o valor da barra de progresso
            }
            else // Tempo esgotado ou valor chegou a zero
            {
                timerCronometro.Stop(); // Para o timer
                cronometroIniciado = false; // Sinaliza que o cronômetro não está mais ativo
                if (_isMonitoring) // Se o teste ainda estava rodando
                {
                    LogHistoricalEvent("Tempo do cronômetro esgotado. Finalizando teste automaticamente.", Color.Orange);
                    btnParar_Click(this, EventArgs.Empty); // Chama a lógica de parada do teste
                }
            }
        }


        private void btnDefinir_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox inputTempoCronometro = this.Controls.Find("textBox1_tempoCronometro", true).FirstOrDefault() as System.Windows.Forms.TextBox;

            if (inputTempoCronometro == null || inputTempoCronometro.IsDisposed)
            {
                MessageBox.Show(this, "Controle para definir tempo do cronômetro não encontrado ou foi descartado.", "Erro UI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!cronometroIniciado) // Permite definir apenas se o cronômetro não estiver rodando
            {
                if (int.TryParse(inputTempoCronometro.Text, out int valorMinutos) && valorMinutos > 0)
                {
                    valorDefinidoCronometro = valorMinutos; // Armazena o valor em minutos
                    tempoCronometroDefinidoManualmente = true; // Sinaliza que foi definido
                    if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
                    {
                        // Configura a barra de progresso para o novo tempo
                        circularProgressBar1.Maximum = valorDefinidoCronometro * 60 > 0 ? valorDefinidoCronometro * 60 : 1; // Em segundos
                        circularProgressBar1.Value = valorDefinidoCronometro * 60; // Valor inicial
                        circularProgressBar1.Invalidate(); // Redesenha
                    }
                    MessageBox.Show(this, $"Tempo do cronômetro definido para {valorDefinidoCronometro} minutos.", "Cronômetro Definido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(this, "Por favor, insira um valor numérico inteiro positivo válido em minutos.", "Erro de Entrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, "O cronômetro já está em execução. Limpe ou finalize o teste atual para definir um novo tempo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox inputTempoCronometro = this.Controls.Find("textBox1_tempoCronometro", true).FirstOrDefault() as System.Windows.Forms.TextBox;

            if (inputTempoCronometro != null && !inputTempoCronometro.IsDisposed && !cronometroIniciado)
            {
                inputTempoCronometro.Text = "0"; // Limpa o TextBox
                valorDefinidoCronometro = 0; // Reseta o valor armazenado
                tempoCronometroDefinidoManualmente = false; // Sinaliza que não há tempo definido
                if (circularProgressBar1 != null && !circularProgressBar1.IsDisposed)
                {
                    // Reseta a barra de progresso
                    circularProgressBar1.Value = 0;
                    circularProgressBar1.Maximum = 100; // Um valor padrão
                    circularProgressBar1.Invalidate();
                }
                MessageBox.Show(this, "Tempo do cronômetro limpo.", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (cronometroIniciado)
            {
                MessageBox.Show(this, "O cronômetro está em execução. Finalize o teste atual para limpar o tempo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_gravar_Click(object sender, EventArgs e)
        {
            if (!_isMonitoring)
            {
                MessageBox.Show("O teste não foi iniciado. Por favor, inicie o teste para gravar os dados.", "Teste Não Iniciado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Verifica se o cronômetro foi definido e se o tempo já esgotou (cronometroIniciado é false após esgotar)
            if (tempoCronometroDefinidoManualmente && !cronometroIniciado && valorDefinidoCronometro > 0 && (circularProgressBar1 == null || circularProgressBar1.Value <= 0))
            {
                MessageBox.Show("O tempo definido para o teste encerrou ou o cronômetro não está ativo. Não é possível gravar novas etapas.", "Tempo Esgotado ou Cronômetro Inativo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SetButtonState(btngravar, false); // Desabilita o botão de gravar
                return;
            }

            int maxEtapasTabela = 7; //dataGridView1.Columns.Count - 1 se as colunas de etapa são fixas após "Parametro"
            if (etapaAtual > maxEtapasTabela)
            {
                MessageBox.Show($"Limite de {maxEtapasTabela} etapas para a tabela foi atingido.", "Limite de Etapas da Tabela", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetButtonState(btngravar, false); // Desabilita o botão de gravar
                return;
            }

            GravarDadosNoDataGridViewEstatico(); // Grava os dados dos TextBoxes na tabela estática (dataGridView1)

            // Lógica para _dadosColetados (para o relatório final, se a estrutura for diferente ou precisar de mais detalhes)
            // Esta parte assume que os dados para _dadosColetados vêm das leituras atuais dos sensores (_currentNumericSensorReadings)
            // e não necessariamente dos TextBoxes (que podem ter formatação ou "N/A")

            var currentEtapaData = new EtapaData
            {
                Etapa = etapaAtual, // A etapa que está sendo gravada
                leituras = new List<SensorData>()
            };

            Dictionary<string, double> readingsSnapshot;
            lock (readingsLock) // Acessa as leituras atuais de forma segura
            {
                readingsSnapshot = new Dictionary<string, double>(_currentNumericSensorReadings);
            }

            // Itera sobre os parâmetros definidos para o DataGridView estático para popular _dadosColetados
            // Isso garante que os dados em _dadosColetados correspondam ao que é esperado na tabela.
            if (_staticDataGridViewParameters != null)
            {
                foreach (var paramInfo in _staticDataGridViewParameters)
                {
                    // Tenta encontrar a leitura correspondente em readingsSnapshot
                    // Isso requer um mapeamento entre paramInfo.SourceTextBoxName (ou uma chave serial associada) e as chaves em readingsSnapshot
                    // Por simplicidade, vamos assumir que paramInfo.SourceTextBoxName pode ser usado ou adaptado para buscar a leitura.
                    // Ex: Se paramInfo.SourceTextBoxName é "sensor_P1" e a chave serial é "P1"
                    string serialKeyForParam = paramInfo.SourceTextBoxName.Replace("sensor_", ""); // Tentativa de mapeamento simples

                    // Casos especiais de mapeamento se necessário:
                    if (paramInfo.SourceTextBoxName == "sensor_CELSUS") serialKeyForParam = "temp";
                    if (paramInfo.SourceTextBoxName == "sensor_RPM") serialKeyForParam = "RPM";
                    // Adicione outros mapeamentos conforme sua lógica de chaves seriais.

                    string valorParaRelatorio = "N/D";
                    string unidadeParaRelatorio = paramInfo.FormattingType; // Pode ser mais específico se tiver um mapa de unidades

                    if (readingsSnapshot.TryGetValue(serialKeyForParam, out double sensorNumericValue))
                    {
                        valorParaRelatorio = sensorNumericValue.ToString("F2", CultureInfo.InvariantCulture); // Formato para o relatório
                                                                                                              // Busca unidade se tiver um mapa de unidades, ex: sensorMapmedida.TryGetValue(serialKeyForParam, out unidadeParaRelatorio);
                    }
                    else if (readingsSnapshot.TryGetValue(paramInfo.SourceTextBoxName, out double fallbackVal))
                    { // Tenta o nome completo do textbox se a chave curta falhar
                        valorParaRelatorio = fallbackVal.ToString("F2", CultureInfo.InvariantCulture);
                    }

                    currentEtapaData.leituras.Add(new SensorData
                    {
                        Sensor = paramInfo.DisplayName, // Usa o nome de exibição para o relatório
                        Valor = valorParaRelatorio,
                        Medidas = unidadeParaRelatorio // Unidade (ex: "BAR", "LPM", "°C")
                    });
                }
            }
            _dadosColetados.Add(currentEtapaData); // Adiciona os dados da etapa à lista para o relatório

            LogHistoricalEvent($"Dados da Etapa {etapaAtual} gravados.", Color.DarkCyan); // Log mais específico
            etapaAtual++; // Incrementa para a próxima coluna/etapa

            if (etapaAtual > maxEtapasTabela) // Se atingiu o limite de etapas da tabela
            {
                MessageBox.Show("Todas as etapas da tabela foram preenchidas.", "Tabela Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetButtonState(btngravar, false); // Desabilita o botão de gravar
            }
        }


        private void GravarDadosNoDataGridViewEstatico()
        {
            if (dataGridView1 == null || dataGridView1.IsDisposed || _staticDataGridViewParameters == null) return;
            if (etapaAtual < 1 || etapaAtual > (dataGridView1.Columns.Count - 1)) // Coluna 0 é "Parametro"
            {
                Debug.WriteLine($"[GravarDGV] Etapa atual ({etapaAtual}) fora do intervalo válido para colunas (1 a {dataGridView1.Columns.Count - 1}).");
                return;
            }

            int targetColumnIndexInDGV = etapaAtual; // Coluna "Etapa 1" é índice 1, "Etapa 2" é índice 2, etc.

            for (int rowIndex = 0; rowIndex < _staticDataGridViewParameters.Count; rowIndex++)
            {
                if (rowIndex >= dataGridView1.Rows.Count) // Segurança: não tentar acessar linha inexistente
                {
                    Debug.WriteLine($"[GravarDGV] rowIndex {rowIndex} fora do intervalo para dataGridView1.Rows.Count {dataGridView1.Rows.Count}");
                    break;
                }

                ParameterRowInfo paramInfo = _staticDataGridViewParameters[rowIndex];
                System.Windows.Forms.TextBox sourceTextBox = null;

                // Tenta encontrar o TextBox pelo nome
                Control[] foundControls = this.Controls.Find(paramInfo.SourceTextBoxName, true);
                if (foundControls.Length > 0 && foundControls[0] is TextBox tb)
                {
                    sourceTextBox = tb;
                }
                else // Tenta encontrar como campo da classe (se não estiver em Controls aninhados ou nomeado diferente no designer)
                {
                    var field = this.GetType().GetField(paramInfo.SourceTextBoxName,
                        System.Reflection.BindingFlags.NonPublic |
                        System.Reflection.BindingFlags.Public | // Adicionado Public para cobrir mais casos
                        System.Reflection.BindingFlags.Instance);
                    if (field != null && field.GetValue(this) is TextBox tbInstance)
                    {
                        sourceTextBox = tbInstance;
                    }
                }

                string formattedValue = "ERRO"; // Valor padrão em caso de falha

                if (sourceTextBox != null && !sourceTextBox.IsDisposed)
                {
                    string rawValue = sourceTextBox.Text;
                    if (string.IsNullOrWhiteSpace(rawValue) || rawValue.Equals("N/A", StringComparison.OrdinalIgnoreCase))
                    {
                        formattedValue = "-"; // Representação para N/A ou vazio na tabela
                    }
                    else
                    {
                        // Tenta converter e formatar o valor
                        if (double.TryParse(rawValue.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
                        {
                            switch (paramInfo.FormattingType.ToLower())
                            {
                                case "float":
                                    formattedValue = numericValue.ToString("0.00", CultureInfo.InvariantCulture); // Duas casas decimais
                                    break;
                                case "temp": // Temperatura como inteiro
                                    formattedValue = ((int)Math.Round(numericValue)).ToString("D", CultureInfo.InvariantCulture); // "D" para inteiro simples
                                    break;
                                case "rpm": // Rotação como inteiro
                                    formattedValue = ((int)Math.Round(numericValue)).ToString("D", CultureInfo.InvariantCulture);
                                    break;
                                default:
                                    formattedValue = numericValue.ToString(CultureInfo.InvariantCulture); // Formato padrão se tipo não reconhecido
                                    break;
                            }
                        }
                        else
                        {
                            formattedValue = "Inválido"; // Se não conseguir converter para double
                        }
                    }
                }
                else
                {
                    Debug.WriteLine($"[GravarDGV] TextBox de origem '{paramInfo.SourceTextBoxName}' para o parâmetro '{paramInfo.DisplayName}' não foi encontrado ou está descartado.");
                    formattedValue = "N/D"; // Se o TextBox não for encontrado
                }

                // Garante que a célula de destino existe
                if (dataGridView1.Rows[rowIndex].Cells.Count > targetColumnIndexInDGV &&
                    dataGridView1.Rows[rowIndex].Cells[targetColumnIndexInDGV] != null)
                {
                    // Atualiza a célula na thread da UI, se necessário
                    if (dataGridView1.InvokeRequired)
                    {
                        int r = rowIndex; int c = targetColumnIndexInDGV; string val = formattedValue;
                        try { dataGridView1.Invoke((MethodInvoker)delegate { dataGridView1.Rows[r].Cells[c].Value = val; }); }
                        catch (ObjectDisposedException) { }
                    }
                    else
                    {
                        dataGridView1.Rows[rowIndex].Cells[targetColumnIndexInDGV].Value = formattedValue;
                    }
                }
            }
        }


        //private void btnrelatoriobomba_Click(object sender, EventArgs e)
        //{
        //    // Verifica se já existe uma janela de relatório aberta para evitar múltiplas instâncias
        //    if (Application.OpenForms.OfType<Realatoriobase>().Any(f => !f.IsDisposed))
        //    {
        //        MessageBox.Show("Uma janela de relatório já está aberta. Por favor, feche-a antes de abrir uma nova.", "Relatório Aberto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //        Application.OpenForms.OfType<Realatoriobase>().First(f => !f.IsDisposed).Focus(); // Foca na janela existente
        //        return;
        //    }

        //    Realatoriobase relatorioForm = new Realatoriobase();

        //    // Coleta dados do cabeçalho do teste
        //    string nomeCliente = this.Controls.Find("textBox6", true).FirstOrDefault() is TextBox tbCliente ? tbCliente.Text : "N/A";
        //    string nomeBomba = this.Controls.Find("textBox5", true).FirstOrDefault() is TextBox tbBomba ? tbBomba.Text : "N/A";
        //    string ordemServico = this.Controls.Find("textBox4", true).FirstOrDefault() is TextBox tbOS ? tbOS.Text : "N/A";


        //    // Prepara os dados da tabela (dataGridView1) para o relatório
        //    List<List<string>> tabelaDadosParaRelatorio = new List<List<string>>();
        //    if (dataGridView1 != null && !dataGridView1.IsDisposed)
        //    {
        //        // Adiciona cabeçalhos das colunas
        //        List<string> headers = new List<string>();
        //        foreach (DataGridViewColumn col in dataGridView1.Columns)
        //        {
        //            headers.Add(col.HeaderText);
        //        }
        //        tabelaDadosParaRelatorio.Add(headers);

        //        // Adiciona dados das linhas
        //        foreach (DataGridViewRow row in dataGridView1.Rows)
        //        {
        //            if (row.IsNewRow) continue; // Pula a linha de adição (se houver)
        //            List<string> rowData = new List<string>();
        //            foreach (DataGridViewCell cell in row.Cells)
        //            {
        //                rowData.Add(cell.Value?.ToString() ?? string.Empty); // Adiciona valor da célula ou string vazia
        //            }
        //            tabelaDadosParaRelatorio.Add(rowData);
        //        }
        //    }

        //    // Passa os dados para o formulário de relatório
        //    // Adapte o método SetDadosTabela conforme a assinatura exata em Realatoriobase
        //    relatorioForm.SetDadosEnsaio(
        //        Inicioteste,
        //        Fimteste,
        //        nomeCliente,
        //        nomeBomba,
        //        ordemServico,
        //        tabelaDadosParaRelatorio, // Dados do dataGridView1
        //        _dadosColetados // Dados coletados por etapa (se a estrutura for diferente ou mais detalhada)
        //    );


        //    // Passa imagens dos gráficos (opcional, se Realatoriobase suportar)
        //    // List<Image> graficos = new List<Image>();
        //    // if (chart1 != null && !chart1.IsDisposed) { using (var ms = new System.IO.MemoryStream()) { chart1.SaveImage(ms, ChartImageFormat.Png); graficos.Add(Image.FromStream(ms)); } }
        //    // if (chart2 != null && !chart2.IsDisposed) { using (var ms = new System.IO.MemoryStream()) { chart2.SaveImage(ms, ChartImageFormat.Png); graficos.Add(Image.FromStream(ms)); } }
        //    // if (chart3 != null && !chart3.IsDisposed) { using (var ms = new System.IO.MemoryStream()) { chart3.SaveImage(ms, ChartImageFormat.Png); graficos.Add(Image.FromStream(ms)); } }
        //    // relatorioForm.SetGraficos(graficos);


        //    relatorioForm.Show(); // Mostra o formulário de relatório
        //}


    }
}