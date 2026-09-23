using System.ComponentModel.DataAnnotations;
using MetaMais.Models;
namespace MetaMais.ViewModels;

public record DetalheObjetivoViewModel(ObjetivoPlanejado Planejado, List<Contribuicao> Contribuicoes, List<HistoricoObjetivo> Historico);
