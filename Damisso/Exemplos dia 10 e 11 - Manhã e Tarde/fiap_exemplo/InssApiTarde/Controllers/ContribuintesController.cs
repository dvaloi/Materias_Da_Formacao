using InssApi.Data;
using InssApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InssApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContribuintesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ContribuintesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lista = await _db.Contribuintes.AsNoTracking().ToListAsync();
        return Ok(lista);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var c = await _db.Contribuintes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return c is null ? NotFound() : Ok(c);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarContribuinteRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Nuit) || req.Nuit.Length != 9)
            return BadRequest("NUIT deve ter 9 dígitos.");

        if (string.IsNullOrWhiteSpace(req.Nome))
            return BadRequest("Nome é obrigatório.");

        var c = new Contribuinte { Nuit = req.Nuit, Nome = req.Nome };
        _db.Contribuintes.Add(c);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CriarContribuinteRequest req)
    {
        var c = await _db.Contribuintes.FindAsync(id);
        if (c is null) return NotFound();

        c.Nuit = req.Nuit;
        c.Nome = req.Nome;
        await _db.SaveChangesAsync();
        return Ok(c);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> PatchStatus(int id, [FromBody] string status)
    {
        var c = await _db.Contribuintes.FindAsync(id);
        if (c is null) return NotFound();

        c.Status = status;
        await _db.SaveChangesAsync();
        return Ok(c);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var c = await _db.Contribuintes.FindAsync(id);
        if (c is null) return NotFound();

        _db.Contribuintes.Remove(c);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
