namespace MyCleanApp.Domain.Entities
{
    public class PublicacionAcademica
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Revista { get; set; } = string.Empty;
        public string Volumen { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int DocenteId { get; set; }
        public byte[]? Archivo { get; set; }

        public Docente? Docente { get; set; }
    }
}