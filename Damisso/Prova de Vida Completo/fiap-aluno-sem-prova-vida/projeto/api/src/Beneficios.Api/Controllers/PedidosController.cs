using Beneficios.Application.DTOs;
using Beneficios.Application.Interfaces;
using Beneficios.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beneficios.Api.Controllers;

// 2º microsserviço — pedidos referenciam ContribuinteId (não compartilham banco com Contribuintes).
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PedidosController(IPedidoBeneficioService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(await service.ListarAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var dto = await service.ObterPorIdAsync(id, ct);
        return dto is null ? NotFound(new { mensagem = "Pedido não encontrado." }) : Ok(dto);
    }

    [HttpGet("contribuinte/{contribuinteId:guid}")]
    public async Task<IActionResult> ListarPorContribuinte(Guid contribuinteId, CancellationToken ct) =>
        Ok(await service.ListarPorContribuinteAsync(contribuinteId, ct));

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarPedidoRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await service.CriarAsync(request, ct);
            return CreatedAtAction(nameof(Obter), new { id = dto.Id }, dto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/aprovar")]
    public async Task<IActionResult> Aprovar(Guid id, CancellationToken ct)
    {
        try
        {
            await service.AprovarAsync(id, ct);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/rejeitar")]
    public async Task<IActionResult> Rejeitar(Guid id, [FromBody] RejeitarPedidoRequest request, CancellationToken ct)
    {
        try
        {
            await service.RejeitarAsync(id, request, ct);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
