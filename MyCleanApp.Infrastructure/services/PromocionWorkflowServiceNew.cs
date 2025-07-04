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
        Task<WorkflowResult> IniciarInformesFinalesAsync(int solicitudId);
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

            solicitud.Estado = "PRESENTADA";
            solicitud.FechaPresentacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Solicitud presentada exitosamente", 
                "La solicitud ha sido enviada a la Dirección de Talento Humano para su procesamiento");
        }

        public async Task<WorkflowResult> RecibirEnTalentoHumanoAsync(int solicitudId, int usuarioId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "PRESENTADA")
                return WorkflowResult.CreateError("La solicitud debe estar en estado PRESENTADA");

            solicitud.Estado = "RECIBIDA_TALENTO_HUMANO";
            solicitud.FechaRecepcionTalentoHumano = DateTime.Now;
            solicitud.VerificadoPor = usuarioId;

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Solicitud recibida en Talento Humano", 
                "Se debe iniciar la verificación de documentos con la lista de verificación");
        }

        public async Task<WorkflowResult> IniciarVerificacionAsync(int solicitudId, int usuarioId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "RECIBIDA_TALENTO_HUMANO" && solicitud.Estado != "PENDIENTE")
                return WorkflowResult.CreateError("La solicitud debe estar recibida en Talento Humano");

            solicitud.Estado = "EN_VERIFICACION";
            solicitud.VerificadoPor = usuarioId;

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Verificación iniciada", 
                "Se ha iniciado la verificación de documentos según la lista de verificación");
        }

        public async Task<WorkflowResult> CompletarVerificacionAsync(int solicitudId, bool documentosValidos, string observaciones)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "EN_VERIFICACION")
                return WorkflowResult.CreateError("La solicitud debe estar en verificación");

            if (documentosValidos)
            {
                solicitud.Estado = "VERIFICADA";
                solicitud.DocumentosVerificados = true;
                solicitud.Observaciones = observaciones;
            }
            else
            {
                solicitud.Estado = "RECHAZADA";
                solicitud.DocumentosVerificados = false;
                solicitud.Observaciones = observaciones;
                solicitud.FechaRespuesta = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            if (documentosValidos)
            {
                return WorkflowResult.CreateSuccess("Verificación completada", 
                    "Los documentos han sido verificados correctamente. La solicitud está lista para enviar a la Comisión");
            }
            else
            {
                return WorkflowResult.CreateSuccess("Solicitud rechazada", 
                    "La solicitud ha sido rechazada por documentos faltantes o inválidos");
            }
        }

        public async Task<WorkflowResult> EnviarAComisionAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "VERIFICADA")
                return WorkflowResult.CreateError("La solicitud debe estar verificada");

            solicitud.Estado = "ENVIADA_COMISION";
            solicitud.FechaEnvioComision = DateTime.Now;

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Solicitud enviada a la Comisión", 
                "La solicitud ha sido enviada a la Comisión Académica de Escalafón y Promoción para su análisis");
        }

        public async Task<WorkflowResult> IniciarAnalisisComisionAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "ENVIADA_COMISION")
                return WorkflowResult.CreateError("La solicitud debe estar enviada a la comisión");

            solicitud.Estado = "EN_ANALISIS_COMISION";

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Análisis iniciado", 
                "La Comisión ha iniciado el análisis de la solicitud");
        }

        public async Task<WorkflowResult> NotificarResultadoAsync(int solicitudId, bool aprobada, string observaciones)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "EN_ANALISIS_COMISION")
                return WorkflowResult.CreateError("La solicitud debe estar en análisis por la comisión");

            solicitud.Estado = aprobada ? "APROBADA" : "RECHAZADA";
            solicitud.Observaciones = observaciones;
            solicitud.FechaRespuesta = DateTime.Now;

            await _context.SaveChangesAsync();

            var mensaje = aprobada ? "aprobada" : "rechazada";
            return WorkflowResult.CreateSuccess($"Resultado notificado", 
                $"La solicitud ha sido {mensaje} por la Comisión");
        }

        public async Task<WorkflowResult> RegistrarRespuestaDocenteAsync(int solicitudId, bool acepta)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            var respuesta = acepta ? "ACEPTA" : "NO_ACEPTA";
            solicitud.Observaciones = $"{solicitud.Observaciones} | Respuesta docente: {respuesta}";

            if (acepta)
            {
                solicitud.Estado = "APROBADA_DOCENTE";
            }
            else
            {
                solicitud.Estado = "EN_APELACION";
            }

            await _context.SaveChangesAsync();

            var mensaje = acepta ? "aceptada" : "rechazada (puede presentar apelación)";
            return WorkflowResult.CreateSuccess("Respuesta registrada", 
                $"La respuesta del docente ha sido registrada como {mensaje}");
        }

        public async Task<WorkflowResult> IniciarInformesFinalesAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "APROBADA_DOCENTE")
                return WorkflowResult.CreateError("La solicitud debe estar aceptada por el docente");

            solicitud.Estado = "INFORMES_FINALES";
            solicitud.Observaciones = $"{solicitud.Observaciones} | Iniciando generación de informes finales - Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}";

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Informes finales iniciados", 
                "Se ha iniciado el proceso de generación de informes finales de promoción académica");
        }

        public async Task<WorkflowResult> PresentarApelacionAsync(int solicitudId, string motivo, string fundamentos)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "DECIDIDO_RECHAZADA")
                return WorkflowResult.CreateError("Solo se puede apelar una solicitud rechazada por la comisión");

            // Verificar que no hayan pasado más de 3 días
            if (solicitud.FechaRespuesta.HasValue)
            {
                var diasTranscurridos = (DateTime.Now - solicitud.FechaRespuesta.Value).Days;
                if (diasTranscurridos > 3)
                    return WorkflowResult.CreateError("El plazo para presentar la apelación ha vencido (3 días máximo)");
            }

            solicitud.Estado = "EN_APELACION";
            solicitud.Observaciones = $"{solicitud.Observaciones} | Apelación: {motivo} - Fundamentos: {fundamentos}";
            solicitud.FechaRespuesta = DateTime.Now; // Actualizar fecha de respuesta

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Apelación presentada", 
                "La apelación ha sido presentada y será revisada por la Comisión");
        }

        public async Task<WorkflowResult> ResolverApelacionAsync(int solicitudId, bool aceptada, string resolucion)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "EN_APELACION")
                return WorkflowResult.CreateError("Debe existir una apelación para resolver");

            if (aceptada)
            {
                // Si la apelación es aceptada, volver a análisis de comisión para reevaluación
                solicitud.Estado = "EN_ANALISIS_COMISION";
                solicitud.Observaciones = $"{solicitud.Observaciones} | APELACIÓN ACEPTADA - Reevaluación solicitada: {resolucion}";
                var mensaje = "Apelación aceptada - solicitud enviada para reevaluación";
                
                await _context.SaveChangesAsync();
                return WorkflowResult.CreateSuccess("Apelación aceptada", mensaje);
            }
            else
            {
                // Si la apelación es rechazada, finalizar como rechazada
                solicitud.Estado = "APELACION_RECHAZADA";
                solicitud.Observaciones = $"{solicitud.Observaciones} | APELACIÓN RECHAZADA: {resolucion}";
                solicitud.FechaRespuesta = DateTime.Now;
                
                await _context.SaveChangesAsync();
                return WorkflowResult.CreateSuccess("Apelación rechazada", 
                    "La apelación ha sido rechazada - decisión original mantenida");
            }
        }

        public async Task<WorkflowResult> GenerarInformeFinalAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "ACEPTADA" && solicitud.Estado != "APELACION_ACEPTADA")
                return WorkflowResult.CreateError("La solicitud debe estar aceptada para generar informe final");

            solicitud.Estado = "INFORME_GENERADO";

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Informe final generado", 
                "Se ha generado el informe final para envío al Consejo Universitario");
        }

        public async Task<WorkflowResult> EnviarAConsejoAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "INFORME_GENERADO")
                return WorkflowResult.CreateError("Debe existir un informe final generado");

            solicitud.Estado = "ENVIADO_CONSEJO";

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Enviado al Consejo Universitario", 
                "La solicitud ha sido enviada al Consejo Universitario para su aprobación final");
        }

        public async Task<WorkflowResult> AprobarEnConsejoAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "ENVIADO_CONSEJO")
                return WorkflowResult.CreateError("La solicitud debe estar enviada al Consejo");

            solicitud.Estado = "APROBADO_CONSEJO";

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Aprobado por el Consejo", 
                "La solicitud ha sido aprobada por el Consejo Universitario");
        }

        public async Task<WorkflowResult> HacerPromocionEfectivaAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            if (solicitud == null)
                return WorkflowResult.CreateError("Solicitud no encontrada");

            if (solicitud.Estado != "APROBADO_CONSEJO")
                return WorkflowResult.CreateError("La solicitud debe estar aprobada por el Consejo");

            // Actualizar el nivel del docente
            if (solicitud.Docente != null && solicitud.NuevoNivelAcademicoId.HasValue)
            {
                solicitud.Docente.NivelAcademicoId = solicitud.NuevoNivelAcademicoId.Value;
                solicitud.Docente.FechaInicioNivel = DateTime.Now;
            }

            solicitud.Estado = "PROMOCION_EFECTIVA";

            await _context.SaveChangesAsync();

            return WorkflowResult.CreateSuccess("Promoción efectiva", 
                "La promoción ha sido hecha efectiva. El docente ha sido promovido al nuevo nivel académico");
        }

        public async Task<List<SolicitudWorkflowDto>> GetSolicitudesPorEstadoAsync(string estado)
        {
            var solicitudes = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                    .ThenInclude(d => d!.Usuario)
                        .ThenInclude(u => u!.Persona)
                .Include(s => s.Docente)
                    .ThenInclude(d => d!.NivelAcademico)
                .Include(s => s.NuevoNivelAcademico)
                .Where(s => s.Estado == estado)
                .Select(s => new SolicitudWorkflowDto
                {
                    Id = s.Id,
                    DocenteId = s.DocenteId,
                    DocenteNombre = s.Docente!.Usuario!.Persona != null ? 
                        $"{s.Docente.Usuario.Persona.Nombres} {s.Docente.Usuario.Persona.Apellidos}" : 
                        "Nombre no disponible",
                    Estado = s.Estado,
                    FechaSolicitud = s.FechaSolicitud,
                    FechaRespuesta = s.FechaRespuesta,
                    Observaciones = s.Observaciones ?? "",
                    NuevoNivelAcademicoId = s.NuevoNivelAcademicoId ?? 0,
                    NuevoNivel = s.NuevoNivelAcademico != null ? (s.NuevoNivelAcademico.nombre ?? "No especificado") : "No especificado",
                    NivelActual = s.Docente.NivelAcademico != null ? (s.Docente.NivelAcademico.nombre ?? "No especificado") : "No especificado",
                    NivelSolicitado = s.NuevoNivelAcademico != null ? (s.NuevoNivelAcademico.nombre ?? "No especificado") : "No especificado",
                    DiasPendientes = s.FechaRespuesta.HasValue ? 0 : (int)(DateTime.Now - s.FechaSolicitud).TotalDays
                })
                .ToListAsync();

            return solicitudes;
        }

        public async Task<List<SolicitudWorkflowDto>> GetSolicitudesVencidasAsync()
        {
            var fechaLimite = DateTime.Now.AddDays(-30);

            var solicitudes = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                    .ThenInclude(d => d!.Usuario)
                        .ThenInclude(u => u!.Persona)
                .Include(s => s.Docente)
                    .ThenInclude(d => d!.NivelAcademico)
                .Include(s => s.NuevoNivelAcademico)
                .Where(s => !s.FechaRespuesta.HasValue && s.FechaSolicitud < fechaLimite)
                .Select(s => new SolicitudWorkflowDto
                {
                    Id = s.Id,
                    DocenteId = s.DocenteId,
                    DocenteNombre = s.Docente!.Usuario!.Persona != null ? 
                        $"{s.Docente.Usuario.Persona.Nombres} {s.Docente.Usuario.Persona.Apellidos}" : 
                        "Nombre no disponible",
                    Estado = s.Estado,
                    FechaSolicitud = s.FechaSolicitud,
                    FechaRespuesta = s.FechaRespuesta,
                    Observaciones = s.Observaciones ?? "",
                    NuevoNivelAcademicoId = s.NuevoNivelAcademicoId ?? 0,
                    NuevoNivel = s.NuevoNivelAcademico != null ? (s.NuevoNivelAcademico.nombre ?? "No especificado") : "No especificado",
                    NivelActual = s.Docente.NivelAcademico != null ? (s.Docente.NivelAcademico.nombre ?? "No especificado") : "No especificado",
                    NivelSolicitado = s.NuevoNivelAcademico != null ? (s.NuevoNivelAcademico.nombre ?? "No especificado") : "No especificado",
                    DiasPendientes = (int)(DateTime.Now - s.FechaSolicitud).TotalDays
                })
                .ToListAsync();

            return solicitudes;
        }

        public async Task ProcesarSolicitudesVencidasAsync()
        {
            var solicitudesVencidas = await GetSolicitudesVencidasAsync();
            
            foreach (var solicitud in solicitudesVencidas)
            {
                var entidad = await _context.SolicitudAvanceRango.FindAsync(solicitud.Id);
                if (entidad != null)
                {
                    entidad.Observaciones = $"{entidad.Observaciones} | VENCIDA: Más de 30 días sin respuesta desde {solicitud.FechaSolicitud:dd/MM/yyyy}";
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
