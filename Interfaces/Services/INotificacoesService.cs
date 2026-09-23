using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Interfaces.Services;

public interface INotificacoesService
{
    Task<NotificacoesViewModel> ObterAsync(int usuarioId);
    Task MarcarLidasAsync(int usuarioId);
}
