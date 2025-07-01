using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace minas.teste.prototype.MVVM.View
{
    public partial class FormAdicionarEvidencia : Form
    {
        // Propriedades públicas para acessar os dados preenchidos no modal
        public string NomeEvidencia { get; private set; }
        public string NomeResponsavel { get; private set; }
        public int EtapaSelecionada { get; private set; }
        public string CaminhoImagemSalva { get; private set; }

        // Controles do Formulário
        private TextBox txtNomeEvidencia;
        private TextBox txtNomeResponsavel;
        private ComboBox cmbEtapas;
        private Button btnCarregar;
        private Button btnCancelar;

        public FormAdicionarEvidencia(int numeroMaximoEtapas)
        {
            InitializeComponentint();
            PopularEtapas(numeroMaximoEtapas);
        }

        private void InitializeComponentint()
        {
            // --- Configuração do Formulário ---
            this.Text = "Adicionar Evidência";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(350, 220);
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // --- Controles ---
            var lblNomeEvidencia = new Label { Text = "Nome da Evidência:", Location = new Point(15, 20), Size = new Size(120, 20) };
            txtNomeEvidencia = new TextBox { Location = new Point(140, 20), Size = new Size(190, 20) };

            var lblNomeResponsavel = new Label { Text = "Responsável:", Location = new Point(15, 60), Size = new Size(120, 20) };
            txtNomeResponsavel = new TextBox { Location = new Point(140, 60), Size = new Size(190, 20) };

            var lblEtapa = new Label { Text = "Associar à Etapa:", Location = new Point(15, 100), Size = new Size(120, 20) };
            cmbEtapas = new ComboBox { Location = new Point(140, 100), Size = new Size(190, 20), DropDownStyle = ComboBoxStyle.DropDownList };

            btnCarregar = new Button { Text = "Carregar Imagem", Location = new Point(140, 160), Size = new Size(110, 30) };
            btnCancelar = new Button { Text = "Cancelar", Location = new Point(260, 160), Size = new Size(70, 30) };

            // --- Eventos ---
            btnCarregar.Click += BtnCarregar_Click;
            btnCancelar.Click += (sender, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            // --- Adicionar Controles ao Form ---
            this.Controls.Add(lblNomeEvidencia);
            this.Controls.Add(txtNomeEvidencia);
            this.Controls.Add(lblNomeResponsavel);
            this.Controls.Add(txtNomeResponsavel);
            this.Controls.Add(lblEtapa);
            this.Controls.Add(cmbEtapas);
            this.Controls.Add(btnCarregar);
            this.Controls.Add(btnCancelar);
        }

        private void PopularEtapas(int numeroMaximoEtapas)
        {
            cmbEtapas.Items.Clear();
            if (numeroMaximoEtapas > 0)
            {
                for (int i = 1; i <= numeroMaximoEtapas; i++)
                {
                    cmbEtapas.Items.Add(i);
                }
                cmbEtapas.SelectedIndex = 0;
            }
            else
            {
                // Se não houver etapas, desabilita o carregamento
                cmbEtapas.Items.Add("N/A");
                cmbEtapas.SelectedIndex = 0;
                btnCarregar.Enabled = false;
            }
        }

        private void BtnCarregar_Click(object sender, EventArgs e)
        {
            // Validação dos campos
            if (string.IsNullOrWhiteSpace(txtNomeEvidencia.Text) || string.IsNullOrWhiteSpace(txtNomeResponsavel.Text) || cmbEtapas.SelectedItem == null)
            {
                MessageBox.Show("Por favor, preencha todos os campos antes de carregar a imagem.", "Campos Obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Selecione uma imagem de evidência";
                ofd.Filter = "Imagens (*.jpg; *.png)|*.jpg;*.png";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var fileInfo = new FileInfo(ofd.FileName);

                    // Valida o tamanho do arquivo (limite de 10MB)
                    if (fileInfo.Length > 10 * 1024 * 1024)
                    {
                        MessageBox.Show("O arquivo selecionado excede o limite de 10 MB.", "Arquivo Muito Grande", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    try
                    {
                        // Define o caminho da pasta de recursos e a cria se não existir
                        string pastaRecursos = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "recurso");
                        Directory.CreateDirectory(pastaRecursos);

                        // Cria um nome de arquivo único para evitar sobreposições
                        string novoNomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(ofd.FileName);
                        string caminhoDestino = Path.Combine(pastaRecursos, novoNomeArquivo);

                        // Copia o arquivo para a pasta de destino
                        File.Copy(ofd.FileName, caminhoDestino);

                        // Armazena os dados nas propriedades públicas
                        this.NomeEvidencia = txtNomeEvidencia.Text;
                        this.NomeResponsavel = txtNomeResponsavel.Text;
                        this.EtapaSelecionada = (int)cmbEtapas.SelectedItem;
                        this.CaminhoImagemSalva = caminhoDestino;

                        // Define o resultado como OK e fecha o modal
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ocorreu um erro ao salvar a imagem: {ex.Message}", "Erro de Arquivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}