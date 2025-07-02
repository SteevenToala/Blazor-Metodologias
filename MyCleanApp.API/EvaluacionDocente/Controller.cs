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
    public async Task<IActionResult> Post([FromBody] EvaluacionDocenteCreateDto dto)
    {
        try
        {
            var evaluacion = new MyCleanApp.Domain.Entities.EvaluacionDocente
            {
                Periodo = dto.Periodo,
                Puntaje = dto.Puntaje,
                DocenteId = dto.DocenteId,
                FechaEvaluacion = dto.FechaEvaluacion,
                TipoEvaluacion = dto.TipoEvaluacion,
                Observaciones = dto.Observaciones,
                Certificado = dto.Certificado,
                Externo = dto.Externo
            };

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

    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<EvaluacionDocenteDto>>> GetByUsuarioId(int usuarioId)
    {
        try
        {
            var docente = await _context.Docente.FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);
            if (docente == null) return NotFound("Docente no encontrado para el usuario dado.");

            var evaluaciones = await _context.EvaluacionDocente
                .Include(e => e.Docente!)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .Include(e => e.Docente!.NivelAcademico)
                .Where(e => e.DocenteId == docente.Id)
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
                        : "Sin nivel",
                    FechaEvaluacion = e.FechaEvaluacion,
                    TipoEvaluacion = e.TipoEvaluacion,
                    Observaciones = e.Observaciones,
                    Certificado = e.Certificado,
                    Externo = e.Externo
                })
                .OrderByDescending(e => e.FechaEvaluacion)
                .ToListAsync();

            return Ok(evaluaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("promedio/{usuarioId}")]
    public async Task<ActionResult<double>> GetPromedioByUsuarioId(int usuarioId)
    {
        try
        {
            var docente = await _context.Docente.FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);
            if (docente == null) return NotFound("Docente no encontrado para el usuario dado.");

            var evaluaciones = await _context.EvaluacionDocente
                .Where(e => e.DocenteId == docente.Id)
                .ToListAsync();

            if (!evaluaciones.Any())
                return Ok(0.0);

            var promedio = evaluaciones.Average(e => e.Puntaje);
            return Ok(Math.Round(promedio, 2));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("archivo/{id}")]
    public async Task<IActionResult> GetArchivo(int id)
    {
        try
        {
            var evaluacion = await _context.EvaluacionDocente.FirstOrDefaultAsync(e => e.Id == id);
            if (evaluacion == null || evaluacion.Certificado == null)
                return NotFound();

            Response.Headers["Content-Disposition"] = "inline; filename=evaluacion.pdf";
            return File(evaluacion.Certificado, "application/pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost("importar")]
    public async Task<IActionResult> ImportarEvaluacionExterna([FromBody] EvaluacionDocenteDto evaluacion)
    {
        try
        {
            // Validación para evitar duplicados
            bool yaExiste = await _context.EvaluacionDocente.AnyAsync(e =>
                e.Periodo == evaluacion.Periodo &&
                e.DocenteId == evaluacion.DocenteId &&
                e.TipoEvaluacion == evaluacion.TipoEvaluacion &&
                e.Externo);

            if (yaExiste)
                return Conflict("La evaluación ya fue importada previamente.");

            var entidad = new MyCleanApp.Domain.Entities.EvaluacionDocente
            {
                Periodo = evaluacion.Periodo,
                Puntaje = evaluacion.Puntaje,
                DocenteId = evaluacion.DocenteId,
                FechaEvaluacion = evaluacion.FechaEvaluacion,
                TipoEvaluacion = evaluacion.TipoEvaluacion,
                Observaciones = evaluacion.Observaciones,
                Certificado = evaluacion.Certificado,
                Externo = true
            };

            _context.EvaluacionDocente.Add(entidad);
            await _context.SaveChangesAsync();
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}