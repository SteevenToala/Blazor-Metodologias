using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.ConsejoUniversitario
{
    [ApiController]
    [Route("api/consejo-universitario")]
    public class ConsejoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ConsejoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("informes-pendientes")]
        public async Task<ActionResult> GetInformesPendientes()
        {
            try
            {
                var informes = await _context.SolicitudAvanceRango
                    .Where(s => s.Estado == "INFORMES_FINALES")
                    .Include(s => s.Docente)
                        .ThenInclude(d => d.Usuario)
                            .ThenInclude(u => u.Persona)
                    .Include(s => s.NuevoNivelAcademico)
                    .Include(s => s.Docente.NivelAcademico)
                    .Select(s => new
                    {
                        Id = s.Id,
                        DocenteNombre = s.Docente.Usuario.Persona.Nombres ?? "",
                        DocenteApellido = s.Docente.Usuario.Persona.Apellidos ?? "",
                        DocenteId = s.DocenteId,
                        NivelActual = s.Docente.NivelAcademico.nombre ?? "",
                        NivelSolicitado = s.NuevoNivelAcademico.nombre ?? "",
                        FechaSolicitud = s.FechaSolicitud,
                        FechaDecision = s.FechaRespuesta,
                        Estado = s.Estado,
                        Observaciones = s.Observaciones ?? ""
                    })
                    .ToListAsync();

                return Ok(informes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("solicitud/{id}/aprobar")]
        public async Task<ActionResult> AprobarSolicitud(int id)
        {
            try
            {
                var solicitud = await _context.SolicitudAvanceRango
                    .Include(s => s.Docente)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (solicitud == null)
                {
                    return NotFound("Solicitud no encontrada");
                }

                // Cambiar estado de la solicitud
                solicitud.Estado = "APROBADO";
                solicitud.FechaRespuesta = DateTime.Now;

                // IMPORTANTE: Actualizar el nivel del docente
                if (solicitud.NuevoNivelAcademicoId.HasValue)
                {
                    solicitud.Docente.NivelAcademicoId = solicitud.NuevoNivelAcademicoId.Value;
                    solicitud.Docente.FechaInicioNivel = DateTime.Now;
                }

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Solicitud aprobada y nivel del docente actualizado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("solicitud/{id}/rechazar")]
        public async Task<ActionResult> RechazarSolicitud(int id, [FromBody] string observaciones = "")
        {
            try
            {
                var solicitud = await _context.SolicitudAvanceRango.FindAsync(id);
                if (solicitud == null)
                {
                    return NotFound("Solicitud no encontrada");
                }

                solicitud.Estado = "RECHAZADO";
                solicitud.FechaRespuesta = DateTime.Now;
                if (!string.IsNullOrEmpty(observaciones))
                {
                    solicitud.Observaciones = observaciones;
                }

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Solicitud rechazada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
