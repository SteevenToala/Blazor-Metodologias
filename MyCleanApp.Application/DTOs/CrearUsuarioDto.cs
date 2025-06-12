public class CrearUsuarioDto
{
    public string Correo { get; set; } = null!;
    public string Contraseña { get; set; } = null!;
    public int RolId { get; set; }
    public int PersonaId { get; set; }
}

public class UsuarioDto
{
    public int Id { get; set; }
    public string Correo { get; set; } = null!;
    public string NombreCompleto { get; set; } = null!;
    public string Rol { get; set; } = null!;
}
