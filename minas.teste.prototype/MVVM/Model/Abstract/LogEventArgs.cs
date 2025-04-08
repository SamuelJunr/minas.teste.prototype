using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.Model.Abstract
{
    /// <summary>
    /// Argumentos para o evento de adição de mensagem de log.
    /// </summary>
    public class LogEventArgs : EventArgs
    {
        /// <summary>
        /// A mensagem de log completa (geralmente já formatada com timestamp).
        /// </summary>
        public string Message { get; }

        public LogEventArgs(string message)
        {
            Message = message;
        }
    }
}
