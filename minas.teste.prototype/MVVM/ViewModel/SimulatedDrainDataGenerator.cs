using System;
using System.Timers; // Use System.Timers.Timer

namespace minas.teste.prototype.MVVM.ViewModel
{
    /// <summary>
    /// EventArgs customizado para o evento DataUpdated do gerador de dados de dreno.
    /// </summary>
    public class DrainDataEventArgs : EventArgs
    {
        public double DrainFlowLpm { get; }
        public double RotationRpm { get; } // Rotação incluída para correlação com dreno

        public DrainDataEventArgs(double drainFlowLpm, double rotationRpm)
        {
            DrainFlowLpm = drainFlowLpm;
            RotationRpm = rotationRpm;
        }
    }

    /// <summary>
    /// Classe que simula a geração de dados de Vazão de Dreno e Rotação.
    /// </summary>
    public class SimulatedDrainDataGenerator : IDisposable // Implementa IDisposable
    {
        // Evento que a Form irá assinar para receber novos dados simulados
        public event EventHandler<DrainDataEventArgs> DataUpdated;

        private System.Timers.Timer _dataTimer; // Timer para gerar dados em tempo real (System.Timers)
        private Random _random; // Para simular dados variados

        // Variáveis para simular dados que mudam ao longo do tempo
        private double _currentDrainFlow = 0.5; // Vazão de dreno inicial em LPM
        private double _currentRotation = 1500; // Rotação inicial em RPM (mantida para correlação)
        private double _timeCounter = 0; // Contador de tempo para simulação

        public SimulatedDrainDataGenerator()
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
            _currentDrainFlow = 0.5;
            _currentRotation = 1500;
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
            _currentDrainFlow = 0.5;
            _currentRotation = 1500;
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
        ~SimulatedDrainDataGenerator()
        {
            Dispose(); // Chama Dispose no finalizador
        }


        // Método chamado a cada disparo do System.Timers.Timer.Elapsed.
        // IMPORTANTE: Este método NÃO roda no thread da UI.
        private void DataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // --- SIMULAÇÃO DE DADOS ---
            _timeCounter++;

            // Simula a vazão de dreno aumentando com a rotação e um pouco de ruído
            // Usamos a rotação simulada aqui para manter a correlação para o Chart 2
            double baseDrainFlow = (_currentRotation / 3000.0) * 5;
            double drainVariation = (_random.NextDouble() - 0.5) * 1;

            _currentDrainFlow = baseDrainFlow + drainVariation;

            if (_currentDrainFlow < 0) _currentDrainFlow = 0;
            if (_currentDrainFlow > 8) _currentDrainFlow = 8;

            // Simula a rotação variando ligeiramente em torno de um valor
            // Mantemos a simulação de rotação aqui para que o Chart 2 (Vazão Dreno vs Rotação)
            // tenha dados de rotação gerados por este gerador.
            _currentRotation = 1500 + 100 * Math.Sin(_timeCounter * 0.1) + (_random.NextDouble() - 0.5) * 20;
            if (_currentRotation < 0) _currentRotation = 0;
            if (_currentRotation > 3000) _currentRotation = 3000;


            // Dispara o evento DataUpdated com os novos dados.
            // A Form que assina este evento precisará usar Invoke/BeginInvoke
            // para atualizar o Chart de forma segura, pois este evento dispara em um thread diferente do UI.
            DataUpdated?.Invoke(this, new DrainDataEventArgs(_currentDrainFlow, _currentRotation));
        }
    }
}
