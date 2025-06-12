using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class SolicitudAvanceRangoController : ControllerBase
{
    private readonly AppDbContext _context;
    public SolicitudAvanceRangoController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<SolicitudAvanceRango>> Get()
    {
        return await _context.SolicitudAvanceRango
            .Include(s => s.Docente)
            .Include(s => s.NuevoNivelAcademico)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SolicitudAvanceRango>> Get(int id)
    {
        var solicitud = await _context.SolicitudAvanceRango
            .Include(s => s.Docente)
            .Include(s => s.NuevoNivelAcademico)
            .FirstOrDefaultAsync(s => s.Id == id);

        return solicitud == null ? NotFound() : Ok(solicitud);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] SolicitudAvanceRango solicitud)
    {
        _context.SolicitudAvanceRango.Add(solicitud);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = solicitud.Id }, solicitud);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] SolicitudAvanceRango solicitud)
    {
        if (id != solicitud.Id) return BadRequest();

        _context.Entry(solicitud).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var solicitud = await _context.SolicitudAvanceRango.FindAsync(id);
        if (solicitud == null) return NotFound();

        _context.SolicitudAvanceRango.Remove(solicitud);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}