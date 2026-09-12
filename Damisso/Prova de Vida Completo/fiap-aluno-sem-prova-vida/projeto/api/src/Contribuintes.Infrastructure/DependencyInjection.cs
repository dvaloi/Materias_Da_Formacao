using Contribuintes.Application.Interfaces;
using Contribuintes.Application.Services;
using Contribuintes.Domain.Repositories;
using Contribuintes.Infrastructure.Persistence;
using Contribuintes.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Contribuintes.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<ContribuintesDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IContribuinteRepository, ContribuinteRepository>();
        services.AddScoped<IContribuinteService, ContribuinteService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
