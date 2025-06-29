namespace MyCleanApp.API.DTOs
{
    public class PublicacionAcademicaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Revista { get; set; } = string.Empty;
        public string Volumen { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int DocenteId { get; set; }
        public byte[]? Archivo { get; set; }
        public bool Externo { get; set; }
    }
}
