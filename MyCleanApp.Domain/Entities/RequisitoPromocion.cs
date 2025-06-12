namespace MyCleanApp.Domain.Entities;

public class RequisitoPromocion
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public int PorcentajeAsignado { get; set; }
}
