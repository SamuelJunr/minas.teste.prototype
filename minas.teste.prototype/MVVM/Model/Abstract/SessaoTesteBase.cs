using System;

namespace minas.teste.prototype.MVVM.Model.Abstract
{
    public abstract class SessaoTesteBase
    {
        public DateTime Fim { get; protected set; }
        public Guid Id { get; protected set; }
        public DateTime Inicio { get; protected set; }

        // Método para finalizar a sessão
        public void FinalizarSessao()
        {
            Fim = DateTime.Now;
        }

        // Método template para o processo de salvamento
        public void ProcessarSalvamento()
        {
            if (!SalvarDados())
            {
                
            }
        }

        // Método abstrato para salvar os dados (deve ser implementado nas classes derivadas)
        public abstract bool SalvarDados();
    }
}