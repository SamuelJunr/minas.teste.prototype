using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Extensions.Logging;
using minas.teste.prototype.Estilo;
using minas.teste.prototype.MVVM.Model.Abstract;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.Service;
using static System.Net.Mime.MediaTypeNames;
using Panel = System.Windows.Forms.Panel;

namespace minas.teste.prototype.MVVM.ViewModel
{
    public class Tela_BombasVM
    {
        private cronometroSK4 _cronometro = new cronometroSK4();
        private System.Windows.Forms.Timer _timerAtualizacao;
        public const string LABEL_CRONOMETRO = "labelCronometro_bomba";
        private HoraDia _Tempo;
        private SessaoBomba _sessaoBomba;
        public event EventHandler<Datapoint_Bar_Rpm> Chart1Data;

        // --- Chart Data Logic Fields ---
        private double _previousRotationRpm = double.NaN;
        private const double RotationTolerance = 10.0; // Tolerance for considering rotation constant (adjust as needed)

        // Data structure for chart points (Pressure in Bar, Flow in Lpm)
        public struct Datapoint_Bar_Lpm
        {
            public double PressureBar { get; set; }
            public double FlowLpm { get; set; }

            public Datapoint_Bar_Lpm(double pressureBar, double flowLpm)
            {
                PressureBar = pressureBar;
                FlowLpm = flowLpm;
            }
        }
        // --- End Chart Data Logic Fields ---


        #region PROPRIEDADES_JANELA
        public void Carregar_configuracao(Form FormView)
        {
            FormView.Text = Properties.Resources.ResourceManager.GetString("MainFormTitle");
            SetupCronometroTimer();
            VincularCronometroLabel(FormView);
        }
        public void Stage_signal(PictureBox stage)
        {
            stage.BackgroundImage = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("off");

        }
        public void VincularRelogioLabel(Label relogio)
        {
            _Tempo = new HoraDia(relogio);

        }

        #endregion

        #region CRONÔMETRO
        // Métodos do Cronômetro
        private void SetupCronometroTimer()
        {
            _timerAtualizacao = new System.Windows.Forms.Timer();
            _timerAtualizacao.Interval = 100;
            _timerAtualizacao.Tick += (s, e) => AtualizarDisplayCronometro();
        }

