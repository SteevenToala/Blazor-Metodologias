namespace MyCleanApp.Client.DTOs
{
    public class ApelacionPromocionDto
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public DateTime FechaApelacion { get; set; }
        public string MotivoApelacion { get; set; } = string.Empty;
        public string DocumentosRespaldo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaRespuesta { get; set; }
        public string RespuestaComision { get; set; } = string.Empty;
        public bool Resuelto { get; set; }
        
        // Para mostrar datos adicionales
        public string DocenteNombre { get; set; } = string.Empty;
        public string NivelSolicitado { get; set; } = string.Empty;
    }

    public class CrearApelacionDto
    {
        public int SolicitudId { get; set; }
        public string MotivoApelacion { get; set; } = string.Empty;
        public string DocumentosRespaldo { get; set; } = string.Empty;
    }
}
