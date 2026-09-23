using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Services;

public class RelatoriosService(IPlanejamentoFinanceiroService planejamento) : IRelatoriosService
{
    public async Task<RelatoriosViewModel> ObterAsync(int usuarioId)
    {
        var painel = await planejamento.ObterAsync(usuarioId);
        var total = painel.Objetivos.Sum(x => x.Objetivo.ValorAcumulado);
        var evolucao = new List<IndicadorRelatorioViewModel>();
        for (var i = 5; i >= 0; i--)
        {
            var mes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(-i); var fim = mes.AddMonths(1);
            var saldo = painel.Objetivos.Where(x => x.Objetivo.DataInicio < fim).Sum(x => x.Objetivo.ValorInicial) + painel.Contribuicoes.Where(c => c.Data < fim).Sum(c => c.Valor);
            evolucao.Add(new(mes.ToString("MMM yyyy"), saldo, total > 0 ? Math.Min(100, saldo / total * 100) : 0));
        }
        var categorias = painel.Despesas.Where(x => x.Ativo).GroupBy(x => x.Categoria).Select(grupo =>
        {
            var valor = grupo.Sum(x => PlanejamentoFinanceiroService.ValorMensal(x, DateTime.Today));
            return new IndicadorRelatorioViewModel(grupo.Key, valor, painel.DespesaMensal > 0 ? valor / painel.DespesaMensal * 100 : 0);
        }).ToList();
        return new(painel, evolucao, categorias);
    }
}
