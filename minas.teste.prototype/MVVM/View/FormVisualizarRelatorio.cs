using System;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using iTextSharp.tool.xml.css;
using iTextSharp.tool.xml.html;

namespace minas.teste.prototype.MVVM.View
{
    public partial class FormVisualizarRelatorio : Form
    {
        private string _htmlContent;

        public FormVisualizarRelatorio(string htmlContent)
        {
            InitializeComponent();

            _htmlContent = htmlContent;

            if (this.webBrowser1 != null)
            {
                webBrowser1.DocumentText = _htmlContent;
            }
            else
            {
                MessageBox.Show("O controle WebBrowser não foi encontrado ou inicializado. Verifique o FormVisualizarRelatorio.Designer.cs", "Erro de Componente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
        }

        private void btnGerarPdf_Click(object sender, EventArgs e)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.FileName = "Relatorio.pdf";
                saveFileDialog.DefaultExt = ".pdf";
                saveFileDialog.Filter = "Arquivo PDF (*.pdf)|*.pdf";

                // Define o diretório inicial para a pasta "Meus Documentos"
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var document = new Document())
                        {
                            using (var writer = PdfWriter.GetInstance(document, new FileStream(saveFileDialog.FileName, FileMode.Create)))
                            {
                                document.Open();

                                using (var sr = new StringReader(_htmlContent))
                                {
                                    XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, sr);
                                }

                                document.Close();
                            }
                        }
                        MessageBox.Show("PDF gerado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao gerar PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}