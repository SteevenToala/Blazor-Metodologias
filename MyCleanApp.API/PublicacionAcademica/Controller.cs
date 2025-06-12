using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class PublicacionAcademicaController : ControllerBase
{
    private readonly AppDbContext _context;
    public PublicacionAcademicaController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<PublicacionAcademica>> Get()
    {
        return await _context.PublicacionAcademica
            .Include(p => p.Docente)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PublicacionAcademica>> Get(int id)
    {
        var publicacion = await _context.PublicacionAcademica
            .Include(p => p.Docente)
            .FirstOrDefaultAsync(p => p.Id == id);

        return publicacion == null ? NotFound() : Ok(publicacion);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] PublicacionAcademicaCreateDto dto)
    {
        var publicacion = new PublicacionAcademica
        {
            Titulo = dto.Titulo,
            Revista = dto.Revista,
            Volumen = dto.Volumen,
            Anio = dto.Anio,
            Tipo = dto.Tipo,
            DocenteId = dto.DocenteId,
            Archivo = dto.Archivo
        };

        _context.PublicacionAcademica.Add(publicacion);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = publicacion.Id }, publicacion);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] PublicacionAcademica publicacion)
    {
        if (id != publicacion.Id) return BadRequest();

        _context.Entry(publicacion).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var publicacion = await _context.PublicacionAcademica.FindAsync(id);
        if (publicacion == null) return NotFound();

        _context.PublicacionAcademica.Remove(publicacion);
        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<object>>> GetByUsuarioId(int usuarioId)
    {
        var docente = await _context.Docente.FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);
        if (docente == null) return NotFound("Docente no encontrado para el usuario dado.");

        var publicaciones = await _context.PublicacionAcademica
            .Where(p => p.DocenteId == docente.Id)
            .Select(p => new
            {
                p.Id,
                p.Titulo,
                p.Revista,
                p.Volumen,
                p.Anio,
                p.Tipo,
                p.Archivo
            })
            .ToListAsync();

        return Ok(publicaciones);
    }
}