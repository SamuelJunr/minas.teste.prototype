using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// Adicione esta linha se EtapaData e SensorData estiverem neste namespace:
using minas.teste.prototype.MVVM.Model.Concrete;

namespace minas.teste.prototype.MVVM.View
{
    public partial class Realatoriobase : Form
    {
        // Campos privados para armazenar os dados recebidos
        private string _inicioTesteRecebido;
        private string _fimTesteRecebido;
        private string _clienteRecebido;
        private string _nomeDoTesteRecebido;
        private string _ordemServicoRecebida;
        private List<List<string>> _dadosGridPrincipalRecebidos;
        private List<EtapaData> _dadosEtapasDetalhadosRecebidos;
        private List<Image> _imagensGraficosRecebidas;

        public Realatoriobase()
        {
            InitializeComponent();
            // Evento Load para popular os controles após o formulário ser carregado
            this.Load += new System.EventHandler(this.Realatoriobase_Load);
        }

        /// <summary>
        /// Método público para receber os dados do ensaio da Tela_Bombas.
        /// </summary>
        public void SetDadosEnsaio(
            string inicioTeste,
            string fimTeste,
            string cliente,
            string nomeDoTeste,
            string ordemServico,
            List<List<string>> dadosGridPrincipal,
            List<EtapaData> dadosEtapasDetalhados, // Vem de _dadosColetados em Tela_Bombas
            List<Image> imagensGraficos)
        {
            // Armazena os dados recebidos nos campos privados da classe
            _inicioTesteRecebido = inicioTeste;
            _fimTesteRecebido = fimTeste;
            _clienteRecebido = cliente;
            _nomeDoTesteRecebido = nomeDoTeste;
            _ordemServicoRecebida = ordemServico;
            _dadosGridPrincipalRecebidos = dadosGridPrincipal;
            _dadosEtapasDetalhadosRecebidos = dadosEtapasDetalhados;
            _imagensGraficosRecebidas = imagensGraficos;

            // Se o formulário já estiver carregado, atualiza os controles.
            // Caso contrário, o evento Load cuidará disso.
            if (this.IsHandleCreated && !this.IsDisposed)
            {
                // Garante que a atualização da UI ocorra na thread da UI
                this.BeginInvoke((MethodInvoker)PopularControlesRelatorio);
            }
        }

        private void Realatoriobase_Load(object sender, EventArgs e)
        {
            // Popula os controles com os dados quando o formulário é carregado
            PopularControlesRelatorio();
        }

