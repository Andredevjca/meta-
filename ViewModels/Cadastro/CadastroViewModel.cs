using System.ComponentModel.DataAnnotations;
using MetaMais.Models;
namespace MetaMais.ViewModels;

public class CadastroViewModel
{
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, StringLength(72, MinimumLength = 8), DataType(DataType.Password)]
    public string Senha { get; set; } = "";
    [Required, StringLength(100)] public string Nome { get; set; } = "";
    [Compare(nameof(Senha), ErrorMessage = "As senhas não coincidem.")] public string ConfirmacaoSenha { get; set; } = "";
}
