using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports; // Necessário para SerialPort.GetPortNames()
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.Estilo;
using minas.teste.prototype.MVVM.Model.Concrete; // Para ConnectionSettingsApplication
using minas.teste.prototype.Properties;
using minas.teste.prototype.Service;             // Para SerialManager (indiretamente via ConnectionSettingsApplication)
using System.Diagnostics;                         // Para Debug.WriteLine

namespace minas.teste.prototype.MVVM.View
{
    public partial class configuracao : Form
    {
        private apresentacao fechar_box; // Certifique-se que 'apresentacao' está acessível ou remova se não for usado aqui.
        private bool _fechamentoForcado = false;

        public configuracao()
        {
            InitializeComponent();
            EstiloFormulario.AplicarEstiloconfiguracao(this); // Assumindo que este método existe
            CarregarConfiguracoesSalvas();
            PopularControlesCom();
        }
        // Add this property to the Settings class definition
        public int BaudRateSelecionado { get; set; }

        private void configuracao_Load(object sender, EventArgs e)
        {
            // O título pode ser definido no designer ou aqui
            // Text = Properties.Resources.ResourceManager.GetString("ConfigureTitle"); 

            // Itens para comboBoxSensorTipo (se ainda relevante para esta tela)
            // comboBoxSensorTipo.Items.AddRange(new object[] { "Temperatura", "Pressão", "Vazão", "Pilotagem", "Dreno", "Rotação" });
        }

