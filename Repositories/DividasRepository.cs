using Dapper;
using MetaMais.Models;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Repositories;

public class DividasRepository(Banco banco) : IDividasRepository
{
    public async Task<bool> PagarParcelaAsync(int id, int usuarioId, int parcelasPagas)
    {
        await using var c = banco.Abrir();
        // A contagem esperada impede que dois envios do mesmo card paguem duas parcelas.
        return await c.ExecuteAsync("""
            UPDATE dividas
            SET DataInicio = CASE
                    WHEN ParcelasPagas + 1 < QuantidadeParcelas
                         AND ValorTotal > (ParcelasPagas + 1) * ValorParcela
                    THEN DATE_ADD(DataInicio, INTERVAL 1 MONTH)
                    ELSE DataInicio
                END,
                ParcelasPagas = ParcelasPagas + 1
            WHERE Id = @id AND UsuarioId = @usuarioId
              AND ParcelasPagas = @parcelasPagas
              AND ParcelasPagas < QuantidadeParcelas
              AND ValorTotal > ParcelasPagas * ValorParcela
            """, new { id, usuarioId, parcelasPagas }) == 1;
    }
    public async Task<List<Divida>> DividasAsync(int usuarioId) { await using var c = banco.Abrir(); return (await c.QueryAsync<Divida>("SELECT * FROM dividas WHERE UsuarioId=@usuarioId ORDER BY DataInicio", new { usuarioId })).ToList(); }
    public async Task SalvarDividaAsync(Divida d) { await using var c = banco.Abrir(); await c.ExecuteAsync(d.Id == 0 ? "INSERT INTO dividas (UsuarioId,Descricao,Credor,ValorTotal,ValorParcela,QuantidadeParcelas,ParcelasPagas,DataInicio) VALUES (@UsuarioId,@Descricao,@Credor,@ValorTotal,@ValorParcela,@QuantidadeParcelas,@ParcelasPagas,@DataInicio)" : "UPDATE dividas SET Descricao=@Descricao,Credor=@Credor,ValorTotal=@ValorTotal,ValorParcela=@ValorParcela,QuantidadeParcelas=@QuantidadeParcelas,ParcelasPagas=@ParcelasPagas,DataInicio=@DataInicio WHERE Id=@Id AND UsuarioId=@UsuarioId", d); }
    public async Task ExcluirAsync(int id, int usuarioId) { await using var c = banco.Abrir(); await c.ExecuteAsync("DELETE FROM dividas WHERE Id=@id AND UsuarioId=@usuarioId", new { id, usuarioId }); }

}
