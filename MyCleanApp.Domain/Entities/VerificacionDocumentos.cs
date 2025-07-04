using System.ComponentModel.DataAnnotations;

namespace MyCleanApp.Domain.Entities
{
    public class VerificacionDocumentos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SolicitudId { get; set; }

        [Required]
        public int ListaVerificacionId { get; set; }

        public bool Verificado { get; set; } = false;

        [StringLength(300)]
        public string? Observaciones { get; set; }

        public DateTime? FechaVerificacion { get; set; }

        public int? VerificadoPor { get; set; } // usuarioId quien verificó

        // Navigation properties
        public virtual SolicitudAvanceRango? SolicitudAvanceRango { get; set; }
        public virtual ListaVerificacion? ListaVerificacion { get; set; }
        public virtual Usuario? VerificadoPorUsuario { get; set; }
    }
}
