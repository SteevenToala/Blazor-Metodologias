namespace MyCleanApp.Domain.Entities;

public class ProyectoInvestigacion
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string RolEnProyecto { get; set; } = null!;
    public int DocenteId { get; set; }
    public Docente Docente { get; set; } = null!;
    public byte[]? Documento { get; set; }
}
