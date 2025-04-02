using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.Model.Abstract
{
    public abstract class PilotingBase : PressureBase
    {
        private double voltage; // Tensão em Volts

        // Herda automaticamente Psi e Bar de PressureBase
        // e adiciona propriedade de tensão elétrica

        public double Voltage
        {
            get => voltage;
            set => voltage = value >= 0 ? value :
                throw new ArgumentException("Tensão não pode ser negativa");
        }

        protected PilotingBase(double initialPressure, double initialVoltage, bool pressureInBar = false)
        {
            // Inicializa pressão conforme unidade especificada
            if (pressureInBar)
                Bar = initialPressure;
            else
                Psi = initialPressure;

            Voltage = initialVoltage;
        }

        // Método para cálculo de potência elétrica
        public double CalculateElectricPower(double current)
        {
            return Voltage * current; // P = V * I
        }

        // Método abstrato para eficiência energética
        public abstract double CalculateEnergyEfficiency();

        // Relação pressão-tensão (exemplo simplificado)
        public virtual double PressureVoltageRatio()
        {
            return Psi / Voltage; // Razão Psi por Volt
        }

        // Conversão para tensão normalizada (0-10V)
        public double NormalizedVoltage(double maxSystemVoltage)
        {
            return (Voltage / maxSystemVoltage) * 10;
        }
    }
}
