namespace ProvaVida.Domain.Enums;

public enum TipoOperacaoProvaVida
{
    Cadastro,
    /// <summary>Consulta com câmera enquanto a prova ainda é válida — não renova.</summary>
    Consulta,
    Renovacao
}
