namespace MyCleanApp.Domain.Entities
{
    public class RequisitoNivelAcademico
    {
        public int Id { get; set; }
        public required int NivelAcademicoId { get; set; }
        public required NivelAcademico NivelAcademico { get; set; }

        public int TipoRequisitoId { get; set; }
        public required TipoRequisito TipoRequisito { get; set; }

        public double ValorRequerido { get; set; }

    }
}