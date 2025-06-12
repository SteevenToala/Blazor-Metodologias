namespace MyCleanApp.Domain.Entities;

public class Docente
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public int NivelAcademicoId { get; set; }
    public NivelAcademico NivelAcademico { get; set; } = null!;
    public DateTime FechaInicioNivel { get; set; }
}
