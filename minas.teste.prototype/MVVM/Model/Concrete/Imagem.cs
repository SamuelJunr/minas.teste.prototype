using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Imagem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        [Column(TypeName = "BLOB")]
        public byte[] Dado { get; set; } // Armazena a imagem em formato binário

        [Column(TypeName = "TEXT")]
        public string Notas { get; set; }

        public int? EmpresaID { get; set; }

        public int? ClienteID { get; set; }

        public int? RelatorioID { get; set; }

        
        public virtual Empresa Empresa { get; set; }

       
        public virtual Cliente Cliente { get; set; }

     
        public virtual Relatorio Relatorio { get; set; }
    }
}
