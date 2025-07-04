namespace MyCleanApp.Client.DTOs
{
    public class SolicitudComisionDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string DocenteNombre { get; set; } = string.Empty;
        public string DocenteApellido { get; set; } = string.Empty;
        public string NivelActual { get; set; } = string.Empty;
        public string NivelSolicitado { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public DateTime? FechaAnalisis { get; set; }
        public DateTime? FechaDecision { get; set; }
        public bool RequiereAnalisis { get; set; }
    }

    public class SolicitudDetalleDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public string DocenteNombre { get; set; } = string.Empty;
        public string DocenteApellido { get; set; } = string.Empty;
        public string DocenteCorreo { get; set; } = string.Empty;
        public string NivelActual { get; set; } = string.Empty;
        public string NivelSolicitado { get; set; } = string.Empty;
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public DateTime? FechaAnalisis { get; set; }
        public DateTime? FechaDecision { get; set; }
        
        // Datos de evaluación
        public List<EvaluacionDocenteDto> Evaluaciones { get; set; } = new();
        public List<CursoCapacitacionDto> Cursos { get; set; } = new();
        public List<PublicacionAcademicaDto> Publicaciones { get; set; } = new();
        public List<ProyectoInvestigacionDto> Proyectos { get; set; } = new();
        public List<RequisitoComisionDto> Requisitos { get; set; } = new();
    }

    public class RequisitoComisionDto
    {
        public int RequisitoId { get; set; }
        public string RequisitoNombre { get; set; } = string.Empty;
        public string RequisitoDescripcion { get; set; } = string.Empty;
        public bool Cumple { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public DateTime? FechaVerificacion { get; set; }
    }

    public class DecisionComisionDto
    {
        public int SolicitudId { get; set; }
        public string Decision { get; set; } = string.Empty; // "APROBADO", "RECHAZADO", "PENDIENTE"
        public string Observaciones { get; set; } = string.Empty;
        public bool RequiereNotificacion { get; set; } = true;
    }

    public class RespuestaDocenteDto
    {
        public int SolicitudId { get; set; }
        public string Respuesta { get; set; } = string.Empty; // "ACEPTA", "APELA"
        public string Observaciones { get; set; } = string.Empty;
    }

    public class ResolucionApelacionDto
    {
        public int ApelacionId { get; set; }
        public string Decision { get; set; } = string.Empty; // "ACEPTA_APELACION", "RECHAZA_APELACION"
        public string Justificacion { get; set; } = string.Empty;
    }

    public class InformeFinalDto
    {
        public int SolicitudId { get; set; }
        public string ResumenProceso { get; set; } = string.Empty;
        public string DecisionFinal { get; set; } = string.Empty;
        public DateTime FechaInforme { get; set; }
        public bool EnviadoConsejoUniversitario { get; set; }
    }
}
