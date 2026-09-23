using MetaMais.ViewModels;
using MetaMais.Interfaces.Services;
using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Repositories;
using MetaMais.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MetaMais.Controllers;

[Authorize]
public class SimuladorController(ISimuladorService simulador) : Controller
{
    private int UsuarioId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    [HttpGet("/Simulador")] public IActionResult Simulador() => View("Index", new SimuladorViewModel());
    [HttpPost("/Simulador/Inverso")]
    public IActionResult Inverso(decimal valorMensal, int meses)
    {
        var resultado = simulador.Simular(valorMensal, meses);
        if (resultado is null) { TempData["Aviso"] = "Informe um valor positivo e um prazo entre 1 e 1200 meses."; return RedirectToAction(nameof(Simulador)); }
        return View("Index", new SimuladorViewModel(resultado));
    }
}
