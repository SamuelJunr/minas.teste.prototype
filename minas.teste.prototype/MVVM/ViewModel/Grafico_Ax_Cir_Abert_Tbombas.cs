using System;
using System.Timers; // Use System.Timers.Timer

namespace minas.teste.prototype.MVVM.ViewModel
{
    /// <summary>
    /// Classe que simula a geração de dados de Rotação (rpm) e Pressão (bar)
    /// para fins de teste do gráfico.
    /// </summary>
    public class Grafico_Ax_Cir_Abert_Tbombas : IDisposable // Implementa IDisposable
    {
        // Evento que a Form irá assinar para receber novos dados simulados
        public event EventHandler<Datapoint_Bar_Rpm> DataUpdated;

        private System.Timers.Timer _dataTimer; // Timer para gerar dados em tempo real (System.Timers)
        private Random _random; // Para simular dados variados

        // Variáveis para simular dados que mudam ao longo do tempo
        private double _currentRotation = 1000;
        private double _currentPressure = 10;
        private double _timeCounter = 0;

        public Grafico_Ax_Cir_Abert_Tbombas()
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
            _currentRotation = 1000;
            _currentPressure = 10;
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
        ~Grafico_Ax_Cir_Abert_Tbombas()
        {
            Dispose(); // Chama Dispose no finalizador
        }


        // Método chamado a cada disparo do System.Timers.Timer.Elapsed.
        // IMPORTANTE: Este método NÃO roda no thread da UI.
        private void DataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // --- SIMULAÇÃO DE DADOS ---
            _timeCounter++;

            // Simula a rotação variando ligeiramente
            _currentRotation = 1000 + 100 * Math.Sin(_timeCounter * 0.1) + (_random.NextDouble() - 0.5) * 20;
            if (_currentRotation < 0) _currentRotation = 0;

            // Simula a pressão baseada na rotação, com alguma variação e tendência a cair
            double basePressure = (_currentRotation / 2000.0) * 200;
            double pressureVariation = (_random.NextDouble() - 0.5) * 30;
            double performanceLoss = _timeCounter * 0.5;

            _currentPressure = basePressure + pressureVariation - performanceLoss;

            if (_currentPressure < 0) _currentPressure = 0;
            if (_currentPressure > 250) _currentPressure = 250;

            // Dispara o evento DataUpdated com os novos dados.
            // A Form que assina este evento precisará usar Invoke/BeginInvoke
            // para atualizar o Chart de forma segura, pois este evento dispara em um thread diferente do UI.
            DataUpdated?.Invoke(this, new Datapoint_Bar_Rpm(_currentRotation, _currentPressure));
        }
    }
}