using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
namespace MetaMais.Services;

public class CalendarioService(IPlanejamentoFinanceiroService planejamento, ICalculadoraObjetivoService calculadora) : ICalendarioService
{
    public async Task<CalendarioViewModel?> ObterAsync(int usuarioId, int? ano, int? mesInformado)
    {
        var a = ano ?? DateTime.Today.Year; var m = mesInformado ?? DateTime.Today.Month;
        if (a < 1900 || a > 2200 || m < 1 || m > 12) return null;
        var mes = new DateTime(a, m, 1); var fim = mes.AddMonths(1).AddDays(-1);
        var painel = await planejamento.ObterAsync(usuarioId);
        var eventos = new List<EventoCalendarioViewModel>();
        foreach (var (lista, tipo, url) in new[] { (painel.Receitas, "receita", "/Receitas"), (painel.Despesas, "despesa", "/Despesas") })
        {
            foreach (var l in lista.Where(x => x.Ativo))
            {
                if (l.Periodicidade == "Eventual") { if (l.Data >= mes && l.Data <= fim) eventos.Add(new(l.Data, l.Descricao, l.Valor, tipo, url)); continue; }
                for (var i = 0; i <= 16000; i++) { var data = CalculadoraObjetivoService.DataPeriodo(l.Data, l.Periodicidade, i); if (data > fim) break; if (data >= mes) eventos.Add(new(data, l.Descricao, l.Valor, tipo, url)); }
            }
        }
        foreach (var d in painel.Dividas.Where(x => x.Status == "Ativa")) { for (var i = d.ParcelasPagas; i < d.QuantidadeParcelas; i++) { var data = d.DataInicio.AddMonths(i); if (data >= mes && data <= fim) eventos.Add(new(data, d.Descricao, d.ValorParcela, "despesa", "/Dividas")); } }
        foreach (var o in painel.Objetivos.Where(x => x.Objetivo.Status == "Ativo"))
        {
            foreach (var data in calculadora.DatasRestantes(o.Objetivo, mes, o.Objetivo.Frequencia).Where(d => d <= fim)) eventos.Add(new(data, "Guardar: " + o.Objetivo.Nome, o.Calculo.PorPeriodo, "meta", "/Objetivos/Contribuicao/" + o.Objetivo.Id));
            if (o.Objetivo.DataLimite >= mes && o.Objetivo.DataLimite <= fim) eventos.Add(new(o.Objetivo.DataLimite, "Prazo: " + o.Objetivo.Nome, o.Objetivo.ValorObjetivo, "meta", "/Objetivos/Detalhes/" + o.Objetivo.Id));
        }
        foreach (var c in painel.Contribuicoes.Where(x => x.Data >= mes && x.Data <= fim)) eventos.Add(new(c.Data, "Guardado: " + c.NomeObjetivo, c.Valor, "receita", "/Objetivos/Detalhes/" + c.ObjetivoId));
        return new CalendarioViewModel(mes, eventos);
    }
}
