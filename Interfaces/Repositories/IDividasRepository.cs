using MetaMais.Models;
namespace MetaMais.Interfaces.Repositories;

public interface IDividasRepository
{
    Task<List<Divida>> DividasAsync(int usuarioId);
    Task SalvarDividaAsync(Divida d);
    Task<bool> PagarParcelaAsync(int id, int usuarioId, int parcelasPagas);
    Task ExcluirAsync(int id, int usuarioId);
}
