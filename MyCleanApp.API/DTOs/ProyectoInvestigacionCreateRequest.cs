public class ProyectoInvestigacionCreateRequest
{
    public string Titulo { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string RolEnProyecto { get; set; } = string.Empty;
    public byte[]? Documento { get; set; }
}