using InssApi.Data;
using InssApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InssApi.Controllers;

/// <summary>Dia 2 manhã (M7) — LINQ, projeção, paginação, transação, async.</summary>
[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _db;

    public PedidosController(AppDbContext db) => _db = db;

    /// <summary>GET /api/pedidos?pagina=1&amp;tamanho=2</summary>
    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] int tamanho = 10,
        CancellationToken ct = default)
    {
        if (pagina < 1) pagina = 1;
        if (tamanho < 1 || tamanho > 50) tamanho = 10;

        var query = _db.Pedidos.AsNoTracking().Include(p => p.Contribuinte);

        var total = await query.CountAsync(ct);
        var itens = await query
            .OrderBy(p => p.Id)
            .Skip((pagina - 1) * tamanho)
            .Take(tamanho)
            .Select(p => new PedidoResumoDto(
                p.Id,
                p.Tipo,
                p.Status,
                p.Contribuinte!.Nome))
            .ToListAsync(ct);

        return Ok(new { total, pagina, tamanho, itens });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var p = await _db.Pedidos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarPedidoRequest req, CancellationToken ct)
    {
        if (req.ContribuinteId <= 0)
            return BadRequest("ContribuinteId inválido.");

        if (string.IsNullOrWhiteSpace(req.Tipo))
            return BadRequest("Tipo é obrigatório (ex.: Velhice ou Invalidez).");

        var existe = await _db.Contribuintes.AnyAsync(c => c.Id == req.ContribuinteId, ct);
        if (!existe)
            return BadRequest("ContribuinteId inexistente.");

        await using var tx = await _db.Database.BeginTransactionAsync(ct);
        try
        {
            var p = new PedidoBeneficio
            {
                ContribuinteId = req.ContribuinteId,
                Tipo = req.Tipo.Trim(),
                Status = "Aberto"
            };
            _db.Pedidos.Add(p);
            await _db.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CriarPedidoRequest req, CancellationToken ct)
    {
        var p = await _db.Pedidos.FindAsync([id], ct);
        if (p is null) return NotFound();

        if (req.ContribuinteId <= 0)
            return BadRequest("ContribuinteId inválido.");

        if (string.IsNullOrWhiteSpace(req.Tipo))
            return BadRequest("Tipo é obrigatório.");

        p.ContribuinteId = req.ContribuinteId;
        p.Tipo = req.Tipo.Trim();
        await _db.SaveChangesAsync(ct);
        return Ok(p);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> PatchStatus(int id, [FromBody] string status, CancellationToken ct)
    {
        var p = await _db.Pedidos.FindAsync([id], ct);
        if (p is null) return NotFound();

        if (string.IsNullOrWhiteSpace(status))
            return BadRequest("Status é obrigatório.");

        p.Status = status.Trim();
        await _db.SaveChangesAsync(ct);
        return Ok(p);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var p = await _db.Pedidos.FindAsync([id], ct);
        if (p is null) return NotFound();

        _db.Pedidos.Remove(p);
        await _db.SaveChangesAsync(ct);
        return NoContent();
    }
}
