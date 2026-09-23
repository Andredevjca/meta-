using System.ComponentModel.DataAnnotations;
namespace MetaMais.Models;

public class Divida : IValidatableObject
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    [Required, StringLength(150)] public string Descricao { get; set; } = "";
    [Required, StringLength(100)] public string Credor { get; set; } = "";
    [Range(typeof(decimal), "0.01", "999999999999.99", ParseLimitsInInvariantCulture = true)] public decimal ValorTotal { get; set; }
    [Range(typeof(decimal), "0.01", "999999999999.99", ParseLimitsInInvariantCulture = true)] public decimal ValorParcela { get; set; }
    [Range(1, 1200)] public int QuantidadeParcelas { get; set; } = 12;
    [Range(0, 1200)] public int ParcelasPagas { get; set; }
    [DataType(DataType.Date)] public DateTime DataInicio { get; set; } = DateTime.Today;
    public decimal ValorRestante => Math.Max(0, ValorTotal - ParcelasPagas * ValorParcela);
    public int ParcelasRestantes => Math.Max(0, QuantidadeParcelas - ParcelasPagas);
    public DateTime DataFinal => DataInicio.AddMonths(QuantidadeParcelas - 1);
    public string Status => ParcelasRestantes == 0 || ValorRestante == 0 ? "Quitada" : "Ativa";
    public IEnumerable<ValidationResult> Validate(ValidationContext context)
    {
        if (ParcelasPagas > QuantidadeParcelas) yield return new("As parcelas pagas não podem superar o total.");
        if (DataInicio.Year < 1900 || DataInicio.Year > 2100) yield return new("Informe uma data entre 1900 e 2100.");
    }
}
