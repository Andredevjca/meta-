using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
namespace MetaMais.Services;

public class ReceitasService(ILancamentosRepository repositorio) : LancamentosService(repositorio, "Receitas"), IReceitasService { }
