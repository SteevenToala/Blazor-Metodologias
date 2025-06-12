namespace MyCleanApp.Domain.Entities
{
    public class SolicitudAvanceRango
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty; // PENDIENTE, APROBADA, RECHAZADA
        public DateTime? FechaRespuesta { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public int NuevoNivelAcademicoId { get; set; }

        public Docente? Docente { get; set; }
        public NivelAcademico? NuevoNivelAcademico { get; set; }
    }
}