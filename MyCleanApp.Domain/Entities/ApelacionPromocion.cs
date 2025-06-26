namespace MyCleanApp.Domain.Entities
{
    public class ApelacionPromocion
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public DateTime FechaApelacion { get; set; }
        public string MotivoApelacion { get; set; } = string.Empty;
        public string DocumentosRespaldo { get; set; } = string.Empty;
        public string Estado { get; set; } = "PENDIENTE"; // PENDIENTE, APROBADA, RECHAZADA
        public DateTime? FechaRespuesta { get; set; }
        public string? RespuestaComision { get; set; }
        public bool Resuelto { get; set; } = false;
        
        // Navigation properties
        public virtual SolicitudAvanceRango? SolicitudAvanceRango { get; set; }
    }
}
