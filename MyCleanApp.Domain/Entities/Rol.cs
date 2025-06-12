namespace MyCleanApp.Domain.Entities
{
    public class Rol
    {
        public int Id { get; set; }
        public string RolNombre { get; set; } = null!;
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}