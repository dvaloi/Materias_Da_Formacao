using Beneficios.Domain.Entities;
using Beneficios.Domain.Repositories;
using Beneficios.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Beneficios.Infrastructure.Repositories;

public class PedidoBeneficioRepository(BeneficiosDbContext context) : IPedidoBeneficioRepository
{
    public async Task<PedidoBeneficio?> ObterPorIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Pedidos.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<PedidoBeneficio>> ListarAsync(CancellationToken ct = default) =>
        await context.Pedidos.OrderByDescending(p => p.DataPedido).ToListAsync(ct);

    public async Task<IReadOnlyList<PedidoBeneficio>> ListarPorContribuinteAsync(
        Guid contribuinteId, CancellationToken ct = default) =>
        await context.Pedidos
            .Where(p => p.ContribuinteId == contribuinteId)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync(ct);

    public async Task AdicionarAsync(PedidoBeneficio pedido, CancellationToken ct = default) =>
        await context.Pedidos.AddAsync(pedido, ct);

    public async Task SalvarAlteracoesAsync(CancellationToken ct = default) =>
        await context.SaveChangesAsync(ct);
}
