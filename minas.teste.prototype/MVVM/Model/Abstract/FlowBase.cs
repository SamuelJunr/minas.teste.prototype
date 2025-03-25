using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.Model.Abstract
{
    public abstract class FlowBase
    {
        private const double gpmPerLpm = 0.264172052; // 1 LPM = ~0.264172 GPM (baseado no galão americano)
        private double flowRateInLpm; // Armazena o valor internamente em LPM

        public double Lpm
        {
            get => flowRateInLpm;
            set => flowRateInLpm = value;
        }

        public double Gpm
        {
            get => flowRateInLpm * gpmPerLpm;
            set => flowRateInLpm = value / gpmPerLpm;
        }

        // Métodos de conversão estáticos
        public static double ConvertLpmToGpm(double lpm) => lpm * gpmPerLpm;
        public static double ConvertGpmToLpm(double gpm) => gpm / gpmPerLpm;
    }
}
