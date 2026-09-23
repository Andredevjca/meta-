using System.ComponentModel.DataAnnotations;
namespace MetaMais.Models;

public class Lancamento
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    [Required, StringLength(150)] public string Descricao { get; set; } = "";
    [Required, StringLength(50)] public string Categoria { get; set; } = "Outros";
    [Range(typeof(decimal), "0.01", "999999999999.99", ParseLimitsInInvariantCulture = true)] public decimal Valor { get; set; }
    [RegularExpression("Mensal|Semanal|Quinzenal|Eventual")] public string Periodicidade { get; set; } = "Mensal";
    [DataType(DataType.Date)] public DateTime Data { get; set; } = DateTime.Today;
    public bool Ativo { get; set; } = true;
    public bool Fixa { get; set; } = true;
}
