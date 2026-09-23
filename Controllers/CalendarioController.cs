using MetaMais.Interfaces.Services;
using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Repositories;
using MetaMais.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MetaMais.Controllers;

[Authorize]
public class CalendarioController(ICalendarioService calendario) : Controller
{
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet("/Calendario")] public async Task<IActionResult> Calendario(int? ano, int? mes) { var model = await calendario.ObterAsync(UsuarioId, ano, mes); return model is null ? BadRequest() : View("Index", model); }
}
