namespace MyCleanApp.API.DTOs
{
    public class DecisionComisionDto
    {
        public int SolicitudId { get; set; }
        public string Decision { get; set; } = string.Empty; // "APROBADO", "RECHAZADO"
        public string Observaciones { get; set; } = string.Empty;
        public bool RequiereNotificacion { get; set; } = true;
    }
}
