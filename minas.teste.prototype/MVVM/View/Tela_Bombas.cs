using System.Drawing;
using System;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.MVVM.ViewModel;
using System.Collections.Generic;
using System.Globalization;

namespace minas.teste.prototype.MVVM.View
{
    public partial class Tela_Bombas : Form
    {
        //          objetos GLOBAIS          //
        //-------------------------------------//
        private Tela_BombasVM _viewModel;
        private apresentacao _fechar_box;
        private List<EtapaData> _dadosColetados = new List<EtapaData>();
        public Dictionary<string, TextBox> sensorMap;
        public Dictionary<string, string> sensorMapmedida;
        private List<string> dadosSensoresSelecionados = new List<string>();
        public List<SensorData> dadosSensores;
        private Timer monitoramentoTimer;
        private Timer timer;
        private TextBoxTrackBarSynchronizer _synchronizerNivel;






        //-------------------------------------//
        //          variaveis GLOBAIS          //
        //-------------------------------------//

        private bool _fechamentoForcado;
        private bool _isMonitoring = false;
        public string Inicioteste;
        public string Fimteste;
        private int etapaAtual = 1;
        public string StatusText;
        public string TbNomeCliente { get; set; } //textbox nome cliente
        public string TbNomeBomba { get; set; } //
        public string TbOrdemServico { get; set; }
        private int valorDefinido;
        private bool cronometroIniciado = false;
        private int tempoTotalSegundos;
        private bool valorDefinidoManualmente = false;
        private Dictionary<string, string> sensorControlMap;




        // Move the initialization of the static dictionary to the constructor or a method,  
        // as static fields cannot reference instance members directly.  

        public Tela_Bombas()
        {
            InitializeComponent();
            Inciaizador_listas();
            
            _viewModel = new Tela_BombasVM();
            _fechar_box = new apresentacao();
            _synchronizerNivel = new TextBoxTrackBarSynchronizer(textBox2, trackBar1, 1, 7);
            dadosSensores = new List<SensorData> { new SensorData() };
            timer = new Timer();
            timer.Interval = 1000; // Intervalo de 1 segundo
            timer.Tick += Timer_Tick;


        }

        public void Inciaizador_listas()
        {

            sensorMap = new Dictionary<string, TextBox>();
            sensorMap.Add("psi_PL", sensor_psi_PL);
            sensorMap.Add("bar_PL", sensor_bar_PL);
            sensorMap.Add("gpm_DR", sensor_gpm_DR);
            sensorMap.Add("lpm_DR", sensor_lpm_DR);
            sensorMap.Add("Press_PSI", sensor_Press_PSI);
            sensorMap.Add("Press_BAR", sensor_Press_BAR);
            sensorMap.Add("rotacao_RPM", sensor_rotacao_RPM);
            sensorMap.Add("Vazao_GPM", sensor_Vazao_GPM);
            sensorMap.Add("Vazao_LPM", sensor_Vazao_LPM);
            sensorMap.Add("Temp_C", sensor_Temp_C);

            sensorMapmedida = new Dictionary<string, string>();
            sensorMapmedida.Add("psi_PL", "psi");
            sensorMapmedida.Add("bar_PL", "bar");
            sensorMapmedida.Add("gpm_DR", "gpm");
            sensorMapmedida.Add("lpm_DR", "lpm");
            sensorMapmedida.Add("Press_PSI", "psi");
            sensorMapmedida.Add("Press_BAR", "bar");
            sensorMapmedida.Add("rotacao_RPM", "rpm");
            sensorMapmedida.Add("Vazao_GPM", "gpm");
            sensorMapmedida.Add("Vazao_LPM", "lpm");
            sensorMapmedida.Add("Temp_C", "celsus");

            sensorControlMap = new Dictionary<string, string>();

            sensorControlMap.Add("Pilotagem PSI", "sensor_psi_PL");
            sensorControlMap.Add("Pilotagem BAR", "sensor_bar_PL");
            sensorControlMap.Add("Dreno GPM", "sensor_gpm_DR");
            sensorControlMap.Add("Dreno LPM", "sensor_lpm_DR");
            sensorControlMap.Add("Pressao PSI", "sensor_Press_PSI");
            sensorControlMap.Add("Pressao BAR", "sensor_Press_BAR");
            sensorControlMap.Add("Rotação RPM", "sensor_rotacao_RPM");
            sensorControlMap.Add("Vazão GPM", "sensor_Vazao_GPM");
            sensorControlMap.Add("Vazão LPM", "sensor_Vazao_LPM");
            sensorControlMap.Add("Temperatura Celsius", "sensor_Temp_C");


            dadosSensoresSelecionados.AddRange(sensorMap.Keys);

        }


