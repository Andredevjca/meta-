using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Models;
using MetaMais.Repositories;
using MetaMais.ViewModels;
namespace MetaMais.Services;
public class PlanejamentoFinanceiroService(IFinanceiroRepository repositorio,ICalculadoraObjetivoService calculadora) : IPlanejamentoFinanceiroService
{
 public static decimal ValorMensal(Lancamento l,DateTime hoje)
 {
  if (!l.Ativo) return 0;
  var inicio=new DateTime(hoje.Year,hoje.Month,1); var fim=inicio.AddMonths(1).AddDays(-1);
  if(l.Periodicidade=="Eventual") return l.Data>=inicio && l.Data<=fim ? l.Valor : 0;
  if(l.Data>fim) return 0;
  if(l.Periodicidade=="Mensal") return l.Valor;
  var dias=l.Periodicidade=="Semanal"?7:15;
  var primeira=l.Data.Date;
  if(primeira<inicio) primeira=primeira.AddDays(decimal.ToInt32(Math.Ceiling((decimal)(inicio-primeira).Days/dias))*dias);
  return primeira>fim ? 0 : (1+(fim-primeira).Days/dias)*l.Valor;
 }
 public async Task<PainelViewModel> ObterAsync(int usuarioId)
 {
  var p=new PainelViewModel { Receitas=await repositorio.LancamentosAsync("Receitas",usuarioId), Despesas=await repositorio.LancamentosAsync("Despesas",usuarioId), Dividas=await repositorio.DividasAsync(usuarioId), Contribuicoes=await repositorio.ContribuicoesAsync(usuarioId) };
  p.Objetivos=(await repositorio.ObjetivosAsync(usuarioId)).Select(o=>new ObjetivoPlanejado(o,calculadora.Calcular(o))).ToList();
  p.ReceitaMensal=p.Receitas.Sum(l=>ValorMensal(l,DateTime.Today)); p.DespesaMensal=p.Despesas.Sum(l=>ValorMensal(l,DateTime.Today));
  p.ParcelasMensais=p.Dividas.Where(d=>d.Status=="Ativa" && d.DataInicio<=new DateTime(DateTime.Today.Year,DateTime.Today.Month,1).AddMonths(1).AddDays(-1)).Sum(d=>Math.Min(d.ValorParcela,d.ValorRestante));
  return p;
 }
}
