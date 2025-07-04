namespace MyCleanApp.API.DTOs
{
    public class VerificacionRequest
    {
        public string TipoDocumento { get; set; } = string.Empty;
        public bool Verificado { get; set; }
        public string? Observaciones { get; set; }
        public string? VerificadoPor { get; set; }
    }
}
