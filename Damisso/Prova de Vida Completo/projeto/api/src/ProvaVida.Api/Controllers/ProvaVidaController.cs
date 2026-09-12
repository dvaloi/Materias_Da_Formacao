using ProvaVida.Application.DTOs;
using ProvaVida.Application.Interfaces;
using ProvaVida.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProvaVida.Api.Controllers;

[ApiController]
[Route("api/prova-vida")]
[AllowAnonymous]
public class ProvaVidaController(IProvaVidaService service) : ControllerBase
{
    /// <summary>1ª vez — cadastro biométrico (só quando ainda não existe).</summary>
    [HttpPost("cadastrar")]
    [ProducesResponseType(typeof(RegistroProvaVidaDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Cadastrar([FromBody] RegistrarProvaVidaRequest request, CancellationToken ct)
    {
        try
        {
            var dto = await service.CadastrarAsync(request, ct);
            return CreatedAtAction(nameof(ObterUltima), new { contribuinteId = dto.ContribuinteId }, dto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Renovação — bate com cadastro existente e atualiza o mesmo registro.</summary>
    [HttpPost("renovar")]
    [ProducesResponseType(typeof(RegistroProvaVidaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Renovar([FromBody] RegistrarProvaVidaRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await service.RenovarAsync(request, ct));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>
    /// Consulta com câmera (prova ainda válida) — autentica rosto + liveness.
    /// Não cria cadastro nem renova validade (estilo banco/gov).
    /// </summary>
    [HttpPost("consultar")]
    [ProducesResponseType(typeof(RegistroProvaVidaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Consultar([FromBody] RegistrarProvaVidaRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await service.ConsultarAsync(request, ct));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    /// <summary>Compatibilidade — cadastrar / consultar / renovar conforme status.</summary>
    [HttpPost("registrar")]
    [ProducesResponseType(typeof(RegistroProvaVidaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Registrar([FromBody] RegistrarProvaVidaRequest request, CancellationToken ct)
    {
        try
        {
            var status = await service.ObterStatusAcessoAsync(request.Nuit, ct);
            if (status.RequerCadastroInicial)
                return Ok(await service.CadastrarAsync(request, ct));
            if (status.RequerRenovacao)
                return Ok(await service.RenovarAsync(request, ct));
            if (status.ProvaValida)
                return Ok(await service.ConsultarAsync(request, ct));
            return BadRequest(new { mensagem = "Estado inválido. Consulte GET status/por-nuit." });
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpGet("ultima/{contribuinteId:guid}")]
    [ProducesResponseType(typeof(RegistroProvaVidaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterUltima(Guid contribuinteId, CancellationToken ct)
    {
        var dto = await service.ObterUltimaAsync(contribuinteId, ct);
        return dto is null
            ? NotFound(new { mensagem = "Nenhuma prova de vida cadastrada." })
            : Ok(dto);
    }

    [HttpGet("historico/{contribuinteId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<RegistroProvaVidaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarHistorico(Guid contribuinteId, CancellationToken ct) =>
        Ok(await service.ListarPorContribuinteAsync(contribuinteId, ct));

    [HttpGet("status/por-nuit/{nuit}")]
    [ProducesResponseType(typeof(StatusAcessoBeneficioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ObterStatus(string nuit, CancellationToken ct)
    {
        try
        {
            return Ok(await service.ObterStatusAcessoAsync(nuit, ct));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message });
        }
    }

    [HttpPost("validar-acesso")]
    [ProducesResponseType(typeof(StatusAcessoBeneficioDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ValidarAcesso([FromBody] ValidarAcessoBeneficioRequest request, CancellationToken ct)
    {
        try
        {
            return Ok(await service.ValidarAcessoBeneficioAsync(request.Nuit, ct));
        }
        catch (DomainException ex)
        {
            return BadRequest(new { mensagem = ex.Message, codigo = "PROVA_VIDA_INVALIDA" });
        }
    }
}
