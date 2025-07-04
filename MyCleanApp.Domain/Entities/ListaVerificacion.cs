namespace MyCleanApp.Domain.Entities
{
    public class ListaVerificacion
    {
        public int Id { get; set; }
        public int NivelAcademicoId { get; set; }
        public string NombreDocumento { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool Obligatorio { get; set; } = true;
        public int Orden { get; set; }
        public bool Activo { get; set; } = true;

        // Navegación
        public virtual NivelAcademico? NivelAcademico { get; set; }
        public virtual ICollection<VerificacionDocumentos> VerificacionDocumentos { get; set; } = new List<VerificacionDocumentos>();
    }
}