        #region LOADS_JANELA    
        private void Tela_Bombas_Load(object sender, EventArgs e)
        {
            _viewModel.Carregar_configuracao(this); // Carrega o estilo do formulário  
            _viewModel.Stage_signal(Stage_box_bomba);
            _viewModel.VincularRelogioLabel(LabelHorariotela);// configura a imagem de teste ligado ou desligado  
            HistoricalEvents.Text = "AGUARDANDO INÍCIO DO ENSAIO...";
            HistoricalEvents.ForeColor = System.Drawing.Color.DarkGreen;// Carrega o histórico de eventos

        }

        #endregion

        #region EVENTOS_FECHAMANETO  
        private void CloseWindows_Click(object sender, EventArgs e)
        {
            _fechamentoForcado = true; // Indica que é um fechamento controlado  
            Menuapp.Instance.Show();
            this.Close();
        }

        private void Tela_Bombas_FormClosing(object sender, FormClosingEventArgs e)
        {

            // Só encerra a aplicação se não for um fechamento controlado  
            if (!_fechamentoForcado)
            {
                _fechar_box.apresentacao_FormClosing(sender, e);
            }
            else
                Menuapp.Instance.Show();

        }
        #endregion

        #region INCIO_TESTE
        private async void btnIniciar_Click(object sender, EventArgs e)
        {
            if (!_viewModel.cabecalhoinicial(textBox6, textBox5, textBox4))
            {
                MessageBox.Show("Favor preencher os campos obrigatórios em DADOS DE ENSAIO.");
                await _viewModel.PiscarLabelsVermelho(label6, label5, label4, 1000);
                return;
            }

            _isMonitoring = true;
            _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5, panel2, panel6, panel11, panel9);
            Inicioteste = DateTime.Now.ToString(); // Certifique-se de que Inicioteste é uma string acessível
            _viewModel.IniciarTesteBomba(Stage_box_bomba);
            trackBar1.Enabled = false;
            InicializarMonitoramento();

            if (valorDefinidoManualmente)
            {
                cronometroIniciado = true;
                int tempoTotalSegundos = valorDefinido * 60;
                circularProgressBar1.Maximum = tempoTotalSegundos;
                circularProgressBar1.Minimum = 0;
                circularProgressBar1.Value = tempoTotalSegundos;
                circularProgressBar1.Invalidate();
                timer.Start();
            }
            else
            {
                // Se o valor não foi definido, desabilita os botões de controle do teste
                button4.Enabled = false; // Supondo que button4 seja o botão "Parar" ou similar
                button6.Enabled = false; // Supondo que button6 seja outro botão de controle
                MessageBox.Show("O cronômetro não foi definido. O teste não será finalizado automaticamente.", "Aviso");
            }
        }

        private void InicializarMonitoramento()
        {
            monitoramentoTimer = new System.Windows.Forms.Timer();
            monitoramentoTimer.Interval = 1000; // Intervalo de 1 segundo
            monitoramentoTimer.Tick += MonitoramentoTimer_Tick;
            monitoramentoTimer.Start();
        }

        private void PararMonitoramento()
        {
            if (monitoramentoTimer != null && monitoramentoTimer.Enabled)
            {
                monitoramentoTimer.Stop();
                monitoramentoTimer.Dispose();
                monitoramentoTimer = null;
            }
        }

        private void MonitoramentoTimer_Tick(object sender, EventArgs e)
        {
            // Obtenha os valores dos controles na View
            _viewModel.MonitorarDados(
                sensor_psi_PL.Text, textBox9.Text, textBox8.Text, checkBox_psi.Checked, panel4, HistoricalEvents,
                sensor_Press_PSI.Text, textBox9.Text, textBox8.Text, checkBox_psi.Checked, panel5, 
                sensor_gpm_DR.Text, textBox14.Text, textBox12.Text, checkBox_gpm.Checked, panel2, 
                sensor_Vazao_GPM.Text, textBox14.Text, textBox12.Text, checkBox_gpm.Checked, panel6, 
                sensor_Press_BAR.Text, textBox11.Text, textBox10.Text, checkBox_bar.Checked, panel5, 
                sensor_bar_PL.Text, textBox11.Text, textBox10.Text, checkBox_bar.Checked, panel4, 
                sensor_rotacao_RPM.Text, textBox18.Text, textBox17.Text, checkBox_rotacao.Checked, panel9, 
                sensor_Vazao_LPM.Text, textBox16.Text, textBox15.Text, checkBox_lpm.Checked, panel6, 
                sensor_lpm_DR.Text, textBox16.Text, textBox15.Text, checkBox_lpm.Checked, panel2, 
                sensor_Temp_C.Text, textBox20.Text, textBox19.Text, checkBox_temperatura.Checked, panel11
            );
        }

