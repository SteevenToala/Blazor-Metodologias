namespace MyCleanApp.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Correo { get; set; }
        public required string PasswordHash { get; set; }

        public string Rol { get; set; } = string.Empty;

        public int PersonaId { get; set; }
        public virtual Persona? Persona { get; set; }

        public bool Activo { get; set; }
    }
}