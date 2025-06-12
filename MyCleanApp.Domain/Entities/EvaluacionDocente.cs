namespace MyCleanApp.Domain.Entities
{
    public class EvaluacionDocente
    {
        public int Id { get; set; }
        public string Periodo { get; set; } = string.Empty;
        public float Puntaje { get; set; }
        public int DocenteId { get; set; }

        public Docente? Docente { get; set; }
    }
}