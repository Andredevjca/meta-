using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
namespace MetaMais.Interfaces.Services;

public interface ICadastroService
{
    bool BancoDisponivel { get; }
    Task<bool> CadastrarAsync(CadastroViewModel model);
}
