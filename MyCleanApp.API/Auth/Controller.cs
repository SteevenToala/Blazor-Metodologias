using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyCleanApp.Application.Interfaces;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    public AuthController(AppDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

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

        var token = GenerateJwtToken(usuario);

        return Ok(new
        {
            token,
            usuario = new
            {
                usuario.Nombre,
                usuario.Correo,
                usuario.Rol
            }
        });
    }
    private string GenerateJwtToken(MyCleanApp.Domain.Entities.Usuario usuario)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Correo),
            new Claim("id", usuario.Id.ToString()),
            new Claim("rol", usuario.Rol),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}