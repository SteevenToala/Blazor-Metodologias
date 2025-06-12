public class PublicacionAcademicaCreateDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Revista { get; set; } = string.Empty;
    public string Volumen { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public int DocenteId { get; set; }
    public byte[]? Archivo { get; set; }
}