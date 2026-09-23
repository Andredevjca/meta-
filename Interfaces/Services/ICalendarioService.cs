using MetaMais.Models;
using MetaMais.ViewModels;
using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
namespace MetaMais.Interfaces.Services;

public interface ICalendarioService
{
    Task<CalendarioViewModel?> ObterAsync(int usuarioId, int? ano, int? mes);
}
