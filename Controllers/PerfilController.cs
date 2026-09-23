using System.Security.Claims;
using MetaMais.Interfaces.Services;
using MetaMais.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MetaMais.Controllers;

[Authorize]
public class PerfilController(IPerfilService perfil) : Controller
{
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet("/Perfil")]
    public async Task<IActionResult> Perfil() { var model = await perfil.ObterAsync(UsuarioId); return model is null ? NotFound() : View("Index", model); }
    [HttpPost("/Perfil")]
    public async Task<IActionResult> Perfil(PerfilViewModel model)
    {
        var atual = await perfil.ObterAsync(UsuarioId); if (atual is null) return NotFound();
        model.Email = atual.Email;
        if (!ModelState.IsValid) return View("Index", model);
        var erro = await perfil.AtualizarAsync(UsuarioId, model);
        if (erro is not null) { ModelState.AddModelError("", erro); return View("Index", model); }
        TempData["Sucesso"] = "Perfil atualizado."; return RedirectToAction(nameof(Perfil));
    }
}
