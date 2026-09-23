using Dapper;
using MySqlConnector;
namespace MetaMais.Repositories;
public class Banco(IConfiguration configuracao, IWebHostEnvironment ambiente, ILogger<Banco> logger)
{
 public bool Disponivel { get; private set; }
 public MySqlConnection Abrir() => new(configuracao.GetConnectionString("MySql"));
 public async Task InicializarAsync()
 {
  try {
   var conexao = new MySqlConnectionStringBuilder(configuracao.GetConnectionString("MySql") ?? throw new InvalidOperationException("Configure a conexão MySql."));
   var nome = conexao.Database;
   if (!System.Text.RegularExpressions.Regex.IsMatch(nome, "^[a-zA-Z0-9_]+$")) throw new InvalidOperationException("Nome do banco inválido.");
   conexao.Database = "";
   await using (var servidor = new MySqlConnection(conexao.ConnectionString))
    await servidor.ExecuteAsync($"CREATE DATABASE IF NOT EXISTS `{nome}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci");
   await using var banco = Abrir();
   await banco.ExecuteAsync(await File.ReadAllTextAsync(Path.Combine(ambiente.ContentRootPath, "Queries", "Esquema.sql")));
   await banco.ExecuteAsync("INSERT IGNORE INTO usuarios (Nome,Email,SenhaHash,TrocarSenha) VALUES (@Nome,@Email,@SenhaHash,1)", new { Nome="Administrador", Email="admin@admin.com", SenhaHash=BCrypt.Net.BCrypt.HashPassword("admin",12) });
   await banco.ExecuteAsync("INSERT IGNORE INTO configuracoes_usuario (UsuarioId) SELECT Id FROM usuarios");
   Disponivel = true;
  } catch (Exception ex) when (ex is MySqlException or InvalidOperationException) {
   logger.LogError("MySQL indisponível. Configure ConnectionStrings:MySql e reinicie. Tipo: {Tipo}", ex.GetType().Name);
  }
 }
}
