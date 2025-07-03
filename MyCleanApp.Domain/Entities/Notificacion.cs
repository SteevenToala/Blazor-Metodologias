using System.ComponentModel.DataAnnotations;

namespace MyCleanApp.Domain.Entities
{
    public enum TipoNotificacion
    {
        SOLICITUD_RECIBIDA,
        DOCUMENTOS_VERIFICADOS,
        DOCUMENTOS_RECHAZADOS,
        ENVIADA_COMISION,
        RESULTADO_COMISION,
        SOLICITUD_APELACION,
        RESULTADO_APELACION,
        INFORME_FINAL,
        APROBACION_CONSEJO,
        SOLICITUD_ANULADA
    }

    public class Notificacion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SolicitudAvanceRangoId { get; set; }

        [Required]
        public int DocenteId { get; set; }

        [Required]
        [StringLength(50)]
        public string Tipo { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        public string Contenido { get; set; } = string.Empty;

        [Required]
        public DateTime FechaEnvio { get; set; }

        public bool Leida { get; set; } = false;

        public DateTime? FechaLectura { get; set; }

        // Navigation properties
        public virtual SolicitudAvanceRango? SolicitudAvanceRango { get; set; }
        public virtual Docente? Docente { get; set; }
    }
}
