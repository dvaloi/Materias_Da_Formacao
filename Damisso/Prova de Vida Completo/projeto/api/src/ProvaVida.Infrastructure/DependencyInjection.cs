using ProvaVida.Application.Interfaces;
using ProvaVida.Application.Services;
using ProvaVida.Domain.Repositories;
using ProvaVida.Infrastructure.Http;
using ProvaVida.Infrastructure.Persistence;
using ProvaVida.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ProvaVida.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString,
        string contribuintesApiBaseUrl)
    {
        services.AddDbContext<ProvaVidaDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddHttpClient<IContribuintesClient, ContribuintesHttpClient>(client =>
        {
            client.BaseAddress = new Uri(contribuintesApiBaseUrl.TrimEnd('/') + "/");
        });

        services.AddScoped<IRegistroProvaVidaRepository, RegistroProvaVidaRepository>();
        services.AddScoped<IPerfilBiometricoRepository, PerfilBiometricoRepository>();
        services.AddScoped<IProvaVidaService, ProvaVidaService>();

        return services;
    }
}
