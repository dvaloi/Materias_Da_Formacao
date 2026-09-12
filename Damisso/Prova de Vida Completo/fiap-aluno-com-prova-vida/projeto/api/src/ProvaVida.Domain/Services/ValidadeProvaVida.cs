namespace ProvaVida.Domain.Services;

/// <summary>
/// Regra INSS didática: prova de vida válida por N meses; depois bloqueia acesso ao benefício.
/// </summary>
public static class ValidadeProvaVida
{
    public const int MesesPadrao = 12;

    public static bool EstaValida(DateTime realizadoEm, DateTime referenciaUtc, int mesesValidade)
    {
        if (mesesValidade <= 0) return false;
        return realizadoEm.AddMonths(mesesValidade) >= referenciaUtc;
    }

    public static DateTime CalcularValidoAte(DateTime realizadoEm, int mesesValidade) =>
        realizadoEm.AddMonths(mesesValidade);

    public static int DiasRestantes(DateTime realizadoEm, DateTime referenciaUtc, int mesesValidade)
    {
        var validoAte = CalcularValidoAte(realizadoEm, mesesValidade);
        var dias = (int)Math.Ceiling((validoAte - referenciaUtc).TotalDays);
        return Math.Max(0, dias);
    }
}
