using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Services;

public class DetalheObjetivoService(IObjetivosRepository repositorio, IContribuicoesRepository contribuicoes, ICalculadoraObjetivoService calculadora) : IDetalheObjetivoService
{
    public async Task<DetalheObjetivoViewModel?> ObterAsync(int usuarioId, int id)
    {
        var objetivo = await repositorio.ObjetivoAsync(id, usuarioId);
        return objetivo is null ? null : new(new(objetivo, calculadora.Calcular(objetivo)), await contribuicoes.ContribuicoesAsync(usuarioId, id), await repositorio.HistoricoAsync(id, usuarioId));
    }
}
