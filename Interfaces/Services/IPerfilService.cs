using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
namespace MetaMais.Interfaces.Services;

public interface IPerfilService
{
    Task<PerfilViewModel?> ObterAsync(int usuarioId);
    Task<string?> AtualizarAsync(int usuarioId, PerfilViewModel model);
}
