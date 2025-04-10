// minas.teste.prototype.MVVM.Model.Concrete.SensorCalibracao.cs (ou local apropriado)
using System;
using System.ComponentModel.DataAnnotations; // Para atributos como [Key], [Required]
using System.ComponentModel.DataAnnotations.Schema; // Para [Table]

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    [Table("SensorCalibracoes")] // Nome da tabela no banco de dados
    public class SensorCalibracoes
    {
        [Key] // Indica que é a chave primária
        public int Id { get; set; } // Chave primária autoincrementável

        [Required] // Campo obrigatório
        [MaxLength(100)] // Tamanho máximo
        public string SensorIdentificador { get; set; } // Um ID único ou nome do sensor que está sendo calibrado

        [Required]
        public DateTime DataCalibracao { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        // Parâmetros de calibração comuns (ajuste conforme necessidade)
        public double ValorOffset { get; set; } // Ex: Zero offset
        public double ValorGanho { get; set; } // Ex: Span ou fator de multiplicação
        public double LeituraBrutaMin { get; set; } // Valor mínimo lido antes da calibração
        public double LeituraBrutaMax { get; set; } // Valor máximo lido antes da calibração
        public double ValorReferenciaMin { get; set; } // Valor de referência para LeituraBrutaMin
        public double ValorReferenciaMax { get; set; } // Valor de referência para LeituraBrutaMax

        [MaxLength(50)]
        public string Unidade { get; set; } // Unidade da medida após calibração (ex: "°C", "bar", "LPM")

        [MaxLength(100)]
        public string UsuarioCalibracao { get; set; } // Quem realizou a calibração

        public string Notas { get; set; } // Campo para observações adicionais

        // Propriedade de navegação opcional se você tiver uma entidade Sensor separada
        // public virtual Sensor Sensor { get; set; }
        // public int SensorId { get; set; } // Chave estrangeira
        
    }
}