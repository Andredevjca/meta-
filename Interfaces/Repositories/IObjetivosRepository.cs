using MetaMais.Models;
namespace MetaMais.Interfaces.Repositories;

public interface IObjetivosRepository
{
    Task<List<Objetivo>> ObjetivosAsync(int usuarioId);
    Task<Objetivo?> ObjetivoAsync(int id, int usuarioId);
    Task<int> SalvarObjetivoAsync(Objetivo o);
    Task<List<HistoricoObjetivo>> HistoricoAsync(int id, int usuarioId);
    Task ExcluirAsync(int id, int usuarioId);
}
