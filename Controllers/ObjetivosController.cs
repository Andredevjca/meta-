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
[Authorize]
public class ObjetivosController(IFinanceiroRepository repositorio,ICalculadoraObjetivoService calculadora):Controller
{
 private int UsuarioId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
 public async Task<IActionResult> Index() => View((await repositorio.ObjetivosAsync(UsuarioId)).Select(o=>new ObjetivoPlanejado(o,calculadora.Calcular(o))).ToList());
 public IActionResult Novo() => View("Formulario",new Objetivo());
 public async Task<IActionResult> Editar(int id) { var o=await repositorio.ObjetivoAsync(id,UsuarioId); return o is null?NotFound():View("Formulario",o); }
 [HttpPost]
 public async Task<IActionResult> Salvar(Objetivo objetivo)
 {
  objetivo.UsuarioId=UsuarioId;
  if(objetivo.Id!=0 && await repositorio.ObjetivoAsync(objetivo.Id,UsuarioId) is null) return NotFound();
  if(objetivo.Id==0 && objetivo.DataLimite.Date<DateTime.Today) ModelState.AddModelError("DataLimite","Escolha um prazo a partir de hoje.");
  if(!ModelState.IsValid) return View("Formulario",objetivo);
  var id=await repositorio.SalvarObjetivoAsync(objetivo); TempData["Sucesso"]="Objetivo salvo. Seu planejamento está atualizado."; return RedirectToAction(nameof(Detalhes),new {id});
 }
 public async Task<IActionResult> Detalhes(int id) { var o=await repositorio.ObjetivoAsync(id,UsuarioId); return o is null?NotFound():View(new DetalheObjetivoViewModel(new(o,calculadora.Calcular(o)),await repositorio.ContribuicoesAsync(UsuarioId,id),await repositorio.HistoricoAsync(id,UsuarioId))); }
 [HttpGet] public async Task<IActionResult> Contribuicao(int id) { var o=await repositorio.ObjetivoAsync(id,UsuarioId); if(o is null) return NotFound(); ViewBag.Objetivo=new ObjetivoPlanejado(o,calculadora.Calcular(o)); return View(new Contribuicao {ObjetivoId=id}); }
 [HttpPost] public async Task<IActionResult> Contribuicao(Contribuicao model)
 {
  var o=await repositorio.ObjetivoAsync(model.ObjetivoId,UsuarioId); if(o is null) return NotFound();
  if(ModelState.IsValid) {
   try { TempData["Sucesso"]=await repositorio.ContribuirAsync(model,UsuarioId); return RedirectToAction(nameof(Detalhes),new {id=model.ObjetivoId}); }
   catch(InvalidOperationException e) {ModelState.AddModelError("",e.Message);}
  }
  ViewBag.Objetivo=new ObjetivoPlanejado(o,calculadora.Calcular(o)); return View(model);
 }
 [HttpPost] public async Task<IActionResult> Excluir(int id) {await repositorio.ExcluirAsync("Objetivos",id,UsuarioId); TempData["Sucesso"]="Objetivo excluído."; return RedirectToAction(nameof(Index));}
 [HttpPost] public async Task<IActionResult> Simular(Objetivo model) {
  if(!ModelState.IsValid) return BadRequest(new {mensagem="Confira os valores e as datas da simulação."});
  model.TotalContribuido=0;model.UltimaContribuicao=null;
  if(model.Id>0) {var atual=await repositorio.ObjetivoAsync(model.Id,UsuarioId);if(atual is null)return NotFound();model.TotalContribuido=atual.TotalContribuido;model.UltimaContribuicao=atual.UltimaContribuicao;}
  return Json(calculadora.Calcular(model));
 }
}
