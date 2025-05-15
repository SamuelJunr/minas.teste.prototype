using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Empresa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "TEXT")] // Explicitly set the type if needed for constraints
        public string CNPJ { get; set; }

        public string Endereco { get; set; }

        public string Email { get; set; }

        public string Telefone { get; set; }

        // Propriedade de navegação para a coleção de Usuários (se houver um relacionamento um-para-muitos)
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
