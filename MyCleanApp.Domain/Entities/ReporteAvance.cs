namespace MyCleanApp.Domain.Entities;

public class ReporteAvance
{
    public int Id { get; set; }
    public DateTime FechaGeneracion { get; set; }
    public int DocenteId { get; set; }
    public Docente Docente { get; set; } = null!;
}
