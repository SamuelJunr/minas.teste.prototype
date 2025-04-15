using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;

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
    }
}
