using Cofrinho.Domain.Interfaces;
using Cofrinho.Infrastructure.Persistence;
using Cofrinho.Infrastructure.Repositories;
using Cofrinho.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cofrinho.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CofrinhoDbContext>(opt => opt.UseSqlite(connectionString));

        services.AddScoped<IMetaRepository, EfMetaRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
     
        return services;
    }
}
