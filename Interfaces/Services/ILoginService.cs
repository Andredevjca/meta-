using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
namespace MetaMais.Interfaces.Services;

public interface ILoginService
{
    bool BancoDisponivel { get; }
    Task<Usuario?> AutenticarAsync(string email, string senha);
}
