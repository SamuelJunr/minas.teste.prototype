using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.Model.Abstract
{
    public abstract class DrainBase : FlowBase
    {
        // Herda automaticamente Lpm, Gpm e métodos de conversão de IFlowRate

        // Propriedades específicas de drenagem
        public string DrainType { get; protected set; }
        public double Diameter { get; set; } // Diâmetro em milímetros
        public string Material { get; set; }

        // Construtor base para inicialização
        protected DrainBase(double flowRate, bool isInGpm = false)
        {
            if (isInGpm)
                Gpm = flowRate;
            else
                Lpm = flowRate;
        }

        // Método abstrato para cálculo de velocidade do fluxo
        public abstract double CalculateFlowVelocity();

        // Método para verificar capacidade de drenagem
        public bool IsOverflowing(double systemPressure)
        {
            // Lógica de exemplo: considerar overflow acima de 80% da capacidade máxima
            return (Lpm > MaxCapacity * 0.8 && systemPressure > 2.5); // Pressão em Bar
        }

        // Propriedade virtual para capacidade máxima (pode ser sobrescrita)
        public virtual double MaxCapacity => 1000; // Capacidade padrão em LPM
    }
}
