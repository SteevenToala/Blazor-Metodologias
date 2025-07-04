using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;
using MyCleanApp.Infrastructure.Services;
using MyCleanApp.API.DTOs;

namespace MyCleanApp.API.ComisionAcademica
{
    [ApiController]
    [Route("api/comision-academica")]
    public class ComisionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IEmailService _emailService;

        public ComisionController(AppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
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
                    .Where(s => s.Estado == "VERIFICADA" || s.Estado == "EN_COMISION" || s.Estado == "EN_EVALUACION" || 
                               s.Estado == "DECIDIDO_APROBADA" || s.Estado == "DECIDIDO_RECHAZADA" || 
                               s.Estado == "APROBADA_DOCENTE" || s.Estado == "APROBADO_COMISION" || s.Estado == "INFORMES_FINALES" || 
                               s.Estado == "ENVIADA_CONSEJO" || s.Estado == "EN_ANALISIS_COMISION")
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
                        RequiereAnalisis = s.Estado == "VERIFICADA" || s.Estado == "EN_ANALISIS_COMISION"
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
        public async Task<ActionResult> EmitirDecision(int id, [FromBody] DecisionComisionDto decisionRequest)
        {
            try
            {
                var solicitud = await _context.SolicitudAvanceRango
                    .Include(s => s.Docente)
                        .ThenInclude(d => d!.Usuario)
                        .ThenInclude(u => u!.Persona)
                    .Include(s => s.Docente)
                        .ThenInclude(d => d!.NivelAcademico)
                    .Include(s => s.NuevoNivelAcademico)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (solicitud == null)
                {
                    return NotFound("Solicitud no encontrada");
                }

                if (solicitud.Estado != "EN_EVALUACION")
                {
                    return BadRequest("La solicitud no se encuentra en estado de evaluación");
                }

                // Establecer el estado específico basado en la decisión
                if (decisionRequest.Decision.ToUpper() == "APROBADO")
                {
                    solicitud.Estado = "DECIDIDO_APROBADA";
                }
                else if (decisionRequest.Decision.ToUpper() == "RECHAZADO")
                {
                    solicitud.Estado = "DECIDIDO_RECHAZADA";
                }
                else
                {
                    return BadRequest("Decisión inválida. Debe ser 'APROBADO' o 'RECHAZADO'");
                }

                solicitud.FechaRespuesta = DateTime.Now;
                
                // Combinar la decisión con las observaciones
                var observacionesCompletas = $"DECISIÓN: {decisionRequest.Decision}";
                if (!string.IsNullOrEmpty(decisionRequest.Observaciones))
                {
                    observacionesCompletas += $" - OBSERVACIONES: {decisionRequest.Observaciones}";
                }
                solicitud.Observaciones = observacionesCompletas;

                await _context.SaveChangesAsync();

                // Enviar correo electrónico de notificación
                if (decisionRequest.RequiereNotificacion && solicitud.Docente?.Usuario != null)
                {
                    var correoDocente = solicitud.Docente.Usuario?.Correo;
                    var nombreDocente = solicitud.Docente.Usuario?.Persona != null 
                        ? $"{solicitud.Docente.Usuario.Persona.Nombres} {solicitud.Docente.Usuario.Persona.Apellidos}"
                        : "Docente";
                    var nivelSolicitado = solicitud.NuevoNivelAcademico?.nombre ?? "Sin información";

                    if (!string.IsNullOrEmpty(correoDocente))
                    {
                        try
                        {
                            var emailEnviado = await _emailService.EnviarCorreoDecisionComision(
                                correoDocente,
                                decisionRequest.Decision,
                                decisionRequest.Observaciones ?? "",
                                nombreDocente,
                                nivelSolicitado
                            );

                            if (!emailEnviado)
                            {
                                // Log del error, pero no falla la operación principal
                                Console.WriteLine($"Advertencia: No se pudo enviar el correo a {correoDocente}");
                            }
                        }
                        catch (Exception emailEx)
                        {
                            // Log del error, pero no falla la operación principal
                            Console.WriteLine($"Error al enviar correo: {emailEx.Message}");
                        }
                    }
                }

                return Ok(new { 
                    mensaje = "Decisión emitida correctamente",
                    decision = decisionRequest.Decision,
                    estadoFinal = solicitud.Estado,
                    fechaDecision = DateTime.Now,
                    correoEnviado = decisionRequest.RequiereNotificacion && 
                                  !string.IsNullOrEmpty(solicitud.Docente?.Usuario?.Correo)
                });
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

        [HttpPost("solicitud/{id}/iniciar-informes")]
        public async Task<ActionResult> IniciarInformes(int id)
        {
            try
            {
                var solicitud = await _context.SolicitudAvanceRango.FindAsync(id);
                if (solicitud == null)
                {
                    return NotFound("Solicitud no encontrada");
                }

                if (solicitud.Estado != "APROBADA_DOCENTE")
                {
                    return BadRequest("La solicitud debe estar aceptada por el docente para iniciar informes");
                }

                // Cambiar el estado a APROBADO_COMISION
                solicitud.Estado = "APROBADO_COMISION";
                solicitud.Observaciones = $"{solicitud.Observaciones} | Comisión preparando informes finales - Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Estado actualizado a 'Preparando Informes'" });
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
                var solicitud = await _context.SolicitudAvanceRango.FindAsync(id);
                if (solicitud == null)
                {
                    return NotFound("Solicitud no encontrada");
                }

                // Solo cambiar el estado de la solicitud
                solicitud.Estado = "INFORMES_FINALES";
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Estado cambiado a 'Informes Finales' correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}
