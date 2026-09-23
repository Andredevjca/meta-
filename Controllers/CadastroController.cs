using MetaMais.Interfaces.Services;
using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Models;
using MetaMais.Repositories;
using MetaMais.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MySqlConnector;
namespace MetaMais.Controllers;

public class CadastroController(ICadastroService cadastro) : Controller
{
    [HttpGet("/Cadastro")] public IActionResult Cadastro() => View("Index", new CadastroViewModel());
    [HttpPost("/Cadastro"), EnableRateLimiting("login")]
    public async Task<IActionResult> Cadastro(CadastroViewModel model)
    {
        if (!cadastro.BancoDisponivel) ModelState.AddModelError("", "O banco de dados ainda não está disponível.");
        if (!ModelState.IsValid) return View("Index", model);
        if (!await cadastro.CadastrarAsync(model)) { ModelState.AddModelError("Email", "Este e-mail já está cadastrado."); return View("Index", model); }
        TempData["Sucesso"] = "Conta criada. Entre para começar seu planejamento."; return Redirect("/Login");
    }
}
