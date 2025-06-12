namespace MyCleanApp.Domain.Entities;

public class SolicitudAvanceRango
{
    public int Id { get; set; }
    public int DocenteId { get; set; }
    public Docente Docente { get; set; } = null!;
    public DateTime FechaSolicitud { get; set; }
    public string Estado { get; set; } = null!; // PENDIENTE, APROBADA, RECHAZADA
    public DateTime? FechaRespuesta { get; set; }
    public int? AdministradorId { get; set; }
    public Administrador? Administrador { get; set; }
    public string? Observaciones { get; set; }
    public int NuevoNivelAcademicoId { get; set; }
    public NivelAcademico NuevoNivelAcademico { get; set; } = null!;
}
