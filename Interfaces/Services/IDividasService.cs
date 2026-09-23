using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Interfaces.Services;

public interface IDividasService
{
    Task<List<Divida>> ListarAsync(int usuarioId);
    Task<Divida?> ObterAsync(int usuarioId, int id);
    Task SalvarAsync(int usuarioId, Divida model);
    Task ExcluirAsync(int usuarioId, int id);
}
