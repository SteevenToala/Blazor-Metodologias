namespace MyCleanApp.Domain.Entities;

public class CumplimientoRequisito
{
    public int Id { get; set; }
    public int DocenteId { get; set; }
    public Docente Docente { get; set; } = null!;
    public int RequisitoId { get; set; }
    public RequisitoPromocion Requisito { get; set; } = null!;
    public bool Cumplido { get; set; }
    public DateTime FechaCumplimiento { get; set; }
}
