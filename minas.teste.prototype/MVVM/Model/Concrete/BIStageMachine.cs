using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class BIStageMachine 
        public int totalsuccess { get; set; }
        // Construtor padrão
        public BIStageMachine()
        {
            // As propriedades já são inicializadas na classe base
        }

        // Opcional: Construtor com parâmetros para inicialização
        public BIStageMachine(int tests, bool failures, bool successes)
        {
            number_of_tests = tests;
            number_of_failures = failures;
            number_of_successes = successes;
        }

        // Métodos adicionais podem ser adicionados aqui conforme necessidade
        public void IncrementTests() => number_of_tests++;
        public int RecordSuccess(bool number_of_successes)
        {
           var cont_successes = Convert.ToInt32(number_of_successes);
           cont_successes++;
            return cont_successes;
        }
        public int RecordFailure(bool number_of_failures)
        {
            var cont_failures = Convert.ToInt32(number_of_failures);
             cont_failures++;
            return cont_failures;
        }

        public double CalculateSuccessRate()
        {
            return number_of_tests == 0
                ? 0
                : totalsuccess = RecordSuccess(number_of_successes) / number_of_tests * 100;
        }
    }
}
