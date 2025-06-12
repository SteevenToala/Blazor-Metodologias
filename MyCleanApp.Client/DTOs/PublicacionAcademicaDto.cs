public class PublicacionAcademicaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Revista { get; set; }= "";
    public string Volumen { get; set; }= "";
    public int Anio { get; set; }
    public string Tipo { get; set; }= "";
    public byte[]? Archivo { get; set; }
}