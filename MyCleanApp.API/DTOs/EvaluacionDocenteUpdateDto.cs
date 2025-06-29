namespace MyCleanApp.API.DTOs
{
    public class EvaluacionDocenteUpdateDto
    {
        public int Id { get; set; }
        public double Puntaje { get; set; }
        public string Periodo { get; set; } = string.Empty;
    }
}
