using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;
using MyCleanApp.API.DTOs;

[ApiController]
[Route("api/[controller]")]
public class EvaluacionDocenteController : ControllerBase
{
    private readonly AppDbContext _context;
    public EvaluacionDocenteController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var evaluaciones = await _context.EvaluacionDocente
                .Include(e => e.Docente!)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .Include(e => e.Docente!.NivelAcademico)
                .Select(e => new EvaluacionDocenteDto
                {
                    Id = e.Id,
                    DocenteId = e.DocenteId,
                    DocenteNombre = e.Docente != null && e.Docente.Usuario != null && e.Docente.Usuario.Persona != null 
                        ? $"{e.Docente.Usuario.Persona.Nombres} {e.Docente.Usuario.Persona.Apellidos}" 
                        : "Sin nombre",
                    DocenteCedula = e.Docente != null && e.Docente.Usuario != null && e.Docente.Usuario.Persona != null 
                        ? e.Docente.Usuario.Persona.Cedula ?? "Sin cédula"
                        : "Sin cédula",
                    Periodo = e.Periodo,
                    Puntaje = e.Puntaje,
                    NivelAcademico = e.Docente != null && e.Docente.NivelAcademico != null 
                        ? e.Docente.NivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel"
                })
                .ToListAsync();

            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("docente/{docenteId}")]
    public async Task<IActionResult> GetByDocente(int docenteId)
    {
        try
        {
            var evaluaciones = await _context.EvaluacionDocente
                .Include(e => e.Docente!)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .Include(e => e.Docente!.NivelAcademico)
                .Where(e => e.DocenteId == docenteId)
                .Select(e => new EvaluacionDocenteDto
                {
                    Id = e.Id,
                    DocenteId = e.DocenteId,
                    DocenteNombre = e.Docente != null && e.Docente.Usuario != null && e.Docente.Usuario.Persona != null 
                        ? $"{e.Docente.Usuario.Persona.Nombres} {e.Docente.Usuario.Persona.Apellidos}" 
                        : "Sin nombre",
                    DocenteCedula = e.Docente != null && e.Docente.Usuario != null && e.Docente.Usuario.Persona != null 
                        ? e.Docente.Usuario.Persona.Cedula ?? "Sin cédula"
                        : "Sin cédula",
                    Periodo = e.Periodo,
                    Puntaje = e.Puntaje,
                    NivelAcademico = e.Docente != null && e.Docente.NivelAcademico != null 
                        ? e.Docente.NivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel"
                })
                .ToListAsync();

            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EvaluacionDocenteDto>> Get(int id)
    {
        try
        {
            var evaluacion = await _context.EvaluacionDocente
                .Include(e => e.Docente!)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .Include(e => e.Docente!.NivelAcademico)
                .Where(e => e.Id == id)
                .Select(e => new EvaluacionDocenteDto
                {
                    Id = e.Id,
                    DocenteId = e.DocenteId,
                    DocenteNombre = e.Docente != null && e.Docente.Usuario != null && e.Docente.Usuario.Persona != null 
                        ? $"{e.Docente.Usuario.Persona.Nombres} {e.Docente.Usuario.Persona.Apellidos}" 
                        : "Sin nombre",
                    DocenteCedula = e.Docente != null && e.Docente.Usuario != null && e.Docente.Usuario.Persona != null 
                        ? e.Docente.Usuario.Persona.Cedula ?? "Sin cédula"
                        : "Sin cédula",
                    Periodo = e.Periodo,
                    Puntaje = e.Puntaje,
                    NivelAcademico = e.Docente != null && e.Docente.NivelAcademico != null 
                        ? e.Docente.NivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel"
                })
                .FirstOrDefaultAsync();

            return evaluacion == null ? NotFound() : Ok(evaluacion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] EvaluacionDocente evaluacion)
    {
        try
        {
            if (evaluacion == null)
                return BadRequest("Los datos de evaluación son requeridos");

            _context.EvaluacionDocente.Add(evaluacion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = evaluacion.Id }, evaluacion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] EvaluacionDocenteUpdateDto updateDto)
    {
        try
        {
            if (updateDto == null)
                return BadRequest("Los datos de evaluación son requeridos");

            if (id != updateDto.Id)
                return BadRequest("El ID no coincide");

            var evaluacion = await _context.EvaluacionDocente.FindAsync(id);
            if (evaluacion == null)
                return NotFound();

            // Update only the fields that can be modified
            evaluacion.Puntaje = updateDto.Puntaje;
            evaluacion.Periodo = updateDto.Periodo;

            _context.Entry(evaluacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var evaluacion = await _context.EvaluacionDocente.FindAsync(id);
            if (evaluacion == null)
                return NotFound();

            _context.EvaluacionDocente.Remove(evaluacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}