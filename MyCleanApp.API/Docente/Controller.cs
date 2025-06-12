using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class DocenteController : ControllerBase
{
    private readonly AppDbContext _context;
    public DocenteController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<Docente>> Get()
    {
        return await _context.Docente
            .Include(d => d.NivelAcademico)
            .Include(d => d.Usuario)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Docente>> Get(int id)
    {
        var docente = await _context.Docente
            .Include(d => d.NivelAcademico)
            .Include(d => d.Usuario)
            .FirstOrDefaultAsync(d => d.Id == id);

        return docente == null ? NotFound() : Ok(docente);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Docente docente)
    {
        _context.Docente.Add(docente);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = docente.Id }, docente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Docente docente)
    {
        if (id != docente.Id) return BadRequest();

        _context.Entry(docente).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var docente = await _context.Docente.FindAsync(id);
        if (docente == null) return NotFound();

        _context.Docente.Remove(docente);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpGet("info/{usuarioId}")]
    public async Task<ActionResult<object>> GetInfoDocentePorUsuarioId(int usuarioId)
    {
        var docente = await _context.Docente
            .Include(d => d.NivelAcademico)
            .FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);

        if (docente == null)
            return NotFound("Docente no encontrado para ese usuario");

        // Años en el nivel actual
        var aniosEnNivel = (DateTime.Now - docente.FechaInicioNivel).TotalDays / 365.25;

        // Publicaciones académicas
        var publicaciones = await _context.PublicacionAcademica
            .CountAsync(p => p.DocenteId == docente.Id);

        // Puntaje promedio de evaluación
        var puntajeEvaluacion = await _context.EvaluacionDocente
            .Where(e => e.DocenteId == docente.Id)
            .AverageAsync(e => (float?)e.Puntaje) ?? 0;

        // Horas totales de capacitación
        var horasCapacitacion = await _context.CursoCapacitacion
            .Where(c => c.DocenteId == docente.Id)
            .SumAsync(c => (int?)c.Horas) ?? 0;

        return Ok(new
        {
            NivelAcademico = docente.NivelAcademico.nombre,
            AniosEnNivel = Math.Floor(aniosEnNivel),
            PublicacionesAcademicas = publicaciones,
            PuntajeEvaluacion = puntajeEvaluacion,
            HorasCapacitacion = horasCapacitacion
        });
    }
}