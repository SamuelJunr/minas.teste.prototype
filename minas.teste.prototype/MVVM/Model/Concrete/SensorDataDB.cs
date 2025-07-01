using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class SensorDataDB
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Column(TypeName = "TEXT")]
        public string SensorName { get; set; } // Nome do sensor, ex: "sensor_MA1_press"

        public double Value { get; set; } // Valor lido do sensor

        [Column(TypeName = "TEXT")]
        public string Unit { get; set; } // Unidade do sensor, ex: "bar", "lpm"

        public DateTime Timestamp { get; set; } = DateTime.Now; // Momento da leitura

        // Chave estrangeira para a Etapa
        public int? EtapaID { get; set; }
        [ForeignKey("EtapaID")]
        public virtual Etapa Etapa { get; set; }

        // Chave estrangeira para a Sessao (para facilitar a consulta de todos os dados de sensor de uma sessão)
        public int? SessaoID { get; set; }
        [ForeignKey("SessaoID")]
        public virtual Sessao Sessao { get; set; }
    }
}