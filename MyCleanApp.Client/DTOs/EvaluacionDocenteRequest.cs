namespace MyCleanApp.Client.DTOs
{
    public class EvaluacionDocenteRequest
    {
        public string Periodo { get; set; } = string.Empty;
        public double Puntaje { get; set; }
        public int DocenteId { get; set; }
        public DateTime? FechaEvaluacion { get; set; }
        public string TipoEvaluacion { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public byte[]? Certificado { get; set; }
        public bool Externo { get; set; } = false;
    }
}
