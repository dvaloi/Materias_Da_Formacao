using Contribuintes.Application.DTOs;
using Contribuintes.Application.Interfaces;
using Contribuintes.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Contribuintes.Api.Controllers;

// Controller "fino": só HTTP. Regras de negócio ficam no Domain / Application (DDD).
// [Authorize] = rota protegida (par no front: ProtectedRoute + JWT no api.js).
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContribuintesController(IContribuinteService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ContribuinteDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(await service.ListarAsync(ct));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ContribuinteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(Guid id, CancellationToken ct)
    {
        var dto = await service.ObterPorIdAsync(id, ct);
        return dto is null ? NotFound(new { mensagem = "Contribuinte não encontrado." }) : Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContribuinteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Criar([FromBody] CriarContribuinteRequest request, CancellationToken ct)
    {
        try
        {
            // Application orquestra; Domain valida (NUIT, salário…). DomainException → 400.
            var dto = await service.CriarAsync(request, ct);
            return CreatedAtAction(nameof(Obter), new { id = dto.Id }, dto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPatch("{id:guid}/desativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Desativar(Guid id, CancellationToken ct)
    {
        try
        {
            await service.DesativarAsync(id, ct);
            return NoContent();
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }
}
