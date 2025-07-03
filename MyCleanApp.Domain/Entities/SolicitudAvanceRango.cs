using System.ComponentModel.DataAnnotations;

namespace MyCleanApp.Domain.Entities
{
    public enum EstadoSolicitudPromocion
    {
        PRESENTADA,
        RECIBIDA_TALENTO_HUMANO,
        EN_VERIFICACION,
        DOCUMENTOS_VERIFICADOS,
        RECHAZADA,
        ENVIADA_COMISION,
        EN_ANALISIS_COMISION,
        RESULTADO_NOTIFICADO,
        ESPERANDO_RESPUESTA,
        RESPUESTA_ACEPTADA,
        EN_APELACION,
        APELACION_EN_ANALISIS,
        APELADA,
        APELACION_RESUELTA,
        INFORME_FINAL_GENERADO,
        ENVIADA_CONSEJO_UNIVERSITARIO,
        ENVIADO_CONSEJO,
        APROBADO_CONSEJO,
        PROMOCION_EFECTIVA,
        SIN_EFECTO,
        APROBADA,
        ANULADA
    }

    public class SolicitudAvanceRango
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DocenteId { get; set; }

        [Required]
        public DateTime FechaSolicitud { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "PENDIENTE";

        public DateTime? FechaRespuesta { get; set; }

        [StringLength(300)]
        public string? Observaciones { get; set; }

        public int? NuevoNivelAcademicoId { get; set; }

        // Campos adicionales según init.sql
        public DateTime? FechaPresentacion { get; set; }
        public DateTime? FechaRecepcionTalentoHumano { get; set; }
        public DateTime? FechaEnvioComision { get; set; }
        public bool DocumentosVerificados { get; set; } = false;
        public int? VerificadoPor { get; set; }
        public int? PlanificacionId { get; set; }

        // Navigation properties
        public virtual Docente? Docente { get; set; }
        public virtual NivelAcademico? NuevoNivelAcademico { get; set; }
        public virtual Usuario? VerificadoPorUsuario { get; set; }
    }
}
