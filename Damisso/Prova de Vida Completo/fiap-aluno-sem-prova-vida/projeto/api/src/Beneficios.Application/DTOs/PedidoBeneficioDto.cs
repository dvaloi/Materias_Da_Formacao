using Beneficios.Domain.Enums;

namespace Beneficios.Application.DTOs;

public record PedidoBeneficioDto(
    Guid Id,
    Guid ContribuinteId,
    string Tipo,
    decimal ValorSolicitado,
    string Status,
    string? MotivoRejeicao,
    DateTime DataPedido
);

public record CriarPedidoRequest(
    Guid ContribuinteId,
    TipoBeneficio Tipo,
    decimal ValorSolicitado
);

public record RejeitarPedidoRequest(string Motivo);