        private void VincularCronometroLabel(Form view)
        {
            _cronometro.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(_cronometro.FormattedElapsed))
                {
                    var label = view.Controls.Find(LABEL_CRONOMETRO, true).FirstOrDefault() as Label;
                    AtualizarLabelSeguro(label, _cronometro.FormattedElapsed);
                }
            };
        }

        public void IniciarCronometro()
        {
            if (_cronometro.IsRunning) return;

            if (_cronometro.GetCurrentTotalElapsedTime() > TimeSpan.Zero)
                _cronometro.AccumulatedElapsed = TimeSpan.Zero;

            _cronometro.StartTime = DateTime.Now;
            _cronometro.IsRunning = true;
            _timerAtualizacao.Start();
        }

        public void PararCronometro()
        {
            if (!_cronometro.IsRunning) return;

            _cronometro.AccumulatedElapsed = _cronometro.GetCurrentTotalElapsedTime();
            _cronometro.IsRunning = false;
            _timerAtualizacao.Stop();
            AtualizarDisplayCronometro();
        }

        private void AtualizarDisplayCronometro()
        {
            _cronometro.UpdateFormattedString();
        }

        private void AtualizarLabelSeguro(Label label, string valor)
        {
            if (label == null) return;

            if (label.InvokeRequired)
                label.BeginInvoke((Action)(() => label.Text = valor));
            else
                label.Text = valor;
        }
        #endregion

        #region VALIDACAO


        public void MonitorarDados(
           string psiPLValor, string psiPLMin, string psiPLMax, bool psiPLAtivo, Panel psiPLPanel, TextBox historicalEvents,
           string pressPSIValor, string pressPSIMin, string pressPSIMax, bool pressPSIAtivo, Panel pressPSIPanel,
           string gpmDRValor, string gpmDRMin, string gpmDRMax, bool gpmDRAtivo, Panel gpmDRPanel,
           string vazaoGPMValor, string vazaoGPMMin, string vazaoGPMMax, bool vazaoGPMAtivo, Panel vazaoGPMPanel,
           string pressBARValor, string pressBARMin, string pressBARMax, bool pressBARAtivo, Panel pressBARPanel,
           string barPLValor, string barPLMin, string barPLMax, bool barPLAtivo, Panel barPLPanel, 
           string rotacaoRPMValor, string rotacaoRPMMin, string rotacaoRPMMax, bool rotacaoRPMAtivo, Panel rotacaoRPMPanel, 
           string vazaoLPMValor, string vazaoLPMMin, string vazaoLPMMax, bool vazaoLPMAtivo, Panel vazaoLPMPanel, 
           string lpmDRValor, string lpmDRMin, string lpmDRMax, bool lpmDRAtivo, Panel lpmDRPanel,
           string tempCValor, string tempCMin, string tempCMax, bool tempCAtivo, Panel tempCPanel
       )
        {
            VerificarRange("Pilotagem PSI", psiPLValor, psiPLMin, psiPLMax, psiPLAtivo, psiPLPanel, historicalEvents);
            VerificarRange("Pressão PSI", pressPSIValor, pressPSIMin, pressPSIMax, pressPSIAtivo, pressPSIPanel, historicalEvents);
            VerificarRange("Dreno GPM", gpmDRValor, gpmDRMin, gpmDRMax, gpmDRAtivo, gpmDRPanel, historicalEvents);
            VerificarRange("Vazão GPM", vazaoGPMValor, vazaoGPMMin, vazaoGPMMax, vazaoGPMAtivo, vazaoGPMPanel, historicalEvents);
            VerificarRange("Pressão BAR", pressBARValor, pressBARMin, pressBARMax, pressBARAtivo, pressBARPanel, historicalEvents);
            VerificarRange("Pilotagem BAR", barPLValor, barPLMin, barPLMax, barPLAtivo, barPLPanel, historicalEvents);
            VerificarRange("Rotação RPM", rotacaoRPMValor, rotacaoRPMMin, rotacaoRPMMax, rotacaoRPMAtivo, rotacaoRPMPanel, historicalEvents);
            VerificarRange("Vazão LPM", vazaoLPMValor, vazaoLPMMin, vazaoLPMMax, vazaoLPMAtivo, vazaoLPMPanel, historicalEvents);
            VerificarRange("Dreno LPM", lpmDRValor, lpmDRMin, lpmDRMax, lpmDRAtivo, lpmDRPanel, historicalEvents);
            VerificarRange("Temperatura Celsus", tempCValor, tempCMin, tempCMax, tempCAtivo, tempCPanel, historicalEvents);
        }

        private void SetPanelImage(Panel panel, string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                panel.BackgroundImage?.Dispose(); // Libera a imagem anterior, se houver
                panel.BackgroundImage = null;
                panel.BackColor = SystemColors.Control; // Cor padrão se nenhum recurso for especificado
                return;
            }

            try
            {
                // Tenta obter o recurso como byte array (mais comum para imagens em Resources)
                object resourceObject = Properties.Resources.ResourceManager.GetObject(resourceName);

                if (resourceObject is byte[] imageBytes)
                {
                    using (var ms = new MemoryStream(imageBytes))
                    {
                        panel.BackgroundImage?.Dispose(); // Libera a imagem anterior
                        panel.BackgroundImage = System.Drawing.Image.FromStream(ms);
                    }
                }
                // Tenta obter como um objeto Image (menos comum, mas possível)
                else if (resourceObject is System.Drawing.Image image)
                {
                    panel.BackgroundImage?.Dispose(); // Libera a imagem anterior
                                                      // Clonar é mais seguro para evitar problemas se o recurso for modificado/liberado em outro lugar
                    panel.BackgroundImage = (System.Drawing.Image)image.Clone();
                }
                else
                {
                    // Recurso não encontrado ou tipo inválido
                    panel.BackgroundImage?.Dispose();
                    panel.BackgroundImage = null;
                    panel.BackColor = Color.Magenta; // Cor de erro para indicar problema no recurso
                    Console.WriteLine($"Alerta: Recurso '{resourceName}' não encontrado ou não é uma imagem válida.");
                    // Ou logar em um arquivo de log
                }
            }
            catch (Exception ex)
            {
                // Erro ao carregar ou definir a imagem
                panel.BackgroundImage?.Dispose();
                panel.BackgroundImage = null;
                panel.BackColor = Color.Red; // Cor de erro crítico
                Console.WriteLine($"Erro ao definir imagem do painel com recurso '{resourceName}': {ex.Message}");
                // Ou logar em um arquivo de log
            }
        }


        /// <summary>
        /// Verifica se o valor de um sensor está dentro do range definido e atualiza
        /// um painel com uma imagem indicativa (_on para fora do range, _off para dentro/inativo).
        /// </summary>
        /// <param name="sensorNome">Nome descritivo do sensor.</param>
        /// <param name="sensorValorTexto">Valor atual do sensor (como string).</param>
        /// <param name="minValorTexto">Valor mínimo permitido (como string).</param>
        /// <param name="maxValorTexto">Valor máximo permitido (como string).</param>
        /// <param name="ativo">Indica se a verificação para este sensor está ativa.</param>
        /// <param name="panelAlerta">O painel cuja imagem de fundo será alterada.</param>
        /// <param name="historicalEvents">TextBox para registrar eventos (ex: valor fora do range).</param>
        private void VerificarRange(string sensorNome, string sensorValorTexto, string minValorTexto, string maxValorTexto, bool ativo, Panel panelAlerta, TextBox historicalEvents)
        {
            string resourceNameOn = null;
            string resourceNameOff = null;

            // Mapeia o nome do sensor para os nomes dos recursos de imagem
            // *** IMPORTANTE: Assumindo que existem recursos _on correspondentes ***
            switch (sensorNome)
            {
                case "Pilotagem PSI":
                    resourceNameOn = "pilotagem_on"; // Assumido
                    resourceNameOff = "pilotagem_off";
                    break;
                case "Pressão PSI":
                    resourceNameOn = "pressao_on"; // Assumido
                    resourceNameOff = "pressao_off";
                    break;
                case "Dreno GPM":
                    resourceNameOn = "dreno_on"; // Assumido
                    resourceNameOff = "dreno_off";
                    break;
                case "Vazão GPM":
                    resourceNameOn = "vazao_on"; // Assumido
                    resourceNameOff = "vazao_off";
                    break;
                case "Pressão BAR":
                    resourceNameOn = "pressao_on"; // Reutilizando imagem de pressão
                    resourceNameOff = "pressao_off";
                    break;
                case "Pilotagem BAR":
                    resourceNameOn = "pilotagem_on"; // Reutilizando imagem de pilotagem
                    resourceNameOff = "pilotagem_off";
                    break;
                case "Rotação RPM":
                    resourceNameOn = "rotacao_on"; // Assumido
                    resourceNameOff = "rotacao_off";
                    break;
                case "Vazão LPM":
                    resourceNameOn = "vazao_on"; // Reutilizando imagem de vazão
                    resourceNameOff = "vazao_off";
                    break;
                case "Dreno LPM":
                    resourceNameOn = "dreno_on"; // Reutilizando imagem de dreno
                    resourceNameOff = "dreno_off";
                    break;
                case "Temperatura Celsus": // Corrigido de "Celsus" para "Celsius" se for o caso
                    resourceNameOn = "termometro_on"; // Assumido
                    resourceNameOff = "termometro_off";
                    break;
                default:
                    Console.WriteLine($"Aviso: Nome de sensor não mapeado para imagens: '{sensorNome}'");
                    // Define uma aparência padrão ou de erro se o sensor não for conhecido
                    SetPanelImage(panelAlerta, null); // Limpa a imagem
                    panelAlerta.BackColor = Color.Gray; // Indica estado desconhecido/não mapeado
                    return; // Sai da função se o sensor não for reconhecido
            }



            // Tenta converter os valores de string para decimal
            bool valorOk = decimal.TryParse(sensorValorTexto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal valorSensor);
            bool minOk = decimal.TryParse(minValorTexto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal valorMinimo);
            bool maxOk = decimal.TryParse(maxValorTexto, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal valorMaximo);

            // Verifica se todas as conversões foram bem-sucedidas
            if (ativo)
            {
                if (valorOk && minOk && maxOk)
                {
                    // Verifica se o valor está fora do range permitido
                    if (valorSensor < valorMinimo || valorSensor > valorMaximo)
                    {
                        // Valor fora do range - mostra imagem 'on' e registra evento
                        SetPanelImage(panelAlerta, resourceNameOff);
                        // Adiciona ao histórico (usando AppendText para não sobrescrever)
                        // Adiciona data/hora para melhor rastreamento
                        historicalEvents.AppendText($"{DateTime.Now:G}: O valor do sensor {sensorNome} ({valorSensor}) está fora do range [{valorMinimo} - {valorMaximo}]." + Environment.NewLine);
                    }
                    
                }
                else
                {
                    // Erro ao converter algum dos valores
                    // Decide como tratar: mostrar imagem 'off'? Mostrar indicador de erro?
                    // Opção 1: Mostrar imagem 'off' (estado seguro/indeterminado)
                    SetPanelImage(panelAlerta, resourceNameOff);

                }
            }
            else
            {
                SetPanelImage(panelAlerta, resourceNameOn);
            }

            
        }

        #endregion

        #region BOTÕES

        public void IniciarTesteBomba(PictureBox stage)
        {
            IniciarCronometro();
            using (var ms = new System.IO.MemoryStream((byte[])Properties.Resources.ResourceManager.GetObject("on")))
            {
                stage.BackgroundImage = System.Drawing.Image.FromStream(ms);
            }
            _sessaoBomba = new SessaoBomba();

        }

        public void FinalizarTesteBomba(PictureBox stage)
        {
            PararCronometro();
            stage.BackgroundImage = (System.Drawing.Image)Properties.Resources.ResourceManager.GetObject("off");
            if (_sessaoBomba != null)
            {
                _sessaoBomba.FinalizarSessao();
                mensagemConfirmacao();
            }
           
        }

        public void mensagemConfirmacao()
        {
            var resultado = MessageBox.Show(
           "Deseja salvar todos os dados do teste?",
           "Confirmação de Salvamento",
           MessageBoxButtons.YesNo,
           MessageBoxIcon.Question,
           MessageBoxDefaultButton.Button1
            );

            if (resultado == DialogResult.Yes)
            {
                _sessaoBomba.ProcessarSalvamento();
            }
            else
            {
                MessageBox.Show(
                    "Dados não foram salvos!",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        public bool cabecalhoinicial(TextBox textBox6, TextBox textBox5, TextBox textBox4)
        {
            if (string.IsNullOrEmpty(textBox6?.Text) ||
                string.IsNullOrEmpty(textBox5?.Text) ||
                string.IsNullOrEmpty(textBox4?.Text))
            {
                return false;
            }
            return true;
        }
        public async Task PiscarLabelsVermelho(Label label1, Label label2, Label label3, int duration)
        {
            Color originalColor1 = label1.ForeColor;
            Color originalColor2 = label2.ForeColor;
            Color originalColor3 = label3.ForeColor;
            int interval = 500; // Intervalo de 500 milissegundos (0.5 segundos) para cada troca de cor
            int steps = duration / interval;

            for (int i = 0; i < steps; i++)
            {
                label1.ForeColor = Color.Red;
                label2.ForeColor = Color.Red;
                label3.ForeColor = Color.Red;
                await Task.Delay(interval);

                label1.ForeColor = originalColor1;
                label2.ForeColor = originalColor2;
                label3.ForeColor = originalColor3;
                await Task.Delay(interval);
            }

            // Garantir que as cores voltem ao original após o período
            label1.ForeColor = originalColor1;
            label2.ForeColor = originalColor2;
            label3.ForeColor = originalColor3;
        }

        public void LimparCamposEntrada(params TextBox[] textBoxes)
        {
            foreach (var textBox in textBoxes)
            {
                textBox.Text = string.Empty;
            }
        }

        #endregion

        #region GRAFICOS

        // Method to provide chart data with rotation check
        public Datapoint_Bar_Lpm? GetChartDataIfRotationConstant(string pressureBarText, string vazaoLpmText, string rotacaoRpmText)
        {
            // Use InvariantCulture for parsing to handle decimal points consistently
            if (double.TryParse(pressureBarText, NumberStyles.Any, CultureInfo.InvariantCulture, out double pressureBar) &&
                double.TryParse(vazaoLpmText, NumberStyles.Any, CultureInfo.InvariantCulture, out double flowLpm) &&
                double.TryParse(rotacaoRpmText, NumberStyles.Any, CultureInfo.InvariantCulture, out double currentRotationRpm))
            {
                // Check if rotation is constant (within tolerance) or if it's the first reading
                if (double.IsNaN(_previousRotationRpm) || Math.Abs(currentRotationRpm - _previousRotationRpm) <= RotationTolerance)
                {
                    _previousRotationRpm = currentRotationRpm; // Update previous rotation
                    return new Datapoint_Bar_Lpm(pressureBar, flowLpm); // Provide data
                }
                else
                {
                    // Rotation is not constant, do not provide data for the chart
                    _previousRotationRpm = currentRotationRpm; // Update previous rotation even if not constant for check in next tick
                    return null;
                }
            }
            // Parsing failed for one or more values
            _previousRotationRpm = double.NaN; // Reset previous rotation on parsing error
            return null;
        }

        // Method to reset the chart data logic state (e.g., on chart clear/reset)
        public void ResetChartDataLogic()
        {
            _previousRotationRpm = double.NaN;
        }
        public void ProcessChartData(double rotation, double pressure)
        {
            // Aqui você poderia adicionar lógica de validação ou transformação antes
            // de enviar os dados para o gráfico, se necessário.

            // Dispara o evento NewChartData com os dados recebidos
            Chart1Data?.Invoke(this, new Datapoint_Bar_Rpm(rotation, pressure));
        }
        #endregion

        #region FLAGS 
        public void AlterarEstadoPaineis(bool ativo, Panel p1, Panel p2,Panel p3,Panel p4,Panel p5,Panel p6)
        {
            // Implemente a mudança visual dos painéis aqui
            if (ativo)
            {
                // Exibe os painéis
                var imageBytes = (byte[])Properties.Resources.ResourceManager.GetObject("pilotagem_on");
                using (var ms = new MemoryStream(imageBytes))
                {
                    p1.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
                var imageBytes1 = (byte[])Properties.Resources.ResourceManager.GetObject("dreno_on");
                using (var ms = new MemoryStream(imageBytes1))
                {
                    p2.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
                var imageBytes2 = (byte[])Properties.Resources.ResourceManager.GetObject("pressao_on");
                using (var ms = new MemoryStream(imageBytes2))
                {
                    p3.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
                var imageBytes3 = (byte[])Properties.Resources.ResourceManager.GetObject("rotacao_on");
                using (var ms = new MemoryStream(imageBytes3))
                {
                    p4.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
                var imageBytes4 = (byte[])Properties.Resources.ResourceManager.GetObject("vazao_on");
                using (var ms = new MemoryStream(imageBytes4))
                {
                    p5.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
                var imageBytes5 = (byte[])Properties.Resources.ResourceManager.GetObject("termometro_on");
                using (var ms = new MemoryStream(imageBytes5))
                {
                    p6.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
            }
            else
            { 
                var imageBytes = (byte[])Properties.Resources.ResourceManager.GetObject("pilotagem_by");
                using (var ms = new MemoryStream(imageBytes))
                {
                    p1.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
                var imageBytes1 = (byte[])Properties.Resources.ResourceManager.GetObject("dreno_by");
                using (var ms = new MemoryStream(imageBytes1))
                {
                    p2.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
                var imageBytes2 = (byte[])Properties.Resources.ResourceManager.GetObject("pressao_by");
                using (var ms = new MemoryStream(imageBytes2))
                {
                    p3.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
                var imageBytes3 = (byte[])Properties.Resources.ResourceManager.GetObject("rotacao_by");
                using (var ms = new MemoryStream(imageBytes3))
                {
                    p4.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
                var imageBytes4 = (byte[])Properties.Resources.ResourceManager.GetObject("vazao_by");
                using (var ms = new MemoryStream(imageBytes4))
                {
                    p5.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
                var imageBytes5 = (byte[])Properties.Resources.ResourceManager.GetObject("termometro_by");
                using (var ms = new MemoryStream(imageBytes5))
                {
                    p6.BackgroundImage = System.Drawing.Image.FromStream(ms);
                }
            }

        }
        #endregion


        public void AtualizarVisualizador(DataGridView fonte,List<SensorData> data )
        {
            // Configurar o DataGridView se necessário
            if (fonte.Columns.Count == 0)
            {
                fonte.AutoGenerateColumns = true;

                fonte.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = "Sensor",
                    HeaderText = "Sensor",
                    Width = 100
                });

                fonte.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = "Valor",
                    HeaderText = "Valor",
                    Width = 80
                });

                fonte.Columns.Add(new DataGridViewTextBoxColumn()
                {
                    DataPropertyName = "Medidas",
                    HeaderText = "Medidas",
                    Width = 60
                });
            }

            // Atualizar a fonte de dados
          fonte.DataSource = null;
          fonte.DataSource = data;
        
        }


        public static void TiposdeTesteBombas()
        {
            //// Defina os tipos de teste disponíveis
            //var tiposDeTeste = new Dictionary<string, string>();
            //{
            //    ["A"] = "conjunto de teste 1",
            //    ["B"] = "conjunto de teste 2 ",
            //    ["C"] = "conjunto de teste 3 "
                
            //};
            //// Exiba os tipos de teste disponíveis
            //foreach (var tipo in tiposDeTeste)
            //{
            //    Console.WriteLine(tipo);
            //}   
        }
    }
}
