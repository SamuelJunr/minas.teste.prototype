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
    public class Pressao_bomba : PressureBase
    {
        public string Location { get; set; }

        [Key]
        
        public int Id { get; set; } // Chave primária
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public Pressao_bomba(double pressure, bool isBar = false)
        {
            if (isBar)
                Bar = pressure;
            else
                Psi = pressure;
        }

        public void UpdatePressure(double arduinoValue, bool isBar = false)
        {
            if (isBar)
                Bar = arduinoValue;
            else
                Psi = arduinoValue;
        }

        public bool IsCriticalPressure() => Psi > 3000;
    }
}
