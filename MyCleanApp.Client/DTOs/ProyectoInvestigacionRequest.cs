namespace MyCleanApp.Client.DTOs
{
    public class ProyectoInvestigacionRequest
    {
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string RolEnProyecto { get; set; } = string.Empty; // Si se usa en backend
        public int DocenteId { get; set; }
        public byte[]? Documento { get; set; }
    }
}
