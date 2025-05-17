using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.ViewModel
{
          public class PumpDataEventArgs : EventArgs
          {
            public double PilotagemPSI { get; }
            public double PilotagemBAR { get; }
            public double DrenoGPM { get; }
            public double DrenoLPM { get; }
            public double PressaoPSI { get; }
            public double PressaoBAR { get; }
            public double RotacaoRPM { get; }
            public double VazaoGPM { get; }
            public double VazaoLPM { get; }
            public double TemperaturaCelsius { get; }

            public PumpDataEventArgs(
                double pilotagemPsi,
                double pilotagemBar,
                double drenoGpm,
                double drenoLpm,
                double pressaoPsi,
                double pressaoBar,
                double rotacaoRpm,
                double vazaoGpm,
                double vazaoLpm,
                double temperaturaCelsius)
            {
                PilotagemPSI = pilotagemPsi;
                PilotagemBAR = pilotagemBar;
                DrenoGPM = drenoGpm;
                DrenoLPM = drenoLpm;
                PressaoPSI = pressaoPsi;
                PressaoBAR = pressaoBar;
                RotacaoRPM = rotacaoRpm;
                VazaoGPM = vazaoGpm;
                VazaoLPM = vazaoLpm;
                TemperaturaCelsius = temperaturaCelsius;
            }
          }

        /// <summary>
        /// Classe que simula a geração de dados para vários sensores de bomba.
        /// </summary>
        public class SimulatedPumpDataGenerator : IDisposable // Implementa IDisposable
        {
            // Evento que a Form irá assinar para receber novos dados simulados
            public event EventHandler<PumpDataEventArgs> DataUpdated;

            private System.Timers.Timer _dataTimer; // Timer para gerar dados em tempo real (System.Timers)
            private Random _random; // Para simular dados variados

            // Variáveis para simular dados que mudam ao longo do tempo
            private double _currentPilotagemPSI = 100; // Pilotagem PSI inicial
            private double _currentPilotagemBAR; // Pilotagem BAR (será calculado a partir do PSI)
            private double _currentDrenoGPM = 5; // Dreno GPM inicial
            private double _currentDrenoLPM; // Dreno LPM (será calculado a partir do GPM)
            private double _currentPressaoPSI = 500; // Pressão PSI inicial
            private double _currentPressaoBAR; // Pressão BAR (será calculado a partir do PSI)
            private double _currentRotacaoRPM = 1800; // Rotação RPM inicial
            private double _currentVazaoGPM = 100; // Vazão GPM inicial
            private double _currentVazaoLPM; // Vazão LPM (será calculado a partir do GPM)
            private double _currentTemperaturaCelsius = 40; // Temperatura Celsius inicial
            private double _timeCounter = 0; // Contador de tempo para simulação

            // Fatores de conversão (aproximados)
            private const double PSI_TO_BAR = 0.0689476;
            private const double BAR_TO_PSI = 14.5038;
            private const double GPM_TO_LPM = 3.78541;
            private const double LPM_TO_GPM = 0.264172;

            public SimulatedPumpDataGenerator()
            {
                _random = new Random();
                _dataTimer = new System.Timers.Timer(); // Instancia System.Timers.Timer
                _dataTimer.Interval = 500; // Intervalo de 0.5 segundos (500 ms) - pode ajustar conforme necessário
                _dataTimer.Elapsed += DataTimer_Elapsed; // Evento correto para System.Timers.Timer
                _dataTimer.AutoReset = true; // Garante que o timer se repita

                // Inicializa valores convertidos
                _currentPilotagemBAR = _currentPilotagemPSI * PSI_TO_BAR;
                _currentDrenoLPM = _currentDrenoGPM * GPM_TO_LPM;
                _currentPressaoBAR = _currentPressaoPSI * PSI_TO_BAR;
                _currentVazaoLPM = _currentVazaoGPM * GPM_TO_LPM;
            }

            /// <summary>
            /// Inicia a geração de dados simulados.
            /// </summary>
            public void Start()
            {
                // Opcional: Resetar o estado da simulação ao iniciar
                Reset(); // Chame Reset para garantir valores iniciais consistentes
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
                _currentPilotagemPSI = 100;
                _currentPilotagemBAR = _currentPilotagemPSI * PSI_TO_BAR;
                _currentDrenoGPM = 5;
                _currentDrenoLPM = _currentDrenoGPM * GPM_TO_LPM;
                _currentPressaoPSI = 500;
                _currentPressaoBAR = _currentPressaoPSI * PSI_TO_BAR;
                _currentRotacaoRPM = 1800;
                _currentVazaoGPM = 100;
                _currentVazaoLPM = _currentVazaoGPM * GPM_TO_LPM;
                _currentTemperaturaCelsius = 40;
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
            ~SimulatedPumpDataGenerator()
            {
                Dispose(); // Chama Dispose no finalizador
            }

            // Método chamado a cada disparo do System.Timers.Timer.Elapsed.
            // IMPORTANTE: Este método NÃO roda no thread da UI.
            private void DataTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                // --- SIMULAÇÃO DE DADOS ---
                _timeCounter++;

                // Simula Pilotagem PSI variando com o tempo e ruído
                _currentPilotagemPSI = 100 + 50 * Math.Sin(_timeCounter * 0.1) + (_random.NextDouble() - 0.5) * 5;
                if (_currentPilotagemPSI < 0) _currentPilotagemPSI = 0;
                if (_currentPilotagemPSI > 200) _currentPilotagemPSI = 200;
                _currentPilotagemBAR = _currentPilotagemPSI * PSI_TO_BAR; // Converte para BAR

                // Simula Dreno GPM com variação e ruído
                _currentDrenoGPM = 5 + 2 * Math.Cos(_timeCounter * 0.15) + (_random.NextDouble() - 0.5) * 1;
                if (_currentDrenoGPM < 0) _currentDrenoGPM = 0;
                if (_currentDrenoGPM > 10) _currentDrenoGPM = 10;
                _currentDrenoLPM = _currentDrenoGPM * GPM_TO_LPM; // Converte para LPM

                // Simula Pressão PSI variando com o tempo e ruído
                _currentPressaoPSI = 500 + 200 * Math.Sin(_timeCounter * 0.07) + (_random.NextDouble() - 0.5) * 20;
                if (_currentPressaoPSI < 0) _currentPressaoPSI = 0;
                if (_currentPressaoPSI > 1000) _currentPressaoPSI = 1000;
                _currentPressaoBAR = _currentPressaoPSI * PSI_TO_BAR; // Converte para BAR

                // Simula Rotação RPM com pequena variação e ruído
                _currentRotacaoRPM = 1800 + 50 * Math.Sin(_timeCounter * 0.05) + (_random.NextDouble() - 0.5) * 10;
                if (_currentRotacaoRPM < 0) _currentRotacaoRPM = 0;
                if (_currentRotacaoRPM > 2500) _currentRotacaoRPM = 2500;

                // Simula Vazão GPM variando com o tempo e ruído, correlacionada com a Rotação
                _currentVazaoGPM = 100 + (_currentRotacaoRPM / 1800 - 1) * 50 + 20 * Math.Cos(_timeCounter * 0.1) + (_random.NextDouble() - 0.5) * 10;
                if (_currentVazaoGPM < 0) _currentVazaoGPM = 0;
                if (_currentVazaoGPM > 200) _currentVazaoGPM = 200;
                _currentVazaoLPM = _currentVazaoGPM * GPM_TO_LPM; // Converte para LPM


                // Simula Temperatura aumentando gradualmente e com ruído
                _currentTemperaturaCelsius = 40 + _timeCounter * 0.05 + (_random.NextDouble() - 0.5) * 1;
                if (_currentTemperaturaCelsius < 30) _currentTemperaturaCelsius = 30;
                if (_currentTemperaturaCelsius > 100) _currentTemperaturaCelsius = 100;

                // Dispara o evento DataUpdated com os novos dados.
                // A Form que assina este evento precisará usar Invoke/BeginInvoke
                // para atualizar os controles da UI de forma segura, pois este evento dispara em um thread diferente do UI.
                DataUpdated?.Invoke(this, new PumpDataEventArgs(
                    _currentPilotagemPSI,
                    _currentPilotagemBAR,
                    _currentDrenoGPM,
                    _currentDrenoLPM,
                    _currentPressaoPSI,
                    _currentPressaoBAR,
                    _currentRotacaoRPM,
                    _currentVazaoGPM,
                    _currentVazaoLPM,
                    _currentTemperaturaCelsius)
                );
            }
        }
}

