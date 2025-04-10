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
    public class Pilotagem_bomba : PilotingBase
    {
        [Key]
        
        public int Id { get; set; } // Chave primária
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public int ValveId { get; set; }



        public Pilotagem_bomba(double pressure, double voltage)
            : base(pressure, voltage) { }

        public override double CalculateEnergyEfficiency()
        {
            return (Psi / Voltage) * 100; // exemplo simplificado
        }

        public void UpdateFromArduino(double pressure, double voltage)
        {
            Psi = pressure;
            Voltage = voltage;
        }
    }
}
