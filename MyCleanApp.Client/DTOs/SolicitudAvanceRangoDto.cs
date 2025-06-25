public class SolicitudAvanceRangoDto
{
    public int Id { get; set; }
    public int DocenteId { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public string Estado { get; set; } = string.Empty;
    public DateTime? FechaRespuesta { get; set; }
    public string Observaciones { get; set; } = string.Empty;
    public int NuevoNivelAcademicoId { get; set; }
}
