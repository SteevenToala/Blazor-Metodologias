namespace MyCleanApp.Client.DTOs
{
    public class RequisitoNivelAcademicoDto
    {
        public int Id { get; set; }
        public int NivelAcademicoId { get; set; }
        public NivelAcademicoInfo? NivelAcademico { get; set; }
        public int TipoRequisitoId { get; set; }
        public TipoRequisitoInfo? TipoRequisito { get; set; }
        public double ValorRequerido { get; set; }
        
        // Helper properties for backward compatibility
        public string NivelAcademicoNombre => NivelAcademico?.Nombre ?? string.Empty;
        public string TipoRequisitoNombre => TipoRequisito?.Nombre ?? string.Empty;
    }
    
    public class NivelAcademicoInfo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
    
    public class TipoRequisitoInfo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
