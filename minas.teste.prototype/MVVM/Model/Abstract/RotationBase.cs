using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.Model.Abstract
{
    public abstract class RotationBase
    {
        private const double secondsPerMinute = 60.0;
        private double rotationsPerMinute; // Armazenamento interno em RPM

        // Propriedades de acesso com conversão automática
        public double RPM
        {
            get => rotationsPerMinute;
            set => rotationsPerMinute = value >= 0 ? value : throw new ArgumentException("RPM não pode ser negativo");
        }

        public double RPS
        {
            get => rotationsPerMinute / secondsPerMinute;
            set => rotationsPerMinute = value * secondsPerMinute;
        }

        // Construtor protegido para inicialização
        protected RotationBase(double initialRotation, bool isRps = false)
        {
            if (isRps)
                RPS = initialRotation;
            else
                RPM = initialRotation;
        }

        // Métodos de conversão estáticos
        public static double ConvertRpmToRps(double rpm) => rpm / secondsPerMinute;
        public static double ConvertRpsToRpm(double rps) => rps * secondsPerMinute;

        // Método para verificação de faixa segura
        public virtual bool IsWithinSafeRange(double minRpm, double maxRpm) => RPM >= minRpm && RPM <= maxRpm;

        // Método abstrato para cálculo de potência/torque (será implementado nas classes filhas)
        public abstract double CalculatePower(double torque);

        // Conversão para velocidade angular (rad/s)
        public double AngularVelocity => RPM * (2 * Math.PI) / secondsPerMinute;

        // Conversão para Hertz (ciclos por segundo)
        public double Hertz => RPS;
    }
}
