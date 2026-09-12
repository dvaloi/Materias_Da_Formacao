using Beneficios.Domain.Entities;

namespace Beneficios.Domain.Repositories;

public interface IPedidoBeneficioRepository
{
    Task<PedidoBeneficio?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<PedidoBeneficio>> ListarAsync(CancellationToken ct = default);
    Task<IReadOnlyList<PedidoBeneficio>> ListarPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default);
    Task AdicionarAsync(PedidoBeneficio pedido, CancellationToken ct = default);
    Task SalvarAlteracoesAsync(CancellationToken ct = default);
}
