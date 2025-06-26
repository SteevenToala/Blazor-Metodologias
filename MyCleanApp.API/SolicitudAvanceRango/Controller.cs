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
    public async Task<ActionResult<IEnumerable<object>>> Get()
    {
        try
        {
            var solicitudes = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .Include(s => s.Docente)
                    .ThenInclude(d => d.NivelAcademico)
                .Include(s => s.NuevoNivelAcademico)
                .Select(s => new
                {
                    s.Id,
                    s.DocenteId,
                    s.FechaSolicitud,
                    Estado = s.Estado ?? "PENDIENTE",
                    s.FechaRespuesta,
                    Observaciones = s.Observaciones ?? "",
                    s.NuevoNivelAcademicoId,
                    DocenteNombre = s.Docente != null && s.Docente.Usuario != null && s.Docente.Usuario.Persona != null
                        ? (s.Docente.Usuario.Persona.Nombres ?? "") + " " + (s.Docente.Usuario.Persona.Apellidos ?? "")
                        : "Sin información",
                    NivelActual = s.Docente != null && s.Docente.NivelAcademico != null
                        ? s.Docente.NivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel",
                    NuevoNivel = s.NuevoNivelAcademico != null
                        ? s.NuevoNivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel"
                })
                .ToListAsync();
            
            return Ok(solicitudes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> Get(int id)
    {
        try
        {
            var solicitud = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .Include(s => s.Docente)
                    .ThenInclude(d => d.NivelAcademico)
                .Include(s => s.NuevoNivelAcademico)
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.DocenteId,
                    s.FechaSolicitud,
                    Estado = s.Estado ?? "PENDIENTE",
                    s.FechaRespuesta,
                    Observaciones = s.Observaciones ?? "",
                    s.NuevoNivelAcademicoId,
                    DocenteNombre = s.Docente != null && s.Docente.Usuario != null && s.Docente.Usuario.Persona != null
                        ? (s.Docente.Usuario.Persona.Nombres ?? "") + " " + (s.Docente.Usuario.Persona.Apellidos ?? "")
                        : "Sin información",
                    NivelActual = s.Docente != null && s.Docente.NivelAcademico != null
                        ? s.Docente.NivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel",
                    NuevoNivel = s.NuevoNivelAcademico != null
                        ? s.NuevoNivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel"
                })
                .FirstOrDefaultAsync();

            return solicitud == null ? NotFound() : Ok(solicitud);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] SolicitudAvanceRango solicitud)
    {
        try
        {
            _context.SolicitudAvanceRango.Add(solicitud);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = solicitud.Id }, solicitud);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] SolicitudAvanceRango solicitud)
    {
        try
        {
            if (id != solicitud.Id) return BadRequest();
            
            _context.Entry(solicitud).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(id);
            if (solicitud == null) return NotFound();

            _context.SolicitudAvanceRango.Remove(solicitud);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }
}