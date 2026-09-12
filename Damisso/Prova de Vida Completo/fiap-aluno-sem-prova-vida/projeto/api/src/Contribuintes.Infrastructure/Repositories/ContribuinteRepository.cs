using Contribuintes.Domain.Entities;
using Contribuintes.Domain.Repositories;
using Contribuintes.Domain.ValueObjects;
using Contribuintes.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Contribuintes.Infrastructure.Repositories;

public class ContribuinteRepository(ContribuintesDbContext context) : IContribuinteRepository
{
    public async Task<Contribuinte?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Contribuintes.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Contribuinte?> ObterPorNuitAsync(Nuit nuit, CancellationToken ct = default) =>
        await context.Contribuintes.FirstOrDefaultAsync(c => c.Nuit == nuit, ct);

    public async Task<IReadOnlyList<Contribuinte>> ListarAsync(CancellationToken ct = default) =>
        await context.Contribuintes.OrderBy(c => c.Nome).ToListAsync(ct);

    public async Task AdicionarAsync(Contribuinte contribuinte, CancellationToken ct = default) =>
        await context.Contribuintes.AddAsync(contribuinte, ct);

    public async Task SalvarAlteracoesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);
}
