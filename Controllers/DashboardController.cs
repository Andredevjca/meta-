using MetaMais.Interfaces.Services;
using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Repositories;
using MetaMais.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MetaMais.Controllers;
[Authorize]
public class DashboardController(IPlanejamentoFinanceiroService planejamento,IFinanceiroRepository repositorio):Controller
{
 private int UsuarioId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 public async Task<IActionResult> Index()=>View(await planejamento.ObterAsync(UsuarioId));
 [HttpGet("/MinhaVidaFinanceira")] public async Task<IActionResult> Vida()=>View("Vida",await planejamento.ObterAsync(UsuarioId));
 [HttpGet("/Contribuicoes")] public async Task<IActionResult> Contribuicoes()=>View(await repositorio.ContribuicoesAsync(UsuarioId));
 [HttpGet("/Calendario")] public async Task<IActionResult> Calendario(int? ano,int? mes) {var a=ano??DateTime.Today.Year;var m=mes??DateTime.Today.Month;if(a<1900||a>2200||m<1||m>12)return BadRequest();ViewBag.Mes=new DateTime(a,m,1);return View(await planejamento.ObterAsync(UsuarioId));}
 [HttpGet("/Relatorios")] public async Task<IActionResult> Relatorios()=>View(await planejamento.ObterAsync(UsuarioId));
 [HttpGet("/Simulador")] public IActionResult Simulador()=>View();
 [HttpPost("/Simulador/Inverso")] public IActionResult Inverso(decimal valorMensal,int meses) {
  if(valorMensal<=0 || valorMensal>999999999999.99m || meses<1 || meses>1200) {TempData["Aviso"]="Informe um valor positivo e um prazo entre 1 e 1200 meses.";return RedirectToAction(nameof(Simulador));}
  ViewBag.Resultado=valorMensal*meses;return View("Simulador");
 }
 [HttpGet("/Notificacoes")] public async Task<IActionResult> Notificacoes() {ViewBag.Painel=await planejamento.ObterAsync(UsuarioId);return View(await repositorio.NotificacoesAsync(UsuarioId));}
 [HttpPost("/Notificacoes/Ler")] public async Task<IActionResult> Ler(){await repositorio.LerNotificacoesAsync(UsuarioId);return RedirectToAction(nameof(Notificacoes));}
 [HttpGet("/Configuracoes")] public IActionResult Configuracoes()=>View();
}
