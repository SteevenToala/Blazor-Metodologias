using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class EvaluacionDocenteController : ControllerBase
{
    private readonly AppDbContext _context;
    public EvaluacionDocenteController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<EvaluacionDocente>> Get()
    {
        return await _context.EvaluacionDocente
            .Include(e => e.Docente)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EvaluacionDocente>> Get(int id)
    {
        var evaluacion = await _context.EvaluacionDocente
            .Include(e => e.Docente)
            .FirstOrDefaultAsync(e => e.Id == id);

        return evaluacion == null ? NotFound() : Ok(evaluacion);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] EvaluacionDocente evaluacion)
    {
        _context.EvaluacionDocente.Add(evaluacion);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = evaluacion.Id }, evaluacion);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] EvaluacionDocente evaluacion)
    {
        if (id != evaluacion.Id) return BadRequest();

        _context.Entry(evaluacion).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var evaluacion = await _context.EvaluacionDocente.FindAsync(id);
        if (evaluacion == null) return NotFound();

        _context.EvaluacionDocente.Remove(evaluacion);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}