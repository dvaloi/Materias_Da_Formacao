using Beneficios.Domain.Entities;
using Beneficios.Domain.Enums;
using Beneficios.Domain.Exceptions;

namespace Beneficios.UnitTests.Domain;

/// <summary>
/// M15 — Testes unitários do Domain (Benefícios). Padrão AAA.
/// Sem HTTP, sem banco: só regras do pedido (criar, aprovar, rejeitar).
/// </summary>
public class PedidoBeneficioTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarPedidoPendente()
    {
        // Arrange + Act
        var pedido = PedidoBeneficio.Criar(Guid.NewGuid(), TipoBeneficio.Reforma, 15000m);

        // Assert
        Assert.Equal(StatusPedido.Pendente, pedido.Status);
        Assert.Equal(TipoBeneficio.Reforma, pedido.Tipo);
    }

    [Fact]
    public void Aprovar_PedidoPendente_DeveAlterarStatus()
    {
        // Arrange
        var pedido = PedidoBeneficio.Criar(Guid.NewGuid(), TipoBeneficio.Pensao, 8000m);

        // Act
        pedido.IniciarAnalise();
        pedido.Aprovar();

        // Assert
        Assert.Equal(StatusPedido.Aprovado, pedido.Status);
    }

    [Fact]
    public void Rejeitar_SemMotivo_DeveLancarExcecao()
    {
        // Arrange
        var pedido = PedidoBeneficio.Criar(Guid.NewGuid(), TipoBeneficio.Doenca, 5000m);
        pedido.IniciarAnalise();

        // Act + Assert — regra de negócio: motivo obrigatório
        Assert.Throws<DomainException>(() => pedido.Rejeitar(""));
    }
}
