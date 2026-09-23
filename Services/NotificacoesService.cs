using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Services;

public class NotificacoesService(IPlanejamentoFinanceiroService planejamento, INotificacoesRepository repositorio) : INotificacoesService
{
    public async Task<NotificacoesViewModel> ObterAsync(int usuarioId) => new(await planejamento.ObterAsync(usuarioId), await repositorio.NotificacoesAsync(usuarioId));
    public Task MarcarLidasAsync(int usuarioId) => repositorio.LerNotificacoesAsync(usuarioId);
}
