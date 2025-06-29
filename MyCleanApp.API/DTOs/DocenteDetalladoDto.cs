namespace MyCleanApp.API.DTOs
{
    public class DocenteDetalladoDto
    {
        public int Id { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Cedula { get; set; } = string.Empty;
        public string NivelAcademico { get; set; } = string.Empty;
        public DateTime FechaInicioNivel { get; set; }
        public double? UltimaEvaluacion { get; set; }
        public int UsuarioId { get; set; }
        public int NivelAcademicoId { get; set; }
    }
}
