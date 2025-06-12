using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyCleanApp.Application.Interfaces;
using MyCleanApp.Infrastructure.Persistence;
using MyCleanApp.Domain.Entities;

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
        // Incluir Rol y Persona si deseas usar también esos datos
        var usuario = await _context.Usuario
            .Include(u=>u.Persona)
            .FirstOrDefaultAsync(u => u.Correo == dto.Correo);

        if (usuario == null || !passwordHasher.Verify(dto.Contraseña, usuario.PasswordHash))
            return Unauthorized("Usuario o contraseña incorrectos");

        var token = GenerateJwtToken(usuario);

        return Ok(new
        {
            token,
            usuario = new
            {
                usuario.Id,
                usuario.Correo,
                Nombre=usuario.Persona.Nombres+" "+usuario.Persona.Apellidos,
                Cedula =usuario.Persona.Cedula,
                Rol = usuario.Rol // mostrar el nombre del rol (e.g., "ADMINISTRADOR")
            }
        });
    }

    private string GenerateJwtToken(Usuario usuario)
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

    public class RegisterDto
    {
        public string Correo { get; set; } = null!;
        public string Contraseña { get; set; } = null!;
        public int RolId { get; set; }
        public string Nombres { get; set; } = null!;
        public string Apellidos { get; set; } = null!;
        public string Cedula { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        public string Rol { get; set; } = null!; // Nueva propiedad Rol
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, [FromServices] IPasswordHasher passwordHasher)
    {
        try
        {
            // Validar correo duplicado
            if (await _context.Usuario.AnyAsync(u => u.Correo == dto.Correo))
                return BadRequest("El correo ya está registrado");
            // Validar cédula duplicada
            if (await _context.Persona.AnyAsync(p => p.Cedula == dto.Cedula))
                return BadRequest("La cédula ya está registrada");

            var persona = new Persona
            {
                Nombres = dto.Nombres,
                Apellidos = dto.Apellidos,
                Cedula = dto.Cedula,
                Telefono = dto.Telefono,
                Direccion = dto.Direccion,
                FechaNacimiento = dto.FechaNacimiento
            };
            _context.Persona.Add(persona);
            await _context.SaveChangesAsync();

            // Asignar manualmente el ID de Usuario
            var usuario = new Usuario
            {
                Correo = dto.Correo,
                PasswordHash = passwordHasher.Hash(dto.Contraseña),
                Rol = dto.Rol, // Cambiado de RolId a Rol
                PersonaId = persona.Id,
                Activo = true
            };
            _context.Usuario.Add(usuario);
            await _context.SaveChangesAsync();

            // Si el rol es DOCENTE, crear registro en Docente
            if (dto.Rol.ToUpper() == "DOCENTE")
            {
                // Buscar el id del nivel académico inicial (DT2)
                var nivelInicial = await _context.NivelAcademico.FirstOrDefaultAsync(n => n.nombre == "DT2");
                if (nivelInicial == null)
                {
                    return StatusCode(500, "No existe el nivel académico inicial 'DT2' en la base de datos.");
                }
                var docente = new Docente
                {
                    UsuarioId = usuario.Id,
                    NivelAcademicoId = nivelInicial.Id,
                    NivelAcademico = nivelInicial,
                    FechaInicioNivel = DateTime.Now
                };
                _context.Docente.Add(docente);
                await _context.SaveChangesAsync();
            }

            return Ok(new { usuario.Id, usuario.Correo });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error en el registro: {ex.Message} {ex.InnerException?.Message}");
        }
    }
}
