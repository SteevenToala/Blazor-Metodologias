namespace MyCleanApp.Domain.Entities
{
    public class ComisionAcademica
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Cargo { get; set; } = string.Empty; // Presidente, Miembro, Secretario, Asesor
        public int UsuarioId { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaDesignacion { get; set; }
        public DateTime? FechaFinPeriodo { get; set; }
        
        // Navigation properties
        public virtual Usuario? Usuario { get; set; }
    }
}
