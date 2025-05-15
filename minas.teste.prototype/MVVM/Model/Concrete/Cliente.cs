using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Cliente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        [Required]
        public string Name { get; set; }

        [Column(TypeName = "TEXT")]
        public string CNPJ { get; set; }

        [Column(TypeName = "TEXT")]
        public string CPF { get; set; }

        public string Endereco { get; set; }

        public string Email { get; set; }

        public string Telefone { get; set; }

        
        public int? EmpresaID { get; set; }

        public virtual Empresa Empresa { get; set; }
    }
}
