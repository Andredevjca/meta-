using MetaMais.Models;
namespace MetaMais.Interfaces.Repositories;

public interface INotificacoesRepository
{
    Task<List<string>> NotificacoesAsync(int usuarioId);
    Task LerNotificacoesAsync(int usuarioId);
}
