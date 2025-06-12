namespace MyCleanApp.Domain.Entities;

public class EvaluacionDocente
{
    public int Id { get; set; }
    public string Periodo { get; set; } = null!;
    public float Puntaje { get; set; }
    public int DocenteId { get; set; }
    public Docente Docente { get; set; } = null!;
}
