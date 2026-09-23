using MetaMais.Interfaces.Services;
using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Models;
using MetaMais.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MetaMais.Controllers;

[Authorize]
public class ReceitasController(IReceitasService servico) : Controller
{
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private const string tipo = "Receitas";
    [HttpGet("/Receitas")]
    public async Task<IActionResult> Index() { ViewBag.Tipo = tipo; return View(await servico.ListarAsync(UsuarioId)); }
    [HttpGet("/Receitas/Editar/{id:int?}")]
    public async Task<IActionResult> Editar(int id = 0) { ViewBag.Tipo = tipo; var l = await servico.ObterAsync(UsuarioId, id); return l is null ? NotFound() : View(l); }
    [HttpPost("/Receitas/Salvar")]
    public async Task<IActionResult> Salvar(Lancamento model) { ViewBag.Tipo = tipo; if (!ModelState.IsValid) return View("Editar", model); await servico.SalvarAsync(UsuarioId, model); TempData["Sucesso"] = "Lançamento salvo."; return Redirect("/" + tipo); }
    [HttpPost("/Receitas/Excluir/{id:int}")]
    public async Task<IActionResult> Excluir(int id) { await servico.ExcluirAsync(UsuarioId, id); TempData["Sucesso"] = "Lançamento excluído."; return Redirect("/" + tipo); }
}
