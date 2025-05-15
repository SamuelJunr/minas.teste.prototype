using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class SensorConfiguracao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        [Required]
        [Column(TypeName = "TEXT")]
        public string Nome { get; set; }

        public int? CalibracaoID { get; set; }

        public int? SensorDataID { get; set; }

       
        public virtual Calibracao Calibracao { get; set; }

       
        public virtual SensorDataDB SensorData { get; set; }
    }
}
