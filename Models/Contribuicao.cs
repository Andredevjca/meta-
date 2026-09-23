using System.ComponentModel.DataAnnotations;
namespace MetaMais.Models;

public class Contribuicao
{
    public int Id { get; set; }
    public int ObjetivoId { get; set; }
    public string NomeObjetivo { get; set; } = "";
    [Range(typeof(decimal), "0.01", "999999999999.99", ParseLimitsInInvariantCulture = true)] public decimal Valor { get; set; }
    [DataType(DataType.Date)] public DateTime Data { get; set; } = DateTime.Today;
    [StringLength(500)] public string? Observacao { get; set; }
}
