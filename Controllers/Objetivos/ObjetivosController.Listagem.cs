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
    public async Task<IActionResult> Index() => View("~/Views/Objetivos/Listagem/Index.cshtml", await listagem.ObterAsync(UsuarioId));
    [HttpPost] public async Task<IActionResult> Excluir(int id) { await listagem.ExcluirAsync(UsuarioId, id); TempData["Sucesso"] = "Objetivo excluído."; return RedirectToAction(nameof(Index)); }
}
