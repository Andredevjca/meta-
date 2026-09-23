using Dapper;
using MetaMais.Models;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Repositories;

public class ContribuicoesRepository(Banco banco, ICalculadoraObjetivoService calculadora) : IContribuicoesRepository
{
    public async Task<string> ContribuirAsync(Contribuicao deposito, int usuarioId)
    {
        await using var c = banco.Abrir(); await c.OpenAsync(); await using var t = await c.BeginTransactionAsync();
        var o = await c.QuerySingleOrDefaultAsync<Objetivo>("SELECT * FROM objetivos WHERE Id=@ObjetivoId AND UsuarioId=@usuarioId FOR UPDATE", new { deposito.ObjetivoId, usuarioId }, t);
        if (o is null || o.Status != "Ativo") throw new InvalidOperationException("Selecione um objetivo ativo.");
        if (deposito.Data.Date > DateTime.Today || deposito.Data.Date < o.DataInicio.Date) throw new InvalidOperationException("A data deve estar entre o início do objetivo e hoje.");
        o.TotalContribuido = await c.ExecuteScalarAsync<decimal>("SELECT COALESCE(SUM(Valor),0) FROM contribuicoes_objetivos WHERE ObjetivoId=@Id", o, t);
        var antes = calculadora.Calcular(o);
        await c.ExecuteAsync("INSERT INTO contribuicoes_objetivos (ObjetivoId,Valor,Data,Observacao) VALUES (@ObjetivoId,@Valor,@Data,@Observacao)", deposito, t);
        o.TotalContribuido += deposito.Valor; o.UltimaContribuicao = deposito.Data;
        var depois = calculadora.Calcular(o);
        var diferenca = deposito.Valor - antes.PorPeriodo;
        var mensagem = depois.Restante == 0 ? "Objetivo alcançado! Você conseguiu conquistar sua meta." : diferenca == 0 ? "Meta cumprida." : $"Você guardou {Math.Abs(diferenca):C} {(diferenca > 0 ? "acima" : "abaixo")} do planejado. Próximas contribuições: {depois.PorPeriodo:C} ({o.Frequencia.ToLower()}).";
        if (depois.Restante == 0) await c.ExecuteAsync("UPDATE objetivos SET Status='Concluído',DataConclusao=@Data WHERE Id=@ObjetivoId", deposito, t);
        await c.ExecuteAsync("INSERT INTO historico_objetivos (ObjetivoId,Descricao) VALUES (@ObjetivoId,@descricao)", new { deposito.ObjetivoId, descricao = $"Contribuição de {deposito.Valor:C}. Saldo: {o.ValorAcumulado:C}. {mensagem}" }, t);
        await c.ExecuteAsync("INSERT INTO notificacoes (UsuarioId,Mensagem) VALUES (@usuarioId,@mensagem)", new { usuarioId, mensagem }, t);
        await t.CommitAsync(); return mensagem;
    }
    public async Task<List<Contribuicao>> ContribuicoesAsync(int usuarioId, int? objetivoId = null) { await using var c = banco.Abrir(); return (await c.QueryAsync<Contribuicao>("SELECT c.*,o.Nome NomeObjetivo FROM contribuicoes_objetivos c JOIN objetivos o ON o.Id=c.ObjetivoId WHERE o.UsuarioId=@usuarioId AND (@objetivoId IS NULL OR o.Id=@objetivoId) ORDER BY c.Data DESC,c.Id DESC", new { usuarioId, objetivoId })).ToList(); }

}
