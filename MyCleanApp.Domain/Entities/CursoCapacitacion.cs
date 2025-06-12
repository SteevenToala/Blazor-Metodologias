namespace MyCleanApp.Domain.Entities
{
    public class CursoCapacitacion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Horas { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DocenteId { get; set; }
        public byte[]? Certificado { get; set; }

        public Docente? Docente { get; set; }
    }
}