using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Interfaces.Services;

public interface IContribuicaoObjetivoService
{
    Task<ObjetivoPlanejado?> ObterAsync(int usuarioId, int id);
    Task<string> ContribuirAsync(int usuarioId, Contribuicao model);
}
