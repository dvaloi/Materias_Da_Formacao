using Contribuintes.Domain.Entities;
using Contribuintes.Domain.ValueObjects;
using Contribuintes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Contribuintes.Infrastructure.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ContribuintesDbContext>();

        // Schema versionado (Migrations). Não usa EnsureCreated.
        await context.Database.MigrateAsync();

        // Se já houver dados, não reinsere (local Docker ou Neon na nuvem)
        if (await context.Contribuintes.AnyAsync()) return;

        // Contribuintes fictícios — demo aula (salários sem viés de gênero)
        var contribuintes = new[]
        {
            Contribuinte.Criar(Nuit.Criar("100200301"), "Ana Celeste Mabunda", new DateOnly(1988, 3, 14), 56000m),
            Contribuinte.Criar(Nuit.Criar("100200302"), "Tomás Viola Matola", new DateOnly(1990, 7, 22), 51000m),
            Contribuinte.Criar(Nuit.Criar("100200303"), "Graça Mussá Tembe", new DateOnly(1987, 11, 5), 52000m),
            Contribuinte.Criar(Nuit.Criar("200300401"), "Carlos Alberto Massingue", new DateOnly(1985, 2, 18), 48000m),
            Contribuinte.Criar(Nuit.Criar("200300402"), "Helena João Macamo", new DateOnly(1989, 6, 9), 49000m),
            Contribuinte.Criar(Nuit.Criar("200300403"), "Eduardo Nunes Sitoe", new DateOnly(1991, 9, 30), 46000m),
            Contribuinte.Criar(Nuit.Criar("200300404"), "Joaquina Salomão Nhamirre", new DateOnly(1992, 1, 12), 47000m),
            Contribuinte.Criar(Nuit.Criar("200300405"), "Filipe Jordão Ubisse", new DateOnly(1986, 4, 27), 44000m),
            Contribuinte.Criar(Nuit.Criar("200300406"), "Luísa Carlos Mondlane", new DateOnly(1984, 8, 3), 45000m),
            Contribuinte.Criar(Nuit.Criar("200300407"), "Manuel Henriques Mucaze", new DateOnly(1988, 12, 16), 43000m)
        };

        context.Contribuintes.AddRange(contribuintes);
        await context.SaveChangesAsync();
    }
}
