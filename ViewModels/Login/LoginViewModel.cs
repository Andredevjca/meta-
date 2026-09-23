using System.ComponentModel.DataAnnotations;
using MetaMais.Models;
namespace MetaMais.ViewModels;

public class LoginViewModel
{
    [Required, EmailAddress] public string Email { get; set; } = "";
    [Required, StringLength(72), DataType(DataType.Password)] public string Senha { get; set; } = "";
    public bool LembrarMe { get; set; }
}
