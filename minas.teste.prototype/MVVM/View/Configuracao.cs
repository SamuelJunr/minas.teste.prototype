using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.Estilo;
using minas.teste.prototype.MVVM.Model.Concrete;
using minas.teste.prototype.Properties;
using minas.teste.prototype.Service;

namespace minas.teste.prototype.MVVM.View
{
    public partial class configuracao : Form
    {
        private apresentacao fechar_box;
        private bool _fechamentoForcado = false;

        public configuracao()
        {
            InitializeComponent();
            EstiloFormulario.AplicarEstiloconfiguracao(this);
            CarregarConfiguracoes();
        }

        private void configuracao_Load(object sender, EventArgs e)
        {
            Text = Properties.Resources.ResourceManager.GetString("ConfigureTitle");
            comboBoxSensorTipo.Items.AddRange(new object[] { "Temperatura", "Pressão", "Vazão", "Pilotagem", "Dreno", "Rotação" });
            textBoxPortaCOM.Text = ConnectionSettingsApplication.PortName;


        }
        private void CarregarConfiguracoes()
        {
            // Exemplo: Carrega o valor salvo no ComboBox apropriado
            // Certifique-se de que os ComboBoxes existam no seu formulário!
            // Se o valor salvo não existir mais na lista, ele não será selecionado.

            // Exemplo para comboBoxSensorTipo (assumindo que você salvou o texto do item)
            if (comboBoxSensorTipo != null && !string.IsNullOrEmpty(Settings.Default.TipoSensorSelecionado))
            {
                comboBoxSensorTipo.SelectedItem = Settings.Default.TipoSensorSelecionado;
                // Se o item não for encontrado, SelectedItem permanecerá null ou o índice -1
                if (comboBoxSensorTipo.SelectedIndex == -1)
                {
                    // Opcional: Lidar com o caso de um valor salvo inválido
                    // Poderia selecionar um padrão ou deixar sem seleção.
                    // Settings.Default.TipoSensorSelecionado = ""; // Limpa a configuração inválida
                    // Settings.Default.Save();
                }
            }
            if (textBoxPortaCOM != null && !string.IsNullOrEmpty(Settings.Default.PortaCOMSelecionada))
            {
                textBoxPortaCOM.Text = Settings.Default.PortaCOMSelecionada;
                
            }
        }
        private void SalvarConfiguracoes()
        {
            // Exemplo: Salva o item selecionado (ou o texto dele)
            // Certifique-se de que os ComboBoxes existam no seu formulário!
            if (comboBoxSensorTipo != null)
            {
                // Salva o texto do item selecionado. Se nada for selecionado, salva string vazia.
                Settings.Default.TipoSensorSelecionado = comboBoxSensorTipo.SelectedItem?.ToString() ?? string.Empty;
            }

            if (textBoxPortaCOM != null)
            {
                Settings.Default.PortaCOMSelecionada = textBoxPortaCOM.Text?.ToString() ?? string.Empty;
            }

            // Adicione aqui a lógica para salvar outras configurações

            // Salva as alterações no arquivo de configuração do usuário
            Settings.Default.Save();
        }
        private void LimparConfiguracoes()
        {
            // Exemplo: Reseta os ComboBoxes para o estado inicial (sem seleção)
            // Certifique-se de que os ComboBoxes existam no seu formulário!
            if (comboBoxSensorTipo != null)
            {
                comboBoxSensorTipo.SelectedIndex = -1; // Remove a seleção
                Settings.Default.TipoSensorSelecionado = string.Empty; // Limpa a configuração salva
            }
            if (textBoxPortaCOM != null)
            {
                
                Settings.Default.PortaCOMSelecionada = string.Empty;
            }

            // Adicione aqui a lógica para limpar outros controles e configurações

            // Salva as alterações (limpas) no arquivo de configuração
            Settings.Default.Save();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            SalvarConfiguracoes();
            MessageBox.Show("Configuração salva com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);


        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Tem certeza que deseja limpar todas as configurações?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                LimparConfiguracoes();
                MessageBox.Show("Configurações foram apagadas. Defina novos parâmetros.", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Stop); // Ícone talvez devesse ser Information
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _fechamentoForcado = true; // Indica que é um fechamento controlado
            Menuapp.Instance.Show();
            this.Close();

        }

        private void Configuracao_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Só encerra a aplicação se não for um fechamento controlado
            if (!_fechamentoForcado)
            {
                fechar_box.apresentacao_FormClosing(sender, e);
            }
            else

              Menuapp.Instance.Show();
        }

        
    }
}
