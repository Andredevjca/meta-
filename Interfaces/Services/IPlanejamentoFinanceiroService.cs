using MetaMais.Models;
using MetaMais.ViewModels;

namespace MetaMais.Interfaces.Services;

public interface IPlanejamentoFinanceiroService
{
    Task<PainelViewModel> ObterAsync(int usuarioId);
}
