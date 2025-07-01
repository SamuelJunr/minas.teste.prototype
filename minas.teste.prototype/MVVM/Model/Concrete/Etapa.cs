using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace minas.teste.prototype.MVVM.Model.Concrete
{
    public class Etapa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

        public DateTime? TerminateTime { get; set; }

        [Column(TypeName = "TEXT")]
        public string Modulo { get; set; }

        public int? Ordem { get; set; }

        [Column(TypeName = "TEXT")]
        public string Modelo { get; set; }

        public int? EmpresaID { get; set; }

        public int? UsuarioID { get; set; }

        // Nova propriedade para relacionar Etapa com Sessao
        public int? SessaoID { get; set; }
        [ForeignKey("SessaoID")]
        public virtual Sessao Sessao { get; set; }

        public virtual Empresa Empresa { get; set; } // Já existe
        public virtual Usuario Usuario { get; set; } // Já existe

        // Adicionar uma coleção de SensorDataDB para relacionar a etapa aos seus dados de sensor
        public virtual ICollection<SensorDataDB> SensorData { get; set; }
    }
}