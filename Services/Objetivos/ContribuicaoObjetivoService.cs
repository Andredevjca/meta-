using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Services;

public class ContribuicaoObjetivoService(IObjetivosRepository objetivos, IContribuicoesRepository contribuicoes, ICalculadoraObjetivoService calculadora) : IContribuicaoObjetivoService
{
    public async Task<ObjetivoPlanejado?> ObterAsync(int usuarioId, int id)
    {
        var objetivo = await objetivos.ObjetivoAsync(id, usuarioId);
        return objetivo is null ? null : new(objetivo, calculadora.Calcular(objetivo));
    }
    public Task<string> ContribuirAsync(int usuarioId, Contribuicao model) => contribuicoes.ContribuirAsync(model, usuarioId);
}
