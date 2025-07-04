using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.ComisionAcademica
{
    [ApiController]
    [Route("api/comision-academica")]
    public class ComisionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComisionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("solicitudes-pendientes")]
        public async Task<ActionResult<IEnumerable<object>>> GetSolicitudesPendientes()
        {
            try
            {
                var solicitudesPendientes = await _context.SolicitudAvanceRango
                    .Include(s => s.Docente)
                        .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                    .Include(s => s.Docente)
                        .ThenInclude(d => d.NivelAcademico)
                    .Include(s => s.NuevoNivelAcademico)
                    .Where(s => s.Estado == "VERIFICADA" || s.Estado == "EN_COMISION" || s.Estado == "EN_EVALUACION" || s.Estado == "DECIDIDA")
                    .Select(s => new
                    {
                        s.Id,
                        s.DocenteId,
                        DocenteNombre = (s.Docente.Usuario != null && s.Docente.Usuario.Persona != null) 
                            ? s.Docente.Usuario.Persona.Nombres + " " + s.Docente.Usuario.Persona.Apellidos 
                            : "Sin información",
                        s.FechaSolicitud,
                        s.Estado,
                        s.Observaciones,
                        NivelActual = s.Docente.NivelAcademico != null ? s.Docente.NivelAcademico.nombre : "Sin nivel",
                        NivelSolicitado = s.NuevoNivelAcademico != null ? s.NuevoNivelAcademico.nombre : "Sin información",
                        s.FechaRespuesta,
                        RequiereAnalisis = s.Estado == "VERIFICADA"
                    })
                    .ToListAsync();

                return Ok(solicitudesPendientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("solicitud/{id}/detalle")]
        public async Task<ActionResult<object>> GetSolicitudDetalle(int id)
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
                    .FirstOrDefaultAsync();

                if (solicitud == null)
                {
                    return NotFound("Solicitud no encontrada");
                }

                // Obtener evaluaciones del docente
                var evaluaciones = await _context.EvaluacionDocente
                    .Where(e => e.DocenteId == solicitud.DocenteId)
                    .Select(e => new
                    {
                        e.Id,
                        e.Periodo,
                        e.Puntaje,
                        e.FechaEvaluacion,
                        e.TipoEvaluacion,
                        e.Observaciones
                    })
                    .ToListAsync();

                // Obtener cursos de capacitación
                var cursos = await _context.CursoCapacitacion
                    .Where(c => c.DocenteId == solicitud.DocenteId)
                    .Select(c => new
                    {
                        c.Id,
                        c.Nombre,
                        c.Horas,
                        c.FechaInicio,
                        c.FechaFin
                    })
                    .ToListAsync();

                // Obtener publicaciones académicas
                var publicaciones = await _context.PublicacionAcademica
                    .Where(p => p.DocenteId == solicitud.DocenteId)
                    .Select(p => new
                    {
                        p.Id,
                        p.Titulo,
                        p.Revista,
                        p.Volumen,
                        p.Anio,
                        p.Tipo
                    })
                    .ToListAsync();

                // Obtener proyectos de investigación
                var proyectos = await _context.ProyectoInvestigacion
                    .Where(p => p.DocenteId == solicitud.DocenteId)
                    .Select(p => new
                    {
                        p.Id,
                        p.Titulo,
                        p.FechaInicio,
                        p.FechaFin,
                        p.RolEnProyecto
                    })
                    .ToListAsync();

                var solicitudDetalle = new
                {
                    solicitud.Id,
                    solicitud.DocenteId,
                    DocenteNombre = (solicitud.Docente.Usuario != null && solicitud.Docente.Usuario.Persona != null) 
                        ? solicitud.Docente.Usuario.Persona.Nombres + " " + solicitud.Docente.Usuario.Persona.Apellidos 
                        : "Sin información",
                    DocenteCorreo = solicitud.Docente.Usuario?.Correo ?? "Sin correo",
                    NivelActual = solicitud.Docente.NivelAcademico?.nombre ?? "Sin nivel",
                    NivelSolicitado = solicitud.NuevoNivelAcademico?.nombre ?? "Sin información",
                    solicitud.FechaSolicitud,
                    solicitud.Estado,
                    solicitud.Observaciones,
                    FechaAnalisis = solicitud.FechaEnvioComision,
                    FechaDecision = solicitud.FechaRespuesta,
                    Evaluaciones = evaluaciones,
                    Cursos = cursos,
                    Publicaciones = publicaciones,
                    Proyectos = proyectos,
                    Requisitos = new List<object>() // Se puede expandir según necesidades
                };

                return Ok(solicitudDetalle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("solicitud/{id}/iniciar-analisis")]
        public async Task<ActionResult> IniciarAnalisisSolicitud(int id)
        {
            try
            {
                var solicitud = await _context.SolicitudAvanceRango.FindAsync(id);
                if (solicitud == null)
                {
                    return NotFound("Solicitud no encontrada");
                }

                solicitud.Estado = "EN_EVALUACION";
                solicitud.FechaEnvioComision = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Análisis iniciado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("solicitud/{id}/emitir-decision")]
        public async Task<ActionResult> EmitirDecision(int id, [FromBody] object decisionRequest)
        {
            try
            {
                var solicitud = await _context.SolicitudAvanceRango.FindAsync(id);
                if (solicitud == null)
                {
                    return NotFound("Solicitud no encontrada");
                }

                // Aquí deberías deserializar decisionRequest según tus DTOs
                solicitud.Estado = "DECIDIDA";
                solicitud.FechaRespuesta = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Decisión emitida correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("apelacion/{id}/resolver")]
        public async Task<ActionResult> ResolverApelacion(int id, [FromBody] object resolucionRequest)
        {
            try
            {
                var apelacion = await _context.ApelacionPromocion.FindAsync(id);
                if (apelacion == null)
                {
                    return NotFound("Apelación no encontrada");
                }

                apelacion.Estado = "RESUELTA";
                apelacion.FechaRespuesta = DateTime.Now;
                apelacion.Resuelto = true;

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Apelación resuelta correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("solicitud/{id}/generar-informe-final")]
        public async Task<ActionResult> GenerarInformeFinal(int id)
        {
            try
            {
                var solicitud = await _context.SolicitudAvanceRango.FindAsync(id);
                if (solicitud == null)
                {
                    return NotFound("Solicitud no encontrada");
                }

                // Crear informe final
                var informe = new InformeFinalPromocion
                {
                    SolicitudId = id,
                    FechaGeneracion = DateTime.Now,
                    Contenido = $"Informe final para solicitud {id}",
                    Estado = "GENERADO",
                    GeneradoPor = 1 // Aquí deberías usar el ID del usuario actual
                };

                _context.InformeFinalPromocion.Add(informe);
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Informe final generado correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("solicitud/{id}/enviar-informe")]
        public async Task<ActionResult> EnviarInforme(int id)
        {
            try
            {
                var informe = await _context.InformeFinalPromocion
                    .Where(i => i.SolicitudId == id)
                    .FirstOrDefaultAsync();

                if (informe == null)
                {
                    return NotFound("Informe no encontrado");
                }

                informe.Estado = "ENVIADO_CONSEJO";
                informe.FechaEnvioConsejo = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Informe enviado al Consejo Universitario correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
