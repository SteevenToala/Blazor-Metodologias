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
                Id = c.Id, // <-- Agregado para exponer el Id
                Nombre = c.Nombre,
                Horas = c.Horas,
                FechaInicio = c.FechaInicio,
                FechaFin = c.FechaFin,
                Certificado = c.Certificado,
                DocenteId = c.DocenteId,
                Externo = c.Externo // <-- AGREGADO
            })
            .ToListAsync();

        return Ok(cursos);
    }

    [HttpPost("importar")]
    public async Task<IActionResult> ImportarCursoExterno([FromBody] CursoCapacitacionDto curso)
    {
        // Validación para evitar duplicados
        bool yaExiste = await _context.CursoCapacitacion.AnyAsync(c =>
            c.Nombre == curso.Nombre &&
            c.FechaInicio == curso.FechaInicio &&
            c.FechaFin == curso.FechaFin &&
            c.DocenteId == curso.DocenteId &&
            c.Externo);

        if (yaExiste)
            return Conflict("El curso ya fue importado previamente.");

        var entidad = new CursoCapacitacion
        {
            Nombre = curso.Nombre,
            Horas = curso.Horas,
            FechaInicio = curso.FechaInicio,
            FechaFin = curso.FechaFin,
            DocenteId = curso.DocenteId,
            Externo = true,
            Certificado = curso.Certificado
        };
        _context.CursoCapacitacion.Add(entidad);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("certificado/{id}")]
    public async Task<IActionResult> GetCertificado(int id)
    {
        var curso = await _context.CursoCapacitacion.FirstOrDefaultAsync(c => c.Id == id);
        if (curso == null || curso.Certificado == null)
            return NotFound();

        Response.Headers["Content-Disposition"] = "inline; filename=certificado.pdf";
        return File(curso.Certificado, "application/pdf");
    }

    [HttpGet("docente/{docenteId}")]
    public async Task<ActionResult<IEnumerable<CursoCapacitacion>>> GetByDocente(int docenteId)
    {
        try
        {
            var capacitaciones = await _context.CursoCapacitacion
                .Where(c => c.DocenteId == docenteId)
                .OrderByDescending(c => c.FechaInicio)
                .ToListAsync();

            return Ok(capacitaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}