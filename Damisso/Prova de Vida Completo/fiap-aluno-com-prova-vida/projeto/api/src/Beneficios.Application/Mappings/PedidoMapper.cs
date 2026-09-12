using Beneficios.Application.DTOs;
using Beneficios.Domain.Entities;

namespace Beneficios.Application.Mappings;

public static class PedidoMapper
{
    public static PedidoBeneficioDto ParaDto(PedidoBeneficio entity) =>
        new(
            entity.Id,
            entity.ContribuinteId,
            entity.Tipo.ToString(),
            entity.ValorSolicitado,
            entity.Status.ToString(),
            entity.MotivoRejeicao,
            entity.DataPedido
        );
}
