namespace MyCleanApp.Domain.Entities
{
    public class ListaVerificacion
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public string TipoDocumento { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public DateTime FechaVerificacion { get; set; }
        public string VerificadoPor { get; set; } = string.Empty;
        public string? Observaciones { get; set; }

        // Navegación
        public SolicitudAvanceRango? SolicitudAvanceRango { get; set; }
    }
}
