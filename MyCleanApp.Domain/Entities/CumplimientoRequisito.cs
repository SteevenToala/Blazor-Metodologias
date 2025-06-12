namespace MyCleanApp.Domain.Entities
{
    public class CumplimientoRequisito
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public int RequisitoId { get; set; }
        public bool Cumplido { get; set; }
        public DateTime FechaCumplimiento { get; set; }

        public Docente? Docente { get; set; }
        public RequisitoPromocion? RequisitoPromocion { get; set; }
    }
}