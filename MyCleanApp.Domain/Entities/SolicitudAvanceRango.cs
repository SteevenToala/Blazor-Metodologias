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
        [StringLength(100)]
        public string RangoSolicitado { get; set; } = string.Empty;

        [Required]
        public DateTime FechaSolicitud { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = EstadoSolicitudPromocion.PRESENTADA.ToString();

        [StringLength(500)]
        public string? Observaciones { get; set; }

        // Workflow fields
        public DateTime? FechaPresentacion { get; set; }
        public DateTime? FechaRecepcionTalentoHumano { get; set; }
        public DateTime? FechaVerificacionDocumentos { get; set; }
        public bool DocumentosVerificados { get; set; }
        public string? ObservacionesVerificacion { get; set; }
        public DateTime? FechaEnvioComision { get; set; }
        public DateTime? FechaAnalisisComision { get; set; }
        public DateTime? FechaNotificacionResultado { get; set; }
        public bool CumpleRequisitos { get; set; }
        public string? ObservacionesComision { get; set; }
        public DateTime? FechaRespuestaDocente { get; set; }
        public bool? AceptaResultado { get; set; }
        public DateTime? FechaApelacion { get; set; }
        public string? MotivoApelacion { get; set; }
        public DateTime? FechaResolucionApelacion { get; set; }
        public string? ResultadoApelacion { get; set; }
        public DateTime? FechaInformeFinal { get; set; }
        public DateTime? FechaEnvioConsejo { get; set; }
        public DateTime? FechaAprobacionConsejo { get; set; }
        
        // Additional workflow fields
        public bool RequiereAtencion { get; set; } = false;
        public int? VerificadoPor { get; set; }
        public int? AnalizadoPor { get; set; }
        public string? FundamentosApelacion { get; set; }
        public DateTime? FechaNotificacion { get; set; }
        public DateTime? FechaLimiteRespuesta { get; set; }
        public bool? RespuestaDocente { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public int? NuevoNivelAcademicoId { get; set; }

        // Navigation properties
        public virtual Docente? Docente { get; set; }
        public virtual Usuario? VerificadoPorUsuario { get; set; }
        public virtual Usuario? AnalizadoPorUsuario { get; set; }
        public virtual NivelAcademico? NuevoNivelAcademico { get; set; }
        public virtual ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
        public virtual InformeFinalPromocion? InformeFinal { get; set; }
    }
}
