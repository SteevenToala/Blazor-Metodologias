using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class PersonaController : ControllerBase
{
    private readonly AppDbContext _context;
    public PersonaController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<Persona>> Get()
    {
        return await _context.Persona.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Persona>> Get(int id)
    {
        var persona = await _context.Persona.FindAsync(id);
        return persona == null ? NotFound() : Ok(persona);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Persona persona)
    {
        _context.Persona.Add(persona);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = persona.Id }, persona);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Persona persona)
    {
        if (id != persona.Id) return BadRequest();

        _context.Entry(persona).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var persona = await _context.Persona.FindAsync(id);
        if (persona == null) return NotFound();

        _context.Persona.Remove(persona);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}