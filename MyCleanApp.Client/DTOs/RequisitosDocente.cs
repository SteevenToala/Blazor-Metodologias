public class UsuarioClas
{
    public int id { get; set; }
    public string correo { get; set; } = string.Empty;
    public string passwordHash { get; set; } = string.Empty;
    public int rolId { get; set; }
    public int personaId { get; set; }
    public bool activo { get; set; }
}

public class Docente
{
    public int id { get; set; }
    public int usuarioId { get; set; }
    public int nivelAcademicoId { get; set; }
    public string fechaInicioNivel { get; set; } = string.Empty;
}

public class CumpleRequisitos
{
    public bool años { get; set; }
    public bool publicaciones { get; set; }
    public bool evaluacion { get; set; }
    public bool capacitacion { get; set; }
    public bool investigacion { get; set; }
}

public class EvaluacionDocenteRequest
{
    public UsuarioClas usuario { get; set; } = new();
    public Docente docente { get; set; } = new();
    public string nivel { get; set; } = string.Empty;
    public int añosEnNivel { get; set; }
    public double promedioEvaluacion { get; set; }
    public int totalHorasCapacitacion { get; set; }
    public int publicaciones { get; set; }
    public int mesesInvestigacion { get; set; }
    public CumpleRequisitos cumpleRequisitos { get; set; } = new();
    public List<string> requisitosFaltantes { get; set; } = new();
}
