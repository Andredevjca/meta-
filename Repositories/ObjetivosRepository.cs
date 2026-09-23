using Dapper;
using MetaMais.Models;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Repositories;

public class ObjetivosRepository(Banco banco) : IObjetivosRepository
{
    private const string ConsultaObjetivos = "SELECT o.*, COALESCE(c.Total,0) TotalContribuido, c.Ultima UltimaContribuicao FROM objetivos o LEFT JOIN (SELECT ObjetivoId,SUM(Valor) Total,MAX(Data) Ultima FROM contribuicoes_objetivos GROUP BY ObjetivoId) c ON c.ObjetivoId=o.Id WHERE o.UsuarioId=@UsuarioId";
    public async Task<List<Objetivo>> ObjetivosAsync(int usuarioId) { await using var c = banco.Abrir(); return (await c.QueryAsync<Objetivo>(ConsultaObjetivos + " ORDER BY o.Prioridade,o.DataLimite", new { usuarioId })).ToList(); }
    public async Task<Objetivo?> ObjetivoAsync(int id, int usuarioId) { await using var c = banco.Abrir(); return await c.QuerySingleOrDefaultAsync<Objetivo>(ConsultaObjetivos + " AND o.Id=@id", new { id, usuarioId }); }
    public async Task<int> SalvarObjetivoAsync(Objetivo o)
    {
        await using var c = banco.Abrir(); await c.OpenAsync(); await using var t = await c.BeginTransactionAsync();
        if (o.Id == 0) o.Id = await c.ExecuteScalarAsync<int>("INSERT INTO objetivos (UsuarioId,Nome,Descricao,Categoria,Icone,ValorObjetivo,ValorInicial,DataInicio,DataLimite,Frequencia,Status,Prioridade) VALUES (@UsuarioId,@Nome,@Descricao,@Categoria,@Icone,@ValorObjetivo,@ValorInicial,@DataInicio,@DataLimite,@Frequencia,@Status,@Prioridade); SELECT LAST_INSERT_ID();", o, t);
        else
        {
            var existente = await c.QuerySingleOrDefaultAsync<Objetivo>("SELECT * FROM objetivos WHERE Id=@Id AND UsuarioId=@UsuarioId FOR UPDATE", o, t);
            if (existente is null) throw new InvalidOperationException("Objetivo não encontrado.");
            await c.ExecuteAsync("UPDATE objetivos SET Nome=@Nome,Descricao=@Descricao,Categoria=@Categoria,Icone=@Icone,ValorObjetivo=@ValorObjetivo,ValorInicial=@ValorInicial,DataInicio=@DataInicio,DataLimite=@DataLimite,Frequencia=@Frequencia,Status=@Status,Prioridade=@Prioridade WHERE Id=@Id AND UsuarioId=@UsuarioId", o, t);
        }
        await c.ExecuteAsync("UPDATE objetivos SET Status=IF(ValorInicial+COALESCE((SELECT SUM(Valor) FROM contribuicoes_objetivos WHERE ObjetivoId=@Id),0)>=ValorObjetivo,'Concluído',IF(Status='Concluído','Ativo',Status)), DataConclusao=IF(ValorInicial+COALESCE((SELECT SUM(Valor) FROM contribuicoes_objetivos WHERE ObjetivoId=@Id),0)>=ValorObjetivo,COALESCE(DataConclusao,CURDATE()),NULL) WHERE Id=@Id AND UsuarioId=@UsuarioId", o, t);
        await c.ExecuteAsync("INSERT INTO historico_objetivos (ObjetivoId,Descricao) VALUES (@Id,@descricao)", new { o.Id, descricao = $"Planejamento salvo: {o.Nome}; objetivo {o.ValorObjetivo:C}; inicial {o.ValorInicial:C}; prazo {o.DataLimite:dd/MM/yyyy}; frequência {o.Frequencia}; status {o.Status}." }, t);
        await t.CommitAsync(); return o.Id;
    }
    public async Task<List<HistoricoObjetivo>> HistoricoAsync(int id, int usuarioId) { await using var c = banco.Abrir(); return (await c.QueryAsync<HistoricoObjetivo>("SELECT h.* FROM historico_objetivos h JOIN objetivos o ON o.Id=h.ObjetivoId WHERE o.Id=@id AND o.UsuarioId=@usuarioId ORDER BY h.Id DESC", new { id, usuarioId })).ToList(); }
    public async Task ExcluirAsync(int id, int usuarioId) { await using var c = banco.Abrir(); await c.ExecuteAsync("DELETE FROM objetivos WHERE Id=@id AND UsuarioId=@usuarioId", new { id, usuarioId }); }

}
