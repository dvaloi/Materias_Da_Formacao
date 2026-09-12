using ProvaVida.Domain.Entities;
using ProvaVida.Domain.Repositories;
using ProvaVida.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ProvaVida.Infrastructure.Repositories;

public class RegistroProvaVidaRepository(ProvaVidaDbContext context) : IRegistroProvaVidaRepository
{
    public async Task AdicionarAsync(RegistroProvaVida registro, CancellationToken ct = default) =>
        await context.Registros.AddAsync(registro, ct);

    public async Task SalvarAlteracoesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);

    public async Task<RegistroProvaVida?> ObterUltimoPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default) =>
        await context.Registros
            .Where(r => r.ContribuinteId == contribuinteId)
            .OrderByDescending(r => r.RealizadoEm)
            .FirstOrDefaultAsync(ct);

    public async Task<IReadOnlyList<RegistroProvaVida>> ListarPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default) =>
        await context.Registros
            .Where(r => r.ContribuinteId == contribuinteId)
            .OrderByDescending(r => r.RealizadoEm)
            .ToListAsync(ct);

    public async Task<RegistroProvaVida?> ObterUltimoPorNuitAsync(string nuit, CancellationToken ct = default) =>
        await context.Registros
            .Where(r => r.Nuit == nuit)
            .OrderByDescending(r => r.RealizadoEm)
            .FirstOrDefaultAsync(ct);

    public async Task<RegistroProvaVida?> ObterCadastroPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default) =>
        await context.Registros
            .Where(r => r.ContribuinteId == contribuinteId)
            .OrderBy(r => r.RealizadoEm)
            .FirstOrDefaultAsync(ct);
}
