using MetaMais.Models;
namespace MetaMais.Interfaces.Repositories;

public interface ILancamentosRepository
{
    Task<List<Lancamento>> LancamentosAsync(string tipo, int usuarioId);
    Task SalvarLancamentoAsync(string tipo, Lancamento l);
    Task ExcluirAsync(string tipo, int id, int usuarioId);
}
