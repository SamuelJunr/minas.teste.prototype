using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minas.teste.prototype.MVVM.View
{
    public partial class Tutorial: Form
    {
        private List<Image> backgroundImages = new List<Image>();
        private int currentIndex = 0;
        private Timer timer = new Timer();
        private apresentacao _fechar_box;
        private bool _fechamentoForcado;

        public Tutorial()
        {
            InitializeComponent();

            // Configura o Timer
            timer.Interval = 10000;
            timer.Tick += Timer_Tick;
            timer.Enabled = true;

            // Carrega as imagens de fundo do Resources.resx
            LoadBackgroundImages();

            // Exibe a primeira imagem com opacidade
            if (backgroundImages.Count > 0)
            {
                backgroundImagePictureBox.Image = backgroundImages[currentIndex];
                SetImageOpacity(backgroundImagePictureBox, 0.9f);
            }

            // Define o formulário como transparente para mostrar o PictureBox como fundo
            this.BackColor = Color.Black; // Escolha uma cor que não esteja nas suas imagens
            this.TransparencyKey = Color.Black; // Define a cor de fundo como transparente

        }

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
        #region Carrossel de Imagens
        private void LoadBackgroundImages()
        {
            try
            {
                // Converte os recursos de byte[] para Image antes de adicioná-los à lista
                if (Properties.Resources.background1 != null)
                {
                    using (var ms = new MemoryStream(Properties.Resources.background1))
                    {
                        backgroundImages.Add(Image.FromStream(ms));
                    }
                }
                if (Properties.Resources.background2 != null)
                {
                    using (var ms = new MemoryStream(Properties.Resources.background2))
                    {
                        backgroundImages.Add(Image.FromStream(ms));
                    }
                }
                
                if (Properties.Resources.background4 != null)
                {
                    using (var ms = new MemoryStream(Properties.Resources.background4))
                    {
                        backgroundImages.Add(Image.FromStream(ms));
                    }
                }
                if (Properties.Resources.background5 != null)
                {
                    using (var ms = new MemoryStream(Properties.Resources.background5))
                    {
                        backgroundImages.Add(Image.FromStream(ms));
                    }
                }

                // Adicione mais imagens conforme necessário
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar um recurso de imagem: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            currentIndex++;
            if (currentIndex >= backgroundImages.Count)
            {
                currentIndex = 0;
            }
            if (backgroundImages.Count > 0)
            {
                backgroundImagePictureBox.Image = backgroundImages[currentIndex];
                SetImageOpacity(backgroundImagePictureBox, 0.9f);
            }
        }

        private void SetImageOpacity(PictureBox pictureBox, float opacity)
        {
            if (pictureBox.Image == null) return;

            Bitmap originalImage = new Bitmap(pictureBox.Image);
            Bitmap transparentImage = new Bitmap(originalImage.Width, originalImage.Height);

            using (Graphics g = Graphics.FromImage(transparentImage))
            {
                ColorMatrix matrix = new ColorMatrix();
                matrix.Matrix33 = opacity;

                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                g.DrawImage(originalImage, new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                            0, 0, originalImage.Width, originalImage.Height, GraphicsUnit.Pixel, attributes);
            }

            pictureBox.Image = transparentImage;
        }
        #endregion


    }
}
