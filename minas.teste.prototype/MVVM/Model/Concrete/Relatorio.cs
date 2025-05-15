using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Relatorio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        [Column(TypeName = "TEXT")]
        public string Contato { get; set; }

        [Column(TypeName = "TEXT")]
        public string NumeroSerie { get; set; }

        [Column(TypeName = "TEXT")]
        public string Descricao { get; set; }

        [Column(TypeName = "TEXT")]
        public string Conclusao { get; set; }

        public int? SessaoID { get; set; }

        public int? EtapaID { get; set; }

        public int? EmpresaID { get; set; }

        public int? ClienteID { get; set; }

        public int? ImagemID { get; set; }

        public int? ModuloID { get; set; }

      
        public virtual Sessao Sessao { get; set; }

     
        public virtual Etapa Etapa { get; set; }

        
        public virtual Empresa Empresa { get; set; }

        
        public virtual Cliente Cliente { get; set; }

        
        public virtual Imagem Imagem { get; set; }

        
        public virtual Modulo Modulo { get; set; }
    }
}
