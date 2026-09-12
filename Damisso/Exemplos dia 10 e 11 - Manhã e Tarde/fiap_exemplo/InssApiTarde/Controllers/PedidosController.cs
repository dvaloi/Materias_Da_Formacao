using InssApi.Data;
using InssApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InssApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly AppDbContext _db;

    public PedidosController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var lista = await _db.Pedidos.AsNoTracking().ToListAsync();
        return Ok(lista);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _db.Pedidos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return p is null ? NotFound() : Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CriarPedidoRequest req)
    {
        if (req.ContribuinteId <= 0)
            return BadRequest("ContribuinteId inválido.");

        if (string.IsNullOrWhiteSpace(req.Tipo))
            return BadRequest("Tipo é obrigatório (ex.: Velhice ou Invalidez).");

        var existe = await _db.Contribuintes.AnyAsync(c => c.Id == req.ContribuinteId);
        if (!existe)
            return BadRequest("Contribuinte não encontrado.");

        var p = new PedidoBeneficio
        {
            ContribuinteId = req.ContribuinteId,
            Tipo = req.Tipo.Trim(),
            Status = "Aberto"
        };
        _db.Pedidos.Add(p);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = p.Id }, p);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CriarPedidoRequest req)
    {
        var p = await _db.Pedidos.FindAsync(id);
        if (p is null) return NotFound();

        if (req.ContribuinteId <= 0)
            return BadRequest("ContribuinteId inválido.");

        if (string.IsNullOrWhiteSpace(req.Tipo))
            return BadRequest("Tipo é obrigatório.");

        p.ContribuinteId = req.ContribuinteId;
        p.Tipo = req.Tipo.Trim();
        await _db.SaveChangesAsync();
        return Ok(p);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> PatchStatus(int id, [FromBody] string status)
    {
        var p = await _db.Pedidos.FindAsync(id);
        if (p is null) return NotFound();

        if (string.IsNullOrWhiteSpace(status))
            return BadRequest("Status é obrigatório.");

        p.Status = status.Trim();
        await _db.SaveChangesAsync();
        return Ok(p);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Pedidos.FindAsync(id);
        if (p is null) return NotFound();

        _db.Pedidos.Remove(p);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
