using MetaMais.Interfaces.Services;
using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Models;
using MetaMais.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MetaMais.Controllers;

[Authorize]
public class DividasController(IDividasService servico) : Controller
{
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpPost("/Dividas/Pagar/{id:int}")]
    public async Task<IActionResult> PagarParcela(int id, int parcelasPagas)
    {
        if (!ModelState.IsValid || parcelasPagas < 0) return BadRequest();
        var pagou = await servico.PagarParcelaAsync(UsuarioId, id, parcelasPagas);
        TempData[pagou ? "Sucesso" : "AvisoDivida"] = pagou
            ? "Parcela paga. Dívida atualizada."
            : "A dívida já foi atualizada ou não possui parcelas em aberto. Confira o card.";
        return RedirectToAction(nameof(Dividas));
    }
    [HttpGet("/Dividas")] public async Task<IActionResult> Dividas() => View("Index", await servico.ListarAsync(UsuarioId));
    [HttpGet("/Dividas/Editar/{id:int?}")]
    public async Task<IActionResult> EditarDivida(int id = 0) { var d = await servico.ObterAsync(UsuarioId, id); return d is null ? NotFound() : View("Editar", d); }
    [HttpPost("/Dividas/Salvar")]
    public async Task<IActionResult> SalvarDivida(Divida model) { if (!ModelState.IsValid) return View("Editar", model); await servico.SalvarAsync(UsuarioId, model); TempData["Sucesso"] = "Dívida atualizada."; return RedirectToAction(nameof(Dividas)); }
    [HttpPost("/Dividas/Excluir/{id:int}")]
    public async Task<IActionResult> ExcluirDivida(int id) { await servico.ExcluirAsync(UsuarioId, id); return RedirectToAction(nameof(Dividas)); }
}
