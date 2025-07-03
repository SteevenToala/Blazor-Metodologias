namespace MyCleanApp.Application.DTOs
{
    public class SolicitudWorkflowDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string DocenteNombre { get; set; } = "";
        public string Estado { get; set; } = "";
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaLimiteRespuesta { get; set; }
        public bool RequiereAtencion { get; set; }
        public string NuevoNivel { get; set; } = "";
        public int DiasPendientes { get; set; }
        public bool EsUrgente => DiasPendientes <= 1;
        public bool EstaVencida => DiasPendientes < 0;
    }

    public class WorkflowResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public object? Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
