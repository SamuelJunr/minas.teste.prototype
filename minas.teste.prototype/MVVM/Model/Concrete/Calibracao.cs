using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Calibracao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        [Required]
        [Column(TypeName = "TEXT")]
        public string Sensor { get; set; }

        public DateTime? DataCalibracao { get; set; }

        public double? ValorOffset { get; set; }

        public double? ValorGanho { get; set; }

        public double? LeituraMin { get; set; }

        public double? LeituraMax { get; set; }

        public double? ValorRefMin { get; set; }

        public double? ValorRefMax { get; set; }

        [Column(TypeName = "TEXT")]
        public string Unidade { get; set; }

        [Column(TypeName = "TEXT")]
        public string Notas { get; set; }

        public int? UsuarioID { get; set; }

        public int? ModuloID { get; set; }

        
        public virtual Usuario Usuario { get; set; }

        
        public virtual Modulo Modulo { get; set; }
    }
}
