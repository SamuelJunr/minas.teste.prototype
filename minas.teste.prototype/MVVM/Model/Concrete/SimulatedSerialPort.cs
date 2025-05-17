using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Timers; // Use System.Timers.Timer for background simulation

namespace minas.teste.prototype.MVVM.ViewModel
{
    // Event arguments to pass received data string
    public class SerialDataReceivedEventArgs : EventArgs
    {
        public string Data { get; }

        public SerialDataReceivedEventArgs(string data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Classe que simula a leitura de dados de uma porta serial no formato especificado.
    /// Em uma aplicação real, esta classe seria substituída pela manipulação real da SerialPort.
    /// </summary>
    public class SimulatedSerialPort : IDisposable
    {
        // Evento que a Form irá assinar para receber novos dados simulados
        public event EventHandler<SerialDataReceivedEventArgs> DataReceived;

        private System.Timers.Timer _simulationTimer; // Timer para simular a chegada de dados (System.Timers)
        private Random _random; // Para simular dados variados

        // Simulated Sensor Values (these will change over time)
        private double _currentP1 = 0.0; // Corresponds to Pressao_BAR in UI
        private double _currentFluxo1 = 8000; // Corresponds to Vazao_LPM in UI
        private double _currentPiloto1 = 10; // Corresponds to Pilotagem_BAR in UI
        private double _currentDreno1 = 0; // Corresponds to Dreno_LPM in UI
        private double _currentRPM = 1100;
        private double _currentTemp = 25;

        private double _timeCounter = 0;

        // Conversion factors (based on user request - applied during parsing in the Form)
        // We store them here for reference, but the parsing logic in the Form will use them.
        private const double BAR_TO_PSI_CONVERSION = 14.5;
        private const double LPM_TO_GPM_CONVERSION = 3.98;


        public SimulatedSerialPort()
        {
            _random = new Random();
            _simulationTimer = new System.Timers.Timer(500); // Simulate data every 500ms
            _simulationTimer.Elapsed += SimulationTimer_Elapsed; // Event correct for System.Timers.Timer
            _simulationTimer.AutoReset = true; // Ensure the timer repeats
        }

        /// <summary>
        /// Inicia a simulação de recebimento de dados.
        /// </summary>
        public void Start()
        {
            // Optional: Trigger an initial data reception immediately on start
            SimulateDataReception();
            _simulationTimer.Start();
        }

        /// <summary>
        /// Para a simulação de recebimento de dados.
        /// </summary>
        public void Stop()
        {
            if (_simulationTimer != null)
            {
                _simulationTimer.Stop();
                // Do not dispose here, only in the Dispose() method of the class.
            }
        }

        /// <summary>
        /// Reseta os valores simulados para o estado inicial.
        /// </summary>
        public void Reset()
        {
            _timeCounter = 0;
            _currentP1 = 0.0;
            _currentFluxo1 = 8000;
            _currentPiloto1 = 10;
            _currentDreno1 = 0;
            _currentRPM = 1100;
            _currentTemp = 25;
            // Optional: Trigger a data reception after reset
            SimulateDataReception();
        }

        /// <summary>
        /// Implementação de IDisposable para garantir a liberação do timer.
        /// </summary>
        public void Dispose()
        {
            Stop(); // Stop the timer
            if (_simulationTimer != null)
            {
                _simulationTimer.Dispose(); // Release timer resources
                _simulationTimer = null; // Set to null
            }
            // Suppress finalization if Dispose is called explicitly
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizador (Destructor) caso Dispose não seja chamado explicitamente.
        /// </summary>
        ~SimulatedSerialPort()
        {
            Dispose(); // Call Dispose in the finalizer
        }

        // Method called by the System.Timers.Timer.Elapsed event.
        // IMPORTANT: This method does NOT run on the UI thread.
        private void SimulationTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Simulate data changes (simple example)
            _timeCounter++;

            // Simulate P1 (Pressão BAR)
            _currentP1 = 100 + 50 * Math.Sin(_timeCounter * 0.05) + (_random.NextDouble() - 0.5) * 2;
            if (_currentP1 < 0) _currentP1 = 0;
            if (_currentP1 > 300) _currentP1 = 300; // Limit to reasonable range

            // Simulate fluxo1 (Vazão LPM)
            _currentFluxo1 = 8000 + 1000 * Math.Cos(_timeCounter * 0.08) + (_random.NextDouble() - 0.5) * 50;
            if (_currentFluxo1 < 0) _currentFluxo1 = 0;
            if (_currentFluxo1 > 10000) _currentFluxo1 = 10000; // Limit to reasonable range


            // Simulate Piloto1 (Pilotagem BAR)
            _currentPiloto1 = 10 + 5 * Math.Sin(_timeCounter * 0.1) + (_random.NextDouble() - 0.5) * 1;
            if (_currentPiloto1 < 0) _currentPiloto1 = 0;
            if (_currentPiloto1 > 50) _currentPiloto1 = 50; // Limit to reasonable range


            // Simulate dreno1 (Dreno LPM)
            _currentDreno1 = 0.5 + 0.2 * Math.Sin(_timeCounter * 0.12) + (_random.NextDouble() - 0.5) * 0.1;
            if (_currentDreno1 < 0) _currentDreno1 = 0;
            if (_currentDreno1 > 5) _currentDreno1 = 5; // Limit to reasonable range


            // Simulate RPM
            _currentRPM = 1100 + 100 * Math.Sin(_timeCounter * 0.06) + (_random.NextDouble() - 0.5) * 20;
            if (_currentRPM < 0) _currentRPM = 0;
            if (_currentRPM > 3000) _currentRPM = 3000; // Limit to reasonable range


            // Simulate Temp (Temperatura Celsius)
            _currentTemp = 25 + 5 * Math.Sin(_timeCounter * 0.03) + (_random.NextDouble() - 0.5) * 0.5;
            if (_currentTemp < 20) _currentTemp = 20;
            if (_currentTemp > 100) _currentTemp = 100; // Limit to reasonable range


            // Format the data string as specified: P1:0.00|fluxo1:8223|Piloto1:10|dreno1:0|RPM:1100|temp:25
            string simulatedData = $"P1:{_currentP1:F2}|fluxo1:{_currentFluxo1:F2}|Piloto1:{_currentPiloto1:F2}|dreno1:{_currentDreno1:F2}|RPM:{_currentRPM:F0}|temp:{_currentTemp:F1}";

            // Raise the event DataReceived with the simulated data string.
            OnDataReceived(new SerialDataReceivedEventArgs(simulatedData));
        }

        // Helper method to raise the event
        protected virtual void OnDataReceived(SerialDataReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, e);
        }

        // Public method to simulate sending data on demand (for button clicks)
        public void SimulateDataReception()
        {
            // Trigger data generation and event firing immediately
            SimulationTimer_Elapsed(this, null); // Pass null as EventArgs is not used in this simulation
        }
    }
}