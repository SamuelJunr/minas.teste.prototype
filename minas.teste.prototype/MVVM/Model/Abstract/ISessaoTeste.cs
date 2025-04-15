using System;

namespace minas.teste.prototype.MVVM.Model.Abstract
{
    public interface ISessaoTeste
    {
        DateTime Fim { get; }
        Guid Id { get; }
        DateTime Inicio { get; }

        void FinalizarSessao();
        void ProcessarSalvamento();
        bool SalvarDados();
    }
}