using InssApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace InssApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContribuintesController : ControllerBase
{
    // Lista em memória (some se fechar a API)
    private static readonly List<Contribuinte> _store = new();

    [HttpGet]
    public IActionResult Get()
        => Ok(_store);

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var c = _store.FirstOrDefault(x => x.Id == id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CriarContribuinteRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Nuit) || req.Nuit.Length != 9)
            return BadRequest("NUIT deve ter 9 dígitos.");

        if (string.IsNullOrWhiteSpace(req.Nome))
            return BadRequest("Nome é obrigatório.");

        var c = new Contribuinte
        {
            Id = _store.Count == 0 ? 1 : _store.Max(x => x.Id) + 1,
            Nuit = req.Nuit,
            Nome = req.Nome
        };
        _store.Add(c);
        return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] CriarContribuinteRequest req)
    {
        var c = _store.FirstOrDefault(x => x.Id == id);
        if (c is null) return NotFound();

        c.Nuit = req.Nuit;
        c.Nome = req.Nome;
        return Ok(c);
    }

    [HttpPatch("{id:int}/status")]
    public IActionResult PatchStatus(int id, [FromBody] string status)
    {
        var c = _store.FirstOrDefault(x => x.Id == id);
        if (c is null) return NotFound();

        c.Status = status;
        return Ok(c);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var c = _store.FirstOrDefault(x => x.Id == id);
        if (c is null) return NotFound();

        _store.Remove(c);
        return NoContent();
    }
}
