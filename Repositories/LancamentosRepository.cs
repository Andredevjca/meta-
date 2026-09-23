using Dapper;
using MetaMais.Models;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Repositories;

public class LancamentosRepository(Banco banco) : ILancamentosRepository
{
    private static string Tabela(string tipo) => tipo switch { "Receitas" => "receitas", "Despesas" => "despesas", _ => throw new ArgumentException("Tipo inválido") };
    public async Task<List<Lancamento>> LancamentosAsync(string tipo, int usuarioId) { await using var c = banco.Abrir(); return (await c.QueryAsync<Lancamento>($"SELECT * FROM {Tabela(tipo)} WHERE UsuarioId=@usuarioId ORDER BY Data DESC", new { usuarioId })).ToList(); }
    public async Task SalvarLancamentoAsync(string tipo, Lancamento l) { await using var c = banco.Abrir(); await c.ExecuteAsync(l.Id == 0 ? $"INSERT INTO {Tabela(tipo)} (UsuarioId,Descricao,Categoria,Valor,Periodicidade,Data,Ativo,Fixa) VALUES (@UsuarioId,@Descricao,@Categoria,@Valor,@Periodicidade,@Data,@Ativo,@Fixa)" : $"UPDATE {Tabela(tipo)} SET Descricao=@Descricao,Categoria=@Categoria,Valor=@Valor,Periodicidade=@Periodicidade,Data=@Data,Ativo=@Ativo,Fixa=@Fixa WHERE Id=@Id AND UsuarioId=@UsuarioId", l); }
    public async Task ExcluirAsync(string tipo, int id, int usuarioId) { await using var c = banco.Abrir(); await c.ExecuteAsync($"DELETE FROM {Tabela(tipo)} WHERE Id=@id AND UsuarioId=@usuarioId", new { id, usuarioId }); }

}
