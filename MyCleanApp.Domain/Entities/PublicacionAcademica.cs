namespace MyCleanApp.Domain.Entities;

public class PublicacionAcademica
{
    public int Id { get; set; }
    public string Titulo { get; set; } = null!;
    public string Revista { get; set; } = null!;
    public string Volumen { get; set; } = null!;
    public int Anio { get; set; }
    public string Tipo { get; set; } = null!;
    public int DocenteId { get; set; }
    public Docente Docente { get; set; } = null!;
    public byte[]? Archivo { get; set; }
}
