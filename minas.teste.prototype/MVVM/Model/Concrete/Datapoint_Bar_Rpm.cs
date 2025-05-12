using System;

namespace minas.teste.prototype.MVVM.ViewModel // Ou o namespace onde você colocou suas classes de Model/ViewModel
{
    /// <summary>
    /// Representa um ponto de dado com Rotação (rpm) e Pressão (bar),
    /// conforme usado pela classe de simulação.
    /// </summary>
    public class Datapoint_Bar_Rpm : EventArgs
    {
        public double Rotation { get; } // Rotação em rpm
        public double Pressure { get; } // Pressão em bar

        public Datapoint_Bar_Rpm(double rotation, double pressure)
        {
            Rotation = rotation;
            Pressure = pressure;
        }
    }
}