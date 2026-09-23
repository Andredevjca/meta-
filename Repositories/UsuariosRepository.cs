using Dapper;
using MetaMais.Models;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Repositories;

public class UsuariosRepository(Banco banco) : IUsuariosRepository
{
    public async Task<Usuario?> UsuarioAsync(string email) { await using var c = banco.Abrir(); return await c.QuerySingleOrDefaultAsync<Usuario>("SELECT * FROM usuarios WHERE Email=@email", new { email }); }
    public async Task<Usuario?> UsuarioAsync(int id) { await using var c = banco.Abrir(); return await c.QuerySingleOrDefaultAsync<Usuario>("SELECT * FROM usuarios WHERE Id=@id", new { id }); }
    public async Task CriarUsuarioAsync(Usuario usuario)
    {
        await using var c = banco.Abrir(); await c.OpenAsync(); await using var t = await c.BeginTransactionAsync();
        var id = await c.ExecuteScalarAsync<int>("INSERT INTO usuarios (Nome,Email,SenhaHash) VALUES (@Nome,@Email,@SenhaHash); SELECT LAST_INSERT_ID();", usuario, t);
        await c.ExecuteAsync("INSERT INTO configuracoes_usuario (UsuarioId) VALUES (@id)", new { id }, t); await t.CommitAsync();
    }
    public async Task PerfilAsync(Usuario u, string? hash) { await using var c = banco.Abrir(); await c.ExecuteAsync("UPDATE usuarios SET Nome=@Nome,Telefone=@Telefone,DataNascimento=@DataNascimento,SenhaHash=COALESCE(@hash,SenhaHash),TrocarSenha=IF(@hash IS NULL,TrocarSenha,0) WHERE Id=@Id", new { u.Nome, u.Telefone, u.DataNascimento, u.Id, hash }); }

}
