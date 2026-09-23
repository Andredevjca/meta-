using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
namespace MetaMais.Services;

public class LoginService(IUsuariosRepository usuarios, Banco banco) : ILoginService
{
    public bool BancoDisponivel => banco.Disponivel;
    public async Task<Usuario?> AutenticarAsync(string email, string senha)
    {
        var usuario = await usuarios.UsuarioAsync(email.Trim());
        return usuario is not null && BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash) ? usuario : null;
    }
}
