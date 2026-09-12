using Beneficios.Application.Interfaces;
using Beneficios.Application.Services;
using Beneficios.Domain.Repositories;
using Beneficios.Infrastructure.Persistence;
using Beneficios.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Beneficios.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<BeneficiosDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IPedidoBeneficioRepository, PedidoBeneficioRepository>();
        services.AddScoped<IPedidoBeneficioService, PedidoBeneficioService>();

        return services;
    }
}
