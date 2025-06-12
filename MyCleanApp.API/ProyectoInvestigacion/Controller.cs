using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class ProyectoInvestigacionController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProyectoInvestigacionController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<ProyectoInvestigacion>> Get()
        => await _context.ProyectoInvestigacion.Include(p => p.Docente).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<ProyectoInvestigacion>> Get(int id)
    {
        var proyecto = await _context.ProyectoInvestigacion.Include(p => p.Docente).FirstOrDefaultAsync(p => p.Id == id);
        return proyecto == null ? NotFound() : Ok(proyecto);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] ProyectoInvestigacion proyecto)
    {
        _context.ProyectoInvestigacion.Add(proyecto);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = proyecto.Id }, proyecto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] ProyectoInvestigacion proyecto)
    {
        if (id != proyecto.Id) return BadRequest();
        _context.Entry(proyecto).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var proyecto = await _context.ProyectoInvestigacion.FindAsync(id);
        if (proyecto == null) return NotFound();
        _context.ProyectoInvestigacion.Remove(proyecto);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}