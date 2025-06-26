namespace MyCleanApp.API.DTOs
{
    public class EvaluacionDocenteDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string DocenteNombre { get; set; } = string.Empty;
        public string DocenteCedula { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
        public double Puntaje { get; set; }
        public string NivelAcademico { get; set; } = string.Empty;
    }
}
