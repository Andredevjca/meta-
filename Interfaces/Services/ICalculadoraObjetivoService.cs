using MetaMais.Models;
using MetaMais.ViewModels;

namespace MetaMais.Interfaces.Services;

public interface ICalculadoraObjetivoService
{
    List<DateTime> DatasRestantes(Objetivo objetivo, DateTime hoje, string frequencia);
    CalculoObjetivo Calcular(Objetivo objetivo, DateTime? referencia = null);
}
