using InssApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace InssApi.Controllers;

/// <summary>
/// Exemplo completo do Passo 9 — para o professor mostrar se os alunos travarem.
/// Mesmo padrão do ContribuintesController.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private static readonly List<PedidoBeneficio> _store = new();

    [HttpGet]
    public IActionResult Get()
        => Ok(_store);

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var p = _store.FirstOrDefault(x => x.Id == id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CriarPedidoRequest req)
    {
        if (req.ContribuinteId <= 0)
            return BadRequest("ContribuinteId inválido.");

        if (string.IsNullOrWhiteSpace(req.Tipo))
            return BadRequest("Tipo é obrigatório (ex.: Velhice ou Invalidez).");

        var p = new PedidoBeneficio
        {
            Id = _store.Count == 0 ? 1 : _store.Max(x => x.Id) + 1,
            ContribuinteId = req.ContribuinteId,
            Tipo = req.Tipo.Trim(),
            Status = "Aberto"
        };
        _store.Add(p);
        return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CriarPedidoRequest req)
    {
        var p = _store.FirstOrDefault(x => x.Id == id);
        if (p is null) return NotFound();

        if (req.ContribuinteId <= 0)
            return BadRequest("ContribuinteId inválido.");

        if (string.IsNullOrWhiteSpace(req.Tipo))
            return BadRequest("Tipo é obrigatório.");

        p.ContribuinteId = req.ContribuinteId;
        p.Tipo = req.Tipo.Trim();
        return Ok(p);
    }

    [HttpPatch("{id:int}/status")]
    public IActionResult PatchStatus(int id, [FromBody] string status)
    {
        var p = _store.FirstOrDefault(x => x.Id == id);
        if (p is null) return NotFound();

        if (string.IsNullOrWhiteSpace(status))
            return BadRequest("Status é obrigatório.");

        p.Status = status.Trim();
        return Ok(p);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var p = _store.FirstOrDefault(x => x.Id == id);
        if (p is null) return NotFound();

        _store.Remove(p);
        return NoContent();
    }
}
