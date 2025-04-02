using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace minas.teste.prototype.Service
{
    public class ApplicationSession
    {
        private static readonly Lazy<ApplicationSession> instance =
            new Lazy<ApplicationSession>(() => new ApplicationSession());

        public static ApplicationSession Instance => instance.Value;

        private readonly Stopwatch sessionTimer;
        public DateTime StartTime { get; private set; }
        public TimeSpan TotalRuntime => sessionTimer.Elapsed;

        public static event EventHandler ApplicationExit;

        private ApplicationSession()
        {
            sessionTimer = new Stopwatch();
            StartTime = DateTime.Now;
            sessionTimer.Start();

            // Configura para salvar ao fechar o aplicativo
            ApplicationSession.ApplicationExit += (s, e) => SaveSession();
        }

        public void SaveSession()
        {
            sessionTimer.Stop();

            // Aqui você pode salvar em banco de dados, arquivo, etc.
            var sessionData = new
            {
                StartTime,
                EndTime = DateTime.Now,
                TotalRuntime.TotalSeconds,
                TotalRuntime.TotalMinutes
            };

            // Exemplo: salvar em um arquivo JSON
            File.WriteAllText("session_log.json", JsonConvert.SerializeObject(sessionData));
        }
    }
}
