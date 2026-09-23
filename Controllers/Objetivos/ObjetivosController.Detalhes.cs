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
    public async Task<IActionResult> Detalhes(int id) { var o = await detalhes.ObterAsync(UsuarioId, id); return o is null ? NotFound() : View("~/Views/Objetivos/Detalhes/Index.cshtml", o); }
}
