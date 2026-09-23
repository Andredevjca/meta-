using System.ComponentModel.DataAnnotations;
using MetaMais.Models;
namespace MetaMais.ViewModels;

public record ObjetivoPlanejado(Objetivo Objetivo, CalculoObjetivo Calculo);
