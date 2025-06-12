public class DashBoardDocente
{
    public string NivelAcademico { get; set; } = "";
    public int AniosEnNivel { get; set; }
    public int PublicacionesAcademicas { get; set; }
    public double PuntajeEvaluacion { get; set; }
    public int HorasCapacitacion { get; set; }
    public int Investigaciones { get; set; } = 0;
    public RequisitoNivel[]? requisitoNivel { get; set; }
    public override string ToString()
    {
        var requisitos = requisitoNivel != null
            ? string.Join(", ", requisitoNivel.Select(r => r.ToString()))
            : "Ninguno";
        return $"NivelAcademico: {NivelAcademico}, AñosEnNivel: {AniosEnNivel}, PublicacionesAcademicas: {PublicacionesAcademicas}, PuntajeEvaluacion: {PuntajeEvaluacion}, HorasCapacitacion: {HorasCapacitacion}, Requisitos: [{requisitos}]";
    }
    public int GetValorRequerido(string nombre)
    {
        return requisitoNivel?.FirstOrDefault(r => r.Nombre == nombre)?.ValorRequerido ?? -1;
    }

}

