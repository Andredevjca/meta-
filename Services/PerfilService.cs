using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
namespace MetaMais.Services;

public class PerfilService(IUsuariosRepository usuarios) : IPerfilService
{
    public async Task<PerfilViewModel?> ObterAsync(int usuarioId)
    {
        var usuario = await usuarios.UsuarioAsync(usuarioId);
        return usuario is null ? null : new PerfilViewModel { Nome = usuario.Nome, Email = usuario.Email, Telefone = usuario.Telefone, DataNascimento = usuario.DataNascimento };
    }
    public async Task<string?> AtualizarAsync(int usuarioId, PerfilViewModel model)
    {
        var usuario = await usuarios.UsuarioAsync(usuarioId);
        if (usuario is null) return "Usuário não encontrado.";
        model.Email = usuario.Email;
        string? hash = null;
        if (!string.IsNullOrEmpty(model.NovaSenha))
        {
            if (model.NovaSenha.Length < 8 || model.NovaSenha.Length > 72 || model.NovaSenha != model.ConfirmacaoSenha) return "Use 8 a 72 caracteres e confirme a nova senha.";
            if (string.IsNullOrEmpty(model.SenhaAtual) || !BCrypt.Net.BCrypt.Verify(model.SenhaAtual, usuario.SenhaHash)) return "A senha atual está incorreta.";
            hash = BCrypt.Net.BCrypt.HashPassword(model.NovaSenha, 12);
        }
        usuario.Nome = model.Nome; usuario.Telefone = model.Telefone; usuario.DataNascimento = model.DataNascimento;
        await usuarios.PerfilAsync(usuario, hash);
        return null;
    }
}
