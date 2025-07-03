using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;
using MyCleanApp.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace MyCleanApp.Infrastructure.Services
{
    public interface IPromocionWorkflowService
    {
        Task<WorkflowResult> PresentarSolicitudAsync(int solicitudId);
        Task<WorkflowResult> RecibirEnTalentoHumanoAsync(int solicitudId, int usuarioId);
        Task<WorkflowResult> IniciarVerificacionAsync(int solicitudId, int usuarioId);
        Task<WorkflowResult> CompletarVerificacionAsync(int solicitudId, bool documentosValidos, string observaciones);
        Task<WorkflowResult> EnviarAComisionAsync(int solicitudId);
        Task<WorkflowResult> IniciarAnalisisComisionAsync(int solicitudId);
        Task<WorkflowResult> NotificarResultadoAsync(int solicitudId, bool aprobada, string observaciones);
        Task<WorkflowResult> RegistrarRespuestaDocenteAsync(int solicitudId, bool acepta);
        Task<WorkflowResult> PresentarApelacionAsync(int solicitudId, string motivo, string fundamentos);
        Task<WorkflowResult> ResolverApelacionAsync(int solicitudId, bool aceptada, string resolucion);
        Task<WorkflowResult> GenerarInformeFinalAsync(int solicitudId);
        Task<WorkflowResult> EnviarAConsejoAsync(int solicitudId);
        Task<WorkflowResult> AprobarEnConsejoAsync(int solicitudId);
        Task<WorkflowResult> HacerPromocionEfectivaAsync(int solicitudId);
        Task<List<SolicitudWorkflowDto>> GetSolicitudesPorEstadoAsync(string estado);
        Task<List<SolicitudWorkflowDto>> GetSolicitudesVencidasAsync();
        Task ProcesarSolicitudesVencidasAsync();
    }

    public class PromocionWorkflowService : IPromocionWorkflowService
    {
        private readonly AppDbContext _context;
        private readonly INotificacionService _notificacionService;

        public PromocionWorkflowService(AppDbContext context, INotificacionService notificacionService)
        {
            _context = context;
            _notificacionService = notificacionService;
        }

        public async Task<WorkflowResult> PresentarSolicitudAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            solicitud.Estado = EstadoSolicitudPromocion.PRESENTADA.ToString();
            solicitud.FechaPresentacion = DateTime.Now;
            solicitud.RequiereAtencion = false;

            await _context.SaveChangesAsync();

            // Notificar a Talento Humano
            await _notificacionService.NotificarNuevaSolicitudAsync(solicitud.Id);

            return WorkflowResult.CreateSuccess("Solicitud presentada exitosamente", 
                "La solicitud ha sido enviada a la Dirección de Talento Humano para su procesamiento");
        }

        public async Task<WorkflowResult> RecibirEnTalentoHumanoAsync(int solicitudId, int usuarioId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.PRESENTADA.ToString())
                return WorkflowResult.CreateError("La solicitud debe estar en estado PRESENTADA");

            solicitud.Estado = EstadoSolicitudPromocion.RECIBIDA_TALENTO_HUMANO.ToString();
            solicitud.FechaRecepcionTalentoHumano = DateTime.Now;
            solicitud.VerificadoPor = usuarioId;
            solicitud.RequiereAtencion = true; // Requiere iniciar verificación

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Solicitud recibida en Talento Humano", 
                "Se debe iniciar la verificación de documentos con la lista de verificación");
        }

        public async Task<WorkflowResult> IniciarVerificacionAsync(int solicitudId, int usuarioId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.RECIBIDA_TALENTO_HUMANO.ToString())
                return WorkflowResult.CreateError("La solicitud debe estar recibida en Talento Humano");

            solicitud.Estado = EstadoSolicitudPromocion.EN_VERIFICACION.ToString();
            solicitud.VerificadoPor = usuarioId;
            solicitud.RequiereAtencion = true;

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Verificación iniciada", 
                "Se ha iniciado la verificación de documentos según la lista de verificación");
        }

        public async Task<WorkflowResult> CompletarVerificacionAsync(int solicitudId, bool documentosValidos, string observaciones)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.EN_VERIFICACION.ToString())
                return WorkflowResult.CreateError("La solicitud debe estar en verificación");

            if (documentosValidos)
            {
                solicitud.Estado = EstadoSolicitudPromocion.DOCUMENTOS_VERIFICADOS.ToString();
                solicitud.DocumentosVerificados = true;
                solicitud.RequiereAtencion = true; // Requiere envío a comisión
                solicitud.Observaciones = observaciones;

                await _context.SaveChangesAsync();

                return WorkflowResult.CreateSuccess("Documentos verificados correctamente", 
                    "La solicitud está lista para ser enviada a la Comisión Académica");
            }
            else
            {
                solicitud.Estado = EstadoSolicitudPromocion.RECHAZADA.ToString();
                solicitud.FechaRespuesta = DateTime.Now;
                solicitud.Observaciones = $"Documentos insuficientes: {observaciones}";
                solicitud.RequiereAtencion = false;

                await _context.SaveChangesAsync();

                // Notificar al docente del rechazo por documentos
                await _notificacionService.NotificarRechazoDocumentosAsync(solicitud.DocenteId, observaciones);

                return WorkflowResult.CreateSuccess("Solicitud rechazada por documentos", 
                    "Los documentos no cumplen con los requisitos de la lista de verificación");
            }
        }

        public async Task<WorkflowResult> EnviarAComisionAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.DOCUMENTOS_VERIFICADOS.ToString())
                return WorkflowResult.CreateError("Los documentos deben estar verificados primero");

            solicitud.Estado = EstadoSolicitudPromocion.ENVIADA_COMISION.ToString();
            solicitud.FechaEnvioComision = DateTime.Now;
            solicitud.RequiereAtencion = false; // Ahora es responsabilidad de la comisión

            await _context.SaveChangesAsync();

            // Notificar a la Comisión Académica
            await _notificacionService.NotificarComisionNuevaSolicitudAsync(solicitud.Id);

            return WorkflowResult.CreateSuccess("Solicitud enviada a Comisión Académica", 
                "La Comisión tiene máximo 10 días hábiles para analizar la solicitud");
        }

        public async Task<WorkflowResult> IniciarAnalisisComisionAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.ENVIADA_COMISION.ToString())
                return WorkflowResult.CreateError("La solicitud debe estar enviada a la comisión");

            solicitud.Estado = EstadoSolicitudPromocion.EN_ANALISIS_COMISION.ToString();
            solicitud.RequiereAtencion = true; // Comisión debe completar análisis

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Análisis iniciado por la Comisión", 
                "La Comisión Académica ha iniciado el análisis de la solicitud");
        }

        public async Task<WorkflowResult> NotificarResultadoAsync(int solicitudId, bool aprobada, string observaciones)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.EN_ANALISIS_COMISION.ToString())
                return WorkflowResult.CreateError("La solicitud debe estar en análisis");

            solicitud.Estado = EstadoSolicitudPromocion.RESULTADO_NOTIFICADO.ToString();
            solicitud.FechaNotificacion = DateTime.Now;
            solicitud.FechaLimiteRespuesta = DateTime.Now.AddDays(3); // 3 días para responder
            solicitud.Observaciones = observaciones;
            solicitud.RequiereAtencion = false; // Ahora espera respuesta del docente

            await _context.SaveChangesAsync();

            // Cambiar a estado de espera
            await CambiarAEsperandoRespuestaAsync(solicitudId);

            // Notificar al docente
            await _notificacionService.NotificarResultadoComisionAsync(solicitud.DocenteId, aprobada, observaciones, solicitud.FechaLimiteRespuesta.Value);

            return WorkflowResult.CreateSuccess("Resultado notificado al docente", 
                $"El docente tiene hasta el {solicitud.FechaLimiteRespuesta.Value:dd/MM/yyyy} para responder");
        }

        private async Task CambiarAEsperandoRespuestaAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud != null)
            {
                solicitud.Estado = EstadoSolicitudPromocion.ESPERANDO_RESPUESTA.ToString();
                solicitud.RequiereAtencion = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<WorkflowResult> RegistrarRespuestaDocenteAsync(int solicitudId, bool acepta)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.ESPERANDO_RESPUESTA.ToString())
                return WorkflowResult.CreateError("No se está esperando respuesta del docente");

            if (DateTime.Now > solicitud.FechaLimiteRespuesta)
                return WorkflowResult.CreateError("El plazo para responder ha vencido");

            solicitud.RespuestaDocente = acepta;
            solicitud.FechaRespuesta = DateTime.Now;

            if (acepta)
            {
                solicitud.Estado = EstadoSolicitudPromocion.RESPUESTA_ACEPTADA.ToString();
                solicitud.RequiereAtencion = true; // Proceder con informe final

                await _context.SaveChangesAsync();

                return WorkflowResult.CreateSuccess("Respuesta registrada - Aceptada", 
                    "El docente ha aceptado el resultado. Se procederá con el informe final");
            }
            else
            {
                // Si no acepta, automaticamente se inicia apelación
                solicitud.Estado = EstadoSolicitudPromocion.EN_APELACION.ToString();
                solicitud.RequiereAtencion = false; // Espera que docente presente apelación formal

                await _context.SaveChangesAsync();

                return WorkflowResult.CreateSuccess("Respuesta registrada - No acepta", 
                    "El docente no acepta el resultado. Debe presentar apelación formal en 3 días");
            }
        }

        public async Task<WorkflowResult> PresentarApelacionAsync(int solicitudId, string motivo, string fundamentos)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.EN_APELACION.ToString())
                return WorkflowResult.CreateError("La solicitud debe estar en estado de apelación");

            // Crear registro de apelación
            var apelacion = new ApelacionPromocion
            {
                SolicitudId = solicitudId,
                FechaApelacion = DateTime.Now,
                MotivoApelacion = motivo,
                DocumentosRespaldo = fundamentos,
                Estado = "PENDIENTE"
            };

            _context.ApelacionPromocion.Add(apelacion);
            
            solicitud.Estado = EstadoSolicitudPromocion.APELACION_EN_ANALISIS.ToString();
            solicitud.RequiereAtencion = true; // Comisión debe analizar apelación

            await _context.SaveChangesAsync();

            // Notificar a la comisión
            await _notificacionService.NotificarNuevaApelacionAsync(solicitud.Id, motivo);

            return WorkflowResult.CreateSuccess("Apelación presentada", 
                "La apelación ha sido presentada y será analizada por la Comisión en 3 días");
        }

        public async Task<WorkflowResult> ResolverApelacionAsync(int solicitudId, bool aceptada, string resolucion)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.APELACION_EN_ANALISIS.ToString())
                return WorkflowResult.CreateError("La solicitud debe estar en análisis de apelación");

            var apelacion = await _context.ApelacionPromocion
                .FirstOrDefaultAsync(a => a.SolicitudId == solicitudId && a.Estado == "PENDIENTE");

            if (apelacion != null)
            {
                apelacion.Estado = aceptada ? "APROBADA" : "RECHAZADA";
                apelacion.FechaRespuesta = DateTime.Now;
                apelacion.RespuestaComision = resolucion;
                apelacion.Resuelto = true;
            }

            solicitud.Estado = EstadoSolicitudPromocion.APELACION_RESUELTA.ToString();
            solicitud.Observaciones += $"\nApelación: {resolucion}";
            solicitud.RequiereAtencion = true; // Proceder según resultado

            await _context.SaveChangesAsync();

            // Notificar al docente
            await _notificacionService.NotificarResultadoApelacionAsync(solicitud.DocenteId, aceptada, resolucion);

            return WorkflowResult.CreateSuccess("Apelación resuelta", 
                $"La apelación ha sido {(aceptada ? "aceptada" : "rechazada")}");
        }

        public async Task<WorkflowResult> GenerarInformeFinalAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango
                .Include(s => s.Docente!)
                    .ThenInclude(d => d.Usuario!)
                        .ThenInclude(u => u.Persona!)
                .Include(s => s.NuevoNivelAcademico!)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            var estadosValidos = new[] { 
                EstadoSolicitudPromocion.RESPUESTA_ACEPTADA.ToString(),
                EstadoSolicitudPromocion.APELACION_RESUELTA.ToString()
            };

            if (!estadosValidos.Contains(solicitud.Estado))
                return WorkflowResult.CreateError("La solicitud debe estar aceptada o con apelación resuelta");

            var informe = new InformeFinalPromocion
            {
                SolicitudId = solicitudId,
                FechaGeneracion = DateTime.Now,
                Contenido = GenerarContenidoInforme(solicitud),
                Estado = "GENERADO",
                GeneradoPor = solicitud.VerificadoPor
            };

            _context.InformeFinalPromocion.Add(informe);
            
            solicitud.Estado = EstadoSolicitudPromocion.INFORME_FINAL_GENERADO.ToString();
            solicitud.RequiereAtencion = true; // Enviar a Consejo

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Informe final generado", 
                "El informe final ha sido generado y está listo para enviar al Consejo Universitario");
        }

        public async Task<WorkflowResult> EnviarAConsejoAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.INFORME_FINAL_GENERADO.ToString())
                return WorkflowResult.CreateError("Debe generarse el informe final primero");

            var informe = await _context.InformeFinalPromocion
                .FirstOrDefaultAsync(i => i.SolicitudId == solicitudId);

            if (informe != null)
            {
                informe.Estado = "ENVIADO_CONSEJO";
                informe.FechaEnvioConsejo = DateTime.Now;
            }

            solicitud.Estado = EstadoSolicitudPromocion.ENVIADO_CONSEJO.ToString();
            solicitud.RequiereAtencion = false; // Espera decisión del Consejo

            await _context.SaveChangesAsync();

            // Notificar al Consejo Universitario
            await _notificacionService.NotificarConsejoNuevoInformeAsync(solicitud.Id);

            return WorkflowResult.CreateSuccess("Informe enviado al Consejo Universitario", 
                "El informe final ha sido enviado al Honorable Consejo Universitario para su aprobación");
        }

        public async Task<WorkflowResult> AprobarEnConsejoAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.ENVIADO_CONSEJO.ToString())
                return WorkflowResult.CreateError("La solicitud debe estar enviada al Consejo");

            var informe = await _context.InformeFinalPromocion
                .FirstOrDefaultAsync(i => i.SolicitudId == solicitudId);

            if (informe != null)
            {
                informe.Estado = "APROBADO";
                informe.FechaAprobacionConsejo = DateTime.Now;
            }

            solicitud.Estado = EstadoSolicitudPromocion.APROBADO_CONSEJO.ToString();
            solicitud.RequiereAtencion = true; // Hacer efectiva la promoción

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Aprobado por el Consejo Universitario", 
                "La promoción ha sido aprobada por el Consejo. Se procederá a hacer efectiva la promoción");
        }

        public async Task<WorkflowResult> HacerPromocionEfectivaAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != EstadoSolicitudPromocion.APROBADO_CONSEJO.ToString())
                return WorkflowResult.CreateError("La promoción debe estar aprobada por el Consejo");

            // Actualizar nivel del docente
            if (solicitud.Docente != null && solicitud.NuevoNivelAcademicoId.HasValue)
            {
                solicitud.Docente.NivelAcademicoId = solicitud.NuevoNivelAcademicoId.Value;
                solicitud.Docente.FechaInicioNivel = DateTime.Now;
            }

            solicitud.Estado = EstadoSolicitudPromocion.PROMOCION_EFECTIVA.ToString();
            solicitud.RequiereAtencion = false; // Proceso completado
            solicitud.FechaRespuesta = DateTime.Now;

            await _context.SaveChangesAsync();

            // Notificar al docente de la promoción efectiva
            await _notificacionService.NotificarPromocionEfectivaAsync(solicitud.DocenteId, solicitud.NuevoNivelAcademicoId ?? 0);

            return WorkflowResult.CreateSuccess("Promoción efectiva", 
                "La promoción ha sido hecha efectiva. El docente ha sido promovido oficialmente");
        }

        public async Task<List<SolicitudWorkflowDto>> GetSolicitudesPorEstadoAsync(string estado)
        {
            var solicitudes = await _context.SolicitudAvanceRango
                .Include(s => s.Docente!)
                    .ThenInclude(d => d.Usuario!)
                        .ThenInclude(u => u.Persona!)
                .Include(s => s.NuevoNivelAcademico!)
                .Where(s => s.Estado == estado)
                .ToListAsync();

            return solicitudes.Select(s => new SolicitudWorkflowDto
            {
                Id = s.Id,
                DocenteId = s.DocenteId,
                DocenteNombre = (s.Docente?.Usuario?.Persona != null) ?
                    $"{s.Docente.Usuario.Persona.Nombres} {s.Docente.Usuario.Persona.Apellidos}" : "N/A",
                Estado = s.Estado,
                FechaSolicitud = s.FechaSolicitud,
                FechaLimiteRespuesta = s.FechaLimiteRespuesta,
                RequiereAtencion = s.RequiereAtencion,
                NuevoNivel = s.NuevoNivelAcademico?.nombre ?? "N/A",
                DiasPendientes = s.FechaLimiteRespuesta.HasValue ? 
                    (int)(s.FechaLimiteRespuesta.Value - DateTime.Now).TotalDays : 0
            }).ToList();
        }

        public async Task<List<SolicitudWorkflowDto>> GetSolicitudesVencidasAsync()
        {
            var ahora = DateTime.Now;
            var solicitudes = await _context.SolicitudAvanceRango
                .Include(s => s.Docente!)
                    .ThenInclude(d => d.Usuario!)
                        .ThenInclude(u => u.Persona!)
                .Include(s => s.NuevoNivelAcademico!)
                .Where(s => s.FechaLimiteRespuesta.HasValue && 
                           s.FechaLimiteRespuesta < ahora && 
                           s.Estado == EstadoSolicitudPromocion.ESPERANDO_RESPUESTA.ToString())
                .ToListAsync();

            return solicitudes.Select(s => new SolicitudWorkflowDto
            {
                Id = s.Id,
                DocenteId = s.DocenteId,
                DocenteNombre = (s.Docente?.Usuario?.Persona != null) ?
                    $"{s.Docente.Usuario.Persona.Nombres} {s.Docente.Usuario.Persona.Apellidos}" : "N/A",
                Estado = s.Estado,
                FechaSolicitud = s.FechaSolicitud,
                FechaLimiteRespuesta = s.FechaLimiteRespuesta,
                RequiereAtencion = s.RequiereAtencion,
                NuevoNivel = s.NuevoNivelAcademico?.nombre ?? "N/A",
                DiasPendientes = s.FechaLimiteRespuesta.HasValue ? 
                    (int)(ahora - s.FechaLimiteRespuesta.Value).TotalDays : 0
            }).ToList();
        }

        public async Task ProcesarSolicitudesVencidasAsync()
        {
            var vencidas = await _context.SolicitudAvanceRango
                .Where(s => s.FechaLimiteRespuesta.HasValue && 
                           s.FechaLimiteRespuesta < DateTime.Now && 
                           s.Estado == EstadoSolicitudPromocion.ESPERANDO_RESPUESTA.ToString())
                .ToListAsync();

            foreach (var solicitud in vencidas)
            {
                solicitud.Estado = EstadoSolicitudPromocion.SIN_EFECTO.ToString();
                solicitud.RequiereAtencion = false;
                solicitud.Observaciones += "\nSolicitud sin efecto por no responder en el plazo establecido";
                
                // Notificar al docente
                await _notificacionService.NotificarSolicitudSinEfectoAsync(solicitud.DocenteId);
            }

            await _context.SaveChangesAsync();
        }

        private string GenerarContenidoInforme(SolicitudAvanceRango solicitud)
        {
            return $@"
INFORME FINAL DE PROMOCIÓN ACADÉMICA

Docente: {solicitud.Docente?.Usuario?.Persona?.Nombres} {solicitud.Docente?.Usuario?.Persona?.Apellidos}
Promoción solicitada: {solicitud.NuevoNivelAcademico?.nombre}
Fecha de solicitud: {solicitud.FechaSolicitud:dd/MM/yyyy}

PROCESO REALIZADO:
- Verificación de documentos completada exitosamente
- Análisis por la Comisión Académica de Escalafón y Promoción
- Cumplimiento de requisitos reglamentarios verificado

OBSERVACIONES:
{solicitud.Observaciones}

RECOMENDACIÓN:
Se recomienda al Honorable Consejo Universitario la aprobación de la promoción solicitada.

Fecha: {DateTime.Now:dd/MM/yyyy}
Comisión Académica de Escalafón y Promoción
Universidad Técnica de Ambato
";
        }
    }
}

