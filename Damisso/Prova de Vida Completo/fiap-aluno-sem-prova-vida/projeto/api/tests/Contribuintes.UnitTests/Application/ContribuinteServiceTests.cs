using Contribuintes.Application.DTOs;
using Contribuintes.Application.Services;
using Contribuintes.Domain.Entities;
using Contribuintes.Domain.Repositories;
using Contribuintes.Domain.ValueObjects;
using Moq;

namespace Contribuintes.UnitTests.Application;

/// <summary>
/// M15 — Testes com Moq: isolamos o Service do Repository (SOLID: D).
/// Arrange / Act / Assert em cada método.
/// </summary>
public class ContribuinteServiceTests
{
    private readonly Mock<IContribuinteRepository> _repositoryMock = new();
    private readonly ContribuinteService _service;

    public ContribuinteServiceTests()
    {
        _service = new ContribuinteService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CriarAsync_ComNuitDuplicado_DeveLancarExcecao()
    {
        // Arrange
        var nuit = Nuit.Criar("123456789");
        var existente = Contribuinte.Criar(nuit, "Existente", new DateOnly(1980, 1, 1), 40000m);

        _repositoryMock.Setup(r => r.ObterPorNuitAsync(nuit, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existente);

        var request = new CriarContribuinteRequest("123456789", "Novo", new DateOnly(1990, 1, 1), 30000m);

        // Act & Assert
        await Assert.ThrowsAsync<Contribuintes.Domain.Exceptions.DomainException>(
            () => _service.CriarAsync(request));
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarListaMapeada()
    {
        // Arrange
        var nuit = Nuit.Criar("123456789");
        var lista = new List<Contribuinte>
        {
            Contribuinte.Criar(nuit, "Carlos", new DateOnly(1985, 3, 15), 45000m)
        };

        _repositoryMock.Setup(r => r.ListarAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(lista);

        // Act
        var resultado = await _service.ListarAsync();

        // Assert
        Assert.Single(resultado);
        Assert.Equal("123456789", resultado[0].Nuit);
        Assert.Equal(1350m, resultado[0].ContribuicaoMensal);
    }
}