        private void PopularControlesCom()
        {
            // Popular ComboBox de Portas COM
            try
            {
                comboBoxPortaCOM.Items.Clear();
                string[] portasDisponiveis = SerialPort.GetPortNames();
                if (portasDisponiveis.Length > 0)
                {
                    comboBoxPortaCOM.Items.AddRange(portasDisponiveis);
                    // Tenta selecionar a porta salva anteriormente, se existir na lista
                    if (!string.IsNullOrEmpty(Settings.Default["PortaCOMSelecionada"] as string) && portasDisponiveis.Contains(Settings.Default["PortaCOMSelecionada"] as string))
                    {
                        comboBoxPortaCOM.SelectedItem = Settings.Default["PortaCOMSelecionada"] as string;
                    }
                    else if (comboBoxPortaCOM.Items.Count > 0)
                    {
                        comboBoxPortaCOM.SelectedIndex = 0; // Seleciona a primeira por padrão se nada estiver salvo ou se a salva não existir
                    }
                }
                else
                {
                    lblStatusConexao.Text = "Nenhuma porta COM encontrada.";
                    comboBoxPortaCOM.Items.Add("Nenhuma porta disponível");
                    comboBoxPortaCOM.SelectedIndex = 0;
                    comboBoxPortaCOM.Enabled = false;
                    btnTestarConexao.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao listar portas COM: {ex.Message}");
                MessageBox.Show($"Erro ao listar portas COM: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatusConexao.Text = "Erro ao listar portas COM.";
                comboBoxPortaCOM.Enabled = false;
                btnTestarConexao.Enabled = false;
            }

            // Popular ComboBox de Baud Rates
            comboBoxBaudRate.Items.Clear();
            List<int> baudRatesComuns = new List<int> { 9600, 19200, 38400, 57600, 115200, 250000 };
            foreach (int rate in baudRatesComuns)
            {
                comboBoxBaudRate.Items.Add(rate);
            }

            // Tenta selecionar o baud rate salvo ou um padrão
            int baudRateSalvo = 0;
            if (Settings.Default.Properties["BaudRateSelecionado"] != null)
            {
                object valor = Settings.Default["BaudRateSelecionado"];
                if (valor is int)
                    baudRateSalvo = (int)valor;
                else if (valor != null)
                {
                    int.TryParse(valor.ToString(), out baudRateSalvo);
                }
            }
            if (baudRateSalvo > 0 && baudRatesComuns.Contains(baudRateSalvo))
            {
                comboBoxBaudRate.SelectedItem = baudRateSalvo;
            }
            else if (comboBoxBaudRate.Items.Contains(115200))
            {
                comboBoxBaudRate.SelectedItem = 115200;
            }
            else if (comboBoxBaudRate.Items.Count > 0)
            {
                comboBoxBaudRate.SelectedIndex = 0;
            }
        }


        private void CarregarConfiguracoesSalvas()
        {
            // Carrega outras configurações como TipoSensorSelecionado, se houver
            if (comboBoxSensorTipo != null && !string.IsNullOrEmpty(Settings.Default.TipoSensorSelecionado))
            {
                // Certifique-se que comboBoxSensorTipo.Items já foi populado antes daqui
                // comboBoxSensorTipo.SelectedItem = Settings.Default.TipoSensorSelecionado;
                // ... (lógica de tratamento se o item não for encontrado)
            }

            // O carregamento da porta e baud rate já está parcialmente coberto em PopularControlesCom()
            // Aqui você pode carregar o status da conexão anterior se desejar,
            // embora geralmente seja melhor testar a conexão ativamente.
            AtualizarStatusConexaoLabel();
        }

        private void SalvarConfiguracoesPorta()
        {
            if (comboBoxPortaCOM.SelectedItem != null && comboBoxPortaCOM.SelectedItem.ToString() != "Nenhuma porta disponível")
            {
                Settings.Default.PortaCOMSelecionada = comboBoxPortaCOM.SelectedItem.ToString();
            }
            else if (comboBoxPortaCOM.Text != "Nenhuma porta disponível") // Se for um TextBox, usa Text
            {
                Settings.Default.PortaCOMSelecionada = comboBoxPortaCOM.Text;
            }
            else
            {
                Settings.Default.PortaCOMSelecionada = string.Empty;
            }


            if (comboBoxBaudRate.SelectedItem != null)
            {
                Settings.Default.BaudRateSelecionado = (int)comboBoxBaudRate.SelectedItem;
            }
            else
            {
                Settings.Default.BaudRateSelecionado = 0; // Ou um valor padrão inválido
            }

            // Salva outras configurações
            // if (comboBoxSensorTipo != null)
            // {
            //    Settings.Default.TipoSensorSelecionado = comboBoxSensorTipo.SelectedItem?.ToString() ?? string.Empty;
            // }

            Settings.Default.Save();
            Debug.WriteLine("Configurações de porta e baud rate salvas.");
        }


        // Este é o botão que substitui o antigo "Salvar Configurações" para aplicar tudo,
        // incluindo a tentativa de conexão. Se você quiser botões separados, ajuste.
        private async void btnSalvarAplicarConfiguracoes_Click(object sender, EventArgs e) // Renomeado de metroButton1_Click
        {
            string portaSelecionada = comboBoxPortaCOM.SelectedItem?.ToString();
            if (portaSelecionada == "Nenhuma porta disponível") portaSelecionada = null; // Trata caso de nenhuma porta

            if (string.IsNullOrEmpty(portaSelecionada) || comboBoxBaudRate.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione uma Porta COM e um Baud Rate válidos.", "Entrada Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblStatusConexao.Text = "Seleção de porta/baud rate inválida.";
                lblStatusConexao.ForeColor = Color.Red;
                return;
            }

            int baudRateSelecionado = (int)comboBoxBaudRate.SelectedItem;

            lblStatusConexao.Text = $"Tentando conectar a {portaSelecionada} @ {baudRateSelecionado} bps...";
            lblStatusConexao.ForeColor = Color.Orange;
            this.Cursor = Cursors.WaitCursor; // Feedback visual de processamento
            btnTestarConexao.Enabled = false; // Desabilita botão durante a tentativa

            // Tenta atualizar a conexão usando ConnectionSettingsApplication
            // Envolve a lógica em Task.Run para não bloquear a UI, especialmente se Connect demorar.
            bool sucesso = false;
            await Task.Run(() => // Executa a lógica de conexão em uma thread de segundo plano
            {
                // A classe ConnectionSettingsApplication já gerencia a instância _persistentSerialManager
                // e sua lógica de Connect/Disconnect.
                sucesso = ConnectionSettingsApplication.UpdateConnection(portaSelecionada, baudRateSelecionado);
            });

            this.Cursor = Cursors.Default;
            btnTestarConexao.Enabled = true;

            if (sucesso)
            {
                SalvarConfiguracoesPorta(); // Salva as configurações apenas se a conexão for bem-sucedida
                lblStatusConexao.Text = $"Conectado com sucesso a {ConnectionSettingsApplication.CurrentPortName} @ {ConnectionSettingsApplication.CurrentBaudRate} bps.";
                lblStatusConexao.ForeColor = Color.Green;
                MessageBox.Show("Conexão estabelecida e configurações salvas!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Não salva as configurações se a conexão falhar
                lblStatusConexao.Text = $"Falha ao conectar a {portaSelecionada}. Verifique a porta, o dispositivo ou se já está em uso.";
                lblStatusConexao.ForeColor = Color.Red;
                MessageBox.Show(lblStatusConexao.Text, "Falha na Conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void btnLimparConfigGeral_Click(object sender, EventArgs e) // Renomeado de metroButton2_Click
        {
            var result = MessageBox.Show("Tem certeza que deseja limpar todas as configurações salvas (incluindo tipo de sensor, etc.)?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                // Limpa configurações relacionadas a esta tela
                Settings.Default.PortaCOMSelecionada = string.Empty;
                Settings.Default.BaudRateSelecionado = 0;
                // Settings.Default.TipoSensorSelecionado = string.Empty; // Se relevante
                Settings.Default.Save();

                PopularControlesCom(); // Recarrega os defaults nos combo boxes
                lblStatusConexao.Text = "Configurações limpas. Conexão atual (se houver) não foi afetada.";
                lblStatusConexao.ForeColor = SystemColors.ControlText;

                // Se você quiser também desconectar a porta serial ativa ao limpar:
                // ConnectionSettingsApplication.CloseAllConnections();
                // AtualizarStatusConexaoLabel();

                MessageBox.Show("Configurações salvas foram limpas.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void AtualizarStatusConexaoLabel()
        {
            if (ConnectionSettingsApplication.IsCurrentlyConnected)
            {
                lblStatusConexao.Text = $"Conectado a {ConnectionSettingsApplication.CurrentPortName} @ {ConnectionSettingsApplication.CurrentBaudRate} bps.";
                lblStatusConexao.ForeColor = Color.Green;
            }
            else
            {
                if (!string.IsNullOrEmpty(ConnectionSettingsApplication.CurrentPortName))
                {
                    lblStatusConexao.Text = $"Desconectado. Última tentativa: {ConnectionSettingsApplication.CurrentPortName}.";
                    lblStatusConexao.ForeColor = Color.Red;
                }
                else
                {
                    lblStatusConexao.Text = "Desconectado. Nenhuma porta configurada.";
                    lblStatusConexao.ForeColor = SystemColors.ControlText;
                }
            }
        }

        private void btnVoltar_Click(object sender, EventArgs e) // Renomeado de button1_Click
        {
            _fechamentoForcado = true;
            // if (Menuapp.Instance != null) Menuapp.Instance.Show(); // Garanta que Menuapp.Instance existe e é acessível
            this.Close();
        }

        private void Configuracao_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_fechamentoForcado)
            {
                // A lógica original de fechar_box.apresentacao_FormClosing pode ser para fechar a aplicação inteira.
                // Se fechar_box for a tela principal de 'apresentacao', e o objetivo é sair da app:
                // Application.Exit(); 
                // Ou chame o método de fechamento da sua tela principal se ela gerencia isso.
                // Exemplo: if (fechar_box != null && !fechar_box.IsDisposed) fechar_box.Close(); else Application.Exit();
            }
            // else
            // {
            //    if (Menuapp.Instance != null && !Menuapp.Instance.IsDisposed) Menuapp.Instance.Show();
            // }
        }

        // Botão para apenas testar a conexão sem salvar permanentemente na hora.
        // O salvamento ocorreria com o "Salvar e Aplicar"
        private async void btnTestarConexao_Click(object sender, EventArgs e)
        {
            string portaSelecionada = comboBoxPortaCOM.SelectedItem?.ToString();
            if (portaSelecionada == "Nenhuma porta disponível") portaSelecionada = null;

            if (string.IsNullOrEmpty(portaSelecionada) || comboBoxBaudRate.SelectedItem == null)
            {
                MessageBox.Show("Por favor, selecione uma Porta COM e um Baud Rate válidos para testar.", "Entrada Inválida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            int baudRateSelecionado = (int)comboBoxBaudRate.SelectedItem;

            lblStatusConexao.Text = $"Testando conexão com {portaSelecionada} @ {baudRateSelecionado} bps...";
            lblStatusConexao.ForeColor = Color.Orange;
            this.Cursor = Cursors.WaitCursor;
            btnTestarConexao.Enabled = false; // Evitar múltiplos cliques

            bool sucessoTeste = false;
            SerialManager tempManager = null; // Manager temporário para teste

            await Task.Run(() =>
            {
                try
                {
                    tempManager = new SerialManager(); // Cria uma nova instância APENAS para o teste
                    // Adiciona um pequeno listener para ver se a conexão abre
                    bool conexaoAberta = false;
                    EventHandler<bool> statusHandler = (s, connected) => { conexaoAberta = connected; };
                    tempManager.ConnectionStatusChanged += statusHandler;

                    tempManager.Connect(portaSelecionada, baudRateSelecionado);
                    sucessoTeste = tempManager.IsConnected; // Verifica o status após a tentativa

                    // Importante: Se o teste for apenas para ver se a porta abre, desconecte imediatamente
                    // para não manter a porta ocupada por este manager temporário.
                    // A conexão "real" será feita pelo PersistentSerialManager se o usuário aplicar.
                    if (sucessoTeste)
                    {
                        Debug.WriteLine($"Teste de conexão com {portaSelecionada} bem-sucedido. Desconectando manager de teste.");
                        tempManager.Disconnect(); // Libera a porta
                    }
                    tempManager.ConnectionStatusChanged -= statusHandler;

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Erro durante teste de conexão: {ex.Message}");
                    sucessoTeste = false;
                }
                finally
                {
                    tempManager?.Dispose(); // Garante que o manager temporário seja descartado
                }
            });

            this.Cursor = Cursors.Default;
            btnTestarConexao.Enabled = true;

            if (sucessoTeste)
            {
                lblStatusConexao.Text = $"Teste com {portaSelecionada} bem-sucedido! Clique em 'Salvar & Aplicar' para usar esta conexão.";
                lblStatusConexao.ForeColor = Color.DarkGreen; // Um verde diferente para "teste OK"
            }
            else
            {
                lblStatusConexao.Text = $"Falha no teste com {portaSelecionada}. Verifique as configurações ou se a porta está em uso.";
                lblStatusConexao.ForeColor = Color.Red;
            }
        }

        // Adicione um botão de Atualizar Portas COM no seu formulário (btnAtualizarPortasCOM)
        private void btnAtualizarPortasCOM_Click(object sender, EventArgs e)
        {
            lblStatusConexao.Text = "Atualizando lista de portas COM...";
            lblStatusConexao.ForeColor = SystemColors.ControlText;
            Application.DoEvents(); // Força a atualização da UI

            string portaAnteriormenteSelecionada = comboBoxPortaCOM.SelectedItem?.ToString();
            string baudAnteriormenteSelecionado = comboBoxBaudRate.SelectedItem?.ToString();


            PopularControlesCom(); // Repopula as portas

            // Tenta restaurar a seleção anterior se ainda existir
            if (!string.IsNullOrEmpty(portaAnteriormenteSelecionada) && comboBoxPortaCOM.Items.Contains(portaAnteriormenteSelecionada))
            {
                comboBoxPortaCOM.SelectedItem = portaAnteriormenteSelecionada;
            }
            if (!string.IsNullOrEmpty(baudAnteriormenteSelecionado) && comboBoxBaudRate.Items.Contains(Convert.ToInt32(baudAnteriormenteSelecionado)))
            {
                comboBoxBaudRate.SelectedItem = Convert.ToInt32(baudAnteriormenteSelecionado);
            }

            if (comboBoxPortaCOM.Enabled)
            {
                lblStatusConexao.Text = "Lista de portas COM atualizada.";
            }
            else
            {
                lblStatusConexao.Text = "Nenhuma porta COM encontrada após atualização.";
            }
        }
    }
}