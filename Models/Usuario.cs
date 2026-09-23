using System.ComponentModel.DataAnnotations;
namespace MetaMais.Models;

public class Usuario
{
    public int Id { get; set; }
    [Required, StringLength(100)] public string Nome { get; set; } = "";
    [Required, EmailAddress, StringLength(190)] public string Email { get; set; } = "";
    public string SenhaHash { get; set; } = "";
    [StringLength(30)] public string? Telefone { get; set; }
    public DateTime? DataNascimento { get; set; }
    public bool TrocarSenha { get; set; }
}
