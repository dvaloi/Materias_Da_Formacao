using Contribuintes.Application.DTOs;
using Contribuintes.Domain.Entities;

namespace Contribuintes.Application.Mappings;

public static class ContribuinteMapper
{
    public static ContribuinteDto ParaDto(Contribuinte entity) =>
        new(
            entity.Id,
            entity.Nuit.Valor,
            entity.Nome,
            entity.DataNascimento,
            entity.SalarioMensal,
            entity.Situacao.ToString(),
            entity.Perfil.ToString(),
            entity.CalcularContribuicaoMensal()
        );

    public static PensionistaResumoDto ParaPensionistaResumo(Contribuinte entity) =>
        new(entity.Id, entity.Nuit.Valor, entity.Nome, entity.DataNascimento);
}
