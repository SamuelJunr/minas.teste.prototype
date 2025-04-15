using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.Model.Abstract
{
    public abstract class SessaoTeste : ISessaoTeste
    {
        public Guid Id { get; protected set; }
        public DateTime Inicio { get; protected set; }
        public DateTime Fim { get; protected set; }
        protected string ErrorMessage { get; set; }

        protected SessaoTeste()
        {
            Id = Guid.NewGuid();
            Inicio = DateTime.Now;
        }

        // Método para finalizar a sessão
        public void FinalizarSessao()
        {
            Fim = DateTime.Now;
        }

        // Método abstrato para salvar os dados (deve ser implementado nas classes derivadas)
        public abstract bool SalvarDados();

        // Método para mostrar erros (implementação será fornecida nas classes derivadas)
        protected abstract void MostrarErro();

        // Método template para o processo de salvamento
        public void ProcessarSalvamento()
        {
            if (!SalvarDados())
            {
                MostrarErro();
            }
        }
    }
}
