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
    public class Rotacao_bomba : RotationBase
    {
        public string MotorId { get; set; }

        [Key]
        
        public int Id { get; set; } // Chave primária
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public Rotacao_bomba(double rotation, bool isRps = false) : base(rotation, isRps) { }

        public override double CalculatePower(double torque)
        {
            return torque * AngularVelocity; // Potência em Watts
        }

        public void UpdateRotation(double arduinoValue, bool isRps = false)
        {
            if (isRps)
                RPS = arduinoValue;
            else
                RPM = arduinoValue;
        }
    }
}
