using Contribuintes.Domain.Exceptions;

namespace Contribuintes.Domain.ValueObjects;

/// <summary>
/// Value Object — Número Único de Identificação Tributária (Moçambique).
/// Imutável, comparado por valor, não por referência (DDD).
/// </summary>
public sealed record Nuit
{
    public string Valor { get; }

    private Nuit(string valor) => Valor = valor;

    public static Nuit Criar(string valor)
    {
        // Mesma regra do front (ContribuinteForm) — aqui é a validação que realmente protege a API
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("NUIT é obrigatório.");

        var limpo = valor.Trim().Replace(" ", "");

        if (limpo.Length != 9 || !limpo.All(char.IsDigit))
            throw new DomainException("NUIT deve conter exatamente 9 dígitos numéricos.");

        return new Nuit(limpo);
    }

    public override string ToString() => Valor;
}
