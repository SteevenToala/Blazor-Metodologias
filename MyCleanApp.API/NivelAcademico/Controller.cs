using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class NivelAcademicoController : ControllerBase
{
    private readonly AppDbContext _context;
    public NivelAcademicoController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<NivelAcademico>> Get()
    {
        return await _context.NivelAcademico.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<NivelAcademico>> Get(int id)
    {
        var nivel = await _context.NivelAcademico.FindAsync(id);
        return nivel == null ? NotFound() : Ok(nivel);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] NivelAcademico nivel)
    {
        _context.NivelAcademico.Add(nivel);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = nivel.Id }, nivel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] NivelAcademico nivel)
    {
        if (id != nivel.Id) return BadRequest();

        _context.Entry(nivel).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var nivel = await _context.NivelAcademico.FindAsync(id);
        if (nivel == null) return NotFound();

        _context.NivelAcademico.Remove(nivel);
        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpGet("{nombre}/requisitos")]
    public async Task<ActionResult<IEnumerable<object>>> GetRequisitosPorNombre(string nombre)
    {
        var nivel = await _context.NivelAcademico
            .FirstOrDefaultAsync(n => n.nombre == nombre);

        if (nivel == null)
            return NotFound("Nivel académico no encontrado");

        var requisitos = await _context.RequisitoNivelAcademico
            .Where(r => r.NivelAcademicoId == nivel.Id)
            .Include(r => r.TipoRequisito) // <-- Añade esto si no lo tienes
            .Select(r => new
            {
                Id = r.Id,
                Nombre = r.TipoRequisito.Nombre,
                ValorRequerido = r.ValorRequerido
            })
            .ToListAsync();

        return Ok(requisitos);
    }

}