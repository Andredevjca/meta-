using System.ComponentModel.DataAnnotations;
namespace MetaMais.ViewModels;

public class PerfilViewModel
{
    [Required, StringLength(100)] public string Nome { get; set; } = "";
    public string Email { get; set; } = "";
    [StringLength(30)] public string? Telefone { get; set; }
    public DateTime? DataNascimento { get; set; }
    public string? SenhaAtual { get; set; }
    [StringLength(72, MinimumLength = 8)] public string? NovaSenha { get; set; }
    [Compare(nameof(NovaSenha), ErrorMessage = "Confirme a nova senha.")] public string? ConfirmacaoSenha { get; set; }
}
