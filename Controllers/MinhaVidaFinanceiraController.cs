using MetaMais.Interfaces.Services;
using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Repositories;
using MetaMais.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MetaMais.Controllers;

[Authorize]
public class MinhaVidaFinanceiraController(IPlanejamentoFinanceiroService planejamento) : Controller
{
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet("/MinhaVidaFinanceira")] public async Task<IActionResult> Vida() => View("Index", await planejamento.ObterAsync(UsuarioId));
}
