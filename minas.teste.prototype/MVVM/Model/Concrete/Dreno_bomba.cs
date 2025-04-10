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
    public class Dreno_bomba : DrainBase
    {
        [Key]
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public Dreno_bomba(double flowRate, bool isInGpm) : base(flowRate)
        {
            DrainType = "Dreno Principal";
            Diameter = 200;
            Material = "Aço Inox";
            if (isInGpm)
            {
                Gpm = flowRate;
            }
            else
            {
                Lpm = flowRate;
            }
        }

        // Removed unused parameter 'v' to resolve IDE0060
        public Dreno_bomba() : base(0)
        {
        }

        public override double CalculateFlowVelocity()
        {
            double area = Math.PI * Math.Pow(Diameter / 2000, 2); // Área em m²
            return (Lpm / 60000) / area; // Velocidade em m/s
        }

        public void UpdateFlow(double arduinoValue)
        {
            Lpm = arduinoValue;
        }
    }
}
