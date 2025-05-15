using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Conexao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        [Required]
        [Column(TypeName = "TEXT")]
        public string PortaName { get; set; }

        public int? BaudRate { get; set; }

        [Column(TypeName = "TEXT")]
        public string Arduino { get; set; }

        public int? SessaoID { get; set; }

        public virtual Sessao Sessao { get; set; }
    }
}
