using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
           string psiPLValor, string psiPLMin, string psiPLMax, bool psiPLAtivo, Panel psiPLPanel,
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
            VerificarRange("psi_PL", psiPLValor, psiPLMin, psiPLMax, psiPLAtivo, psiPLPanel);
            VerificarRange("Press_PSI", pressPSIValor, pressPSIMin, pressPSIMax, pressPSIAtivo, pressPSIPanel);
            VerificarRange("gpm_DR", gpmDRValor, gpmDRMin, gpmDRMax, gpmDRAtivo, gpmDRPanel);
            VerificarRange("Vazao_GPM", vazaoGPMValor, vazaoGPMMin, vazaoGPMMax, vazaoGPMAtivo, vazaoGPMPanel);
            VerificarRange("Press_BAR", pressBARValor, pressBARMin, pressBARMax, pressBARAtivo, pressBARPanel);
            VerificarRange("bar_PL", barPLValor, barPLMin, barPLMax, barPLAtivo, barPLPanel);
            VerificarRange("rotacao_RPM", rotacaoRPMValor, rotacaoRPMMin, rotacaoRPMMax, rotacaoRPMAtivo, rotacaoRPMPanel);
            VerificarRange("Vazao_LPM", vazaoLPMValor, vazaoLPMMin, vazaoLPMMax, vazaoLPMAtivo, vazaoLPMPanel);
            VerificarRange("lpm_DR", lpmDRValor, lpmDRMin, lpmDRMax, lpmDRAtivo, lpmDRPanel);
            VerificarRange("Temp_C", tempCValor, tempCMin, tempCMax, tempCAtivo, tempCPanel);
        }

        private void VerificarRange(string sensorNome, string sensorValorTexto, string minValorTexto, string maxValorTexto, bool ativo, Panel panelAlerta)
        {
            if (ativo)
            {
                if (decimal.TryParse(sensorValorTexto, out decimal valorSensor) &&
                    decimal.TryParse(minValorTexto, out decimal valorMinimo) &&
                    decimal.TryParse(maxValorTexto, out decimal valorMaximo))
                {
                    if (valorSensor < valorMinimo || valorSensor > valorMaximo)
                    {
                        MessageBox.Show($"O valor do sensor {sensorNome} ({valorSensor}) está fora do range [{valorMinimo} - {valorMaximo}].", "Alerta de Monitoramento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        panelAlerta.BackColor = Color.Yellow; // Ou outra cor de destaque
                    }
                    else
                    {
                        panelAlerta.BackColor = SystemColors.Control; // Volta à cor padrão
                    }
                }
                else if (!string.IsNullOrEmpty(sensorValorTexto) || !string.IsNullOrEmpty(minValorTexto) || !string.IsNullOrEmpty(maxValorTexto))
                {
                    MessageBox.Show($"Erro ao converter os valores para o sensor {sensorNome}. Verifique se os campos de valor, mínimo e máximo contêm números válidos.", "Erro de Conversão", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    panelAlerta.BackColor = Color.LightCoral; // Indica um erro de conversão
                }
                else
                {
                    panelAlerta.BackColor = SystemColors.Control; // Se os campos estiverem vazios e o checkbox ativo, não há alerta
                }
            }
            else
            {
                panelAlerta.BackColor = SystemColors.Control; // Se o checkbox não estiver ativo, o painel volta à cor padrão
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
            _sessaoBomba.FinalizarSessao();
            mensagemConfirmacao();


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
                fonte.AutoGenerateColumns = false;

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
                    HeaderText = "Unidade",
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
