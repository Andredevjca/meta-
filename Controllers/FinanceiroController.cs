using MetaMais.Interfaces.Repositories;
using System.Security.Claims;
using MetaMais.Models;
using MetaMais.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace MetaMais.Controllers;
[Authorize]
public class FinanceiroController(IFinanceiroRepository repositorio):Controller
{
 private int UsuarioId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 private static bool Valido(string tipo)=>tipo is "Receitas" or "Despesas";
 [HttpGet("/{tipo:regex(^Receitas$|^Despesas$)}")]
 public async Task<IActionResult> Index(string tipo) {if(!Valido(tipo))return NotFound();ViewBag.Tipo=tipo;return View(await repositorio.LancamentosAsync(tipo,UsuarioId));}
 [HttpGet("/{tipo:regex(^Receitas$|^Despesas$)}/Editar/{id:int?}")]
 public async Task<IActionResult> Editar(string tipo,int id=0) {if(!Valido(tipo))return NotFound();ViewBag.Tipo=tipo;var l=id==0?new Lancamento():(await repositorio.LancamentosAsync(tipo,UsuarioId)).FirstOrDefault(x=>x.Id==id);return l is null?NotFound():View(l);}
 [HttpPost("/{tipo:regex(^Receitas$|^Despesas$)}/Salvar")]
 public async Task<IActionResult> Salvar(string tipo,Lancamento model) {if(!Valido(tipo))return NotFound();model.UsuarioId=UsuarioId;ViewBag.Tipo=tipo;if(!ModelState.IsValid)return View("Editar",model);await repositorio.SalvarLancamentoAsync(tipo,model);TempData["Sucesso"]="Lançamento salvo.";return Redirect("/"+tipo);}
 [HttpPost("/{tipo:regex(^Receitas$|^Despesas$)}/Excluir/{id:int}")]
 public async Task<IActionResult> Excluir(string tipo,int id) {if(!Valido(tipo))return NotFound();await repositorio.ExcluirAsync(tipo,id,UsuarioId);TempData["Sucesso"]="Lançamento excluído.";return Redirect("/"+tipo);}
 [HttpGet("/Dividas")] public async Task<IActionResult> Dividas()=>View(await repositorio.DividasAsync(UsuarioId));
 [HttpGet("/Dividas/Editar/{id:int?}")]
 public async Task<IActionResult> EditarDivida(int id=0) {var d=id==0?new Divida():(await repositorio.DividasAsync(UsuarioId)).FirstOrDefault(x=>x.Id==id);return d is null?NotFound():View(d);}
 [HttpPost("/Dividas/Salvar")]
 public async Task<IActionResult> SalvarDivida(Divida model) {model.UsuarioId=UsuarioId;if(!ModelState.IsValid)return View("EditarDivida",model);await repositorio.SalvarDividaAsync(model);TempData["Sucesso"]="Dívida atualizada.";return RedirectToAction(nameof(Dividas));}
 [HttpPost("/Dividas/Excluir/{id:int}")]
 public async Task<IActionResult> ExcluirDivida(int id) {await repositorio.ExcluirAsync("Dividas",id,UsuarioId);return RedirectToAction(nameof(Dividas));}
}
