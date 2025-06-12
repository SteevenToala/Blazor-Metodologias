namespace MyCleanApp.Domain.Entities
{
    public class RequisitoPromocion
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int PorcentajeAsignado { get; set; }
    }
}