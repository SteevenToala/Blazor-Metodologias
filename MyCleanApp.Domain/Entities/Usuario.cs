namespace MyCleanApp.Domain.Entities
{
    public class Usuario
    {
        public int Id { get; set; }
        public required string Nombre { get; set; }
        public required string Correo { get; set; }
        public required string Contraseña { get; set; }
        public required string Rol { get; set; }

    }
}