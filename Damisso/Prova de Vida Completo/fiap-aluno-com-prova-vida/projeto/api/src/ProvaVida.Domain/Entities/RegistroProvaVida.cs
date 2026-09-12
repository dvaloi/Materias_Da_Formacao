using ProvaVida.Domain.Enums;
using ProvaVida.Domain.Exceptions;

namespace ProvaVida.Domain.Entities;

/// <summary>
/// Registro de Prova de Vida — referencia pensionista por GUID (sem FK cross-database).
/// </summary>
public class RegistroProvaVida
{
    public Guid Id { get; private set; }
    public Guid ContribuinteId { get; private set; }
    public string Nuit { get; private set; } = string.Empty;
    public DateTime RealizadoEm { get; private set; }
    public decimal ScoreLiveness { get; private set; }
    public string FaceHash { get; private set; } = string.Empty;
    public bool MovimentosDetectados { get; private set; }
    public StatusProvaVida Status { get; private set; }
    public string? Observacao { get; private set; }

    private RegistroProvaVida() { }

    public static RegistroProvaVida Registrar(
        Guid contribuinteId,
        string nuit,
        decimal scoreLiveness,
        string faceHash,
        bool movimentosDetectados,
        decimal scoreMinimo)
    {
        if (contribuinteId == Guid.Empty)
            throw new DomainException("ContribuinteId é obrigatório.");

        if (string.IsNullOrWhiteSpace(nuit) || nuit.Length != 9)
            throw new DomainException("NUIT inválido.");

        if (string.IsNullOrWhiteSpace(faceHash))
            throw new DomainException("Hash biométrico é obrigatório.");

        if (!movimentosDetectados)
            throw new DomainException("Liveness não confirmado — movimentos faciais não detectados.");

        if (scoreLiveness < scoreMinimo)
            throw new DomainException($"Score de liveness insuficiente ({scoreLiveness:F0}%). Mínimo: {scoreMinimo:F0}%.");

        return new RegistroProvaVida
        {
            Id = Guid.NewGuid(),
            ContribuinteId = contribuinteId,
            Nuit = nuit,
            RealizadoEm = DateTime.UtcNow,
            ScoreLiveness = scoreLiveness,
            FaceHash = faceHash,
            MovimentosDetectados = movimentosDetectados,
            Status = StatusProvaVida.Concluida
        };
    }

    /// <summary>
    /// Renovação — atualiza o mesmo registro (não cria cadastro novo).
    /// </summary>
    public void Renovar(decimal scoreLiveness, string faceHash, bool movimentosDetectados, decimal scoreMinimo)
    {
        if (!movimentosDetectados)
            throw new DomainException("Liveness não confirmado — movimentos faciais não detectados.");

        if (scoreLiveness < scoreMinimo)
            throw new DomainException($"Score de liveness insuficiente ({scoreLiveness:F0}%). Mínimo: {scoreMinimo:F0}%.");

        if (string.IsNullOrWhiteSpace(faceHash))
            throw new DomainException("Hash biométrico é obrigatório.");

        RealizadoEm = DateTime.UtcNow;
        ScoreLiveness = scoreLiveness;
        FaceHash = faceHash;
        MovimentosDetectados = movimentosDetectados;
        Status = StatusProvaVida.Concluida;
        Observacao = "Renovação";
    }
}