        /// <summary>
        /// Preenche os controles da UI do formulário de relatório com os dados armazenados.
        /// </summary>
        private void PopularControlesRelatorio()
        {
            if (this.IsDisposed) return;

            // --- Preencher Informações do Cabeçalho ---
            // Dados do Cliente (cuiGroupBox1)
            if (textBox1 != null) textBox1.Text = _clienteRecebido ?? "N/A"; // Cliente

            // Informações Gerais (cuiGroupBox2)
            if (textBox5 != null) textBox5.Text = _nomeDoTesteRecebido ?? "N/A"; // Módulo (Nome do Teste)
            if (textBox14 != null) textBox14.Text = _ordemServicoRecebida ?? "N/A"; // Ordem de Serviço

            // Data (groupBox1) - Exibindo data de início do teste e hora atual do relatório
            if (label1 != null) //
            {
                label1.Text = !string.IsNullOrEmpty(_inicioTesteRecebido) ?
                                 $"Início: {_inicioTesteRecebido}\nFim: {_fimTesteRecebido}\nGerado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}" :
                                 DateTime.Now.ToString("dd/MM/yyyy");
                label1.TextAlign = ContentAlignment.TopLeft; // Ajustar alinhamento se necessário
                label1.AutoSize = true; // Ajustar tamanho
            }

            // Título do Formulário
            this.Text = $"Relatório: {_nomeDoTesteRecebido} (OS: {_ordemServicoRecebida})";

            // --- Preencher DataGridView Principal (dataGridView1 - Dados Coletados) ---
            if (dataGridView1 != null && _dadosGridPrincipalRecebidos != null && _dadosGridPrincipalRecebidos.Count > 0)
            {
                dataGridView1.Rows.Clear();
                dataGridView1.Columns.Clear();

                List<string> headers = _dadosGridPrincipalRecebidos[0];
                foreach (string header in headers)
                {
                    string colName = header.Replace(" ", "_").Replace("(", "").Replace(")", "").Replace(".", "");
                    if (string.IsNullOrWhiteSpace(colName)) colName = $"Coluna{dataGridView1.Columns.Count}";
                    int suffix = 0;
                    string originalColName = colName;
                    while (dataGridView1.Columns.Contains(colName)) // Garante nomes de coluna únicos
                    {
                        suffix++;
                        colName = $"{originalColName}_{suffix}";
                    }
                    dataGridView1.Columns.Add(colName, header);
                }

                for (int i = 1; i < _dadosGridPrincipalRecebidos.Count; i++)
                {
                    if (_dadosGridPrincipalRecebidos[i] != null)
                    {
                        dataGridView1.Rows.Add(_dadosGridPrincipalRecebidos[i].Take(dataGridView1.ColumnCount).ToArray());
                    }
                }
                dataGridView1.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }

            // --- Preencher DataGridView de Detalhes das Etapas (dataGridView2) ---
            // Este grid está dentro de panel9, que tem um label "Relatórios Anteriores" acima.
            // Vamos assumir que dataGridView2 é para os detalhes do ensaio atual.
            if (dataGridView2 != null && _dadosEtapasDetalhadosRecebidos != null)
            {
                dataGridView2.Rows.Clear();
                dataGridView2.Columns.Clear();
                dataGridView2.Columns.Add("EtapaCol", "Etapa");
                dataGridView2.Columns.Add("SensorCol", "Sensor/Parâmetro");
                dataGridView2.Columns.Add("ValorCol", "Valor");
                dataGridView2.Columns.Add("UnidadeCol", "Unidade");

                foreach (var etapaData in _dadosEtapasDetalhadosRecebidos)
                {
                    if (etapaData.leituras != null)
                    {
                        foreach (var leitura in etapaData.leituras)
                        {
                            dataGridView2.Rows.Add(etapaData.Etapa, leitura.Sensor, leitura.Valor, leitura.Medidas);
                        }
                        // Adicionar uma linha em branco ou separador visual entre etapas se desejar
                        // dataGridView2.Rows.Add("", "---", "---", "---");
                    }
                }
                dataGridView2.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }

            // --- Preencher Imagens dos Gráficos ---
            if (_imagensGraficosRecebidas != null)
            {
                if (_imagensGraficosRecebidas.Count > 0 && pictureBox1 != null && _imagensGraficosRecebidas[0] != null) //
                {
                    pictureBox1.Image = _imagensGraficosRecebidas[0];
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom; // Para ajustar a imagem
                }
                if (_imagensGraficosRecebidas.Count > 1 && pictureBox2 != null && _imagensGraficosRecebidas[1] != null) //
                {
                    pictureBox2.Image = _imagensGraficosRecebidas[1];
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom; // Para ajustar a imagem
                }
                // Se houver um terceiro gráfico (_imagensGraficosRecebidas[2]),
                // você precisaria de um terceiro PictureBox no seu Realatoriobase.Designer.cs
            }

            // Os TextBoxes restantes (textBox2, textBox3, textBox4, textBox6, textBox7, etc.)
            // não têm dados diretos passados via SetDadosEnsaio pelos parâmetros atuais.
            // Eles podem ser para entrada manual no relatório ou preenchidos de outra fonte.
            // O mesmo se aplica para textBox17 (Conclusão) e comboBox1 (Status).

            this.Refresh(); // Garante que o formulário seja redesenhado
        }
    }
}