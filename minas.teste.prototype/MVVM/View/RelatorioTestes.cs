using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minas.teste.prototype.MVVM.View
{
    public partial class RelatorioTestes : Form
    {
        // Campos privados para armazenar os dados recebidos do teste
        private readonly string _cliente;
        private readonly string _nomeBomba;
        private readonly string _ordemServico;
        private readonly string _modulo;
        private readonly int _numeroEtapas;
        private readonly string _tempoTotalTeste;

        // --- NOVOS CONTROLES ADICIONADOS ---
        

        public RelatorioTestes()
        {
            // Este construtor é usado pelo designer.
            // A chamada para InitializeComponent() será feita aqui.
            // Para esta implementação programática, chamaremos nosso método de inicialização.
            InitializeCustomComponents();
        }

        public RelatorioTestes(string cliente, string nomeBomba, string ordemServico, string modulo, int numeroEtapas, string tempoTotalTeste) : this()
        {
            _cliente = cliente;
            _nomeBomba = nomeBomba;
            _ordemServico = ordemServico;
            _modulo = modulo;
            _numeroEtapas = numeroEtapas;
            _tempoTotalTeste = tempoTotalTeste;

            this.Load += new System.EventHandler(this.RelatorioTestes_Load);
        }

        // Método que substitui o InitializeComponent do designer
        private void InitializeCustomComponents()
        {
            // Configuração básica do formulário
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Text = "Relatório de Teste";
            this.StartPosition = FormStartPosition.CenterScreen;

            // Painel para organizar as informações do cabeçalho
            Panel headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 80;
            this.Controls.Add(headerPanel);

            // Labels para os títulos
            Label lblCliente = new Label() { Text = "Cliente:", Location = new Point(10, 10), Font = new Font(this.Font, FontStyle.Bold) };
            Label lblOS = new Label() { Text = "Ordem de Serviço:", Location = new Point(10, 30), Font = new Font(this.Font, FontStyle.Bold) };
            Label lblMod = new Label() { Text = "Módulo:", Location = new Point(10, 50), Font = new Font(this.Font, FontStyle.Bold) };

            

            headerPanel.Controls.Add(lblCliente);
            headerPanel.Controls.Add(lblOS);
            headerPanel.Controls.Add(lblMod);
            headerPanel.Controls.Add(lblClienteValue);
            headerPanel.Controls.Add(lblOSValue);
            headerPanel.Controls.Add(lblModuloValue);

            
            // DataGridView para exibir as evidências
            dataGridView1 = new DataGridView();
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = true;
            this.Controls.Add(dataGridView1);
            dataGridView1.BringToFront(); // Garante que o Grid fique visível

            // Configuração das colunas do DataGridView
            ConfigurarDataGridView();
        }

        private void ConfigurarDataGridView()
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("NomeEvidencia", "Nome da Evidência");
            dataGridView1.Columns.Add("Etapa", "Etapa Associada");
            dataGridView1.Columns.Add("Responsavel", "Responsável");
            dataGridView1.Columns.Add("DataHora", "Data do Carregamento");
            dataGridView1.Columns.Add("CaminhoArquivo", "Caminho do Arquivo");

            // Oculta a coluna com o caminho do arquivo
            dataGridView1.Columns["CaminhoArquivo"].Visible = false;

            // Ajusta o tamanho das colunas
            dataGridView1.Columns["NomeEvidencia"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns["Responsavel"].Width = 150;
            dataGridView1.Columns["Etapa"].Width = 80;
            dataGridView1.Columns["DataHora"].Width = 150;
        }


        private void RelatorioTestes_Load(object sender, EventArgs e)
        {
            // Preenche os valores dos labels no cabeçalho
            this.lblClienteValue.Text = _cliente;
            this.lblOSValue.Text = _ordemServico;
            this.lblModuloValue.Text = _modulo;
        }

        private void BtnCarregarEvidencia_Click(object sender, EventArgs e)
        {
            // 1. Verifica o limite de 7 imagens
            if (dataGridView1.Rows.Count >= 7)
            {
                MessageBox.Show("O limite de 7 evidências por relatório foi atingido.", "Limite Excedido", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 2. Abre o formulário modal para adicionar a evidência
            using (var modal = new FormAdicionarEvidencia(_numeroEtapas))
            {
                // 3. Se o usuário confirmar e fechar o modal com OK
                if (modal.ShowDialog(this) == DialogResult.OK)
                {
                    // 4. Adiciona os dados retornados pelo modal ao DataGridView
                        dataGridView1.Rows.Add(
                        modal.NomeEvidencia,
                        modal.EtapaSelecionada,
                        modal.NomeResponsavel,
                        DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        modal.CaminhoImagemSalva
                    );
                }
            }
        }
    }
}