public class CursoCapacitacionCreateReques
{
    public string Nombre { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public string Certificado { get; set; } = string.Empty;
    public int Horas { get; set; }
    public bool Externo { get; set; } = false;
}