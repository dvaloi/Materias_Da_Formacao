using System.Text.Json;
using ProvaVida.Domain.Exceptions;
using ProvaVida.Domain.Services;

namespace ProvaVida.Domain.Entities;

/// <summary>
/// Template biométrico de referência — cadastro inicial do pensionista.
/// Renovações comparam embedding facial 128D (face-api / FaceNet).
/// </summary>
public class PerfilBiometrico
{
    public Guid Id { get; private set; }
    public Guid ContribuinteId { get; private set; }
    public string Nuit { get; private set; } = string.Empty;
    public string FaceHashReferencia { get; private set; } = string.Empty;
    public string? FaceEmbeddingJson { get; private set; }
    public DateTime CadastradoEm { get; private set; }
    public DateTime AtualizadoEm { get; private set; }

    private PerfilBiometrico() { }

    public static PerfilBiometrico Criar(
        Guid contribuinteId,
        string nuit,
        string faceHash,
        float[] faceEmbedding)
    {
        if (contribuinteId == Guid.Empty)
            throw new DomainException("ContribuinteId é obrigatório.");
        if (string.IsNullOrWhiteSpace(faceHash))
            throw new DomainException("Hash biométrico de referência é obrigatório.");
        if (faceEmbedding is null || faceEmbedding.Length < 64)
            throw new DomainException("Embedding facial inválido — use reconhecimento real no app mobile.");

        var agora = DateTime.UtcNow;
        return new PerfilBiometrico
        {
            Id = Guid.NewGuid(),
            ContribuinteId = contribuinteId,
            Nuit = nuit,
            FaceHashReferencia = faceHash,
            FaceEmbeddingJson = JsonSerializer.Serialize(faceEmbedding),
            CadastradoEm = agora,
            AtualizadoEm = agora
        };
    }

    public IReadOnlyList<float>? ObterEmbedding()
    {
        if (string.IsNullOrWhiteSpace(FaceEmbeddingJson)) return null;
        return JsonSerializer.Deserialize<float[]>(FaceEmbeddingJson);
    }

    /// <summary>
    /// Consulta ou renovação: compara embedding com o template — não altera o cadastro.
    /// </summary>
    public decimal VerificarIdentidade(float[] novoEmbedding, decimal scoreLiveness)
    {
        if (scoreLiveness < 70m)
            throw new DomainException($"Liveness insuficiente ({scoreLiveness:F0}%). Mínimo: 70%.");

        var referencia = ObterEmbedding();
        if (referencia is null || referencia.Count == 0)
            throw new DomainException(
                "Cadastro sem template facial. Refaça o cadastro inicial pelo app mobile com reconhecimento facial.");

        if (novoEmbedding is null || novoEmbedding.Length < 64)
            throw new DomainException("Embedding facial é obrigatório para validar o rosto.");

        var similaridade = ComparadorFacial.SimilaridadeCosseno(referencia, novoEmbedding);

        if (similaridade < ComparadorFacial.LimiarMinimo)
            throw new DomainException(
                $"Rosto não confere com o cadastro ({similaridade * 100:F0}%). " +
                $"Similaridade mínima: {ComparadorFacial.LimiarMinimo * 100:F0}%.");

        return similaridade * 100m;
    }

    /// <summary>Renovação: valida identidade e registra o momento da checagem no perfil.</summary>
    public decimal ConfirmarIdentidadeNaRenovacao(float[] novoEmbedding, decimal scoreLiveness)
    {
        var similaridade = VerificarIdentidade(novoEmbedding, scoreLiveness);
        AtualizadoEm = DateTime.UtcNow;
        return similaridade;
    }
}
