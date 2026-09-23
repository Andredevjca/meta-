using MetaMais.Interfaces.Services;
using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Models;
using MetaMais.Repositories;
using MetaMais.Services;
using MetaMais.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MetaMais.Controllers;

public partial class ObjetivosController
{
    public IActionResult Novo() => View("~/Views/Objetivos/Formulario/Index.cshtml", new Objetivo());
    public async Task<IActionResult> Editar(int id) { var o = await formulario.ObterAsync(UsuarioId, id); return o is null ? NotFound() : View("~/Views/Objetivos/Formulario/Index.cshtml", o); }
    [HttpPost]
    public async Task<IActionResult> Salvar(Objetivo objetivo)
    {
        if (objetivo.Id != 0 && await formulario.ObterAsync(UsuarioId, objetivo.Id) is null) return NotFound();
        if (formulario.ValidarPrazo(objetivo) is string erro) ModelState.AddModelError("DataLimite", erro);
        if (!ModelState.IsValid) return View("~/Views/Objetivos/Formulario/Index.cshtml", objetivo);
        var id = await formulario.SalvarAsync(UsuarioId, objetivo); TempData["Sucesso"] = "Objetivo salvo. Seu planejamento está atualizado."; return RedirectToAction(nameof(Detalhes), new { id });
    }
    [HttpPost]
    public async Task<IActionResult> Simular(Objetivo model)
    {
        if (!ModelState.IsValid) return BadRequest(new { mensagem = "Confira os valores e as datas da simulação." });
        var calculo = await formulario.SimularAsync(UsuarioId, model);
        return calculo is null ? NotFound() : Json(calculo);
    }
}
