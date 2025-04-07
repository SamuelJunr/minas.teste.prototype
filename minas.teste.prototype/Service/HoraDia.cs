using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace minas.teste.prototype.Service
{
    public class Hora : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }

    public class HoraDia : IDisposable
    {
        private Label _targetLabel;
        private Timer _timer;
        private string _timeFormat = "HH:mm:ss";

        // Construtor: Recebe o Label que exibirá a hora
        public HoraDia(Label targetLabel)
        {
            _targetLabel = targetLabel ?? throw new ArgumentNullException(nameof(targetLabel));
            InitializeTimer();
        }

        // Formato personalizável (padrão: 24h)
        public string TimeFormat
        {
            get => _timeFormat;
            set => _timeFormat = !string.IsNullOrEmpty(value) ? value : "HH:mm:ss";
        }

        private void InitializeTimer()
        {
            _timer = new Timer();
            _timer.Interval = 1000; // Atualiza a cada 1 segundo
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTime();
        }

        // Atualiza o Label com a hora atual
        public void UpdateTime()
        {
            if (_targetLabel.InvokeRequired) // Thread-safe
            {
                _targetLabel.Invoke(new Action(UpdateTime));
            }
            else
            {
                _targetLabel.Text = DateTime.Now.ToString(_timeFormat);
            }
        }

        // Liberar recursos
        public void Dispose()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _targetLabel = null;
        }
    }
}
