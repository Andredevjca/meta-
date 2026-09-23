using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
namespace MetaMais.Services;

public class SimuladorService : ISimuladorService
{
    public decimal? Simular(decimal valorMensal, int meses) =>
     valorMensal <= 0 || valorMensal > 999999999999.99m || meses < 1 || meses > 1200 ? null : valorMensal * meses;
}
