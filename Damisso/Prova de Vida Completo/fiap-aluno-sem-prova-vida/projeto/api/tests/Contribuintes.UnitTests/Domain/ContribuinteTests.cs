using Contribuintes.Domain.Entities;
using Contribuintes.Domain.Exceptions;
using Contribuintes.Domain.ValueObjects;

namespace Contribuintes.UnitTests.Domain;

/// <summary>
/// Testes unitários do Domain — padrão AAA (Arrange, Act, Assert).
/// M15: Testes rápidos, sem dependências externas.
/// </summary>
public class ContribuinteTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarContribuinte()
    {
        // Arrange
        var nuit = Nuit.Criar("123456789");

        // Act
        var contribuinte = Contribuinte.Criar(nuit, "Carlos Manuel", new DateOnly(1985, 3, 15), 45000m);

        // Assert
        Assert.Equal("Carlos Manuel", contribuinte.Nome);
        Assert.Equal(45000m, contribuinte.SalarioMensal);
        Assert.Equal(1350m, contribuinte.CalcularContribuicaoMensal()); // 3% de 45000
    }

    [Fact]
    public void Criar_ComNuitInvalido_DeveLancarDomainException()
    {
        Assert.Throws<DomainException>(() => Nuit.Criar("123"));
    }

    [Fact]
    public void Criar_ComSalarioZero_DeveLancarDomainException()
    {
        var nuit = Nuit.Criar("123456789");
        Assert.Throws<DomainException>(() =>
            Contribuinte.Criar(nuit, "Teste", new DateOnly(1990, 1, 1), 0));
    }

    [Fact]
    public void ElegivelParaReforma_Com60Anos_DeveRetornarTrue()
    {
        var nuit = Nuit.Criar("123456789");
        var contribuinte = Contribuinte.Criar(nuit, "Reformado", new DateOnly(1960, 1, 1), 50000m);

        Assert.True(contribuinte.ElegivelParaReforma(new DateOnly(2026, 8, 25)));
    }

    [Fact]
    public void Desativar_ContribuinteAtivo_DeveAlterarSituacao()
    {
        var nuit = Nuit.Criar("123456789");
        var contribuinte = Contribuinte.Criar(nuit, "Teste", new DateOnly(1990, 1, 1), 30000m);

        contribuinte.Desativar();

        Assert.Equal(Contribuintes.Domain.Enums.SituacaoContribuinte.Inativo, contribuinte.Situacao);
    }
}
