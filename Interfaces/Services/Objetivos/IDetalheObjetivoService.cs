using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Interfaces.Services;

public interface IDetalheObjetivoService
{
    Task<DetalheObjetivoViewModel?> ObterAsync(int usuarioId, int id);
}
