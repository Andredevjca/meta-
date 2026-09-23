using MetaMais.Models;
namespace MetaMais.Interfaces.Repositories;

public interface IDividasRepository
{
    Task<List<Divida>> DividasAsync(int usuarioId);
    Task SalvarDividaAsync(Divida d);
    Task ExcluirAsync(int id, int usuarioId);
}
