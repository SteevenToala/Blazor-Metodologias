namespace MyCleanApp.API.DTOs
{
    public class EvaluacionDocenteUpdateDto
    {
        public int Id { get; set; }
        public double Puntaje { get; set; }
        public string Periodo { get; set; } = string.Empty;
        public string TipoEvaluacion { get; set; } = string.Empty;
        public DateTime FechaEvaluacion { get; set; }
        public string? Observaciones { get; set; }
        public byte[]? Certificado { get; set; }
    }
}