        #endregion

        #region FIM_TESTE
        private void btnParar_Click(object sender, EventArgs e)
        {
            timer.Stop();
            cronometroIniciado = false;
            _isMonitoring = false;
            _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5, panel2, panel6, panel11, panel9);
            Fimteste = DateTime.Now.ToString();
            trackBar1.Enabled = true;
            PararMonitoramento();
            _viewModel.FinalizarTesteBomba(Stage_box_bomba);
            

        }


        #endregion

        #region BOTÕES_MEDIDAS 
        private void unidade_medidapilotagem1_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_psi_PL.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pilotagem",
                    Valor = valorAtual,
                    Medidas = "psi"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }


        private void unidade_medidapilotagem2_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_bar_PL.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pilotagem",
                    Valor = valorAtual,
                    Medidas = "bar"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidadreno1_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_lpm_DR.Text; // Nota: O original estava como gpm, mantive o nome do controle

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "dreno",
                    Valor = valorAtual,
                    Medidas = "gpm" // Medida definida como gpm
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidadreno2_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_lpm_DR.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "dreno",
                    Valor = valorAtual,
                    Medidas = "lpm"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidapressao1_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_Press_PSI.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pressão",
                    Valor = valorAtual,
                    Medidas = "psi"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidapressao2_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_Press_BAR.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "pressão",
                    Valor = valorAtual,
                    Medidas = "bar"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidarota_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_rotacao_RPM.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "rotação",
                    Valor = valorAtual,
                    Medidas = "rpm"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidasvazao1_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_Vazao_GPM.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "vazão",
                    Valor = valorAtual,
                    Medidas = "gpm"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidasvazao2_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_Vazao_LPM.Text;

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = "vazão",
                    Valor = valorAtual,
                    Medidas = "lpm"
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void unidade_medidatemp_Click(object sender, EventArgs e)
        {
            if (_isMonitoring)
            {
                // Capturar o valor atual do TextBox
                string valorAtual = sensor_Temp_C.Text;
                string sendorname = sensor_Temp_C.Tag.ToString();

                // Adicionar à lista
                dadosSensores.Add(new SensorData
                {
                    Sensor = sendorname,
                    Valor = valorAtual,
                    Medidas = "celsus" // Pode querer corrigir para "celsius" se for o caso
                });

                // Atualizar o DataGridView
                _viewModel.AtualizarVisualizador(visualizador, dadosSensores);
            }
            else
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
            }
        }

        private void btn_gravar_Click(object sender, EventArgs e)
        {
            if (!_isMonitoring)
            {
                MessageBox.Show("Teste não iniciado, favor iniciar o teste para capturar os dados.");
                return; // Sai do método se não estiver monitorando
            }

           
            if (etapaAtual <= _synchronizerNivel.maxControlEtapas )
            {
                dataGridViewLoad();
                AtualizarDataGridView();
                foreach (var item in dadosSensoresSelecionados)
                {
                    if (sensorMap.ContainsKey(item))
                    {
                        string valorAtual = sensorMap[item].Text;
                        string medida = sensorMapmedida[item];
                        string sendorname = sensorMap[item].Tag.ToString();

                        // Criar uma nova instância de EtapaData e inicializar a lista de leituras
                        var etapaData = new EtapaData
                        {
                            Etapa = etapaAtual,
                            leituras = new List<SensorData>()
                        };

                        // Adicionar os dados do sensor à lista de leituras
                        etapaData.leituras.Add(new SensorData
                        {
                            Sensor = sendorname,
                            Valor = valorAtual,
                            Medidas = medida
                        });

                        // Adicionar a etapa à lista de dados coletados
                        _dadosColetados.Add(etapaData);
                    }
                }

                // Incrementa a etapa atual
                etapaAtual++;
            }
            else
            {
                MessageBox.Show("Limite de etapas atingido.");
            }

        }

        private void btnDefinir_Click(object sender, EventArgs e)
        {
            if (!cronometroIniciado)
            {
                if (int.TryParse(textBox1.Text, out int valor))
                {
                    valorDefinido = valor;
                    valorDefinidoManualmente = true;
                    // Habilita os botões button4 e button6 após definir um valor
                    button4.Enabled = false;
                    button6.Enabled = true;
                    MessageBox.Show($"O valor {valorDefinido} foi definido.", "Sucesso");
                }
                else
                {
                    MessageBox.Show("Por favor, insira um valor numérico válido no TextBox.", "Erro");
                }
            }
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            if (!cronometroIniciado)
            {
                textBox1.Text = "0";
                valorDefinidoManualmente = false;
                // Desabilita os botões button4 e button6 ao limpar o textbox
                button4.Enabled = true;
                button6.Enabled = true;
            }
        }
        #endregion


        #region VALIDAÇÕES 
        /// <summary>
        /// Valida os TextBoxes de mínimo e máximo associados a um CheckBox.
        /// Verifica se são numéricos, não negativos e se min <= max.
        /// Desmarca o CheckBox e exibe uma mensagem se a validação falhar.
        /// </summary>
        /// <param name="checkBox">O CheckBox que está sendo validado.</param>
        /// <param name="minTextBox">O TextBox que contém o valor mínimo.</param>
        /// <param name="maxTextBox">O TextBox que contém o valor máximo.</param>
        /// <param name="nomeUnidade">O nome da unidade/medição (ex: "PSI", "GPM") para usar na mensagem de erro.</param>
        /// <returns>True se a validação passar, False caso contrário.</returns>
        private bool ValidarMinMaxCheckBox(CheckBox checkBox, TextBox minTextBox, TextBox maxTextBox, string nomeUnidade)
        {
            // Tenta converter os valores, usando InvariantCulture para consistência (ponto decimal)
            bool minOk = decimal.TryParse(minTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMinimo);
            bool maxOk = decimal.TryParse(maxTextBox.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal valorMaximo);

            string erroMsg = null; // Armazena a mensagem de erro específica

            // 1. Valida se são números
            if (!minOk || !maxOk)
            {
                erroMsg = $"Os valores de Mínimo e Máximo para {nomeUnidade} devem ser numéricos.";
            }
            // 2. Valida se não são negativos (só faz sentido se forem números)
            else if (valorMinimo < 0 || valorMaximo < 0)
            {
                erroMsg = $"Os valores de Mínimo e Máximo para {nomeUnidade} não podem ser menores que 0.";
            }
            // 3. Valida se min <= max (só faz sentido se não forem negativos)
            else if (valorMinimo > valorMaximo)
            {
                erroMsg = $"O valor Mínimo para {nomeUnidade} não pode ser maior que o valor Máximo .";
            }

            // Se houve algum erro (erroMsg foi definida)
            if (erroMsg != null)
            {
                // Desmarca o CheckBox que disparou a validação
                checkBox.Checked = false;

                // Mostra a mensagem de erro
                MessageBox.Show(erroMsg + $"\nA verificação {nomeUnidade} foi desativada.",
                                $"Erro de Validação - {nomeUnidade}",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                minTextBox.Clear(); // Equivalente a minTextBox.Text = "";
                maxTextBox.Clear(); // Equivalente a maxTextBox.Text = "";

               
                // Opcional: Foca no TextBox com problema
                if (!minOk || (minOk && valorMinimo < 0) || (minOk && maxOk && valorMinimo > valorMaximo))
                {
                    minTextBox.Focus();
                    minTextBox.SelectAll();
                }
                else if (!maxOk || (maxOk && valorMaximo < 0))
                {
                    maxTextBox.Focus();
                    maxTextBox.SelectAll();
                }

                return false; // Indica que a validação falhou
            }

            return true; // Indica que a validação passou
        }

        private void checkBox_psi_CheckedChanged(object sender, EventArgs e)
        {
            // 'sender' é o CheckBox que disparou o evento
            CheckBox cb = sender as CheckBox;
            // Só valida se o checkbox FOI MARCADO
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para PSI
                ValidarMinMaxCheckBox(cb, textBox9, textBox8, "PSI");
            }
        }

        private void checkBox_gpm_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para GPM
                ValidarMinMaxCheckBox(cb, textBox14, textBox12, "GPM");
            }
        }

        private void checkBox_bar_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para BAR
                ValidarMinMaxCheckBox(cb, textBox11, textBox10, "BAR");
            }
        }

        private void checkBox_rotacao_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para Rotação
                ValidarMinMaxCheckBox(cb, textBox18, textBox17, "Rotação (RPM)");
            }
        }

        private void checkBox_lpm_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para LPM
                ValidarMinMaxCheckBox(cb, textBox16, textBox15, "LPM");
            }
        }

        private void checkBox_temperatura_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb != null && cb.Checked)
            {
                // Chama a função reutilizável com os TextBoxes e nome corretos para Temperatura
                ValidarMinMaxCheckBox(cb, textBox20, textBox19, "Temperatura (°C)");
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (circularProgressBar1.Value > 0)
            {
                circularProgressBar1.Value--;
            }
            else
            {
                timer.Stop();
                cronometroIniciado = false; // Permite definir e limpar novamente
                btnParar_Click(this, EventArgs.Empty);
            }
        }

        #endregion

        public int ObterValorDefinido()
        {
            return valorDefinido;
        }

        private void tableLayoutPanel15_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnrelatoriobomba_Click(object sender, EventArgs e)
        {
            Realatoriobase relatorio = new Realatoriobase();
            //relatorio.GerarRelatorio(dadosSensores, _dadosColetados, Inicioteste, Fimteste);
            relatorio.Show();
            this.Hide();

        }

        public void dataGridViewLoad()
        {
            // Inicializa o DataGridView
            dataGridView1.Size = new Size(852, 174);
            dataGridView1.ColumnCount = 11; // Etapa + 10 sensores
            dataGridView1.Columns[0].HeaderText = "Etapa";
            dataGridView1.Columns[1].HeaderText = "Pilotagem PSI";
            dataGridView1.Columns[2].HeaderText = "Pilotagem BAR";
            dataGridView1.Columns[3].HeaderText = "Dreno GPM";
            dataGridView1.Columns[4].HeaderText = "Dreno LPM";
            dataGridView1.Columns[5].HeaderText = "Pressao PSI";
            dataGridView1.Columns[6].HeaderText = "Pressao BAR";
            dataGridView1.Columns[7].HeaderText = "Rotação RPM";
            dataGridView1.Columns[8].HeaderText = "Vazão GPM";
            dataGridView1.Columns[9].HeaderText = "Vazão LPM";
            dataGridView1.Columns[10].HeaderText = "Temperatura Celsius";

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        public void AtualizarDataGridView()
        {
            // Obtém os valores dos sensores
            string pilotagemPsi = sensor_psi_PL.Text;
            string pilotagemBar = sensor_bar_PL.Text;
            string drenoGpm = sensor_gpm_DR.Text;
            string drenoLpm = sensor_lpm_DR.Text;
            string pressaoPsi = sensor_Press_PSI.Text;
            string pressaoBar = sensor_Press_BAR.Text;
            string rotacaoRpm = sensor_rotacao_RPM.Text;
            string vazaoGpm = sensor_Vazao_GPM.Text;
            string vazaoLpm = sensor_Vazao_LPM.Text;
            string temperaturaCelsius = sensor_Temp_C.Text;

            // Cria uma nova linha com os dados
            string[] novaLinha = new string[]
            {
            $"Etapa {etapaAtual}",
            pilotagemPsi,
            pilotagemBar,
            drenoGpm,
            drenoLpm,
            pressaoPsi,
            pressaoBar,
            rotacaoRpm,
            vazaoGpm,
            vazaoLpm,
            temperaturaCelsius
            };

            // Adiciona a nova linha ao DataGridView
            if (dataGridView1.Rows.Count < _synchronizerNivel.maxControlEtapas)
            {
                dataGridView1.Rows.Add(novaLinha);
                
            }
            else
            {
                // Se já atingiu o máximo de linhas, você pode optar por:
                // 1. Remover a primeira linha e adicionar a nova (comportamento de histórico)
                // 2. Exibir uma mensagem informando que o limite foi atingido
                // 3. Desabilitar o botão de gravação
                
                dataGridView1.Rows.Add(novaLinha);
                // etapaAtual permanece o mesmo ou você pode decidir incrementá-lo
            }
        }


    }
}

