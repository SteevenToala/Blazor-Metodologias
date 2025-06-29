using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class ApelacionesController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public ApelacionesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetApelaciones()
    {
        try
        {
            var apelaciones = await _context.ApelacionPromocion
                .Include(a => a.SolicitudAvanceRango)
                .ThenInclude(s => s.Docente)
                .ThenInclude(d => d.Usuario)
                .ThenInclude(u => u.Persona)
                .Include(a => a.SolicitudAvanceRango.Docente.NivelAcademico)
                .Include(a => a.SolicitudAvanceRango.NuevoNivelAcademico)
                .OrderByDescending(a => a.FechaApelacion)
                .Select(a => new
                {
                    a.Id,
                    SolicitudId = a.SolicitudId,
                    DocenteNombre = a.SolicitudAvanceRango != null && 
                                   a.SolicitudAvanceRango.Docente != null && 
                                   a.SolicitudAvanceRango.Docente.Usuario != null && 
                                   a.SolicitudAvanceRango.Docente.Usuario.Persona != null
                        ? (a.SolicitudAvanceRango.Docente.Usuario.Persona.Nombres ?? "") + " " + 
                          (a.SolicitudAvanceRango.Docente.Usuario.Persona.Apellidos ?? "")
                        : "",
                    NivelActual = a.SolicitudAvanceRango != null && 
                                 a.SolicitudAvanceRango.Docente != null && 
                                 a.SolicitudAvanceRango.Docente.NivelAcademico != null
                        ? a.SolicitudAvanceRango.Docente.NivelAcademico.nombre ?? ""
                        : "",
                    NivelSolicitado = a.SolicitudAvanceRango != null && 
                                     a.SolicitudAvanceRango.NuevoNivelAcademico != null
                        ? a.SolicitudAvanceRango.NuevoNivelAcademico.nombre ?? ""
                        : "",
                    CambioSolicitado = (a.SolicitudAvanceRango != null && 
                                       a.SolicitudAvanceRango.Docente != null && 
                                       a.SolicitudAvanceRango.Docente.NivelAcademico != null
                            ? a.SolicitudAvanceRango.Docente.NivelAcademico.nombre ?? ""
                            : "") + " → " + 
                                      (a.SolicitudAvanceRango != null && 
                                       a.SolicitudAvanceRango.NuevoNivelAcademico != null
                            ? a.SolicitudAvanceRango.NuevoNivelAcademico.nombre ?? ""
                            : ""),
                    FechaPresentacion = a.FechaApelacion,
                    a.MotivoApelacion,
                    a.DocumentosRespaldo,
                    a.Estado,
                    a.FechaRespuesta,
                    a.RespuestaComision,
                    a.Resuelto
                })
                .ToListAsync();

            return Ok(apelaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApelacionPromocion>> GetApelacion(int id)
    {
        try
        {
            var apelacion = await _context.ApelacionPromocion
                .Include(a => a.SolicitudAvanceRango)
                .ThenInclude(s => s.Docente)
                .ThenInclude(d => d.Usuario)
                .ThenInclude(u => u.Persona)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (apelacion == null)
            {
                return NotFound(new { error = "Apelación no encontrada" });
            }

            return Ok(apelacion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApelacionPromocion>> CreateApelacion([FromBody] ApelacionPromocion apelacion)
    {
        try
        {
            if (apelacion == null)
            {
                return BadRequest(new { error = "Los datos de la apelación son requeridos" });
            }

            // Verificar que la solicitud existe
            var solicitudExists = await _context.SolicitudAvanceRango.AnyAsync(s => s.Id == apelacion.SolicitudId);
            if (!solicitudExists)
            {
                return BadRequest(new { error = "La solicitud especificada no existe" });
            }

            apelacion.Estado = "PENDIENTE";
            apelacion.Resuelto = false;

            _context.ApelacionPromocion.Add(apelacion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetApelacion), new { id = apelacion.Id }, apelacion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateApelacion(int id, [FromBody] ApelacionPromocion apelacion)
    {
        try
        {
            if (id != apelacion.Id)
            {
                return BadRequest(new { error = "El ID no coincide" });
            }

            var existingApelacion = await _context.ApelacionPromocion.FindAsync(id);
            if (existingApelacion == null)
            {
                return NotFound(new { error = "Apelación no encontrada" });
            }

            existingApelacion.MotivoApelacion = apelacion.MotivoApelacion;
            existingApelacion.DocumentosRespaldo = apelacion.DocumentosRespaldo;
            existingApelacion.Estado = apelacion.Estado;
            existingApelacion.FechaRespuesta = apelacion.FechaRespuesta;
            existingApelacion.RespuestaComision = apelacion.RespuestaComision;
            existingApelacion.Resuelto = apelacion.Resuelto;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteApelacion(int id)
    {
        try
        {
            var apelacion = await _context.ApelacionPromocion.FindAsync(id);
            if (apelacion == null)
            {
                return NotFound(new { error = "Apelación no encontrada" });
            }

            _context.ApelacionPromocion.Remove(apelacion);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("verificaciones-procedimiento")]
    public ActionResult<IEnumerable<object>> GetVerificacionesProcedimiento()
    {
        try
        {
            // Simulated data for procedure verifications
            var verificaciones = new object[]
            {
                new { 
                    Id = 1, 
                    Titulo = "Verificación de documentación completa",
                    Estado = "COMPLETADO",
                    FechaVerificacion = DateTime.Now.AddDays(-1),
                    Observaciones = "Todos los documentos están en orden"
                },
                new { 
                    Id = 2, 
                    Titulo = "Verificación de plazos de presentación",
                    Estado = "PENDIENTE",
                    FechaVerificacion = (DateTime?)null,
                    Observaciones = "Pendiente de revisión"
                }
            };

            return Ok(verificaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("pendientes")]
    public async Task<ActionResult<IEnumerable<object>>> GetApelacionesPendientes()
    {
        try
        {
            var apelacionesPendientes = await _context.ApelacionPromocion
                .Include(a => a.SolicitudAvanceRango)
                .ThenInclude(s => s.Docente)
                .ThenInclude(d => d.Usuario)
                .ThenInclude(u => u.Persona)
                .Where(a => a.Estado == "PENDIENTE")
                .Select(a => new
                {
                    a.Id,
                    DocenteNombre = a.SolicitudAvanceRango != null && 
                                   a.SolicitudAvanceRango.Docente != null && 
                                   a.SolicitudAvanceRango.Docente.Usuario != null && 
                                   a.SolicitudAvanceRango.Docente.Usuario.Persona != null
                        ? (a.SolicitudAvanceRango.Docente.Usuario.Persona.Nombres ?? "") + " " + 
                          (a.SolicitudAvanceRango.Docente.Usuario.Persona.Apellidos ?? "")
                        : "",
                    FechaPresentacion = a.FechaApelacion,
                    a.MotivoApelacion,
                    a.Estado
                })
                .ToListAsync();

            return Ok(apelacionesPendientes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }
}
