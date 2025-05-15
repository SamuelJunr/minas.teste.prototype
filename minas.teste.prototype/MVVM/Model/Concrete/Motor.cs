using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Motor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        public double? DrenoLPM { get; set; }

        public double? PressaoPSI { get; set; }

        public double? PressaoBAR { get; set; }

        public double? VazaoGPM { get; set; }

        public double? VazaoLPM { get; set; }

        public double? RotacaoRPM { get; set; }

        public double? TemperaturaCelsius { get; set; }

        public int? SessaoID { get; set; }

        public int? EtapaID { get; set; }

       
        public virtual Sessao Sessao { get; set; }

        
        public virtual Etapa Etapa { get; set; }
    }
}
