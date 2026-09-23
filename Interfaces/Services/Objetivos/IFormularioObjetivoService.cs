using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Interfaces.Services;

public interface IFormularioObjetivoService
{
    Task<Objetivo?> ObterAsync(int usuarioId, int id);
    string? ValidarPrazo(Objetivo objetivo);
    Task<int> SalvarAsync(int usuarioId, Objetivo objetivo);
    Task<CalculoObjetivo?> SimularAsync(int usuarioId, Objetivo model);
}
