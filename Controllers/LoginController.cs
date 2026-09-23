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

public class LoginController(ILoginService login) : Controller
{
    [HttpGet("/Login")] public IActionResult Login() { ViewBag.BancoDisponivel = login.BancoDisponivel; return View("Index", new LoginViewModel()); }
    [HttpPost("/Login"), EnableRateLimiting("login")]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        ViewBag.BancoDisponivel = login.BancoDisponivel;
        if (!login.BancoDisponivel) ModelState.AddModelError("", "Configure o MySQL e reinicie a aplicação para entrar.");
        if (!ModelState.IsValid) return View("Index", model);
        var usuario = await login.AutenticarAsync(model.Email, model.Senha);
        if (usuario is null) { ModelState.AddModelError("", "E-mail ou senha inválidos."); return View("Index", model); }
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), new Claim(ClaimTypes.Name, usuario.Nome)], CookieAuthenticationDefaults.AuthenticationScheme)), new AuthenticationProperties { IsPersistent = model.LembrarMe });
        if (usuario.TrocarSenha) TempData["Aviso"] = "Bem-vindo! Recomendamos alterar a senha inicial em Meu perfil.";
        return RedirectToAction("Index", "Dashboard");
    }
    [HttpPost("/Sair"), Authorize] public async Task<IActionResult> Sair() { await HttpContext.SignOutAsync(); return Redirect("/Login"); }
}
