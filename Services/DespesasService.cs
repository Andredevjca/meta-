using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Services;

public class DespesasService(ILancamentosRepository repositorio) : LancamentosService(repositorio, "Despesas"), IDespesasService { }
