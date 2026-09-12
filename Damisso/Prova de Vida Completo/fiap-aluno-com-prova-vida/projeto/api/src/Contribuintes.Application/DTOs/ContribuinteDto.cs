namespace Contribuintes.Application.DTOs;

public record ContribuinteDto(
    Guid Id,
    string Nuit,
    string Nome,
    DateOnly DataNascimento,
    decimal SalarioMensal,
    string Situacao,
    string Perfil,
    decimal ContribuicaoMensal
);

public record PensionistaResumoDto(
    Guid Id,
    string Nuit,
    string Nome,
    DateOnly DataNascimento
);

public record CriarContribuinteRequest(
    string Nuit,
    string Nome,
    DateOnly DataNascimento,
    decimal SalarioMensal
);

public record LoginRequest(string Email, string Senha);

public record LoginResponse(string Token, string Nome, DateTime ExpiraEm);
