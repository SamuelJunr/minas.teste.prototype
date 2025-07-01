using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports; // Necessário para SerialPort.GetPortNames()
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.Estilo;
using minas.teste.prototype.MVVM.Model.Concrete; // Para ConnectionSettingsApplication
using minas.teste.prototype.Properties;
using minas.teste.prototype.Service;             // Para SerialManager (indiretamente via ConnectionSettingsApplication)
using System.Diagnostics;
using System.Web.Script.Serialization;                         // Para Debug.WriteLine
// Para serialização JSON do dicionário de calibração. Adicione referência a System.Web.Extensions.


namespace minas.teste.prototype.MVVM.View
{
    public partial class configuracao : Form
    {
        private apresentacao fechar_box; // Certifique-se que 'apresentacao' está acessível ou remova se não for usado aqui.
        private bool _fechamentoForcado = false;

        // Dados para Sensores e Módulos
        private Dictionary<string, string> _sensoresNomeAmigavel; // Chave: "HA1", Valor: "Sensor 1 Pressão Hidro A"
        private List<string> _modulos;
        private Dictionary<string, string> _sensorMascaras; // Chave: "HA1", Valor: "000.00000" (máscara para MaskedTextBox)
        private Dictionary<string, string> _calibrationCoefficients; // Chave: "HA1", Valor: "123.45678"

        private const string CalibrationSettingsKey = "CalibrationCoefficientsJSON";


        public configuracao()
        {
            InitializeComponent();
            EstiloFormulario.AplicarEstiloconfiguracao(this);
            InitializeSensorAndModuleData();
            LoadCalibrationCoefficients(); // Carregar antes de PopularControlesCom
            PopularControlesCom(); // Popula Sensores, Módulos, Portas, BaudRates e tenta carregar configurações salvas
            CarregarConfiguracoesSalvas(); // Carrega configurações de conexão e seleção anterior de sensor/módulo
                                           // AtualizarStatusConexaoLabel(); // Chamado dentro de CarregarConfiguracoesSalvas ou PopularControlesCom
            comboBoxSensorTipo.SelectedIndexChanged += ComboBoxSensorTipo_SelectedIndexChanged;
        }

        private void InitializeSensorAndModuleData()
        {
            _sensoresNomeAmigavel = new Dictionary<string, string>
            {
                {"HA1", "Sensor 1 Pressão Hidro A"}, {"HA2", "Sensor 2 Pressão Hidro A"},
                {"HB1", "Sensor 1 Pressão Hidro B"}, {"HB2", "Sensor 2 Pressão Hidro B"},
                {"MA1", "Sensor 1 Pressão Motor A"}, {"MA2", "Sensor 2 Pressão Motor A"},
                {"MB1", "Sensor 1 Pressão Motor B"}, {"MB2", "Sensor 2 Pressão Motor B"},
                {"TEM", "Sensor Temperatura"}, {"ROT", "Sensor Rotação"},
                {"DR1", "Sensor 1 Volume Dreno"}, {"DR2", "Sensor 2 Volume Dreno"},
                {"DR3", "Sensor 3 Volume Dreno"}, {"DR4", "Sensor 4 Volume Dreno"},
                {"PL1", "Sensor 1 Pressão Pilotagem"}, {"PL2", "Sensor 2 Pressão Pilotagem"},
                {"PL3", "Sensor 3 Pressão Pilotagem"}, {"PL4", "Sensor 4 Pressão Pilotagem"},
                {"PR1", "Sensor 1 Pressão Base"}, {"PR2", "Sensor 2 Pressão Base"},
                {"PR3", "Sensor 3 Pressão Base"}, {"PR4", "Sensor 4 Pressão Base"},
                {"VZ1", "Sensor 1 Volume Vazão"}, {"VZ2", "Sensor 2 Volume Vazão"},
                {"VZ3", "Sensor 3 Volume Vazão"}, {"VZ4", "Sensor 4 Volume Vazão"}
            };

            _sensorMascaras = new Dictionary<string, string>
            {
                {"HA1", "000.00000"}, {"HA2", "000.00000"}, {"HB1", "000.00000"}, {"HB2", "000.00000"},
                {"MA1", "000.00000"}, {"MA2", "000.00000"}, {"MB1", "000.00000"}, {"MB2", "000.00000"},
                {"TEM", "000"}, {"ROT", "000"},
                {"DR1", "000.00000"}, {"DR2", "000.00000"}, {"DR3", "000.00000"}, {"DR4", "000.00000"},
                {"PL1", "000.00000"}, {"PL2", "000.00000"}, {"PL3", "000.00000"}, {"PL4", "000.00000"},
                {"PR1", "000.00000"}, {"PR2", "000.00000"}, {"PR3", "000.00000"}, {"PR4", "000.00000"},
                {"VZ1", "000.00000"}, {"VZ2", "000.00000"}, {"VZ3", "000.00000"}, {"VZ4", "000.00000"}
            };
            // Using "0" as digit placeholder. "." is a literal. Mask: "999.99999" for optional, "000.00000" for required.

            _modulos = new List<string>
            {
                "Bombas", "Cilindros", "Eletroválvulas", "Motor", "Direção", "Comandos"
            };

            _calibrationCoefficients = new Dictionary<string, string>();
        }

