using System;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.ViewModel;

namespace minas.teste.prototype.MVVM.View
{
    public partial class Conexao : Form
    {
        private conexao_arduino_VM _arduino;
        private int _contadorCliques = 0;

        public Conexao()
        {
            InitializeComponent();
        }

        private void Conexao_Load(object sender, EventArgs e)
        {
            _arduino = new conexao_arduino_VM(textBox1); // txtDados = TextBox da UI

            // Tenta detectar e conectar automaticamente
            string portaDetectada = _arduino.DetectarPortaArduino();
            if (_arduino.Conectar(portaDetectada)) // Modificado para receber porta
            {
                lblStatus.Text = "PORTA ABERTA";
                cuiTextBox22.Content = "RECEBENDO DADOS";
                cuiTextBox21.Content = portaDetectada; // Atualiza a textbox com a porta
                pictureBox1.Image = Properties.Resources.on;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            else
            {
                lblStatus.Text = "PORTA FECHADA";
                cuiTextBox21.Content = "Nenhuma porta detectada";
                pictureBox1.Image = Properties.Resources.off;
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        private void txtPorta_TextChanged(object sender, EventArgs e)
        {
            // Se necessário, implemente lógica de alteração de porta
        }

        

        
        private void cuiTextBox21_ContentChanged(object sender, EventArgs e)
        {

        }

        private void bitButton1_Click(object sender, EventArgs e)
        {
            bool receber;

            if (_contadorCliques == 0)
            {
                // Primeiro clique - ativa recebimento
                receber = true;
                _arduino.ToggleRecebimento(receber);
                _contadorCliques++;
            }
            else
            {
                // Cliques subsequentes - desativa recebimento
                receber = false;
                _arduino.ToggleRecebimento(receber);
                cuiTextBox22.Content = "RECEBIMENTO PAUSADO";
                _contadorCliques = 0; // Zera o contador
            }

            
        }

        private void Conexao_FormClosing(object sender, FormClosingEventArgs e)
        {
            Menuapp menuForm = new Menuapp();
            menuForm.Show();
            this.Close();
            _arduino.Desconectar();
        }
    }
}