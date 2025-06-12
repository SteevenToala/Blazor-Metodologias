using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Application.Interfaces;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;


[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly AppDbContext _context;
    public UsuariosController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<UsuarioDto>> Get()
    {
        return await _context.Usuario
            .Include(u => u.Persona)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Correo = u.Correo,
                NombreCompleto = u.Persona.Nombres + " " + u.Persona.Apellidos,
                Rol = u.Rol
            }).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UsuarioDto>> Get(int id)
    {
        var usuario = await _context.Usuario
            .Include(u => u.Persona)
            .Where(u => u.Id == id)
            .Select(u => new UsuarioDto
            {
                Id = u.Id,
                Correo = u.Correo,
                NombreCompleto = u.Persona.Nombres + " " + u.Persona.Apellidos,
                Rol = u.Rol
            }).FirstOrDefaultAsync();

        return usuario == null ? NotFound() : Ok(usuario);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CrearUsuarioDto dto, [FromServices] IPasswordHasher passwordHasher)
    {
        var usuario = new Usuario
        {
            Correo = dto.Correo,
            PasswordHash = passwordHasher.Hash(dto.Contraseña),
            Rol = dto.Rol,
            PersonaId = dto.PersonaId,
            Activo = true
        };

        _context.Usuario.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = usuario.Id }, new { usuario.Id, usuario.Correo });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] CrearUsuarioDto dto, [FromServices] IPasswordHasher passwordHasher)
    {
        var usuario = await _context.Usuario.FindAsync(id);
        if (usuario == null) return NotFound();

        usuario.Correo = dto.Correo;
        usuario.PasswordHash = passwordHasher.Hash(dto.Contraseña);
        usuario.Rol = dto.Rol;
        usuario.PersonaId = dto.PersonaId;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var usuario = await _context.Usuario.FindAsync(id);
        if (usuario == null) return NotFound();

        _context.Usuario.Remove(usuario);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
