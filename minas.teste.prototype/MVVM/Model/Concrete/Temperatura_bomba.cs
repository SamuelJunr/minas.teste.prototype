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
    public class Temperatura_bomba : TemperatureBase
    {
        public Temperatura_bomba(double temp) : base(temp) { }

        [Key]
        
        public int Id { get; set; } // Chave primária
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public override double CalculateThermalExpansion(double initialLength, double coefficient)
        {
            return initialLength * coefficient * Celsius;
        }

        public void UpdateTemperature(double arduinoValue)
        {
            Celsius = arduinoValue; // Assume que Arduino envia em Celsius
        }

        public override string CheckForExtremeConditions()
        {
            if (Celsius > 100) return "Superaquecimento Crítico!";
            if (Celsius < 5) return "Risco de Congelamento";
            return "Temperatura OK";
        }
    }
}
