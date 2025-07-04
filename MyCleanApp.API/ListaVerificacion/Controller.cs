using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class ListaVerificacionController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public ListaVerificacionController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("pendientes")]
    public async Task<ActionResult<IEnumerable<object>>> GetSolicitudesPendientes()
    {
        try
        {
            var solicitudesPendientes = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                .ThenInclude(d => d.Usuario)
                .ThenInclude(u => u.Persona)
                .Include(s => s.NuevoNivelAcademico)
                .Where(s => s.Estado == "PENDIENTE" || s.Estado == "RECIBIDA")
                .Select(s => new
                {
                    s.Id,
                    s.DocenteId,
                    DocenteNombre = (s.Docente.Usuario.Persona.Nombres ?? "") + " " + (s.Docente.Usuario.Persona.Apellidos ?? ""),
                    s.FechaSolicitud,
                    s.Estado,
                    s.Observaciones,
                    NivelActual = s.Docente.NivelAcademico.nombre ?? "",
                    NivelSolicitado = s.NuevoNivelAcademico.nombre ?? "",
                    s.FechaRespuesta
                })
                .ToListAsync();

            return Ok(solicitudesPendientes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost("{solicitudId}/verificacion")]
    public async Task<ActionResult> RegistrarVerificacion(int solicitudId, [FromBody] VerificacionRequest request)
    {
        try
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
            {
                return NotFound("Solicitud no encontrada");
            }

            // Crear registro de verificación
            var verificacion = new VerificacionDocumentos
            {
                SolicitudId = solicitudId,
                ListaVerificacionId = request.ListaVerificacionId,
                Verificado = request.Verificado,
                FechaVerificacion = DateTime.Now,
                VerificadoPor = request.VerificadoPorId,
                Observaciones = request.Observaciones
            };

            _context.VerificacionDocumentos.Add(verificacion);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Verificación registrada correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPut("{solicitudId}/estado")]
    public async Task<ActionResult> ActualizarEstadoSolicitud(int solicitudId, [FromBody] EstadoSolicitudRequest request)
    {
        try
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
            {
                return NotFound("Solicitud no encontrada");
            }

            solicitud.Estado = request.NuevoEstado;
            solicitud.FechaRespuesta = DateTime.Now;
            solicitud.Observaciones = request.Observaciones ?? "";

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Estado de solicitud actualizado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("{solicitudId}/verificaciones")]
    public async Task<ActionResult<IEnumerable<object>>> GetVerificacionesSolicitud(int solicitudId)
    {
        try
        {
            var verificaciones = await _context.VerificacionDocumentos
                .Where(v => v.SolicitudId == solicitudId)
                .Include(v => v.ListaVerificacion)
                .Include(v => v.VerificadoPorUsuario)
                .Select(v => new
                {
                    v.Id,
                    TipoDocumento = v.ListaVerificacion!.NombreDocumento,
                    v.Verificado,
                    v.FechaVerificacion,
                    VerificadoPor = v.VerificadoPorUsuario != null && v.VerificadoPorUsuario.Persona != null ? 
                        v.VerificadoPorUsuario.Persona.Nombres + " " + v.VerificadoPorUsuario.Persona.Apellidos : "Sistema",
                    v.Observaciones
                })
                .ToListAsync();

            return Ok(verificaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost("{solicitudId}/enviar-comision")]
    public async Task<ActionResult> EnviarAComision(int solicitudId)
    {
        try
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
            {
                return NotFound("Solicitud no encontrada");
            }

            // Verificar que todos los documentos estén verificados
            var verificacionesPendientes = await _context.VerificacionDocumentos
                .Where(v => v.SolicitudId == solicitudId && !v.Verificado)
                .CountAsync();

            if (verificacionesPendientes > 0)
            {
                return BadRequest("No se puede enviar a comisión. Hay documentos pendientes de verificación.");
            }

            // Cambiar estado y enviar a comisión
            solicitud.Estado = "EN_COMISION";
            solicitud.FechaRespuesta = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Solicitud enviada a Comisión Académica correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}

public class VerificacionRequest
{
    public int ListaVerificacionId { get; set; }
    public bool Verificado { get; set; }
    public string? Observaciones { get; set; }
    public int? VerificadoPorId { get; set; }
}

public class EstadoSolicitudRequest
{
    public string NuevoEstado { get; set; } = "";
    public string? Observaciones { get; set; }
}
