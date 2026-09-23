using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Interfaces.Services;

public interface IRelatoriosService
{
    Task<RelatoriosViewModel> ObterAsync(int usuarioId);
}
