using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Interfaces.Services;

public interface IListaObjetivosService
{
    Task<List<ObjetivoPlanejado>> ObterAsync(int usuarioId);
    Task ExcluirAsync(int usuarioId, int id);
}
