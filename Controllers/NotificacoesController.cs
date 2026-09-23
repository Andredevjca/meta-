using MetaMais.Interfaces.Services;
using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Repositories;
using MetaMais.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MetaMais.Controllers;

[Authorize]
public class NotificacoesController(INotificacoesService notificacoes) : Controller
{
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet("/Notificacoes")] public async Task<IActionResult> Notificacoes() { return View("Index", await notificacoes.ObterAsync(UsuarioId)); }
    [HttpPost("/Notificacoes/Ler")] public async Task<IActionResult> Ler() { await notificacoes.MarcarLidasAsync(UsuarioId); return RedirectToAction(nameof(Notificacoes)); }
}
