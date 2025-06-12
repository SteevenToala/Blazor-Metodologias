namespace MyCleanApp.Domain.Entities
{
    public class Docente
    {
        public int Id { get; set; }
        public required int NivelAcademicoId { get; set; }
        public required NivelAcademico NivelAcademico { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public DateTime FechaInicioNivel { get; set; }

    }
}