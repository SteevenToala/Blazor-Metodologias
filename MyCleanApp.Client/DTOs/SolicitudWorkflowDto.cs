public class SolicitudWorkflowDto
{
    public int Id { get; set; }
    public int DocenteId { get; set; }
    public string DocenteNombre { get; set; } = "";
    public string Estado { get; set; } = "";
    public DateTime FechaSolicitud { get; set; }
    public DateTime? FechaLimiteRespuesta { get; set; }
    public DateTime? FechaRespuesta { get; set; }
    public string Observaciones { get; set; } = "";
    public int NuevoNivelAcademicoId { get; set; }
    public bool RequiereAtencion { get; set; }
    public string NuevoNivel { get; set; } = "";
    public string NivelActual { get; set; } = "";
    public string NivelSolicitado { get; set; } = "";
    public int DiasPendientes { get; set; }
    public bool EsUrgente => DiasPendientes <= 1;
    public bool EstaVencida => DiasPendientes < 0;
}
