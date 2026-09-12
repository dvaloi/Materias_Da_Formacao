using Contribuintes.Application.DTOs;
using Contribuintes.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Contribuintes.Api.Controllers;

/// <summary>
/// Endpoints públicos para o app mobile de Prova de Vida (demo local).
/// TODO: autenticação dedicada (certificado / OAuth device) antes de produção.
/// </summary>
[ApiController]
[Route("api/contribuintes/pensionistas")]
[AllowAnonymous]
public class PensionistasController(IContribuinteService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PensionistaResumoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(CancellationToken ct) =>
        Ok(await service.ListarPensionistasAsync(ct));

    [HttpGet("por-nuit/{nuit}")]
    [ProducesResponseType(typeof(PensionistaResumoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorNuit(string nuit, CancellationToken ct)
    {
        var dto = await service.ObterPensionistaPorNuitAsync(nuit, ct);
        return dto is null
            ? NotFound(new { mensagem = "Pensionista não encontrado para este NUIT." })
            : Ok(dto);
    }
}
