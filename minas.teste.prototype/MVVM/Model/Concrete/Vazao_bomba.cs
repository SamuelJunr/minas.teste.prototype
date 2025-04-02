using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using minas.teste.prototype.MVVM.Model.Abstract;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    
        public class Vazao_bomba : FlowBase
        {
            public string PumpModel { get; set; }

            [Key]
            
            public int Id { get; set; } // Chave primária
            public DateTime Timestamp { get; set; } = DateTime.Now;

            public Vazao_bomba(double flowRate, bool isGpm = false)
            {
                if (isGpm)
                  Gpm = flowRate;
                else
                  Lpm = flowRate;
            }

            public void UpdateFromArduino(double rawValue, bool isGpm = false)
            {
                if (isGpm)
                   Gpm = rawValue;
                else
                   Lpm = rawValue;
            }

            public string GetFlowStatus()
            {
                 if (Lpm > 500)
                  return "Alto Fluxo";
                 else if (Lpm < 100)
                   return "Baixo Fluxo";
                 else
                   return "Operação Normal";
            }

        }
    
}
