using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Application.Interfaces;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    public AuthController(AppDbContext context) => _context = context;

    public class LoginDto
    {
        public required string Correo { get; set; }
        public required string Contraseña { get; set; }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, [FromServices] IPasswordHasher passwordHasher)
    {
        var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Correo == dto.Correo);
        if (usuario == null || !passwordHasher.Verify(dto.Contraseña, usuario.Contraseña))
            return Unauthorized("Usuario o contraseña incorrectos");

        // Aquí puedes retornar un token o los datos del usuario
        return Ok(new { usuario.Id, usuario.Nombre, usuario.Correo, usuario.Rol });
    }
}