using InssApi.Data;
using InssApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InssApi.Controllers;

/// <summary>Dia 2 manhã (M7) — ainda _db. Include + NUIT duplicado + async.</summary>
[ApiController]
[Route("api/[controller]")]
public class ContribuintesController : ControllerBase
{
    private readonly AppDbContext _db;

    public ContribuintesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var lista = await _db.Contribuintes.AsNoTracking().ToListAsync(ct);
        return Ok(lista);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var c = await _db.Contribuintes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return c is null ? NotFound() : Ok(c);
    }

    /// <summary>Pedidos daquele NUIT — Include da relação 1:N.</summary>
    [HttpGet("{id:int}/pedidos")]
    public async Task<IActionResult> PedidosDoContribuinte(int id, CancellationToken ct)
    {
        var c = await _db.Contribuintes
            .AsNoTracking()
            .Include(x => x.Pedidos)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return c is null ? NotFound() : Ok(c.Pedidos);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarContribuinteRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Nuit) || req.Nuit.Length != 9)
            return BadRequest("NUIT deve ter 9 dígitos.");

        if (string.IsNullOrWhiteSpace(req.Nome))
            return BadRequest("Nome é obrigatório.");

        if (await _db.Contribuintes.AnyAsync(c => c.Nuit == req.Nuit, ct))
            return BadRequest("NUIT já cadastrado.");

        var c = new Contribuinte { Nuit = req.Nuit, Nome = req.Nome };
        _db.Contribuintes.Add(c);
        await _db.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = c.Id }, c);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CriarContribuinteRequest req, CancellationToken ct)
    {
        var c = await _db.Contribuintes.FindAsync([id], ct);
        if (c is null) return NotFound();

        c.Nuit = req.Nuit;
        c.Nome = req.Nome;
        await _db.SaveChangesAsync(ct);
        return Ok(c);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> PatchStatus(int id, [FromBody] string status, CancellationToken ct)
    {
        var c = await _db.Contribuintes.FindAsync([id], ct);
        if (c is null) return NotFound();

        c.Status = status;
        await _db.SaveChangesAsync(ct);
        return Ok(c);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var c = await _db.Contribuintes.FindAsync([id], ct);
        if (c is null) return NotFound();

        _db.Contribuintes.Remove(c);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
