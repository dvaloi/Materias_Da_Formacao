using Contribuintes.Domain.Entities;
using Contribuintes.Domain.ValueObjects;

namespace Contribuintes.Domain.Repositories;

/// <summary>
/// Interface no Domain — Dependency Inversion (SOLID: D).
/// A implementação fica na Infrastructure.
/// </summary>
public interface IContribuinteRepository
{
    Task<Contribuinte?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<Contribuinte?> ObterPorNuitAsync(Nuit nuit, CancellationToken ct = default);
    Task<IReadOnlyList<Contribuinte>> ListarAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Contribuinte>> ListarPensionistasAsync(CancellationToken ct = default);
    Task<Contribuinte?> ObterPensionistaPorNuitAsync(Nuit nuit, CancellationToken ct = default);
    Task AdicionarAsync(Contribuinte contribuinte, CancellationToken ct = default);
    Task SalvarAlteracoesAsync(CancellationToken ct = default);
}
