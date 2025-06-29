namespace MyCleanApp.Client.DTOs
{
    public class CursoCapacitacionDto
    {
        public int Id { get; set; } // <-- Agregado para identificar el curso
        public string Nombre { get; set; } = string.Empty;
        public int Horas { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DocenteId { get; set; }
        public bool Externo { get; set; }
        public byte[]? Certificado { get; set; }
    }
}