using Cofrinho.Application.Interfaces;
using Cofrinho.Application.Services.UseCases;
using Cofrinho.Application.UseCases;
using Cofrinho.Console.Application.Services.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Cofrinho.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICriarMetaUseCase, CriarMetaUseCase>();
        services.AddScoped<IObterMetaPorNomeUseCase, ObterMetaPorNomeUseCase>();
        services.AddScoped<IListarMetasUseCase, ListarMetasUseCase>();
        services.AddScoped<IDepositarUseCase, DepositarUseCase>();
        services.AddScoped<ISacarUseCase, SacarUseCase>();
        services.AddScoped<IGerarExtratoUseCase, GerarExtratoUseCase>();
        services.AddScoped<IGerarExtratoGeralUseCase, GerarExtratoGeralUseCase>();

        return services;
    }
}
