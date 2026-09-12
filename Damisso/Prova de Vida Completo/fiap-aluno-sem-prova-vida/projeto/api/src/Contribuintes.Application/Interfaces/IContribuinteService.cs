using Contribuintes.Application.DTOs;

namespace Contribuintes.Application.Interfaces;

/// <summary>
/// Single Responsibility (SOLID: S) — apenas operações de contribuintes.
/// </summary>
public interface IContribuinteService
{
    Task<IReadOnlyList<ContribuinteDto>> ListarAsync(CancellationToken ct = default);
    Task<ContribuinteDto?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<ContribuinteDto> CriarAsync(CriarContribuinteRequest request, CancellationToken ct = default);
    Task DesativarAsync(Guid id, CancellationToken ct = default);
}

public interface IAuthService
{
    Task<LoginResponse?> AutenticarAsync(LoginRequest request, CancellationToken ct = default);
}
