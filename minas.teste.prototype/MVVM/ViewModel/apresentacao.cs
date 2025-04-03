using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.Service;

namespace minas.teste.prototype
{
    internal class apresentacao
    {
        public void apresentacao_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                DialogResult result = MessageBox.Show(
                    "Tem certeza que deseja fechar a aplicação?",
                    "Confirmar Fechamento",
                    MessageBoxButtons.OKCancel, // Botões alterados para OK/Cancel
                    MessageBoxIcon.Question
                );

                // Se clicar em "OK" ou no "X" da MessageBox, fecha a aplicação
                if (result == DialogResult.OK)
                {
                    ApplicationSession.Instance.SaveSession(); // Corrigido para usar a instância
                    Application.Exit();
                }
                // Se clicar em "Cancelar", mantém o formulário aberto
                else
                {
                    e.Cancel = true;
                }
            }
        }

        public static void ExibirMensagemBoasVindas()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Bem-vindo(a) ao Supervisório SK4!");
            sb.AppendLine(); // Linha em branco
                             //sb.AppendLine($"Olá, {nomeUsuario}!");
            sb.AppendLine($"Você está utilizando a versão: {Properties.Resources.ResourceManager.GetString("versaoAplicacao")}");
            sb.AppendLine(); // Linha em branco
            sb.AppendLine("Esperamos que você tenha uma ótima experiência.");
            sb.AppendLine("Se precisar de ajuda, consulte a documentação ou entre em contato através do nosso site.");
            string mensagem = sb.ToString();
            string titulo = "Boas-Vindas";
            MessageBox.Show(mensagem, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
