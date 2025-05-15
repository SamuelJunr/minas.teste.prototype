using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        [Required]
        public string Name { get; set; }

        
        public int? EmpresaID { get; set; }

        public bool IsAdmin { get; set; } = false;

        // Propriedade de navegação para a tabela Empresa (opcional, mas recomendável para relacionamentos)
        public virtual Empresa Empresa { get; set; }
    }
}
