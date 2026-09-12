using Beneficios.Application.DTOs;
using Beneficios.Application.Interfaces;
using Beneficios.Application.Mappings;
using Beneficios.Domain.Entities;
using Beneficios.Domain.Exceptions;
using Beneficios.Domain.Repositories;

namespace Beneficios.Application.Services;

public class PedidoBeneficioService(IPedidoBeneficioRepository repository) : IPedidoBeneficioService
{
    public async Task<IReadOnlyList<PedidoBeneficioDto>> ListarAsync(CancellationToken ct = default)
    {
        var lista = await repository.ListarAsync(ct);
        return lista.Select(PedidoMapper.ParaDto).ToList();
    }

    public async Task<PedidoBeneficioDto?> ObterPorIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await repository.ObterPorIdAsync(id, ct);
        return entity is null ? null : PedidoMapper.ParaDto(entity);
    }

    public async Task<IReadOnlyList<PedidoBeneficioDto>> ListarPorContribuinteAsync(
        Guid contribuinteId, CancellationToken ct = default)
    {
        var lista = await repository.ListarPorContribuinteAsync(contribuinteId, ct);
        return lista.Select(PedidoMapper.ParaDto).ToList();
    }

    public async Task<PedidoBeneficioDto> CriarAsync(CriarPedidoRequest request, CancellationToken ct = default)
    {
        var pedido = PedidoBeneficio.Criar(request.ContribuinteId, request.Tipo, request.ValorSolicitado);
        await repository.AdicionarAsync(pedido, ct);
        await repository.SalvarAlteracoesAsync(ct);
        return PedidoMapper.ParaDto(pedido);
    }

    public async Task AprovarAsync(Guid id, CancellationToken ct = default)
    {
        var pedido = await repository.ObterPorIdAsync(id, ct)
            ?? throw new DomainException("Pedido não encontrado.");

        pedido.IniciarAnalise();
        pedido.Aprovar();
        await repository.SalvarAlteracoesAsync(ct);
    }

    public async Task RejeitarAsync(Guid id, RejeitarPedidoRequest request, CancellationToken ct = default)
    {
        var pedido = await repository.ObterPorIdAsync(id, ct)
            ?? throw new DomainException("Pedido não encontrado.");

        pedido.IniciarAnalise();
        pedido.Rejeitar(request.Motivo);
        await repository.SalvarAlteracoesAsync(ct);
    }
}
