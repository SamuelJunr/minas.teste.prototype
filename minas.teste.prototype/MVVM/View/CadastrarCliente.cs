using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using minas.teste.prototype.MVVM.Model.Concrete;

namespace minas.teste.prototype.MVVM.View
{
    public partial class CadastrarCliente : Form
    {
        
        List<Cliente> cliente;
        string siglaEstado;

        // Construtor
        public CadastrarCliente()
        {
            InitializeComponent();
            cliente = new List<Cliente>(); // inicializa a lista 

            // Add opções
            ComboKey.Items.Add("CPF");
            ComboKey.Items.Add("CNPJ"); 

            ComboKey.SelectedIndex = 1; // Deixa o elemento 1 do combo selecionado por default

            boxestados.Items.Add("AC - Acre");
            boxestados.Items.Add("AL - Alagoas");
            boxestados.Items.Add("AP - Amapá");
            boxestados.Items.Add("AM - Amazonas");
            boxestados.Items.Add("BA - Bahia");
            boxestados.Items.Add("CE - Ceará");
            boxestados.Items.Add("DF - Distrito Federal");
            boxestados.Items.Add("ES - Espírito Santo");
            boxestados.Items.Add("GO - Goiás");
            boxestados.Items.Add("MA - Maranhão");
            boxestados.Items.Add("MT - Mato Grosso");
            boxestados.Items.Add("MS - Mato Grosso do Sul");
            boxestados.Items.Add("MG - Minas Gerais");
            boxestados.Items.Add("PA - Pará");
            boxestados.Items.Add("PB - Paraíba");
            boxestados.Items.Add("PR - Paraná");
            boxestados.Items.Add("PE - Pernambuco");
            boxestados.Items.Add("PI - Piauí");
            boxestados.Items.Add("RJ - Rio de Janeiro");
            boxestados.Items.Add("RN - Rio Grande do Norte");
            boxestados.Items.Add("RS - Rio Grande do Sul");
            boxestados.Items.Add("RO - Rondônia");
            boxestados.Items.Add("RR - Roraima");
            boxestados.Items.Add("SC - Santa Catarina");
            boxestados.Items.Add("SP - São Paulo");
            boxestados.Items.Add("SE - Sergipe");
            boxestados.Items.Add("TO - Tocantins");

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private bool ValidarEmail()
        {
            // Verifica se o campo está vazio ou contém apenas espaços em branco
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("O campo de e-mail não pode estar vazio!", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return false;
            }

            // Verifica se o texto contém o caractere '@'
            if (!txtEmail.Text.Contains("@"))
            {
                MessageBox.Show("O e-mail deve conter o caractere '@'!", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtEmail.Focus();
                return false;
            }

            // Validação adicional opcional (formato básico)
            if (!txtEmail.Text.Contains("."))
            {
                MessageBox.Show("O e-mail deve conter um domínio válido (ex: .com, .net)!",
                               "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmail.Focus();
                return false;
            }

            return true;
        }
        private void btnCadastrar_Click(object sender, EventArgs e)
        {
            int index = -1; // Utilizado para verificar se a pessoa já está cadastrada (-1 = não cadastrada)

            foreach (Cliente clientelist in cliente)
            {
                // verifica se a pessoa já está cadastrada (Compara os nomes da lista com o valor passado no textBox)
                if(clientelist.Name== txtNome.Text)
                {
                    index = cliente.IndexOf(clientelist);
                    // MessageBox.Show("Usuário já cadastrado!");
                }
            }

            // Verifica se os campos obrigatórios estão preenchidos
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("Preencha o logradouro!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox1.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBox3.Text))
            {
                MessageBox.Show("Preencha a cidade!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox3.Focus();
                return;
            }

            // Verifica se os campos foram preenchidos
            if (txtNome.Text == "")
            {
                MessageBox.Show("Preencha o campo nome."); // Exibe caixa com mensagem
                txtNome.Focus(); // Foca o cursor no campo nome
                return; // Sai do método e finaliza execução do código (não permite avançar sem o preenchimento)
            }
            
            if(txtTelefone.Text == "(  )      -")
            {
                MessageBox.Show("Preencha o campo telefone."); // Exibe caixa com mensagem
                txtTelefone.Focus(); // Foca o cursor no campo telefone
                return; // Sai do método e finaliza execução do código (não permite avançar sem o preenchimento)
            }


            if(identificacaobox.Text != "")
            {
                switch (ComboKey.SelectedItem.ToString())
                {
                    case "CPF":
                        if (identificacaobox.Text.Length != 11)
                        {
                            MessageBox.Show("O CPF deve conter 11 dígitos.");
                            identificacaobox.Focus();
                            return;
                        }
                        break;
                    case "CNPJ":
                        if (identificacaobox.Text.Length != 14)
                        {
                            MessageBox.Show("O CNPJ deve conter 14 dígitos.");
                            identificacaobox.Focus();
                            return;
                        }
                        break;
                    default:
                        MessageBox.Show("Selecione um tipo de documento válido.");
                        ComboKey.Focus();
                        return;
                }
            }
            else
            {
                MessageBox.Show("Preencha o campo telefone."); // Exibe caixa com mensagem
                identificacaobox.Focus(); // Foca o cursor no campo telefone
                return; // Sai do método e finaliza execução do código (não permite avançar sem o preenchimento)
            }

            if (!ValidarEmail())
            {
                return; // Impede a continuação se a validação falhar
            }

            if (boxestados.SelectedItem == null || string.IsNullOrEmpty(boxestados.SelectedItem.ToString()))
            {
                MessageBox.Show("Selecione um estado!", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                boxestados.Focus();
            }
            else
            {
                try
                {
                    string estadoSelecionado = boxestados.SelectedItem.ToString();

                    // Divide o texto usando o hífen como separador
                    string[] partes = estadoSelecionado.Split('-');

                    if (partes.Length < 1)
                    {
                        MessageBox.Show("Formato do estado inválido!", "Erro",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Pega os 2 primeiros caracteres e remove espaços
                    string sigla = partes[0].Trim();

                    // Valida se a sigla tem 2 caracteres
                    if (sigla.Length != 2)
                    {
                        MessageBox.Show("Sigla do estado inválida!", "Erro",
                                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Usa a sigla (em maiúsculas para garantir)
                    siglaEstado = sigla.ToUpper();
                    MessageBox.Show($"Sigla selecionada: {siglaEstado}", "Sucesso",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao processar estado: {ex.Message}", "Erro",
                                   MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


            // Dados preenchidos -> cria objeto do tipo pessoa com os dados e insere na lista
            Cliente ospeople = new Cliente();
            ospeople.Name = txtNome.Text;
            ospeople.Empresa = "";
            ospeople.CNPJ = ComboKey.Items.IndexOf(1);
            ospeople.CPF = ComboKey.Items.IndexOf(0);
            ospeople.Telefone = txtTelefone.Text;
            ospeople.Endereco = $"{textBox1.Text.Trim()} - {textBox3.Text.Trim()} - {textBox4.Text.Trim()} - {siglaEstado}";
             


            // Decide se vai cadastrar uma nova pessoa ou atualizar registro
            if(index < 0)
            {
                // -1 = Nâo existe, então cadastra.
                cliente.Add(ospeople);
            }
            else
            {
                // a partir do 0, cadastro existe, então atualiza.
                cliente[index] = ospeople;
            }


            // Chama o método para limpar todos os campos
            btnLimpar_Click(btnLimpar, EventArgs.Empty); // objeto passado é o próprio btnLimpar -> sem info para evento

            // Lista todos os registros
            Listar();
        }

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            // Utilizado para pegar o índice do registro a ser excluído
            int index = lista.SelectedIndex; // selectedIndex => selecionado ao clicar no registro
            cliente.RemoveAt(index); // exclui

            Listar(); // Exibe lista atualizada
        }

        private void btnLimpar_Click(object sender, EventArgs e)
        {
            txtNome.Text = ""; 
            
            ComboKey.SelectedItem = 1; // elemento 1 selecionado por padrão
            txtTelefone.Text = "";
            boxestados.SelectedIndex = 0;
            textBox1.Text = "";
            textBox3.Text = "";
            textBox4.Text = ""; // Limpa todos os campos
            txtNome.Focus(); // Foca no campo Nome
            txtEmail.Text = ""; // Limpa o campo de email
        }

        private void Listar()
        {
            lista.Items.Clear(); // Acessa o controle e remote todos os registros

            // Percorre a lista e exibe todos os registros
            foreach (Cliente ospeople in cliente)
            {
                lista.Items.Add($"{ospeople.Name} | {ospeople.Telefone}"); // Add pelo nome
            }

        }

        private void lista_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = lista.SelectedIndex; // Pega o indice do elemento selecionado na lista (controle)
            Pessoa p = pessoas[index]; // Pega o objeto salvo

            // Passa os valores do objeto para os campos
            txtNome.Text = p.Nome;
            txtData.Text = p.DataNascimento;
            comboEC.SelectedItem = p.EstadoCivil; 
            txtTelefone.Text = p.Telefone;
            checkCasa.Checked = p.CasaPropria;
            checkVeiculo.Checked = p.Veiculo;

            // Verificação para saber o sexo selecionado
            switch (p.Sexo)
            {
                case 'M':
                    radioM.Checked = true; 
                    break;
                case 'F':
                    radioF.Checked = true;
                    break;
                default:
                   radioO.Checked = true;
                    break;
            }
            txtNome.Focus(); 
        }

        p
    }
}
