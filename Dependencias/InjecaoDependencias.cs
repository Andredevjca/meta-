using MetaMais.Interfaces.Repositories;
using MetaMais.Interfaces.Services;
using MetaMais.Repositories;
using MetaMais.Services;

namespace MetaMais.Dependencias;

public static class InjecaoDependencias
{
    public static IServiceCollection AdicionarDependencias(this IServiceCollection services)
    {
        services.AddSingleton<Banco>();
        services.AddScoped<IUsuariosRepository, UsuariosRepository>();
        services.AddScoped<IObjetivosRepository, ObjetivosRepository>();
        services.AddScoped<IContribuicoesRepository, ContribuicoesRepository>();
        services.AddScoped<ILancamentosRepository, LancamentosRepository>();
        services.AddScoped<IDividasRepository, DividasRepository>();
        services.AddScoped<INotificacoesRepository, NotificacoesRepository>();
        services.AddSingleton<ICalculadoraObjetivoService, CalculadoraObjetivoService>();
        services.AddScoped<IPlanejamentoFinanceiroService, PlanejamentoFinanceiroService>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<ICadastroService, CadastroService>();
        services.AddScoped<IPerfilService, PerfilService>();
        services.AddScoped<ISimuladorService, SimuladorService>();
        services.AddScoped<ICalendarioService, CalendarioService>();
        services.AddScoped<IReceitasService, ReceitasService>();
        services.AddScoped<IDespesasService, DespesasService>();
        services.AddScoped<IDividasService, DividasService>();
        services.AddScoped<INotificacoesService, NotificacoesService>();
        services.AddScoped<IRelatoriosService, RelatoriosService>();
        services.AddScoped<IListaObjetivosService, ListaObjetivosService>();
        services.AddScoped<IDetalheObjetivoService, DetalheObjetivoService>();
        services.AddScoped<IFormularioObjetivoService, FormularioObjetivoService>();
        services.AddScoped<IContribuicaoObjetivoService, ContribuicaoObjetivoService>();
        return services;
    }
}
