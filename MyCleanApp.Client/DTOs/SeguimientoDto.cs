namespace MyCleanApp.Client.DTOs
{
    public class SeguimientoPlazoDto
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public string TipoEvento { get; set; } = string.Empty;
        public DateTime FechaEvento { get; set; }
        public DateTime FechaLimite { get; set; }
        public bool Cumplido { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        
        // Campos calculados
        public int DiasRestantes { get; set; }
        public bool VencidoPlazo { get; set; }
        public string DocenteNombre { get; set; } = string.Empty;
    }

    public class ComisionAcademicaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaDesignacion { get; set; }
        public DateTime? FechaFinPeriodo { get; set; }
    }

    public class InformeFinalPromocionDto
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaEnvioConsejo { get; set; }
        public DateTime? FechaAprobacionConsejo { get; set; }
        public string GeneradoPor { get; set; } = string.Empty;
        
        // Datos adicionales para mostrar
        public string DocenteNombre { get; set; } = string.Empty;
        public string NivelSolicitado { get; set; } = string.Empty;
    }
}
