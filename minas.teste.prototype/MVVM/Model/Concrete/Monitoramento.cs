using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Monitoramento
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        public double? PSImin { get; set; }

        public double? BARmin { get; set; }

        public double? GPMmin { get; set; }

        public double? LPMmin { get; set; }

        public double? RPMmin { get; set; }

        public double? CELSUSmin { get; set; }

        public double? PSImax { get; set; }

        public double? BARmax { get; set; }

        public double? GPMmax { get; set; }

        public double? LPMmax { get; set; }

        public double? RPMmax { get; set; }

        public double? CELSUSmax { get; set; }

        public int? ModuloID { get; set; }

        
        public virtual Modulo Modulo { get; set; }
    }
}
