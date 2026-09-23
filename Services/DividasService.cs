using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Services;

public class DividasService(IDividasRepository repositorio) : IDividasService
{
    public Task<List<Divida>> ListarAsync(int usuarioId) => repositorio.DividasAsync(usuarioId);
    public async Task<Divida?> ObterAsync(int usuarioId, int id) => id == 0 ? new Divida() : (await ListarAsync(usuarioId)).FirstOrDefault(x => x.Id == id);
    public Task SalvarAsync(int usuarioId, Divida model) { model.UsuarioId = usuarioId; return repositorio.SalvarDividaAsync(model); }
    public Task ExcluirAsync(int usuarioId, int id) => repositorio.ExcluirAsync(id, usuarioId);
}
