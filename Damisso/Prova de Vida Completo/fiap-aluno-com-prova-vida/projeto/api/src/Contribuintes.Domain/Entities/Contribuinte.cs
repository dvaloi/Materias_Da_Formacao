using Contribuintes.Domain.Enums;
using Contribuintes.Domain.Exceptions;
using Contribuintes.Domain.ValueObjects;

namespace Contribuintes.Domain.Entities;

/// <summary>
/// Aggregate Root — trabalhador registrado no INSS Moçambique.
/// Encapsula regras (DDD): criação, contribuição 3%, desativação, elegibilidade reforma.
/// Controllers e Services chamam estes métodos — não recalculam a regra “na mão”.
/// </summary>
public class Contribuinte
{
    public Guid Id { get; private set; }
    public Nuit Nuit { get; private set; } = null!;
    public string Nome { get; private set; } = string.Empty;
    public DateOnly DataNascimento { get; private set; }
    public decimal SalarioMensal { get; private set; }
    public SituacaoContribuinte Situacao { get; private set; }
    public PerfilContribuinte Perfil { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private Contribuinte() { }

    public static Contribuinte Criar(Nuit nuit, string nome, DateOnly dataNascimento, decimal salarioMensal)
    {
        ValidarNome(nome);
        ValidarIdade(dataNascimento);
        ValidarSalario(salarioMensal);

        return new Contribuinte
        {
            Id = Guid.NewGuid(),
            Nuit = nuit,
            Nome = nome.Trim(),
            DataNascimento = dataNascimento,
            SalarioMensal = salarioMensal,
            Situacao = SituacaoContribuinte.Ativo,
            Perfil = PerfilContribuinte.TrabalhadorAtivo,
            CriadoEm = DateTime.UtcNow
        };
    }

    public static Contribuinte CriarPensionista(Nuit nuit, string nome, DateOnly dataNascimento, decimal valorPensao)
    {
        ValidarNome(nome);
        ValidarIdadePensionista(dataNascimento);
        ValidarSalario(valorPensao);

        return new Contribuinte
        {
            Id = Guid.NewGuid(),
            Nuit = nuit,
            Nome = nome.Trim(),
            DataNascimento = dataNascimento,
            SalarioMensal = valorPensao,
            Situacao = SituacaoContribuinte.Ativo,
            Perfil = PerfilContribuinte.Pensionista,
            CriadoEm = DateTime.UtcNow
        };
    }

    public void MarcarComoPensionista()
    {
        ValidarIdadePensionista(DataNascimento);
        Perfil = PerfilContribuinte.Pensionista;
    }

    public bool EhPensionista() => Perfil == PerfilContribuinte.Pensionista;

    public void AtualizarSalario(decimal novoSalario)
    {
        ValidarSalario(novoSalario);
        SalarioMensal = novoSalario;
    }

    public void Desativar()
    {
        if (Situacao == SituacaoContribuinte.Inativo)
            throw new DomainException("Contribuinte já está inativo.");

        Situacao = SituacaoContribuinte.Inativo;
    }

    public decimal CalcularContribuicaoMensal()
    {
        // Regra didática INSS: contribuição = 3% do salário (MZN)
        const decimal TaxaContribuicao = 0.03m;
        return Math.Round(SalarioMensal * TaxaContribuicao, 2);
    }

    public bool ElegivelParaReforma(DateOnly dataReferencia)
    {
        var idade = dataReferencia.Year - DataNascimento.Year;
        if (dataReferencia < DataNascimento.AddYears(idade)) idade--;
        return idade >= 60 && Situacao == SituacaoContribuinte.Ativo;
    }

    private static void ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Trim().Length < 3)
            throw new DomainException("Nome deve ter pelo menos 3 caracteres.");
    }

    private static void ValidarIdade(DateOnly dataNascimento)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var idade = hoje.Year - dataNascimento.Year;
        if (hoje < dataNascimento.AddYears(idade)) idade--;

        if (idade < 16)
            throw new DomainException("Contribuinte deve ter pelo menos 16 anos.");
    }

    private static void ValidarSalario(decimal salario)
    {
        if (salario <= 0)
            throw new DomainException("Salário mensal deve ser maior que zero.");
    }

    private static void ValidarIdadePensionista(DateOnly dataNascimento)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var idade = hoje.Year - dataNascimento.Year;
        if (hoje < dataNascimento.AddYears(idade)) idade--;

        if (idade < 60)
            throw new DomainException("Pensionista deve ter pelo menos 60 anos.");
    }
}
