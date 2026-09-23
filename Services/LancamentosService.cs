using MetaMais.Models;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Services;

public abstract class LancamentosService(ILancamentosRepository repositorio, string tipo) : ILancamentosService
{
    public Task<List<Lancamento>> ListarAsync(int usuarioId) => repositorio.LancamentosAsync(tipo, usuarioId);
    public async Task<Lancamento?> ObterAsync(int usuarioId, int id) => id == 0 ? new Lancamento() : (await ListarAsync(usuarioId)).FirstOrDefault(x => x.Id == id);
    public Task SalvarAsync(int usuarioId, Lancamento model) { model.UsuarioId = usuarioId; return repositorio.SalvarLancamentoAsync(tipo, model); }
    public Task ExcluirAsync(int usuarioId, int id) => repositorio.ExcluirAsync(tipo, id, usuarioId);
}
