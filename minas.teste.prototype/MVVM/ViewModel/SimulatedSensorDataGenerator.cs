using System;
using System.Timers; // Use System.Timers.Timer

namespace minas.teste.prototype.MVVM.ViewModel
{
    /// <summary>
    /// EventArgs customizado para o evento DataUpdated, contendo dados simulados.
    /// </summary>
    public class SensorDataEventArgs : EventArgs
    {
        public double PressureBar { get; }
        public double FlowLpm { get; }
        public double RotationRpm { get; }
        public double TemperatureC { get; } // Adicionado: Temperatura em Celsius

        public SensorDataEventArgs(double pressureBar, double flowLpm, double rotationRpm, double temperatureC)
        {
            PressureBar = pressureBar;
            FlowLpm = flowLpm;
            RotationRpm = rotationRpm;
            TemperatureC = temperatureC; // Inicializa a temperatura
        }
    }

    /// <summary>
    /// Classe que simula a geração de dados de Pressão, Vazão Principal, Rotação e Temperatura
    /// para fins de teste dos gráficos.
    /// </summary>
    public class SimulatedSensorDataGenerator : IDisposable // Implementa IDisposable
    {
        // Evento que a Form irá assinar para receber novos dados simulados
        public event EventHandler<SensorDataEventArgs> DataUpdated;

        private System.Timers.Timer _dataTimer; // Timer para gerar dados em tempo real (System.Timers)
        private Random _random; // Para simular dados variados

        // Variáveis para simular dados que mudam ao longo do tempo
        private double _currentPressure = 50; // Pressão inicial em BAR
        private double _currentFlow = 20; // Vazão inicial em LPM
        private double _currentRotation = 1500; // Rotação inicial em RPM
        private double _currentTemperature = 30; // Temperatura inicial em Celsius
        private double _timeCounter = 0; // Contador de tempo para simulação

        public SimulatedSensorDataGenerator()
        {
            _random = new Random();
            _dataTimer = new System.Timers.Timer(); // Instancia System.Timers.Timer
            _dataTimer.Interval = 1000; // Intervalo de 1 segundo (1000 ms)
            _dataTimer.Elapsed += DataTimer_Elapsed; // Evento correto para System.Timers.Timer
            _dataTimer.AutoReset = true; // Garante que o timer se repita
        }

        /// <summary>
        /// Inicia a geração de dados simulados.
        /// </summary>
        public void Start()
        {
            // Opcional: Resetar o estado da simulação ao iniciar
            _timeCounter = 0;
            _currentPressure = 50;
            _currentFlow = 20;
            _currentRotation = 1500;
            _currentTemperature = 30; // Resetar temperatura também
            _dataTimer.Start();
        }

        /// <summary>
        /// Para a geração de dados simulados.
        /// </summary>
        public void Stop()
        {
            if (_dataTimer != null)
            {
                _dataTimer.Stop();
                // Não dispose aqui, apenas no método Dispose() da classe.
            }
        }

        /// <summary>
        /// Reseta os valores simulados para o estado inicial.
        /// </summary>
        public void Reset()
        {
            _timeCounter = 0;
            _currentPressure = 50;
            _currentFlow = 20;
            _currentRotation = 1500;
            _currentTemperature = 30;
        }


        /// <summary>
        /// Implementação de IDisposable para garantir a liberação do timer.
        /// </summary>
        public void Dispose()
        {
            Stop(); // Para o timer
            if (_dataTimer != null)
            {
                _dataTimer.Dispose(); // Libera os recursos do timer
                _dataTimer = null; // Define como null
            }
            // Suprime a finalização se Dispose for chamado explicitamente
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizador (Destructor) caso Dispose não seja chamado explicitamente.
        /// </summary>
        ~SimulatedSensorDataGenerator()
        {
            Dispose(); // Chama Dispose no finalizador
        }


        // Método chamado a cada disparo do System.Timers.Timer.Elapsed.
        // IMPORTANTE: Este método NÃO roda no thread da UI.
        private void DataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // --- SIMULAÇÃO DE DADOS ---
            _timeCounter++;

            // Simula a pressão variando com o tempo e um pouco de ruído
            _currentPressure = 50 + 100 * Math.Sin(_timeCounter * 0.05) + (_random.NextDouble() - 0.5) * 10;
            if (_currentPressure < 0) _currentPressure = 0;
            if (_currentPressure > 250) _currentPressure = 250;

            // Simula a vazão variando com o tempo e um pouco de ruído
            _currentFlow = 20 + 30 * Math.Cos(_timeCounter * 0.08) + (_random.NextDouble() - 0.5) * 5;
            if (_currentFlow < 0) _currentFlow = 0;
            if (_currentFlow > 80) _currentFlow = 80;

            // Simula a rotação variando ligeiramente em torno de um valor
            _currentRotation = 1500 + 100 * Math.Sin(_timeCounter * 0.1) + (_random.NextDouble() - 0.5) * 20;
            if (_currentRotation < 0) _currentRotation = 0;
            if (_currentRotation > 3000) _currentRotation = 3000;

            // Simula a temperatura aumentando gradualmente com o tempo e um pouco de ruído
            _currentTemperature = 30 + _timeCounter * 0.1 + (_random.NextDouble() - 0.5) * 2;
            if (_currentTemperature < 20) _currentTemperature = 20; // Temperatura mínima
            if (_currentTemperature > 80) _currentTemperature = 80; // Temperatura máxima


            // Dispara o evento DataUpdated com os novos dados, incluindo temperatura.
            // A Form que assina este evento precisará usar Invoke/BeginInvoke
            // para atualizar os controles da UI de forma segura, pois este evento dispara em um thread diferente do UI.
            DataUpdated?.Invoke(this, new SensorDataEventArgs(_currentPressure, _currentFlow, _currentRotation, _currentTemperature));
        }
    }
}
