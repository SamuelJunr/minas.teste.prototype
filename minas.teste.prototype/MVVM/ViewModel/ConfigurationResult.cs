using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace minas.teste.prototype.MVVM.ViewModel
{
    public class ConfigurationResult
    {
        public string SelectedPressureUnit { get; set; }
        public string SelectedFlowUnit { get; set; }
        public List<string> SelectedReadingIds { get; set; }

        public ConfigurationResult()
        {
            SelectedReadingIds = new List<string>();
            // Definir padrões aqui se desejar que uma nova configuração já venha com unidades padrão
            SelectedPressureUnit = "bar";
            SelectedFlowUnit = "lpm";
        }
    }

}
