using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
namespace MetaMais.Services;

public class CadastroService(IUsuariosRepository usuarios, Banco banco) : ICadastroService
{
    public bool BancoDisponivel => banco.Disponivel;
    public async Task<bool> CadastrarAsync(CadastroViewModel model)
    {
        try
        {
            await usuarios.CriarUsuarioAsync(new Usuario { Nome = model.Nome.Trim(), Email = model.Email.Trim(), SenhaHash = BCrypt.Net.BCrypt.HashPassword(model.Senha, 12) });
            return true;
        }
        catch (MySqlConnector.MySqlException ex) when (ex.Number == 1062) { return false; }
    }
}
