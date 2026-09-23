using MetaMais.Interfaces.Services;
using MetaMais.Models;
using MetaMais.ViewModels;
namespace MetaMais.Services;

public class CalculadoraObjetivoService : ICalculadoraObjetivoService
{
    public static DateTime DataPeriodo(DateTime inicio, string frequencia, int numero) => frequencia switch
    {
        "Semanal" => inicio.Date.AddDays(7 * numero),
        "Quinzenal" => inicio.Date.AddDays(15 * numero),
        _ => inicio.Date.AddMonths(numero)
    };
    public List<DateTime> DatasRestantes(Objetivo objetivo, DateTime hoje, string frequencia)
    {
        var datas = new List<DateTime>(); var corte = hoje.Date;
        if (objetivo.UltimaContribuicao is DateTime ultima && ultima.Date >= corte) corte = ultima.Date.AddDays(1);
        for (var i = 1; i <= 16000; i++)
        {
            var data = DataPeriodo(objetivo.DataInicio, frequencia, i);
            if (data > objetivo.DataLimite.Date) break;
            if (data >= corte) datas.Add(data);
        }
        if (objetivo.DataLimite.Date >= corte && !datas.Contains(objetivo.DataLimite.Date)) datas.Add(objetivo.DataLimite.Date);
        return datas;
    }
    public CalculoObjetivo Calcular(Objetivo objetivo, DateTime? referencia = null)
    {
        var hoje = (referencia ?? DateTime.Today).Date;
        var restante = Math.Max(0, objetivo.ValorObjetivo - objetivo.ValorAcumulado);
        var datas = DatasRestantes(objetivo, hoje, objetivo.Frequencia);
        decimal Necessario(string frequencia) => restante == 0 ? 0 : Math.Ceiling(restante / Math.Max(1, DatasRestantes(objetivo, hoje, frequencia).Count) * 100) / 100;
        var valor = restante == 0 ? 0 : Math.Ceiling(restante / Math.Max(1, datas.Count) * 100) / 100;
        var percentual = objetivo.ValorObjetivo <= 0 ? 0 : Math.Min(100, objetivo.ValorAcumulado / objetivo.ValorObjetivo * 100);
        DateTime? previsao = restante == 0 ? objetivo.DataConclusao ?? hoje : objetivo.DataLimite.Date >= hoje ? objetivo.DataLimite : null;
        return new(restante, percentual, datas.Count, valor, Necessario("Mensal"), Necessario("Quinzenal"), Necessario("Semanal"), datas.Count > 0 && restante > 0 ? datas[0] : null, restante > 0 && objetivo.DataLimite.Date < hoje, previsao);
    }
}
