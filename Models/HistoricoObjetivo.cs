using System.ComponentModel.DataAnnotations;
namespace MetaMais.Models;

public class HistoricoObjetivo
{
    public string Descricao { get; set; } = "";
    public DateTime DataInclusao { get; set; }
}

