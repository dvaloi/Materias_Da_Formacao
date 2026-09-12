namespace ProvaVida.Domain.Services;

/**
 * Comparação 1:1 de embeddings faciais (ex.: FaceNet / face-api 128D).
 *
 * TODO (produção INSS Moçambique — SDK pago):
 * Quando o app usar FaceTec / iProov / AWS Rekognition Liveness (ou similar),
 * a comparação 1:1 pode passar a ser feita pelo provedor (session + match score).
 * Neste caso, adaptar ValidarEmbedding / limiar aqui ou delegar a um
 * IComparadorFacial injetável, sem mudar as regras de Cadastro / Consulta / Renovação.
 */
public static class ComparadorFacial
{
    /// <summary>Similaridade mínima para mesma pessoa (0–1). Endurecido para demo INSS.</summary>
    public const decimal LimiarMinimo = 0.72m;

    public static decimal SimilaridadeCosseno(IReadOnlyList<float> referencia, IReadOnlyList<float> candidato)
    {
        if (referencia.Count == 0 || candidato.Count == 0)
            return 0m;

        if (referencia.Count != candidato.Count)
            throw new ArgumentException("Embeddings com dimensões diferentes.");

        double dot = 0, normA = 0, normB = 0;
        for (var i = 0; i < referencia.Count; i++)
        {
            var a = referencia[i];
            var b = candidato[i];
            dot += a * b;
            normA += a * a;
            normB += b * b;
        }

        if (normA == 0 || normB == 0) return 0m;

        var cos = dot / (Math.Sqrt(normA) * Math.Sqrt(normB));
        cos = Math.Clamp(cos, -1, 1);
        return Math.Round((decimal)cos, 4);
    }
}
