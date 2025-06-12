using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class CursoCapacitacionController : ControllerBase
{
    private readonly AppDbContext _context;
    public CursoCapacitacionController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<CursoCapacitacion>> Get()
    {
        return await _context.CursoCapacitacion
            .Include(c => c.Docente)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CursoCapacitacion>> Get(int id)
    {
        var curso = await _context.CursoCapacitacion
            .Include(c => c.Docente)
            .FirstOrDefaultAsync(c => c.Id == id);

        return curso == null ? NotFound() : Ok(curso);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CursoCapacitacion curso)
    {
        _context.CursoCapacitacion.Add(curso);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = curso.Id }, curso);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] CursoCapacitacion curso)
    {
        if (id != curso.Id) return BadRequest();

        _context.Entry(curso).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var curso = await _context.CursoCapacitacion.FindAsync(id);
        if (curso == null) return NotFound();

        _context.CursoCapacitacion.Remove(curso);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}