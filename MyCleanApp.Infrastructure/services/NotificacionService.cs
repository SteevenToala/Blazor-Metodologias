using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MyCleanApp.Infrastructure.Services
{
    public interface INotificacionService
    {
        Task NotificarNuevaSolicitudAsync(int solicitudId);
        Task NotificarRechazoDocumentosAsync(int docenteId, string observaciones);
        Task NotificarComisionNuevaSolicitudAsync(int solicitudId);
        Task NotificarResultadoComisionAsync(int docenteId, bool aprobada, string observaciones, DateTime fechaLimite);
        Task NotificarNuevaApelacionAsync(int solicitudId, string motivo);
        Task NotificarResultadoApelacionAsync(int docenteId, bool aceptada, string resolucion);
        Task NotificarConsejoNuevoInformeAsync(int solicitudId);
        Task NotificarPromocionEfectivaAsync(int docenteId, int nuevoNivelId);
        Task NotificarSolicitudSinEfectoAsync(int docenteId);
        Task<List<NotificacionDto>> GetNotificacionesPorUsuarioAsync(int usuarioId);
        Task<List<NotificacionDto>> GetNotificacionesPendientesAsync();
    }

    public class NotificacionService : INotificacionService
    {
        private readonly AppDbContext _context;

        public NotificacionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task NotificarNuevaSolicitudAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            if (solicitud == null) return;

            var mensaje = $"Nueva solicitud de promoción recibida del docente " +
                         $"{solicitud.Docente?.Usuario?.Persona?.Nombres} {solicitud.Docente?.Usuario?.Persona?.Apellidos}";

            await CrearNotificacionAsync(
                destinatarioRol: "TALENTO_HUMANO",
                tipo: "NUEVA_SOLICITUD",
                titulo: "Nueva Solicitud de Promoción",
                mensaje: mensaje,
                solicitudId: solicitudId
            );
        }

        public async Task NotificarRechazoDocumentosAsync(int docenteId, string observaciones)
        {
            var docente = await _context.Docente
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.Id == docenteId);

            if (docente?.Usuario == null) return;

            await CrearNotificacionAsync(
                destinatarioId: docente.Usuario.Id,
                tipo: "RECHAZO_DOCUMENTOS",
                titulo: "Solicitud Rechazada - Documentos Insuficientes",
                mensaje: $"Su solicitud de promoción ha sido rechazada debido a documentos insuficientes: {observaciones}",
                docenteId: docenteId
            );
        }

        public async Task NotificarComisionNuevaSolicitudAsync(int solicitudId)
        {
            var solicitud = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            if (solicitud == null) return;

            var mensaje = $"Nueva solicitud para análisis de la Comisión Académica. " +
                         $"Docente: {solicitud.Docente?.Usuario?.Persona?.Nombres} {solicitud.Docente?.Usuario?.Persona?.Apellidos}. " +
                         $"Plazo máximo: 10 días hábiles.";

            await CrearNotificacionAsync(
                destinatarioRol: "COMISION_ACADEMICA",
                tipo: "SOLICITUD_COMISION",
                titulo: "Nueva Solicitud para Análisis",
                mensaje: mensaje,
                solicitudId: solicitudId,
                fechaLimite: DateTime.Now.AddDays(10) // 10 días hábiles
            );
        }

        public async Task NotificarResultadoComisionAsync(int docenteId, bool aprobada, string observaciones, DateTime fechaLimite)
        {
            var docente = await _context.Docente
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.Id == docenteId);

            if (docente?.Usuario == null) return;

            var titulo = aprobada ? "Solicitud Aprobada por la Comisión" : "Solicitud No Aprobada por la Comisión";
            var mensaje = $"La Comisión Académica ha {(aprobada ? "aprobado" : "no aprobado")} su solicitud de promoción. " +
                         $"Observaciones: {observaciones}. " +
                         $"Tiene hasta el {fechaLimite:dd/MM/yyyy} para responder (aceptar o apelar).";

            await CrearNotificacionAsync(
                destinatarioId: docente.Usuario.Id,
                tipo: aprobada ? "SOLICITUD_APROBADA" : "SOLICITUD_NO_APROBADA",
                titulo: titulo,
                mensaje: mensaje,
                docenteId: docenteId,
                fechaLimite: fechaLimite
            );
        }

        public async Task NotificarNuevaApelacionAsync(int solicitudId, string motivo)
        {
            var solicitud = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .FirstOrDefaultAsync(s => s.Id == solicitudId);

            if (solicitud == null) return;

            var mensaje = $"Nueva apelación presentada por el docente " +
                         $"{solicitud.Docente?.Usuario?.Persona?.Nombres} {solicitud.Docente?.Usuario?.Persona?.Apellidos}. " +
                         $"Motivo: {motivo}. Plazo para resolver: 3 días hábiles.";

            await CrearNotificacionAsync(
                destinatarioRol: "COMISION_ACADEMICA",
                tipo: "NUEVA_APELACION",
                titulo: "Nueva Apelación Presentada",
                mensaje: mensaje,
                solicitudId: solicitudId,
                fechaLimite: DateTime.Now.AddDays(3)
            );
        }

        public async Task NotificarResultadoApelacionAsync(int docenteId, bool aceptada, string resolucion)
        {
            var docente = await _context.Docente
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.Id == docenteId);

            if (docente?.Usuario == null) return;

            var titulo = aceptada ? "Apelación Aceptada" : "Apelación Rechazada";
            var mensaje = $"Su apelación ha sido {(aceptada ? "aceptada" : "rechazada")} por la Comisión Académica. " +
                         $"Resolución: {resolucion}";

            await CrearNotificacionAsync(
                destinatarioId: docente.Usuario.Id,
                tipo: aceptada ? "APELACION_ACEPTADA" : "APELACION_RECHAZADA",
                titulo: titulo,
                mensaje: mensaje,
                docenteId: docenteId
            );
        }

        public async Task NotificarConsejoNuevoInformeAsync(int solicitudId)
        {
            var mensaje = "Nuevo informe final de promoción disponible para revisión y aprobación del Consejo Universitario.";

            await CrearNotificacionAsync(
                destinatarioRol: "CONSEJO_UNIVERSITARIO",
                tipo: "INFORME_CONSEJO",
                titulo: "Nuevo Informe Final para Aprobación",
                mensaje: mensaje,
                solicitudId: solicitudId
            );
        }

        public async Task NotificarPromocionEfectivaAsync(int docenteId, int nuevoNivelId)
        {
            var docente = await _context.Docente
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.Id == docenteId);

            var nivel = await _context.NivelAcademico.FindAsync(nuevoNivelId);

            if (docente?.Usuario == null || nivel == null) return;

            var mensaje = $"¡Felicitaciones! Su promoción al nivel {nivel.nombre} ha sido aprobada y es efectiva desde hoy. " +
                         $"Por favor, actualice su información académica.";

            await CrearNotificacionAsync(
                destinatarioId: docente.Usuario.Id,
                tipo: "PROMOCION_EFECTIVA",
                titulo: "Promoción Efectiva",
                mensaje: mensaje,
                docenteId: docenteId
            );
        }

        public async Task NotificarSolicitudSinEfectoAsync(int docenteId)
        {
            var docente = await _context.Docente
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.Id == docenteId);

            if (docente?.Usuario == null) return;

            var mensaje = "Su solicitud de promoción ha quedado sin efecto por no haber respondido en el plazo establecido de 3 días hábiles.";

            await CrearNotificacionAsync(
                destinatarioId: docente.Usuario.Id,
                tipo: "SOLICITUD_SIN_EFECTO",
                titulo: "Solicitud Sin Efecto",
                mensaje: mensaje,
                docenteId: docenteId
            );
        }

        public async Task<List<NotificacionDto>> GetNotificacionesPorUsuarioAsync(int usuarioId)
        {
            return await _context.Notificacion
                .Where(n => n.DocenteId == usuarioId)
                .OrderByDescending(n => n.FechaEnvio)
                .Select(n => new NotificacionDto
                {
                    Id = n.Id,
                    Tipo = n.Tipo,
                    Titulo = n.Titulo,
                    Mensaje = n.Contenido,
                    FechaCreacion = n.FechaEnvio,
                    FechaLimite = null, // Not in new entity structure
                    Leida = n.Leida,
                    Urgente = false, // Can be calculated based on business logic
                    SolicitudId = n.SolicitudAvanceRangoId
                })
                .ToListAsync();
        }

        public async Task<List<NotificacionDto>> GetNotificacionesPendientesAsync()
        {
            return await _context.Notificacion
                .Where(n => !n.Leida)
                .OrderByDescending(n => n.FechaEnvio)
                .Select(n => new NotificacionDto
                {
                    Id = n.Id,
                    Tipo = n.Tipo,
                    Titulo = n.Titulo,
                    Mensaje = n.Contenido,
                    FechaCreacion = n.FechaEnvio,
                    FechaLimite = null, // Not in new entity structure
                    Leida = n.Leida,
                    Urgente = false, // Can be calculated based on business logic
                    SolicitudId = n.SolicitudAvanceRangoId,
                    DestinatarioRol = "DOCENTE" // Default role, can be enhanced later
                })
                .ToListAsync();
        }

        private async Task CrearNotificacionAsync(
            int? destinatarioId = null,
            string? destinatarioRol = null,
            string tipo = "",
            string titulo = "",
            string mensaje = "",
            int? solicitudId = null,
            int? docenteId = null,
            DateTime? fechaLimite = null)
        {
            var notificacion = new Notificacion
            {
                SolicitudAvanceRangoId = solicitudId ?? 0,
                DocenteId = docenteId ?? destinatarioId ?? 0,
                Tipo = tipo,
                Titulo = titulo,
                Contenido = mensaje,
                FechaEnvio = DateTime.Now,
                Leida = false
            };

            _context.Notificacion.Add(notificacion);
            await _context.SaveChangesAsync();
        }
    }

    // DTO para notificaciones
    public class NotificacionDto
    {
        public int Id { get; set; }
        public string Tipo { get; set; } = "";
        public string Titulo { get; set; } = "";
        public string Mensaje { get; set; } = "";
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaLimite { get; set; }
        public bool Leida { get; set; }
        public bool Urgente { get; set; }
        public int? SolicitudId { get; set; }
        public string? DestinatarioRol { get; set; }
        public string TiempoTranscurrido => ObtenerTiempoTranscurrido();

        private string ObtenerTiempoTranscurrido()
        {
            var tiempo = DateTime.Now - FechaCreacion;
            if (tiempo.TotalMinutes < 60)
                return $"hace {(int)tiempo.TotalMinutes} minutos";
            if (tiempo.TotalHours < 24)
                return $"hace {(int)tiempo.TotalHours} horas";
            return $"hace {(int)tiempo.TotalDays} días";
        }
    }
}
