using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.Model.Abstract
{
    public abstract class PressureBase
    {
        private const double psiPerBar = 14.503773773; // Fator de conversão exato
        private double pressureInPsi; // Armazena o valor internamente em PSI

        public double Psi
        {
            get => pressureInPsi;
            set => pressureInPsi = value;
        }

        public double Bar
        {
            get => pressureInPsi / psiPerBar;
            set => pressureInPsi = value * psiPerBar;
        }

        // Métodos de conversão estáticos para uso geral
        public static double ConvertPsiToBar(double psi) => psi / psiPerBar;
        public static double ConvertBarToPsi(double bar) => bar * psiPerBar;
    }
}
