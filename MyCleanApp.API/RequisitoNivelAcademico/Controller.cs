using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class RequisitoNivelAcademicoController : ControllerBase
{
    private readonly AppDbContext _context;
    public RequisitoNivelAcademicoController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<RequisitoNivelAcademico>> Get()
    {
        return await _context.RequisitoNivelAcademico
            .Include(r => r.NivelAcademico)
            .Include(r => r.TipoRequisito)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RequisitoNivelAcademico>> Get(int id)
    {
        var requisito = await _context.RequisitoNivelAcademico
            .Include(r => r.NivelAcademico)
            .Include(r => r.TipoRequisito)
            .FirstOrDefaultAsync(r => r.Id == id);

        return requisito == null ? NotFound() : Ok(requisito);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] RequisitoNivelAcademico requisito)
    {
        _context.RequisitoNivelAcademico.Add(requisito);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = requisito.Id }, requisito);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] RequisitoNivelAcademico requisito)
    {
        if (id != requisito.Id) return BadRequest();

        _context.Entry(requisito).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var requisito = await _context.RequisitoNivelAcademico.FindAsync(id);
        if (requisito == null) return NotFound();

        _context.RequisitoNivelAcademico.Remove(requisito);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}