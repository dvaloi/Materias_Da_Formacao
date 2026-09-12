using Beneficios.Application.DTOs;

namespace Beneficios.Application.Interfaces;

public interface IPedidoBeneficioService
{
    Task<IReadOnlyList<PedidoBeneficioDto>> ListarAsync(CancellationToken ct = default);
    Task<PedidoBeneficioDto?> ObterPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<PedidoBeneficioDto>> ListarPorContribuinteAsync(Guid contribuinteId, CancellationToken ct = default);
    Task<PedidoBeneficioDto> CriarAsync(CriarPedidoRequest request, CancellationToken ct = default);
    Task AprovarAsync(Guid id, CancellationToken ct = default);
    Task RejeitarAsync(Guid id, RejeitarPedidoRequest request, CancellationToken ct = default);
}
