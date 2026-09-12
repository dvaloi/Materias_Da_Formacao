using Contribuintes.Application.DTOs;
using Contribuintes.Application.Interfaces;
using Contribuintes.Application.Mappings;
using Contribuintes.Domain.Entities;
using Contribuintes.Domain.Exceptions;
using Contribuintes.Domain.Repositories;
using Contribuintes.Domain.ValueObjects;

namespace Contribuintes.Application.Services;

// Application: orquestra Domain + repositório. Não conhece HTTP nem EF Core.
public class ContribuinteService(IContribuinteRepository repository) : IContribuinteService
{
    public async Task<IReadOnlyList<ContribuinteDto>> ListarAsync(CancellationToken ct = default)
    {
        var lista = await repository.ListarAsync(ct);
        return lista.Select(ContribuinteMapper.ParaDto).ToList();
    }

    public async Task<ContribuinteDto?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repository.ObterPorIdAsync(id, ct);
        return entity is null ? null : ContribuinteMapper.ParaDto(entity);
    }

    public async Task<ContribuinteDto> CriarAsync(CriarContribuinteRequest request, CancellationToken ct = default)
    {
        // Value Object NUIT valida formato; Domain Contribuinte.Criar valida nome/idade/salário
        var nuit = Nuit.Criar(request.Nuit);

        var existente = await repository.ObterPorNuitAsync(nuit, ct);
        if (existente is not null)
            throw new DomainException($"Já existe contribuinte com NUIT {nuit}.");

        var contribuinte = Contribuinte.Criar(nuit, request.Nome, request.DataNascimento, request.SalarioMensal);
        await repository.AdicionarAsync(contribuinte, ct);
        await repository.SalvarAlteracoesAsync(ct);

        return ContribuinteMapper.ParaDto(contribuinte);
    }

    public async Task DesativarAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repository.ObterPorIdAsync(id, ct)
            ?? throw new DomainException("Contribuinte não encontrado.");

        entity.Desativar();
        await repository.SalvarAlteracoesAsync(ct);
    }
}
