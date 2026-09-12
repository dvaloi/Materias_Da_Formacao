namespace ProvaVida.Application.DTOs;

public record RegistroProvaVidaDto(
    Guid Id,
    Guid ContribuinteId,
    string Nuit,
    DateTime RealizadoEm,
    decimal ScoreLiveness,
    bool MovimentosDetectados,
    string Status,
    string? Observacao,
    string TipoOperacao,
    bool IdentidadeConfirmada,
    decimal? SimilaridadeFacial
);

public record StatusAcessoBeneficioDto(
    Guid ContribuinteId,
    string Nuit,
    string Nome,
    bool PodeAcessarBeneficio,
    bool ProvaValida,
    bool PossuiProvaRegistrada,
    bool PossuiCadastroBiometrico,
    bool RequerCadastroInicial,
    bool RequerRenovacao,
    string ProximaAcao,
    DateTime? UltimaProvaEm,
    DateTime? ValidaAte,
    int DiasRestantes,
    int MesesValidade,
    string? MotivoBloqueio,
    string? AcaoRequerida
);

public record ValidarAcessoBeneficioRequest(string Nuit);

public record RegistrarProvaVidaRequest(
    string Nuit,
    decimal ScoreLiveness,
    bool MovimentosDetectados,
    string ImagemBase64,
    float[]? FaceEmbedding = null
);

public record PensionistaExternoDto(
    Guid Id,
    string Nuit,
    string Nome,
    DateOnly DataNascimento
);
