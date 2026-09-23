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
    [HttpGet] public async Task<IActionResult> Contribuicao(int id) { var o = await contribuicao.ObterAsync(UsuarioId, id); if (o is null) return NotFound(); ViewBag.Objetivo = o; return View("~/Views/Objetivos/Contribuicao/Index.cshtml", new Contribuicao { ObjetivoId = id }); }
    [HttpPost]
    public async Task<IActionResult> Contribuicao(Contribuicao model)
    {
        var o = await contribuicao.ObterAsync(UsuarioId, model.ObjetivoId); if (o is null) return NotFound();
        if (ModelState.IsValid)
        {
            try { TempData["Sucesso"] = await contribuicao.ContribuirAsync(UsuarioId, model); return RedirectToAction(nameof(Detalhes), new { id = model.ObjetivoId }); }
            catch (InvalidOperationException e) { ModelState.AddModelError("", e.Message); }
        }
        ViewBag.Objetivo = o; return View("~/Views/Objetivos/Contribuicao/Index.cshtml", model);
    }
}
