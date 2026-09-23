using MetaMais.Models;
using MetaMais.ViewModels;

namespace MetaMais.Interfaces.Repositories;

public interface IFinanceiroRepository
{
    Task<Usuario?> UsuarioAsync(string email);
    Task<Usuario?> UsuarioAsync(int id);
    Task CriarUsuarioAsync(Usuario usuario);
    Task PerfilAsync(Usuario u, string? hash);
    Task<List<Objetivo>> ObjetivosAsync(int usuarioId);
    Task<Objetivo?> ObjetivoAsync(int id,int usuarioId);
    Task<int> SalvarObjetivoAsync(Objetivo o);
    Task<string> ContribuirAsync(Contribuicao deposito,int usuarioId);
    Task<List<Contribuicao>> ContribuicoesAsync(int usuarioId,int? objetivoId=null);
    Task<List<HistoricoObjetivo>> HistoricoAsync(int id,int usuarioId);
    Task<List<Lancamento>> LancamentosAsync(string tipo,int usuarioId);
    Task SalvarLancamentoAsync(string tipo,Lancamento l);
    Task<List<Divida>> DividasAsync(int usuarioId);
    Task SalvarDividaAsync(Divida d);
    Task ExcluirAsync(string tipo,int id,int usuarioId);
    Task<List<string>> NotificacoesAsync(int usuarioId);
    Task LerNotificacoesAsync(int usuarioId);
}
