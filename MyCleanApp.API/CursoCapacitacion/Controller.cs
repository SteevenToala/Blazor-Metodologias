using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Application.DTOs;
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

    [HttpPost("usuario/{usuarioId}")]
    public async Task<IActionResult> CrearCursoCapacitacion(int usuarioId, [FromBody] CursoCapacitacionCreateRequest request)
    {
        // Buscar el docente por el usuarioId
        var docente = await _context.Docente.FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);
        if (docente == null)
            return NotFound("Docente no encontrado para el usuario dado.");

        // Calcular las horas (puedes ajustar la lógica según tu necesidad)
        var curso = new CursoCapacitacion
        {
            Nombre = request.Nombre,
            FechaInicio = request.FechaInicio,
            FechaFin = request.FechaFin,
            Horas = request.Horas,
            Certificado = request.Certificado,
            DocenteId = docente.Id
        };

        _context.CursoCapacitacion.Add(curso);
        await _context.SaveChangesAsync();

        return Ok(new { curso.Id });
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
    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<CursoCapacitacionDto>>> GetCursosByUsuarioId(int usuarioId)
    {
        var docente = await _context.Docente.FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);
        if (docente == null)
            return NotFound("Docente no encontrado para el usuario dado.");

        var cursos = await _context.CursoCapacitacion
            .Where(c => c.DocenteId == docente.Id)
            .Select(c => new CursoCapacitacionDto
            {
                Nombre = c.Nombre,
                Horas = c.Horas,
                FechaFin = c.FechaFin,
                Certificado = c.Certificado
            })
            .ToListAsync();

        return Ok(cursos);
    }
}