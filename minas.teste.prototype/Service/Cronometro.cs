using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace minas.teste.prototype.Service
{

    public class cronometroSK4 : INotifyPropertyChanged
    {
        private TimeSpan _accumulatedElapsed;
        private DateTime? _startTime;
        private bool _isRunning;
        private string _formattedElapsed;

        public string Description { get; set; } // Algum outro dado da linha

        // Tempo acumulado quando o cronômetro estava parado
        public TimeSpan AccumulatedElapsed
        {
            get => _accumulatedElapsed;
            set { _accumulatedElapsed = value; NotifyPropertyChanged(); UpdateFormattedString(); }
        }

        // Momento em que o cronômetro foi iniciado pela última vez (null se parado)
        public DateTime? StartTime
        {
            get => _startTime;
            set { _startTime = value; NotifyPropertyChanged(); }
        }

        // Indica se o cronômetro está rodando
        public bool IsRunning
        {
            get => _isRunning;
            set { _isRunning = value; NotifyPropertyChanged(); }
        }

        // String formatada para exibição na célula (ex: "00:00.0")
        public string FormattedElapsed
        {
            get => _formattedElapsed;
            // O setter é privado ou protegido porque só deve ser atualizado internamente
            private set { _formattedElapsed = value; NotifyPropertyChanged(); }
        }

        // Construtor
        public cronometroSK4()
        {
            Description = "Nova Tarefa"; // Valor padrão
            _accumulatedElapsed = TimeSpan.Zero;
            _isRunning = false;
            _startTime = null;
            UpdateFormattedString(); // Define o valor inicial "00:00.0"
        }

        // Calcula o tempo total decorrido atual (acumulado + tempo desde o último start)
        public TimeSpan GetCurrentTotalElapsedTime()
        {
            TimeSpan currentTotal = _accumulatedElapsed;
            if (_isRunning && _startTime.HasValue)
            {
                currentTotal += (DateTime.Now - _startTime.Value);
            }
            return currentTotal;
        }

        // Atualiza a string formatada (chamar no Timer ou após ações)
        public void UpdateFormattedString()
        {
            TimeSpan total = GetCurrentTotalElapsedTime();
            // Formato mm:ss.f (minutos, segundos, décimos de segundo)
            // Outras opções: "hh\\:mm\\:ss" (horas, minutos, segundos)
            FormattedElapsed = string.Format("{0:mm\\:ss\\.f}", total);
        }

        // --- Implementação de INotifyPropertyChanged ---
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
