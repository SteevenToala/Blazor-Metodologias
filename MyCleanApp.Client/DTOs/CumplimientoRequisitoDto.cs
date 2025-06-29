namespace MyCleanApp.Client.DTOs
{
    public class CumplimientoRequisitoDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public int RequisitoId { get; set; }
        public bool Cumplido { get; set; }
        public DateTime FechaCumplimiento { get; set; }
        public DocenteInfo? Docente { get; set; }
        public RequisitoInfo? Requisito { get; set; }
        
        // Helper properties for backward compatibility
        public string RequisitoNombre => Requisito?.Nombre ?? string.Empty;
        public int PorcentajeAsignado { get; set; }
    }
    
    public class DocenteInfo
    {
        public int Id { get; set; }
        public int NivelAcademicoId { get; set; }
        public int UsuarioId { get; set; }
    }
    
    public class RequisitoInfo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
