using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Sessao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now; // Usado como "Inicio do Teste"

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; } // Usado para calcular "Fim do Teste" e "Duração"

        // Novas propriedades para os dados dos textboxes da classe Tela_Bombas
        [Column(TypeName = "TEXT")]
        public string NomeClienteTextBox { get; set; } // Equivalente a TbNomeCliente

        [Column(TypeName = "TEXT")]
        public string NomeBombaTextBox { get; set; } // Equivalente a TbNomeBomba

        [Column(TypeName = "TEXT")]
        public string OrdemServicoTextBox { get; set; } // Equivalente a TbOrdemServico

        public double? DuracaoMinutos { get; set; } // Tempo de execução em minutos

        // As propriedades ModuloID, Name, ClienteID, EmpresaID, UsuarioID e as virtuais já existem
        public int? ModuloID { get; set; }
        public string Name { get; set; } // Pode ser usado para o nome da sessão ou tipo de teste
        public int? ClienteID { get; set; }
        public int? EmpresaID { get; set; }
        public int? UsuarioID { get; set; }

        public virtual Modulo Modulo { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual Empresa Empresa { get; set; }
        public virtual Usuario Usuario { get; set; }

        // Adicionar uma coleção de Etapas para relacionar a sessão às suas etapas
        public virtual ICollection<Etapa> Etapas { get; set; }
    }
}