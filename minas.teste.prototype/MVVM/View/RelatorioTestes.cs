using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        // Lista para armazenar os dados simulados dos relatórios
        private List<MockReportData> mockReports;

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
        public RelatorioTestes()
        {
            InitializeComponent();
            InitializeMockReportsDataGridView(); // Initialize DataGridView for mock reports
            this.Salvarbtn.Click += new System.EventHandler(this.button3_Click); // Adiciona o evento de clique para o botão Salvarbtn (button3)
        }

        private void RelatorioTestes_Load(object sender, EventArgs e)
        {
            // Preenche os valores dos labels no cabeçalho
            this.lblClienteValue.Text = _cliente;
            this.lblOSValue.Text = _ordemServico;
            this.lblModuloValue.Text = _modulo;
            this.label1.Text = DateTime.Now.ToShortDateString(); // Set current date
        }
        private string CarregarTemplateHTML()
        {
            // **SUBSTITUA ESTE CONTEÚDO PELO CONTEÚDO DO SEU arquivo vm.html**
            // E ADICIONE OS PLACEHOLDERS PARA OS NOVOS CAMPOS.
            string htmlTemplate = @"
            <!DOCTYPE html>
            <html>
            <head>
                <title>Relatório de Testes</title>
                <style>
                    /* Estilos CSS básicos */
                    body { font-family: Arial, sans-serif; margin: 20px; }
                    .container { max-width: 900px; margin: 0 auto; border: 1px solid #ccc; padding: 20px; }
                    h1, h2 { text-align: center; color: #333; }
                    .info-section { margin-bottom: 20px; border: 1px solid #eee; padding: 10px; background-color: #f9f9f9; }
                    .info-section p { margin: 5px 0; }
                    .info-section strong { display: inline-block; width: 150px; }
                    table { width: 100%; border-collapse: collapse; margin-top: 20px; }
                    th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                    th { background-color: #f2f2f2; }
                    .evidence-container { margin-top: 20px; }
                    .evidence-image { max-width: 300px; height: auto; display: block; margin: 5px 0; }
                </style>
            </head>
            <body>
                <div class='container'>
                    <h1>Relatório de Testes</h1>
                    
                    <div class='info-section'>
                        <h2>Dados do Relatório</h2>
                        <p><strong>Data:</strong> {{Data}}</p>
                        <p><strong>Ordem de Serviço:</strong> {{OrdemServico}}</p>
                        <p><strong>Cliente:</strong> {{Cliente}}</p>
                        <p><strong>CNPJ:</strong> {{CNPJ}}</p>
                        <p><strong>Código do Cliente:</strong> {{CodigoCliente}}</p>
                        <p><strong>Email:</strong> {{Email}}</p>
                        <p><strong>Contato:</strong> {{Contato}}</p>
                        <p><strong>Telefone:</strong> {{Telefone}}</p>
                        <p><strong>Rua:</strong> {{Rua}}</p>
                        <p><strong>Bairro:</strong> {{Bairro}}</p>
                        <p><strong>CEP:</strong> {{CEP}}</p>
                        <p><strong>Cidade:</strong> {{Cidade}}</p>
                        <p><strong>Número de Série:</strong> {{NumeroSerie}}</p>
                        <p><strong>Nota Fiscal:</strong> {{NotaFiscal}}</p>
                        <p><strong>Descrição:</strong> {{Descricao}}</p>
                        <p><strong>Técnico Responsável:</strong> {{TecnicoResponsavel}}</p>
                        <p><strong>Módulo:</strong> {{Modulo}}</p>
                    </div>

                    <div class='info-section'>
                        <h2>Conclusão do Laudo</h2>
                        <p>{{ConclusaoLaudo}}</p>
                    </div>

                    <div class='info-section'>
                        <h2>Especificação do Teste</h2>
                        <p>{{EspecificacaoTeste}}</p>
                    </div>

                    <h2>Evidências Anexadas</h2>
                    <div class='evidence-container'>
                        {{TabelaEvidencias}}
                    </div>

                    </div>
            </body>
            </html>";
            return htmlTemplate;
        }

        private string PreencherTemplateHTML(MockReportData report, List<DataGridViewRow> evidencias)
        {
            string html = CarregarTemplateHTML();

            // Substituição dos campos do MockReportData
            html = html.Replace("{{Data}}", report.Data.ToShortDateString());
            html = html.Replace("{{OrdemServico}}", report.OrdemServico);
            html = html.Replace("{{Cliente}}", report.Cliente);
            html = html.Replace("{{CNPJ}}", report.CNPJ);
            html = html.Replace("{{CodigoCliente}}", report.CodigoCliente);
            html = html.Replace("{{Email}}", report.Email);
            html = html.Replace("{{Contato}}", report.Contato);
            html = html.Replace("{{Telefone}}", report.Telefone);
            html = html.Replace("{{Rua}}", report.Rua);
            html = html.Replace("{{Bairro}}", report.Bairro);
            html = html.Replace("{{CEP}}", report.CEP);
            html = html.Replace("{{Cidade}}", report.Cidade);
            html = html.Replace("{{NumeroSerie}}", report.NumeroSerie);
            html = html.Replace("{{NotaFiscal}}", report.NotaFiscal);
            html = html.Replace("{{Descricao}}", report.Descricao);
            html = html.Replace("{{TecnicoResponsavel}}", report.TecnicoResponsavel);
            html = html.Replace("{{Modulo}}", report.Modulo);
            html = html.Replace("{{ConclusaoLaudo}}", report.ConclusaoLaudo);
            html = html.Replace("{{EspecificacaoTeste}}", report.EspecificacaoTeste);

            // Preenchimento da tabela de evidências
            string tabelaEvidenciasHTML = "<table><tr><th>Nome</th><th>Etapa</th><th><th>Responsável</th><th>Data/Hora</th><th>Evidência</th></tr>";
            foreach (DataGridViewRow row in evidencias)
            {
                // Certifique-se de que as colunas existem e os valores não são nulos antes de tentar acessá-los
                string caminhoImagem = row.Cells["CaminhoImagemSalva"]?.Value?.ToString();
                string nomeEvidencia = row.Cells["NomeEvidencia"]?.Value?.ToString();
                string etapa = row.Cells["EtapaSelecionada"]?.Value?.ToString();
                string responsavel = row.Cells["NomeResponsavel"]?.Value?.ToString();
                string dataHora = row.Cells["DataHora"]?.Value?.ToString();

                tabelaEvidenciasHTML += $"<tr><td>{nomeEvidencia}</td><td>{etapa}</td><td>{responsavel}</td><td>{dataHora}</td>";
                if (!string.IsNullOrEmpty(caminhoImagem) && File.Exists(caminhoImagem))
                {
                    try
                    {
                        // Converte a imagem para Base64 para incorporar diretamente no HTML
                        byte[] imageBytes = File.ReadAllBytes(caminhoImagem);
                        string base64Image = Convert.ToBase64String(imageBytes);
                        // Assumimos que as imagens são PNG para o tipo MIME, ajuste se necessário
                        tabelaEvidenciasHTML += $"<td><img class='evidence-image' src='data:image/png;base64,{base64Image}'/></td></tr>";
                    }
                    catch (Exception ex)
                    {
                        tabelaEvidenciasHTML += $"<td>(Erro ao carregar imagem: {ex.Message})</td></tr>";
                    }
                }
                else
                {
                    tabelaEvidenciasHTML += "<td>(Imagem não encontrada ou caminho inválido)</td></tr>";
                }
            }
            tabelaEvidenciasHTML += "</table>";
            html = html.Replace("{{TabelaEvidencias}}", tabelaEvidenciasHTML);

            return html;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvRelatorios.SelectedRows.Count > 0)
            {
                // Correção aqui: Acessa a primeira linha selecionada diretamente
                MockReportData selectedReport = dgvRelatorios.SelectedRows[0].DataBoundItem as MockReportData;

                if (selectedReport != null)
                {
                    // Obtenha as linhas de evidência do dataGridView1
                    List<DataGridViewRow> evidencias = new List<DataGridViewRow>();
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            evidencias.Add(row);
                        }
                    }

                    // Preencher o template HTML
                    string htmlContent = PreencherTemplateHTML(selectedReport, evidencias);

                    // Abrir o modal de visualização
                    FormVisualizarRelatorio visualizarForm = new FormVisualizarRelatorio(htmlContent);
                    visualizarForm.ShowDialog(this);
                }
                else
                {
                    MessageBox.Show("O item selecionado não é um relatório válido.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Selecione um relatório na lista para gerar o PDF.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
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

        // New Class to hold mock report data
        private class MockReportData
        {
            public DateTime Data { get; set; }
            public string OrdemServico { get; set; }
            public string Cliente { get; set; }
            public string CNPJ { get; set; }
            public string CodigoCliente { get; set; }
            public string Email { get; set; }
            public string Contato { get; set; }
            public string Telefone { get; set; }
            public string Rua { get; set; }
            public string Bairro { get; set; }
            public string CEP { get; set; }
            public string Cidade { get; set; }
            public string NumeroSerie { get; set; }
            public string NotaFiscal { get; set; }
            public string Descricao { get; set; }
            public string TecnicoResponsavel { get; set; }
            public string Modulo { get; set; }
            public string ConclusaoLaudo { get; set; }
            public string EspecificacaoTeste { get; set; }
        }

        // Method to generate mock report data
        private List<MockReportData> LoadMockReportData()
        {
            return new List<MockReportData>
            {
                new MockReportData
                {
                    Data = new DateTime(2025, 6, 10),
                    OrdemServico = "OS-2025-001",
                    Cliente = "Empresa Alpha",
                    CNPJ = "11.222.333/0001-44",
                    CodigoCliente = "CLI-001",
                    Email = "contato@alpha.com",
                    Contato = "João Silva",
                    Telefone = "(31) 98765-4321",
                    Rua = "Rua A, 123",
                    Bairro = "Centro",
                    CEP = "30110-000",
                    Cidade = "Belo Horizonte",
                    NumeroSerie = "SN12345",
                    NotaFiscal = "NF-001-2025",
                    Descricao = "Manutenção preventiva do equipamento X.",
                    TecnicoResponsavel = "Maria Oliveira",
                    Modulo = "Módulo 1",
                    ConclusaoLaudo = "Equipamento em perfeito funcionamento após manutenção. Todas as verificações realizadas com sucesso.",
                    EspecificacaoTeste = "Teste de desempenho e segurança."
                },
                new MockReportData
                {
                    Data = new DateTime(2025, 7, 5),
                    OrdemServico = "OS-2025-002",
                    Cliente = "Indústria Beta",
                    CNPJ = "55.666.777/0001-88",
                    CodigoCliente = "CLI-002",
                    Email = "sac@beta.com",
                    Contato = "Ana Souza",
                    Telefone = "(11) 2345-6789",
                    Rua = "Av. Principal, 456",
                    Bairro = "Industrial",
                    CEP = "01000-000",
                    Cidade = "São Paulo",
                    NumeroSerie = "SN67890",
                    NotaFiscal = "NF-002-2025",
                    Descricao = "Reparo no sistema de controle de qualidade.",
                    TecnicoResponsavel = "Carlos Mendes",
                    Modulo = "Módulo 2",
                    ConclusaoLaudo = "Problema no sensor corrigido. Sistema operando dentro das especificações.",
                    EspecificacaoTeste = "Teste de calibração e funcionalidade."
                },
                 new MockReportData
                {
                    Data = new DateTime(2025, 7, 15),
                    OrdemServico = "OS-2025-003",
                    Cliente = "Comércio Gama",
                    CNPJ = "99.888.777/0001-11",
                    CodigoCliente = "CLI-003",
                    Email = "vendas@gama.com",
                    Contato = "Pedro Lima",
                    Telefone = "(21) 3333-1111",
                    Rua = "Travessa Curta, 789",
                    Bairro = "Comercial",
                    CEP = "20000-000",
                    Cidade = "Rio de Janeiro",
                    NumeroSerie = "SN11223",
                    NotaFiscal = "NF-003-2025",
                    Descricao = "Instalação de novo software de gestão.",
                    TecnicoResponsavel = "Fernanda Costa",
                    Modulo = "Módulo 3",
                    ConclusaoLaudo = "Software instalado e configurado com sucesso. Treinamento da equipe realizado.",
                    EspecificacaoTeste = "Teste de integração e usabilidade."
                }
            };
        }

        // Initialize DataGridView for mock reports
        private void InitializeMockReportsDataGridView()
        {
            dgvRelatorios.AutoGenerateColumns = false;
            dgvRelatorios.Columns.Add("colData", "Data");
            dgvRelatorios.Columns.Add("colOrdemServico", "Ordem de Serviço");
            dgvRelatorios.Columns["colData"].DataPropertyName = "Data";
            dgvRelatorios.Columns["colOrdemServico"].DataPropertyName = "OrdemServico";
            dgvRelatorios.Columns["colData"].DefaultCellStyle.Format = "dd/MM/yyyy";
            dgvRelatorios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRelatorios.ReadOnly = true;
        }


        // Event handler for the "Carregar Relatórios Simulados" button
        private void btnLoadMockData_Click(object sender, EventArgs e)
        {
            mockReports = LoadMockReportData();
            dgvRelatorios.DataSource = mockReports;
        }

        // Event handler for DataGridView cell click
        private void dgvRelatorios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) // Ensure a valid row is clicked
            {
                MockReportData selectedReport = dgvRelatorios.Rows[e.RowIndex].DataBoundItem as MockReportData;
                if (selectedReport != null)
                {
                    // Populate the textboxes with selected mock data
                    lblClienteValue.Text = selectedReport.Cliente;
                    textBox2.Text = selectedReport.CNPJ;
                    textBox3.Text = selectedReport.CodigoCliente;
                    textBox4.Text = selectedReport.Email;
                    textBox7.Text = selectedReport.Contato;
                    textBox8.Text = selectedReport.Telefone;
                    textBox9.Text = selectedReport.Rua;
                    textBox10.Text = selectedReport.Bairro;
                    textBox11.Text = selectedReport.CEP;
                    textBox12.Text = selectedReport.Cidade;
                    lblOSValue.Text = selectedReport.OrdemServico;
                    textBox6.Text = selectedReport.NumeroSerie;
                    textBox13.Text = selectedReport.NotaFiscal;
                    textBox15.Text = selectedReport.Descricao;
                    textBox16.Text = selectedReport.TecnicoResponsavel;
                    lblModuloValue.Text = selectedReport.Modulo;
                    textBox17.Text = selectedReport.ConclusaoLaudo;
                    textBox18.Text = selectedReport.EspecificacaoTeste;
                    label1.Text = selectedReport.Data.ToShortDateString();
                }
            }
        }

        // Event handler for the Salvarbtn (button3) click
        private void button3_Click(object sender, EventArgs e)
        {
            // 1. Validar se os campos obrigatórios estão preenchidos
            List<string> missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(label1.Text) || !DateTime.TryParse(label1.Text, out DateTime dataReport))
            {
                missingFields.Add("Data (formato dd/MM/yyyy)");
            }
            if (string.IsNullOrWhiteSpace(lblOSValue.Text))
            {
                missingFields.Add("Ordem de Serviço");
            }
            if (string.IsNullOrWhiteSpace(lblClienteValue.Text))
            {
                missingFields.Add("Cliente");
            }
            if (string.IsNullOrWhiteSpace(textBox2.Text)) // CNPJ
            {
                missingFields.Add("CNPJ");
            }
            if (string.IsNullOrWhiteSpace(textBox15.Text)) // Descricao
            {
                missingFields.Add("Descrição");
            }
            if (string.IsNullOrWhiteSpace(textBox16.Text)) // TecnicoResponsavel
            {
                missingFields.Add("Técnico Responsável");
            }
            if (string.IsNullOrWhiteSpace(textBox17.Text)) // ConclusaoLaudo
            {
                missingFields.Add("Conclusão do Laudo");
            }
            if (string.IsNullOrWhiteSpace(textBox18.Text)) // EspecificacaoTeste
            {
                missingFields.Add("Especificação do Teste");
            }

            // Se houver campos faltando, exiba a mensagem e não continue
            if (missingFields.Any())
            {
                string message = "Os seguintes campos obrigatórios não foram preenchidos:\n\n" +
                                 string.Join("\n", missingFields) +
                                 "\n\nPor favor, preencha-os antes de salvar.";
                MessageBox.Show(message, "Dados Faltando", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Não permite salvar se os campos estiverem vazios
            }

            // Se todos os campos obrigatórios estiverem preenchidos, proceda com o salvamento
            // 2. Criar um novo objeto MockReportData com os dados atuais dos TextBoxes
            MockReportData newReport = new MockReportData
            {
                Data = DateTime.Parse(label1.Text), // Converte o texto da data para DateTime
                OrdemServico = lblOSValue.Text,
                Cliente = lblClienteValue.Text,
                CNPJ = textBox2.Text,
                CodigoCliente = textBox3.Text,
                Email = textBox4.Text,
                Contato = textBox7.Text,
                Telefone = textBox8.Text,
                Rua = textBox9.Text,
                Bairro = textBox10.Text,
                CEP = textBox11.Text,
                Cidade = textBox12.Text,
                NumeroSerie = textBox6.Text,
                NotaFiscal = textBox13.Text,
                Descricao = textBox15.Text,
                TecnicoResponsavel = textBox16.Text,
                Modulo = lblModuloValue.Text,
                ConclusaoLaudo = textBox17.Text,
                EspecificacaoTeste = textBox18.Text
            };

            // 3. Inicializa a lista mockReports se for nula
            if (mockReports == null)
            {
                mockReports = new List<MockReportData>();
            }

            // 4. Adicionar o novo relatório à lista de relatórios simulados
            mockReports.Add(newReport);

            // 5. Atualizar o DataSource do DataGridView para exibir o novo registro
            dgvRelatorios.DataSource = null; // Limpa o DataSource para forçar a atualização
            dgvRelatorios.DataSource = mockReports;

            // 6. Exibir mensagem de sucesso
            MessageBox.Show("Novo relatório salvo com sucesso!", "Salvo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}