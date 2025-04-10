using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace minas.teste.prototype.Teste.Tela_Bomba
{
    public class TestDataGenerator
    {
        private Timer _timer;
        private Random _random;
        private DateTime _startTime;
        private List<char> _labels;
        private object _lock = new object();

        public event Action<string> OnDataGenerated;

        public TestDataGenerator()
        {
            _random = new Random();
            _labels = new List<char>();

            // Cria labels de A até L (12 caracteres)
            for (char c = 'A'; c <= 'L'; c++)
            {
                _labels.Add(c);
            }

            _timer = new Timer(1000); // Intervalo de 1 segundo
            _timer.Elapsed += GenerateData;
        }

        public void StartGeneration()
        {
            _startTime = DateTime.Now;
            _timer.Start();
        }

        public void StopGeneration()
        {
            _timer.Stop();
        }

        private void GenerateData(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                // Verifica se passaram 100 segundos
                if ((DateTime.Now - _startTime).TotalSeconds >= 100)
                {
                    StopGeneration();
                    return;
                }

                List<string> dataPoints = new List<string>();

                foreach (var label in _labels)
                {
                    // Gera número aleatório com 3 casas decimais
                    int value = _random.Next(0, 301);
                    dataPoints.Add($"{label}:{value:F3}");
                }

                string formattedData = string.Join("|", dataPoints);
                OnDataGenerated?.Invoke(formattedData);
            }
        }
    }
}
