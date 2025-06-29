namespace MyCleanApp.API.DTOs
{
    public class ProyectoInvestigacionCreateDto
    {
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string RolEnProyecto { get; set; } = string.Empty;
        public int DocenteId { get; set; }
        public byte[]? Documento { get; set; }
    }
}
