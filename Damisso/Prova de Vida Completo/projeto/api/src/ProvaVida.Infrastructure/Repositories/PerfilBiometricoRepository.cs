using ProvaVida.Domain.Entities;
using ProvaVida.Domain.Repositories;
using ProvaVida.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ProvaVida.Infrastructure.Repositories;

public class PerfilBiometricoRepository(ProvaVidaDbContext context) : IPerfilBiometricoRepository
{
    public async Task<PerfilBiometrico?> ObterPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default) =>
        await context.PerfisBiometricos.FirstOrDefaultAsync(p => p.ContribuinteId == contribuinteId, ct);

    public async Task AdicionarAsync(PerfilBiometrico perfil, CancellationToken ct = default) =>
        await context.PerfisBiometricos.AddAsync(perfil, ct);

    public async Task SalvarAlteracoesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);
}
