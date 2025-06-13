namespace MyCleanApp.Application.DTOs
{
    public class CursoCapacitacionDto
    {
        public string Nombre { get; set; } = string.Empty;
        public int Horas { get; set; }
        public DateTime FechaFin { get; set; }
        public byte[]? Certificado { get; set; }
    }
}