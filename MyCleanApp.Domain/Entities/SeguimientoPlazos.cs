using System.ComponentModel.DataAnnotations;

namespace MyCleanApp.Domain.Entities
{
    public class SeguimientoPlazos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SolicitudId { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoEvento { get; set; } = string.Empty; // 'NOTIFICACION_RESULTADO', 'PLAZO_RESPUESTA', 'APELACION', 'RESPUESTA_APELACION'

        [Required]
        public DateTime FechaEvento { get; set; }

        public DateTime FechaLimite { get; set; }

        public bool Cumplido { get; set; } = false;

        [StringLength(300)]
        public string? Observaciones { get; set; }

        // Navigation properties
        public virtual SolicitudAvanceRango? SolicitudAvanceRango { get; set; }
    }
}
