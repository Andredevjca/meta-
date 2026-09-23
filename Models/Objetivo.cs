using System.ComponentModel.DataAnnotations;
namespace MetaMais.Models;

public class Objetivo : IValidatableObject
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    [Required(ErrorMessage = "Informe o nome."), StringLength(100)] public string Nome { get; set; } = "";
    [StringLength(1000)] public string? Descricao { get; set; }
    [Required, StringLength(50)] public string Categoria { get; set; } = "Outro";
    [RegularExpression("fa-(plane|house|car|graduation-cap|piggy-bank|bag-shopping|laptop|bullseye)")] public string Icone { get; set; } = "fa-bullseye";
    [Range(typeof(decimal), "0.01", "999999999999.99", ParseLimitsInInvariantCulture = true)] public decimal ValorObjetivo { get; set; }
    [Range(typeof(decimal), "0", "999999999999.99", ParseLimitsInInvariantCulture = true)] public decimal ValorInicial { get; set; }
    [DataType(DataType.Date)] public DateTime DataInicio { get; set; } = DateTime.Today;
    [DataType(DataType.Date)] public DateTime DataLimite { get; set; } = DateTime.Today.AddYears(1);
    [RegularExpression("Semanal|Quinzenal|Mensal")] public string Frequencia { get; set; } = "Mensal";
    [RegularExpression("Ativo|Pausado|Cancelado|Concluído")] public string Status { get; set; } = "Ativo";
    [Range(1, 3)] public int Prioridade { get; set; } = 2;
    public decimal TotalContribuido { get; set; }
    public DateTime? UltimaContribuicao { get; set; }
    public DateTime? DataConclusao { get; set; }
    public decimal ValorAcumulado => ValorInicial + TotalContribuido;
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (DataLimite.Date < DataInicio.Date) yield return new("O prazo deve ser posterior à data inicial.", [nameof(DataLimite)]);
        if (DataLimite.Year > DateTime.Today.Year + 100 || DataInicio.Year < 1900) yield return new("Informe datas entre 1900 e os próximos 100 anos.");
    }
}
