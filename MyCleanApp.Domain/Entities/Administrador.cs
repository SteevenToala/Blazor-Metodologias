namespace MyCleanApp.Domain.Entities;

public class Administrador
{
    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
}
