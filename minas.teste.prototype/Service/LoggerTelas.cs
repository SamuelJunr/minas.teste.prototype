using System;
using System.Collections.Generic; // Para gerenciar histórico se necessário
using System.Text; // Para StringBuilder se necessário
using minas.teste.prototype.MVVM.Model.Abstract;

namespace minas.teste.prototype.Service
{
        /// <summary>
        /// Classe estática responsável por gerar e distribuir mensagens de log.
        /// </summary>
        public static class LoggerTelas
        {
            /// <summary>
            /// Evento disparado sempre que uma nova mensagem de log é adicionada.
            /// Formulários ou outros componentes podem subscrever a este evento.
            /// </summary>
            public static event EventHandler<LogEventArgs> LogMessageAdded;

            // Opcional: Armazenar um histórico recente de logs, se necessário
            // private static readonly List<string> logHistory = new List<string>();
            // private const int MAX_HISTORY = 100;

            /// <summary>
            /// Registra uma nova mensagem de log.
            /// </summary>
            /// <param name="message">A mensagem a ser registrada.</param>
            public static void Log(string message)
            {
                try
                {
                    // 1. Formatar a mensagem (adicionar timestamp)
                    string formattedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";

                    // Opcional: Adicionar ao histórico interno
                    // lock (logHistory) // Necessário se o histórico for acessado por múltiplas threads
                    // {
                    //     logHistory.Add(formattedMessage);
                    //     if (logHistory.Count > MAX_HISTORY)
                    //     {
                    //         logHistory.RemoveRange(0, logHistory.Count - MAX_HISTORY);
                    //     }
                    // }

                    // 2. Disparar o evento para notificar os ouvintes (subscribers)
                    //    Usa ?. para verificar se há algum assinante antes de disparar.
                    //    Passa 'null' como sender (por ser estático) e os argumentos do evento.
                    LogMessageAdded?.Invoke(null, new LogEventArgs(formattedMessage));
                }
                catch (Exception ex)
                {
                    // Fallback em caso de erro no próprio logger
                    System.Diagnostics.Debug.WriteLine($"ERRO NO LOGGER: {ex.Message}");
                }
            }

            // Opcional: Método para obter o histórico, se implementado
            // public static IEnumerable<string> GetLogHistory()
            // {
            //     lock (logHistory)
            //     {
            //         return new List<string>(logHistory); // Retorna cópia para thread-safety
            //     }
            // }
        }
}

