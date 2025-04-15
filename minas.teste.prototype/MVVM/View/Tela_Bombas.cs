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
using minas.teste.prototype.MVVM.Model.Abstract;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.Service;
using minas.teste.prototype.MVVM.ViewModel;

namespace minas.teste.prototype.MVVM.View
{
    public partial class Tela_Bombas : Form
    {
        //-------------------------------------//
        //          objetos GLOBAIS          //
        //-------------------------------------//
        private Tela_BombasVM _viewModel;
        private apresentacao _fechar_box;
        private List<SensorData> _dadosSensores = new List<SensorData>();
        private List<EtapaData> _dadosColetados = new List<EtapaData>(); 
        public Dictionary<string, TextBox> sensorMap;
        public Dictionary<string, string> sensorMapmedida;
        




        //-------------------------------------//
        //          variaveis GLOBAIS          //
        //-------------------------------------//

        private bool _fechamentoForcado;
        private bool _isMonitoring = false;
        public string Inicioteste;
        public string Fimteste;
        private int etapaAtual = 0;
        private const int LIMITE_ETAPAS = 7;
        private List<string> dadosSensoresSelecionados = new List<string>();
        public List<SensorData> dadosSensores;



        // Move the initialization of the static dictionary to the constructor or a method,  
        // as static fields cannot reference instance members directly.  

        public Tela_Bombas()
        {
            InitializeComponent();
            Inciaizador_listas();
            _viewModel = new Tela_BombasVM();
            _fechar_box = new apresentacao();
            dadosSensores = new List<SensorData> { new SensorData() };



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

            dadosSensoresSelecionados.AddRange(sensorMap.Keys);

        }


        #region LOADS_JANELA    
        private void Tela_Bombas_Load(object sender, EventArgs e)
        {
            _viewModel.Carregar_configuracao(this); // Carrega o estilo do formulário  
            _viewModel.Stage_signal(Stage_box_bomba);
            _viewModel.VincularRelogioLabel(LabelHorariotela);// configura a imagem de teste ligado ou desligado  

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
        private void btnIniciar_Click(object sender, EventArgs e)
        {
            _isMonitoring = true;
            _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5,panel7, panel6, panel8, panel9);
            Inicioteste = DateTime.Now.ToString();
            _viewModel.IniciarTesteBomba(Stage_box_bomba);

        }

        private void btnParar_Click(object sender, EventArgs e)
        {
            _isMonitoring = false;
            _viewModel.AlterarEstadoPaineis(_isMonitoring, panel4, panel5, panel7, panel6, panel8, panel9);
            Fimteste = DateTime.Now.ToString();
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
            if (etapaAtual <= LIMITE_ETAPAS)
            {
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
        #endregion
    }
}
