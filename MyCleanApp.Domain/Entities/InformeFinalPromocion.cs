using System.ComponentModel.DataAnnotations;

namespace MyCleanApp.Domain.Entities
{
    public class InformeFinalPromocion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SolicitudAvanceRangoId { get; set; }
        
        // Alternative property name for compatibility
        public int SolicitudId 
        { 
            get => SolicitudAvanceRangoId; 
            set => SolicitudAvanceRangoId = value; 
        }

        [Required]
        public string Contenido { get; set; } = string.Empty;

        [Required]
        public DateTime FechaCreacion { get; set; }
        
        // Alternative property name for compatibility
        public DateTime FechaGeneracion 
        { 
            get => FechaCreacion; 
            set => FechaCreacion = value; 
        }

        public DateTime? FechaAprobacion { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; } = "GENERADO";

        [StringLength(500)]
        public string? Observaciones { get; set; }

        public int? AprobadoPorUsuarioId { get; set; }
        
        // Alternative property name for compatibility
        public int? GeneradoPor 
        { 
            get => AprobadoPorUsuarioId; 
            set => AprobadoPorUsuarioId = value; 
        }
        
        public DateTime? FechaEnvioConsejo { get; set; }
        public DateTime? FechaAprobacionConsejo { get; set; }

        // Navigation properties
        public virtual SolicitudAvanceRango? SolicitudAvanceRango { get; set; }
        public virtual Usuario? AprobadoPorUsuario { get; set; }
    }
}
