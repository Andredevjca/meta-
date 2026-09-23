using System.ComponentModel.DataAnnotations;
namespace MetaMais.Models;
public class Usuario
{
 public int Id { get; set; }
 [Required, StringLength(100)] public string Nome { get; set; } = "";
 [Required, EmailAddress, StringLength(190)] public string Email { get; set; } = "";
 public string SenhaHash { get; set; } = "";
 [StringLength(30)] public string? Telefone { get; set; }
 public DateTime? DataNascimento { get; set; }
 public bool TrocarSenha { get; set; }
}
public class Objetivo : IValidatableObject
{
 public int Id { get; set; }
 public int UsuarioId { get; set; }
 [Required(ErrorMessage="Informe o nome."), StringLength(100)] public string Nome { get; set; } = "";
 [StringLength(1000)] public string? Descricao { get; set; }
 [Required, StringLength(50)] public string Categoria { get; set; } = "Outro";
 [RegularExpression("fa-(plane|house|car|graduation-cap|piggy-bank|bag-shopping|laptop|bullseye)")] public string Icone { get; set; } = "fa-bullseye";
 [Range(typeof(decimal), "0.01", "999999999999.99", ParseLimitsInInvariantCulture=true)] public decimal ValorObjetivo { get; set; }
 [Range(typeof(decimal), "0", "999999999999.99", ParseLimitsInInvariantCulture=true)] public decimal ValorInicial { get; set; }
 [DataType(DataType.Date)] public DateTime DataInicio { get; set; } = DateTime.Today;
 [DataType(DataType.Date)] public DateTime DataLimite { get; set; } = DateTime.Today.AddYears(1);
 [RegularExpression("Semanal|Quinzenal|Mensal")] public string Frequencia { get; set; } = "Mensal";
 [RegularExpression("Ativo|Pausado|Cancelado|Concluído")] public string Status { get; set; } = "Ativo";
 [Range(1,3)] public int Prioridade { get; set; } = 2;
 public decimal TotalContribuido { get; set; }
 public DateTime? UltimaContribuicao { get; set; }
 public DateTime? DataConclusao { get; set; }
 public decimal ValorAcumulado => ValorInicial + TotalContribuido;
 public IEnumerable<ValidationResult> Validate(ValidationContext context) {
  if (DataLimite.Date < DataInicio.Date) yield return new("O prazo deve ser posterior à data inicial.", [nameof(DataLimite)]);
  if (DataLimite.Year > DateTime.Today.Year + 100 || DataInicio.Year < 1900) yield return new("Informe datas entre 1900 e os próximos 100 anos.");
 }
}
public class Lancamento
{
 public int Id { get; set; }
 public int UsuarioId { get; set; }
 [Required, StringLength(150)] public string Descricao { get; set; } = "";
 [Required, StringLength(50)] public string Categoria { get; set; } = "Outros";
 [Range(typeof(decimal),"0.01","999999999999.99", ParseLimitsInInvariantCulture=true)] public decimal Valor { get; set; }
 [RegularExpression("Mensal|Semanal|Quinzenal|Eventual")] public string Periodicidade { get; set; } = "Mensal";
 [DataType(DataType.Date)] public DateTime Data { get; set; } = DateTime.Today;
 public bool Ativo { get; set; } = true;
 public bool Fixa { get; set; } = true;
}
public class Divida : IValidatableObject
{
 public int Id { get; set; }
 public int UsuarioId { get; set; }
 [Required, StringLength(150)] public string Descricao { get; set; } = "";
 [Required, StringLength(100)] public string Credor { get; set; } = "";
 [Range(typeof(decimal),"0.01","999999999999.99", ParseLimitsInInvariantCulture=true)] public decimal ValorTotal { get; set; }
 [Range(typeof(decimal),"0.01","999999999999.99", ParseLimitsInInvariantCulture=true)] public decimal ValorParcela { get; set; }
 [Range(1,1200)] public int QuantidadeParcelas { get; set; } = 12;
 [Range(0,1200)] public int ParcelasPagas { get; set; }
 [DataType(DataType.Date)] public DateTime DataInicio { get; set; } = DateTime.Today;
 public decimal ValorRestante => Math.Max(0, ValorTotal - ParcelasPagas * ValorParcela);
 public int ParcelasRestantes => Math.Max(0, QuantidadeParcelas - ParcelasPagas);
 public DateTime DataFinal => DataInicio.AddMonths(QuantidadeParcelas - 1);
 public string Status => ParcelasRestantes == 0 || ValorRestante == 0 ? "Quitada" : "Ativa";
 public IEnumerable<ValidationResult> Validate(ValidationContext context) {
  if (ParcelasPagas > QuantidadeParcelas) yield return new("As parcelas pagas não podem superar o total.");
  if (DataInicio.Year < 1900 || DataInicio.Year > 2100) yield return new("Informe uma data entre 1900 e 2100.");
 }
}
public class Contribuicao
{
 public int Id { get; set; }
 public int ObjetivoId { get; set; }
 public string NomeObjetivo { get; set; } = "";
 [Range(typeof(decimal),"0.01","999999999999.99", ParseLimitsInInvariantCulture=true)] public decimal Valor { get; set; }
 [DataType(DataType.Date)] public DateTime Data { get; set; } = DateTime.Today;
 [StringLength(500)] public string? Observacao { get; set; }
}
public class HistoricoObjetivo
{
 public string Descricao { get; set; } = "";
 public DateTime DataInclusao { get; set; }
}

