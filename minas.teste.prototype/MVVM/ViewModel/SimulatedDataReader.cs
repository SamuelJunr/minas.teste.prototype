// SimulatedDataReader.cs
using System;
using System.Collections.Generic;

namespace minas.teste.prototype.Service // Ou um namespace apropriado para utilitários
{
    public class SimulatedDataReader
    {
        private Random _random = new Random();
        private double _currentValue;
        private double _slope;
        private double _noiseLevel;
        private int _dataPointCount;
        private int _currentIndex;

        /// <summary>
        /// Inicializa o gerador de dados simulados com uma tendência linear.
        /// </summary>
        /// <param name="startValue">O valor inicial da série de dados.</param>
        /// <param name="slope">A inclinação da tendência linear. Positivo para crescente, negativo para decrescente.</param>
        /// <param name="noiseLevel">A magnitude do ruído aleatório a ser adicionado.</param>
        public SimulatedDataReader(double startValue, double slope, double noiseLevel)
        {
            _currentValue = startValue;
            _slope = slope;
            _noiseLevel = noiseLevel;
            _currentIndex = 0;
        }

        /// <summary>
        /// Gera um único ponto de dado simulado seguindo a tendência linear e com ruído.
        /// </summary>
        /// <returns>Um valor de ponto flutuante simulado.</returns>
        public double GetNextSimulatedDataPoint()
        {
            // Valor base com tendência linear
            double value = _currentValue + (_slope * _currentIndex);

            // Adicionar ruído aleatório
            value += (_random.NextDouble() * 2 - 1) * _noiseLevel; // Gera um número entre -noiseLevel e +noiseLevel

            _currentIndex++;
            return value;
        }

        /// <summary>
        /// Redefine o gerador para começar a partir do valor inicial novamente.
        /// </summary>
        public void Reset()
        {
            _currentIndex = 0;
            // Opcional: redefinir _currentValue para o valor inicial, se desejar que a série comece do mesmo ponto após o reset
            // _currentValue = initialStartValue; // Você precisaria armazenar o initialStartValue no construtor
        }
    }
}