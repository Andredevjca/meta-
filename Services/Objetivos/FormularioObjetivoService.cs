using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Services;

public class FormularioObjetivoService(IObjetivosRepository repositorio, ICalculadoraObjetivoService calculadora) : IFormularioObjetivoService
{
    public Task<Objetivo?> ObterAsync(int usuarioId, int id) => repositorio.ObjetivoAsync(id, usuarioId);
    public string? ValidarPrazo(Objetivo objetivo) => objetivo.Id == 0 && objetivo.DataLimite.Date < DateTime.Today ? "Escolha um prazo a partir de hoje." : null;
    public Task<int> SalvarAsync(int usuarioId, Objetivo objetivo) { objetivo.UsuarioId = usuarioId; return repositorio.SalvarObjetivoAsync(objetivo); }
    public async Task<CalculoObjetivo?> SimularAsync(int usuarioId, Objetivo model)
    {
        model.TotalContribuido = 0; model.UltimaContribuicao = null;
        if (model.Id > 0) { var atual = await ObterAsync(usuarioId, model.Id); if (atual is null) return null; model.TotalContribuido = atual.TotalContribuido; model.UltimaContribuicao = atual.UltimaContribuicao; }
        return calculadora.Calcular(model);
    }
}
