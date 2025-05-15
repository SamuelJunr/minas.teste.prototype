using minas.teste.prototype.MVVM.Model.Concrete;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

public class Bombas
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { get; set; }

    public DateTime CreateTime { get; set; } = DateTime.Now;
    public DateTime UpdateTime { get; set; } = DateTime.Now;
    public DateTime? TerminateTime { get; set; }

    // Propriedades técnicas
    public double? DrenoGPM { get; set; }
    public double? DrenoLPM { get; set; }
    public double? PilotagemPSI { get; set; }
    public double? PilotagemBAR { get; set; }
    public double? PressaoPSI { get; set; }
    public double? PressaoBAR { get; set; }
    public double? VazaoGPM { get; set; }
    public double? VazaoLPM { get; set; }
    public double? TemperaturaCelsius { get; set; }
    public double? RotacaoRPM { get; set; }

    // Chaves estrangeiras
    public int? SessaoID { get; set; }
    public int? EtapaID { get; set; }

    // Propriedades de navegação
    public virtual Sessao Sessao { get; set; }
    public virtual Etapa Etapa { get; set; }
}