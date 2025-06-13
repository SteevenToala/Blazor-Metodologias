namespace MyCleanApp.Application.DTOs
{
    public class CursoCapacitacionCreateRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public byte[]? Certificado { get; set; }
        public int Horas { get; set; }

    }
}