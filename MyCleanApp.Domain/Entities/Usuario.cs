namespace MyCleanApp.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Correo { get; set; }
        public required string PasswordHash { get; set; }

        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;

        public int PersonaId { get; set; }
        public Persona Persona { get; set; } = null!;

        public bool Activo { get; set; }
    }
}