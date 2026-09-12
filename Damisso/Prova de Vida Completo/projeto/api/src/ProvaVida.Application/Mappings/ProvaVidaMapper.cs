using ProvaVida.Application.DTOs;

using ProvaVida.Domain.Entities;

using ProvaVida.Domain.Enums;



namespace ProvaVida.Application.Mappings;



public static class ProvaVidaMapper

{

    public static RegistroProvaVidaDto ParaDto(

        RegistroProvaVida entity,

        TipoOperacaoProvaVida tipo,

        bool identidadeConfirmada,

        decimal? similaridadeFacial = null) =>

        new(

            entity.Id,

            entity.ContribuinteId,

            entity.Nuit,

            entity.RealizadoEm,

            entity.ScoreLiveness,

            entity.MovimentosDetectados,

            entity.Status.ToString(),

            entity.Observacao,

            tipo.ToString(),

            identidadeConfirmada,

            similaridadeFacial

        );

}

