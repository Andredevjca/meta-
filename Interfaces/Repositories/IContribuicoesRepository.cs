using MetaMais.Models;
namespace MetaMais.Interfaces.Repositories;

public interface IContribuicoesRepository
{
    Task<string> ContribuirAsync(Contribuicao deposito, int usuarioId);
    Task<List<Contribuicao>> ContribuicoesAsync(int usuarioId, int? objetivoId = null);
}
