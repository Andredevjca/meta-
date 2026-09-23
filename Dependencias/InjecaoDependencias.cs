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
        services.AddScoped<IFinanceiroRepository, FinanceiroRepository>();
        services.AddSingleton<ICalculadoraObjetivoService, CalculadoraObjetivoService>();
        services.AddScoped<IPlanejamentoFinanceiroService, PlanejamentoFinanceiroService>();
        return services;
    }
}
