using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using minas.teste.prototype.MVVM.Model.Abstract;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class SessaoBomba : ISessaoTeste
    {
        public Guid Id { get; protected set; }
        public string Nome { get; set; } // Nome da bomba ou identificador
        public DateTime Inicio { get; protected set; }
        public DateTime Fim { get; protected set; }
        public string ErrorMessage { get; set; }
        public SessaoBomba()
        {
            Id = Guid.NewGuid();
            Inicio = DateTime.Now;
        }
        public void FinalizarSessao()
        {
            Fim = DateTime.Now;
        }
        public bool SalvarDados()
        {
            // Implementar lógica de salvamento dos dados da sessão
            return true;
        }
        protected void MostrarErro()
        {
            // Implementar lógica para mostrar erros
        }

        public void ProcessarSalvamento()
        {
            
        }

       
    }
    
}
