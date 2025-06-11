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
    public async Task<IEnumerable<Usuario>> Get() => await _context.Usuario.ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Usuario>> Get(int id)
    {
        var product = await _context.Usuario.FindAsync(id);
        return product == null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CrearUsuarioDto dto, [FromServices] IPasswordHasher passwordHasher)
    {
        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Correo = dto.Correo,
            Rol = dto.Rol,
            Contraseña = passwordHasher.Hash(dto.Contraseña)
        };

        _context.Usuario.Add(usuario);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = usuario.Id }, new { usuario.Id, usuario.Nombre, usuario.Correo, usuario.Rol });
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Usuario usuario)
    {
        if (id != usuario.Id) return BadRequest();
        _context.Entry(usuario).State = EntityState.Modified;
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
