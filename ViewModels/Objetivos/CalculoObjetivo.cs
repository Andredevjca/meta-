using System.ComponentModel.DataAnnotations;
using MetaMais.Models;
namespace MetaMais.ViewModels;

public record CalculoObjetivo(decimal Restante, decimal Percentual, int Periodos, decimal PorPeriodo, decimal Mensal, decimal Quinzenal, decimal Semanal, DateTime? ProximoCheckin, bool Atrasado, DateTime? Previsao);
