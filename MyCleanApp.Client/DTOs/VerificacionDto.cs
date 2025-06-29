namespace MyCleanApp.Client.DTOs
{
    public class SolicitudVerificacionDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string DocenteNombre { get; set; } = string.Empty;
        public string NivelActual { get; set; } = string.Empty;
        public string NivelSolicitado { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaRespuesta { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }

    public class RequisitoVerificacionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public bool EnRevision { get; set; }
        public DateTime? FechaVerificacion { get; set; }
        public string VerificadoPor { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
    }

    public class HistorialVerificacionDto
    {
        public DateTime Fecha { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
    }

    public class VerificacionRequest
    {
        public string TipoDocumento { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public string? Observaciones { get; set; }
        public string? VerificadoPor { get; set; }
    }

    public class EstadoSolicitudRequest
    {
        public string NuevoEstado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }
}
