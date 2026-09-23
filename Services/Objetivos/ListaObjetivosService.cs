using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Services;

public class ListaObjetivosService(IObjetivosRepository repositorio, ICalculadoraObjetivoService calculadora) : IListaObjetivosService
{
    public async Task<List<ObjetivoPlanejado>> ObterAsync(int usuarioId) => (await repositorio.ObjetivosAsync(usuarioId)).Select(o => new ObjetivoPlanejado(o, calculadora.Calcular(o))).ToList();
    public Task ExcluirAsync(int usuarioId, int id) => repositorio.ExcluirAsync(id, usuarioId);
}
