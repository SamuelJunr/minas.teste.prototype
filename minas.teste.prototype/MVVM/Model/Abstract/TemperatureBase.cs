using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.Model.Abstract
{
    public abstract class TemperatureBase
    {
        private const double absoluteZeroCelsius = -273.15;
        private double celsius; // Armazenamento interno em Celsius

        // Propriedades principais com conversão automática
        public double Celsius
        {
            get { return celsius; }
            set { celsius = ValidateTemperature(value, true); }
        }

        public double Fahrenheit
        {
            get { return (celsius * 9 / 5) + 32; }
            set { celsius = (value - 32) * 5 / 9; }
        }

        // Construtor protegido
        protected TemperatureBase(double initialTemp, bool useFahrenheit = false)
        {
            if (useFahrenheit)
                Fahrenheit = initialTemp;
            else
                Celsius = initialTemp;
        }

        // Métodos de conversão estáticos
        public static double ConvertCelsiusToFahrenheit(double celsius)
        {
            return (celsius * 9 / 5) + 32;
        }

        public static double ConvertFahrenheitToCelsius(double fahrenheit)
        {
            return (fahrenheit - 32) * 5 / 9;
        }

        // Validação de temperatura mínima
        private double ValidateTemperature(double value, bool isCelsius)
        {
            double minTemp = isCelsius ? absoluteZeroCelsius : ConvertCelsiusToFahrenheit(absoluteZeroCelsius);
            if (value >= minTemp)
                return value;

            throw new ArgumentOutOfRangeException($"Temperatura não pode ser inferior ao zero absoluto ({minTemp} {(isCelsius ? "°C" : "°F")})");
        }

        // Método para verificação de condições extremas (adaptado para C# 7.3)
        public virtual string CheckForExtremeConditions()
        {
            if (celsius < - 60)
                return "condições extremas Congelamento";

            if (celsius >=  1000)
                return "condições extremas Superaquecimento";
            else
                return "Condições Normais";
        }

        // Método abstrato para cálculos específicos
        public abstract double CalculateThermalExpansion(double initialLength, double coefficient);
    }
}
