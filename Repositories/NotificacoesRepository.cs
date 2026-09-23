using Dapper;
using MetaMais.Models;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Repositories;

public class NotificacoesRepository(Banco banco) : INotificacoesRepository
{
    public async Task<List<string>> NotificacoesAsync(int usuarioId) { await using var c = banco.Abrir(); return (await c.QueryAsync<string>("SELECT Mensagem FROM notificacoes WHERE UsuarioId=@usuarioId AND Lida=0 ORDER BY Id DESC LIMIT 20", new { usuarioId })).ToList(); }
    public async Task LerNotificacoesAsync(int usuarioId) { await using var c = banco.Abrir(); await c.ExecuteAsync("UPDATE notificacoes SET Lida=1 WHERE UsuarioId=@usuarioId", new { usuarioId }); }

}
