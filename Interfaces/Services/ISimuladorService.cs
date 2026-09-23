using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
namespace MetaMais.Interfaces.Services;

public interface ISimuladorService
{
    decimal? Simular(decimal valorMensal, int meses);
}
