using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace minas.teste.prototype.MVVM.ViewModel
{
    public class ReadingData
    {
            public string Id { get; set; } // e.g., "sensor_MA1"
            public string Name { get; set; } // e.g., "P1"
            public double Value { get; set; } // Example value
            public string Type { get; set; } // "pressure", "flow", "fixed"
            public string OriginalUnit { get; set; } // Base unit
            public string DisplayUnit { get; set; } // Unit after configuration

            // Referências aos controles na Tela_Bombas
            public string ValueTextBoxName { get; set; }
            public string UnitLabelName { get; set; }
            public string NameLabelText { get; set; } // O texto que apareceria como nome da leitura na Tela_Bombas

            public ReadingData(string id, string name, double value, string type, string originalUnit, string valueTextBoxName, string unitLabelName, string nameLabelText = null)
            {
                Id = id;
                Name = name; // Usado no ConfigForm
                Value = value;
                Type = type;
                OriginalUnit = originalUnit;
                DisplayUnit = originalUnit;
                ValueTextBoxName = valueTextBoxName;
                UnitLabelName = unitLabelName;
                NameLabelText = nameLabelText ?? name; // Se não fornecido, usa o 'Name'
            }

            public ReadingData Clone()
            {
                return (ReadingData)this.MemberwiseClone();
            }
    }

    
}


   

