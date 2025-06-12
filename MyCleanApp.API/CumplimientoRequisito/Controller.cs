using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class CumplimientoRequisitoController : ControllerBase
{
    private readonly AppDbContext _context;
    public CumplimientoRequisitoController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<CumplimientoRequisito>> Get()
    {
        return await _context.CumplimientoRequisito
            .Include(c => c.Docente)
            .Include(c => c.RequisitoPromocion)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CumplimientoRequisito>> Get(int id)
    {
        var cumplimiento = await _context.CumplimientoRequisito
            .Include(c => c.Docente)
            .Include(c => c.RequisitoPromocion)
            .FirstOrDefaultAsync(c => c.Id == id);

        return cumplimiento == null ? NotFound() : Ok(cumplimiento);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CumplimientoRequisito cumplimiento)
    {
        _context.CumplimientoRequisito.Add(cumplimiento);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = cumplimiento.Id }, cumplimiento);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] CumplimientoRequisito cumplimiento)
    {
        if (id != cumplimiento.Id) return BadRequest();

        _context.Entry(cumplimiento).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var cumplimiento = await _context.CumplimientoRequisito.FindAsync(id);
        if (cumplimiento == null) return NotFound();

        _context.CumplimientoRequisito.Remove(cumplimiento);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}