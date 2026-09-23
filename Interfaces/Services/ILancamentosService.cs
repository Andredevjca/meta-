using MetaMais.Models;
namespace MetaMais.Interfaces.Services;

public interface ILancamentosService
{
    Task<List<Lancamento>> ListarAsync(int usuarioId);
    Task<Lancamento?> ObterAsync(int usuarioId, int id);
    Task SalvarAsync(int usuarioId, Lancamento model);
    Task ExcluirAsync(int usuarioId, int id);
}