        private void LoadCalibrationCoefficients()
        {
            try
            {
                if (Settings.Default.Properties[CalibrationSettingsKey] != null)
                {
                    string json = Settings.Default[CalibrationSettingsKey] as string;
                    if (!string.IsNullOrEmpty(json))
                    {
                        var serializer = new JavaScriptSerializer();
                        _calibrationCoefficients = serializer.Deserialize<Dictionary<string, string>>(json);
                        Debug.WriteLine("Coeficientes de calibração carregados.");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao carregar coeficientes de calibração: {ex.Message}");
                _calibrationCoefficients = new Dictionary<string, string>(); // Reset on error
            }
            if (_calibrationCoefficients == null) _calibrationCoefficients = new Dictionary<string, string>();
        }

        private void SaveCalibrationCoefficients()
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                string json = serializer.Serialize(_calibrationCoefficients);

                // Ensure the setting exists if you are not adding it through the designer
                // This dynamic addition is generally not recommended. Define CalibrationSettingsKey in Settings.settings
                if (Settings.Default.Properties[CalibrationSettingsKey] == null)
                {
                    // This part is tricky without direct access to Settings.settings.
                    // For a robust solution, CalibrationSettingsKey should be pre-defined.
                    // As a fallback, this might work for the session or if the structure supports it.
                    System.Configuration.SettingsProperty property = new System.Configuration.SettingsProperty(CalibrationSettingsKey)
                    {
                        DefaultValue = "{}",
                        IsReadOnly = false,
                        PropertyType = typeof(string),
                        Provider = Settings.Default.Providers["LocalFileSettingsProvider"], // or appropriate provider
                        SerializeAs = System.Configuration.SettingsSerializeAs.String
                    };
                    property.Attributes.Add(typeof(System.Configuration.UserScopedSettingAttribute), new System.Configuration.UserScopedSettingAttribute());

                    // This check is crucial: only add if not already there after a reload or by design
                    bool found = false;
                    foreach (System.Configuration.SettingsProperty sp in Settings.Default.Properties)
                    {
                        if (sp.Name == CalibrationSettingsKey)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        Settings.Default.Properties.Add(property);
                    }
                }

                Settings.Default[CalibrationSettingsKey] = json;
                Settings.Default.Save();
                Debug.WriteLine("Coeficientes de calibração salvos.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao salvar coeficientes de calibração: {ex.Message}");
                MessageBox.Show($"Erro ao salvar coeficientes de calibração: {ex.Message}", "Erro de Salvamento", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void configuracao_Load(object sender, EventArgs e)
        {
            // Text = Properties.Resources.ResourceManager.GetString("ConfigureTitle"); // Se definido em resources
            AtualizarStatusConexaoLabel(); // Atualiza status inicial da conexão
        }

        private void PopularControlesCom()
        {
            // Popular ComboBox de Sensores
            comboBoxSensorTipo.DataSource = null;
            comboBoxSensorTipo.Items.Clear();
            // Ensure _sensoresNomeAmigavel is initialized before this point
            if (_sensoresNomeAmigavel != null)
            {
                comboBoxSensorTipo.DataSource = new BindingSource(_sensoresNomeAmigavel, null);
                comboBoxSensorTipo.DisplayMember = "Value";
                comboBoxSensorTipo.ValueMember = "Key";
            } //

            // Popular ComboBox de Módulos
            comboBoxModulo.DataSource = null;
            comboBoxModulo.Items.Clear();
            // Ensure _modulos is initialized before this point
            if (_modulos != null)
            {
                comboBoxModulo.DataSource = _modulos;
            } //

            // ... (rest of the method to populate COM ports and Baud Rates)
            // Popular ComboBox de Portas COM
            try
            {
                comboBoxPortaCOM.Items.Clear();
                string[] portasDisponiveis = SerialPort.GetPortNames(); //
                if (portasDisponiveis.Length > 0)
                {
                    comboBoxPortaCOM.Items.AddRange(portasDisponiveis); //
                    comboBoxPortaCOM.Enabled = true; //
                    btnTestarConexao.Enabled = true; //
                }
                else
                {
                    lblStatusConexao.Text = "Nenhuma porta COM encontrada."; //
                    comboBoxPortaCOM.Items.Add("Nenhuma porta disponível"); //
                    comboBoxPortaCOM.SelectedIndex = 0; //
                    comboBoxPortaCOM.Enabled = false; //
                    btnTestarConexao.Enabled = false; //
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao listar portas COM: {ex.Message}"); //
                MessageBox.Show($"Erro ao listar portas COM: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error); //
                lblStatusConexao.Text = "Erro ao listar portas COM."; //
                comboBoxPortaCOM.Items.Add("Erro ao listar"); //
                if (comboBoxPortaCOM.Items.Count > 0) comboBoxPortaCOM.SelectedIndex = 0; //
                comboBoxPortaCOM.Enabled = false; //
                btnTestarConexao.Enabled = false; //
            }

            // Popular ComboBox de Baud Rates
            comboBoxBaudRate.Items.Clear();
            List<int> baudRatesComuns = new List<int> { 9600, 19200, 38400, 57600, 115200, 250000 }; //
            foreach (int rate in baudRatesComuns)
            {
                comboBoxBaudRate.Items.Add(rate); //
            }
        }
        private void CarregarConfiguracoesSalvas()
        {
            // Carregar Porta COM e Baud Rate salvos (existing logic from your file)
            if (comboBoxPortaCOM.Enabled)
            {
                string portaSalva = Settings.Default.PortaCOMSelecionada; //
                if (!string.IsNullOrEmpty(portaSalva) && comboBoxPortaCOM.Items.Contains(portaSalva)) //
                {
                    comboBoxPortaCOM.SelectedItem = portaSalva; //
                }
                else if (comboBoxPortaCOM.Items.Count > 0 && comboBoxPortaCOM.Items[0].ToString() != "Nenhuma porta disponível") //
                {
                    if (comboBoxPortaCOM.Items.Count > 0) comboBoxPortaCOM.SelectedIndex = 0; //
                }
            }

            int baudRateSalvo = Settings.Default.BaudRateSelecionado; //
            if (baudRateSalvo > 0 && comboBoxBaudRate.Items.Contains(baudRateSalvo)) //
            {
                comboBoxBaudRate.SelectedItem = baudRateSalvo; //
            }
            else if (comboBoxBaudRate.Items.Contains(115200)) //
            {
                comboBoxBaudRate.SelectedItem = 115200; //
            }
            else if (comboBoxBaudRate.Items.Count > 0) //
            {
                comboBoxBaudRate.SelectedIndex = 0; //
            }

            // Carregar Sensor e Módulo salvos
            // Ensure _sensoresNomeAmigavel is not null before accessing it
            if (_sensoresNomeAmigavel != null)
            {
                // This is the part you asked to adapt:
                string sensorSalvoKey = Settings.Default["TipoSensorSelecionado"] as string; //
                if (!string.IsNullOrEmpty(sensorSalvoKey) && _sensoresNomeAmigavel.ContainsKey(sensorSalvoKey)) //
                {
                    comboBoxSensorTipo.SelectedValue = sensorSalvoKey; //
                }
                else if (comboBoxSensorTipo.Items.Count > 0)
                {
                    comboBoxSensorTipo.SelectedIndex = 0; //
                }
            }
            else if (comboBoxSensorTipo.Items.Count > 0)
            {
                comboBoxSensorTipo.SelectedIndex = 0;
            }


            if (comboBoxSensorTipo.SelectedIndex != -1)
            {
                ComboBoxSensorTipo_SelectedIndexChanged(comboBoxSensorTipo, EventArgs.Empty); //
            }

            // Ensure _modulos is not null before trying to find an item in its bound ComboBox
            if (_modulos != null)
            {
                string moduloSalvo = Settings.Default["ModuloSelecionado"] as string; //
                if (!string.IsNullOrEmpty(moduloSalvo) && comboBoxModulo.Items.Contains(moduloSalvo)) // // Corrected from .contains to .Contains
                {
                    comboBoxModulo.SelectedItem = moduloSalvo; //
                }
                else if (comboBoxModulo.Items.Count > 0)
                {
                    comboBoxModulo.SelectedIndex = 0; //
                }
            }
            else if (comboBoxModulo.Items.Count > 0)
            {
                comboBoxModulo.SelectedIndex = 0;
            }

            AtualizarStatusConexaoLabel(); //
        }



        private void SalvarConfiguracoes() // Nomeado para refletir que salva mais que apenas a porta
        {
            // Salvar Porta COM e Baud Rate
            if (comboBoxPortaCOM.SelectedItem != null && comboBoxPortaCOM.SelectedItem.ToString() != "Nenhuma porta disponível")
            {
                Settings.Default.PortaCOMSelecionada = comboBoxPortaCOM.SelectedItem.ToString(); //
            }
            else
            {
                Settings.Default.PortaCOMSelecionada = string.Empty; //
            }

            if (comboBoxBaudRate.SelectedItem != null)
            {
                Settings.Default.BaudRateSelecionado = (int)comboBoxBaudRate.SelectedItem; //
            }
            else
            {
                Settings.Default.BaudRateSelecionado = 0; //
            }

            // Salvar Sensor e Módulo selecionados
            if (comboBoxSensorTipo.SelectedValue != null)
            {
                Settings.Default["TipoSensorSelecionado"] = comboBoxSensorTipo.SelectedValue.ToString(); // Salva a CHAVE
            }
            else
            {
                Settings.Default["TipoSensorSelecionado"] = string.Empty;
            }

            if (comboBoxModulo.SelectedItem != null)
            {
                Settings.Default["ModuloSelecionado"] = comboBoxModulo.SelectedItem.ToString(); // Crie esta setting
            }
            else
            {
                Settings.Default["ModuloSelecionado"] = string.Empty;
            }

            // Salvar Coeficiente de Calibração (para o sensor selecionado)
            if (comboBoxSensorTipo.SelectedValue != null)
            {
                string sensorKey = comboBoxSensorTipo.SelectedValue.ToString();
                // Remove non-digits if necessary before saving, or save with mask
                // For "000.00000" like masks, Text property should be fine.
                // Consider using maskedTextBoxCoeficiente.TextMaskFormat = MaskFormat.ExcludePromptAndLiterals;
                // and then save Text, if you only want digits.
                // For now, saving the text as displayed (which includes the literal for decimal point)
                string coefficient = maskedTextBoxCoeficiente.Text;
                if (maskedTextBoxCoeficiente.MaskCompleted) // Or some other validation
                {
                    _calibrationCoefficients[sensorKey] = coefficient;
                }
                else if (string.IsNullOrWhiteSpace(maskedTextBoxCoeficiente.Text.Replace(".", "").Trim())) // if effectively empty
                {
                    // If the user clears the textbox, remove the coefficient
                    if (_calibrationCoefficients.ContainsKey(sensorKey))
                    {
                        _calibrationCoefficients.Remove(sensorKey);
                    }
                }
                // else, if not complete but not empty, maybe warn or don't save?
                // For now, we save if MaskCompleted, or remove if empty.
            }
            SaveCalibrationCoefficients(); // Persiste o dicionário inteiro

            Settings.Default.Save(); //
            Debug.WriteLine("Configurações de porta, baud rate, sensor, módulo e calibração salvas.");
        }

        private void ComboBoxSensorTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSensorTipo.SelectedValue != null)
            {
                string sensorKey = comboBoxSensorTipo.SelectedValue.ToString();

                // Aplicar Máscara
                if (_sensorMascaras.ContainsKey(sensorKey))
                {
                    maskedTextBoxCoeficiente.Mask = _sensorMascaras[sensorKey];
                }
                else
                {
                    maskedTextBoxCoeficiente.Mask = ""; // Nenhuma máscara
                }

                // Carregar Coeficiente Salvo
                if (_calibrationCoefficients.ContainsKey(sensorKey))
                {
                    maskedTextBoxCoeficiente.Text = _calibrationCoefficients[sensorKey];
                }
                else
                {
                    maskedTextBoxCoeficiente.Text = ""; // Limpar se não houver coeficiente salvo
                }
            }
<<<<<<< HEAD
            else
=======
        }

        private void btnVoltar_Click(object sender, EventArgs e) // Renomeado de button1_Click
        {
            _fechamentoForcado = true;
            if (Menuapp.Instance != null) Menuapp.Instance.Show(); // Garanta que Menuapp.Instance existe e é acessível
            this.Close();
        }

        private void Configuracao_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_fechamentoForcado)
>>>>>>> 16ee290 (atualizações segurança)
            {
                maskedTextBoxCoeficiente.Mask = "";
                maskedTextBoxCoeficiente.Text = "";
            }
        }


        private async void btnSalvarAplicarConfiguracoes_Click(object sender, EventArgs e)
        {
            string portaSelecionada = comboBoxPortaCOM.SelectedItem?.ToString();
            if (portaSelecionada == "Nenhuma porta disponível") portaSelecionada = null;

            if (string.IsNullOrEmpty(portaSelecionada) || comboBoxBaudRate.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione uma Porta COM e um Baud Rate válidos.", "Entrada Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning); //
                lblStatusConexao.Text = "Seleção de porta/baud rate inválida."; //
                lblStatusConexao.ForeColor = Color.Red; //
                return;
            }

            if (comboBoxSensorTipo.SelectedValue == null)
            {
                MessageBox.Show("Por favor, selecione um Tipo de Sensor.", "Entrada Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (comboBoxModulo.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione um Módulo.", "Entrada Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validação do coeficiente
            string currentSensorKey = comboBoxSensorTipo.SelectedValue.ToString();
            if (_sensorMascaras.ContainsKey(currentSensorKey) && !maskedTextBoxCoeficiente.MaskCompleted)
            {
                // Allow saving if the field is empty (to clear a coefficient)
                if (!string.IsNullOrWhiteSpace(maskedTextBoxCoeficiente.Text.Replace(".", "").Replace("_", "").Trim())) // Check if it's not effectively empty
                {
                    MessageBox.Show($"O coeficiente de calibração para {comboBoxSensorTipo.Text} não está completo. Por favor, preencha corretamente ou limpe o campo.", "Coeficiente Inválido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }


            int baudRateSelecionado = (int)comboBoxBaudRate.SelectedItem; //

            lblStatusConexao.Text = $"Tentando conectar a {portaSelecionada} @ {baudRateSelecionado} bps..."; //
            lblStatusConexao.ForeColor = Color.Orange; //
            this.Cursor = Cursors.WaitCursor; //
            BtnSalvar.Enabled = false; // Renomeei o botão de Salvar no designer para BtnSalvarAplicar
            btnTestarConexao.Enabled = false;

            bool sucessoConexao = false;
            await Task.Run(() =>
            {
                sucessoConexao = ConnectionSettingsApplication.UpdateConnection(portaSelecionada, baudRateSelecionado); //
            });

            this.Cursor = Cursors.Default; //
            BtnSalvar.Enabled = true;
            btnTestarConexao.Enabled = true; //

            if (sucessoConexao)
            {
                SalvarConfiguracoes(); // Salva todas as configurações, incluindo conexão e calibração
                lblStatusConexao.Text = $"Conectado com sucesso a {ConnectionSettingsApplication.CurrentPortName} @ {ConnectionSettingsApplication.CurrentBaudRate} bps."; //
                lblStatusConexao.ForeColor = Color.Green; //
                MessageBox.Show("Conexão estabelecida e configurações salvas!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information); //
            }
            else
            {
                // Mesmo se a conexão falhar, ainda podemos querer salvar as outras configs (sensor, modulo, calibração)
                // O prompt não é explícito, mas geralmente se salva o que for possível.
                // Para seguir o padrão anterior de só salvar porta em caso de sucesso de conexão:
                // SalvarConfiguracoes(); // Se quiser salvar mesmo com falha de conexão (exceto estado da porta)
                // Atualizaria apenas as settings não relacionadas à porta/baud.
                // Ou, como está, não salva nada se a conexão falhar.

                lblStatusConexao.Text = $"Falha ao conectar a {portaSelecionada}. Verifique a porta, o dispositivo ou se já está em uso."; //
                lblStatusConexao.ForeColor = Color.Red; //
                MessageBox.Show(lblStatusConexao.Text, "Falha na Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error); //

                // Decidir se salva as outras configurações mesmo com falha de conexão:
                var salvarParcial = MessageBox.Show("A conexão falhou. Deseja salvar as configurações de sensor, módulo e calibração mesmo assim?", "Salvar Parcialmente?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (salvarParcial == DialogResult.Yes)
                {
                    SalvarConfiguracoes(); // Salva as configurações (a parte da conexão não terá efeito prático se falhou)
                    MessageBox.Show("Configurações de sensor, módulo e calibração salvas. A conexão falhou.", "Configurações Salvas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private void btnLimparConfigGeral_Click(object sender, EventArgs e) // Evento do botão BtnLimpar
        {
            var result = MessageBox.Show("Tem certeza que deseja limpar as configurações de conexão (Porta, Baud Rate) e o coeficiente de calibração para o sensor selecionado?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                // Limpar configurações de conexão
                Settings.Default.PortaCOMSelecionada = string.Empty; //
                Settings.Default.BaudRateSelecionado = 0; //

                // Limpar coeficiente do sensor selecionado
                if (comboBoxSensorTipo.SelectedValue != null)
                {
                    string sensorKey = comboBoxSensorTipo.SelectedValue.ToString();
                    if (_calibrationCoefficients.ContainsKey(sensorKey))
                    {
                        _calibrationCoefficients.Remove(sensorKey);
                        Debug.WriteLine($"Coeficiente para {sensorKey} removido do dicionário em memória.");
                    }
                    maskedTextBoxCoeficiente.Text = ""; // Limpa na UI
                }
                // Persistir o dicionário de calibrações atualizado (com o item removido)
                SaveCalibrationCoefficients();


                // Limpar seleções de sensor/módulo salvas (opcional, mas geralmente bom ao limpar tudo)
                Settings.Default["TipoSensorSelecionado"] = string.Empty;
                Settings.Default["ModuloSelecionado"] = string.Empty; // Se você tiver essa setting

                Settings.Default.Save(); //

                // Recarrega os defaults nos combo boxes de porta/baud e atualiza UI
                PopularControlesCom(); // Isso irá repopular e tentar carregar defaults
                CarregarConfiguracoesSalvas(); // Isso irá tentar restaurar seleções (que agora serão vazias ou defaults)

                // Atualizar label de status
                // Se você quiser também desconectar a porta serial ativa ao limpar:
                // ConnectionSettingsApplication.CloseAllConnections();
                // AtualizarStatusConexaoLabel();

                lblStatusConexao.Text = "Configurações de conexão e calibração do sensor limpas."; //
                lblStatusConexao.ForeColor = SystemColors.ControlText; //
                MessageBox.Show("Configurações de conexão e calibração do sensor selecionado foram limpas.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information); //
            }
        }

        private void AtualizarStatusConexaoLabel()
        {
            if (ConnectionSettingsApplication.IsCurrentlyConnected) //
            {
                lblStatusConexao.Text = $"Conectado a {ConnectionSettingsApplication.CurrentPortName} @ {ConnectionSettingsApplication.CurrentBaudRate} bps."; //
                lblStatusConexao.ForeColor = Color.Green; //
            }
            else
            {
                if (!string.IsNullOrEmpty(ConnectionSettingsApplication.CurrentPortName)) //
                {
                    lblStatusConexao.Text = $"Desconectado. Última tentativa: {ConnectionSettingsApplication.CurrentPortName}."; //
                    lblStatusConexao.ForeColor = Color.Red; //
                }
                else
                {
                    lblStatusConexao.Text = "Desconectado. Nenhuma porta configurada."; //
                    lblStatusConexao.ForeColor = SystemColors.ControlText; //
                }
            }
        }

        private void btnVoltar_Click(object sender, EventArgs e)
        {
            _fechamentoForcado = true; //
            if (Menuapp.Instance != null) Menuapp.Instance.Show(); 
            this.Close(); //
        }

        private void Configuracao_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Salvar coeficientes ao fechar, caso não tenham sido salvos por um clique no botão Salvar.
            // Isso pode ser opcional, dependendo do comportamento desejado.
            // SaveCalibrationCoefficients(); 

            if (!_fechamentoForcado) //
            {
                 Application.Exit(); 
            }
        }

        private async void btnTestarConexao_Click(object sender, EventArgs e)
        {
            string portaSelecionada = comboBoxPortaCOM.SelectedItem?.ToString(); //
            if (portaSelecionada == "Nenhuma porta disponível") portaSelecionada = null; //

            if (string.IsNullOrEmpty(portaSelecionada) || comboBoxBaudRate.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione uma Porta COM e um Baud Rate válidos para testar.", "Entrada Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning); //
                return;
            }
            int baudRateSelecionado = (int)comboBoxBaudRate.SelectedItem; //

            lblStatusConexao.Text = $"Testando conexão com {portaSelecionada} @ {baudRateSelecionado} bps..."; //
            lblStatusConexao.ForeColor = Color.Orange; //
            this.Cursor = Cursors.WaitCursor; //
            btnTestarConexao.Enabled = false; //
            BtnSalvar.Enabled = false;


            bool sucessoTeste = false; //
            SerialManager tempManager = null; //

            await Task.Run(() =>
            {
                try
                {
                    tempManager = new SerialManager(); //
                    bool conexaoAberta = false; //
                    EventHandler<bool> statusHandler = (s, connected) => { conexaoAberta = connected; }; //
                    tempManager.ConnectionStatusChanged += statusHandler; //

                    tempManager.Connect(portaSelecionada, baudRateSelecionado); //
                    sucessoTeste = tempManager.IsConnected; //

                    if (sucessoTeste)
                    {
                        Debug.WriteLine($"Teste de conexão com {portaSelecionada} bem-sucedido. Desconectando manager de teste."); //
                        tempManager.Disconnect(); //
                    }
                    tempManager.ConnectionStatusChanged -= statusHandler; //

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erro durante teste de conexão: {ex.Message}"); //
                    sucessoTeste = false; //
                }
                finally
                {
                    tempManager?.Dispose(); //
                }
            });

            this.Cursor = Cursors.Default; //
            btnTestarConexao.Enabled = true; //
            BtnSalvar.Enabled = true;

            if (sucessoTeste)
            {
                lblStatusConexao.Text = $"Teste com {portaSelecionada} bem-sucedido! Clique em 'Salvar & Aplicar' para usar esta conexão."; //
                lblStatusConexao.ForeColor = Color.DarkGreen; //
            }
            else
            {
                lblStatusConexao.Text = $"Falha no teste com {portaSelecionada}. Verifique as configurações ou se a porta está em uso."; //
                lblStatusConexao.ForeColor = Color.Red; //
                                                        // Após um teste falho, é bom re-atualizar o status para o estado real da conexão persistente
                AtualizarStatusConexaoLabel();
            }
        }

        private void btnAtualizarPortasCOM_Click(object sender, EventArgs e)
        {
            lblStatusConexao.Text = "Atualizando lista de portas COM..."; //
            lblStatusConexao.ForeColor = SystemColors.ControlText; //
            Application.DoEvents(); //

            string portaAnteriormenteSelecionada = comboBoxPortaCOM.SelectedItem?.ToString(); //
            string baudAnteriormenteSelecionado = comboBoxBaudRate.SelectedItem?.ToString(); //

            PopularControlesCom(); // Repopula as portas, sensores, módulos

            // Tenta restaurar a seleção anterior se ainda existir
            if (!string.IsNullOrEmpty(portaAnteriormenteSelecionada) && comboBoxPortaCOM.Items.Contains(portaAnteriormenteSelecionada)) //
            {
                comboBoxPortaCOM.SelectedItem = portaAnteriormenteSelecionada; //
            }
            if (!string.IsNullOrEmpty(baudAnteriormenteSelecionado) && comboBoxBaudRate.Items.Contains(Convert.ToInt32(baudAnteriormenteSelecionado))) //
            {
                comboBoxBaudRate.SelectedItem = Convert.ToInt32(baudAnteriormenteSelecionado); //
            }

            // O CarregarConfiguracoesSalvas pode ser chamado aqui para restaurar tudo,
            // ou apenas as partes de porta/baud como acima.
            // Se PopularControlesCom e CarregarConfiguracoesSalvas forem bem definidos,
            // eles podem ser suficientes.
            CarregarConfiguracoesSalvas(); // Para garantir que sensor/modulo também sejam restaurados se possível.


            if (comboBoxPortaCOM.Enabled) //
            {
                lblStatusConexao.Text = "Lista de portas COM atualizada. Verifique outras seleções."; //
            }
            else
            {
                lblStatusConexao.Text = "Nenhuma porta COM encontrada após atualização."; //
            }
            // Re-atualizar status da conexão principal
            AtualizarStatusConexaoLabel();
        }

        private void maskedTextBoxCoeficiente_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Esta é uma forma simples de restringir a entrada se a máscara não for suficiente.
            // A máscara "000.00000" já restringe a dígitos e ao literal '.'.
            // Se a máscara for "", então este código pode ser útil.
            // char separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
            // if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != separator))
            // {
            //    e.Handled = true;
            // }
            // Permite apenas um separador decimal
            // if ((e.KeyChar == separator) && ((sender as MaskedTextBox).Text.IndexOf(separator) > -1))
            // {
            //    e.Handled = true;
            // }
        }

        private void maskedTextBoxCoeficiente_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            ToolTip tt = new ToolTip();
            tt.ToolTipTitle = "Entrada Inválida";
            tt.Show("Apenas números são permitidos para esta posição na máscara.", maskedTextBoxCoeficiente, 0, -20, 2000);
            //MessageBox.Show("Por favor, insira apenas números no formato da máscara.", "Entrada Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}