using MetaMais.Models;
namespace MetaMais.Interfaces.Repositories;

public interface IUsuariosRepository
{
    Task<Usuario?> UsuarioAsync(string email);
    Task<Usuario?> UsuarioAsync(int id);
    Task CriarUsuarioAsync(Usuario usuario);
    Task PerfilAsync(Usuario u, string? hash);
}
