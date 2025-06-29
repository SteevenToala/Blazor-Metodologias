using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.API.DTOs;
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
    public async Task<IActionResult> Put(int id, [FromBody] PublicacionAcademicaDto dto)
    {
        var publicacion = await _context.PublicacionAcademica.FirstOrDefaultAsync(p => p.Id == id);
        if (publicacion == null) return NotFound();
        publicacion.Titulo = dto.Titulo;
        publicacion.Revista = dto.Revista;
        publicacion.Volumen = dto.Volumen;
        publicacion.Anio = dto.Anio;
        publicacion.Tipo = dto.Tipo;
        publicacion.Archivo = dto.Archivo;
        // No se permite cambiar DocenteId ni Externo por seguridad
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
                p.Archivo,
                p.Externo // <-- AGREGADO
            })
            .ToListAsync();

        return Ok(publicaciones);
    }

    [HttpPost("importar")]
    public async Task<IActionResult> ImportarPublicacionExterna([FromBody] PublicacionAcademicaDto publicacion)
    {
        // Validación para evitar duplicados
        bool yaExiste = await _context.PublicacionAcademica.AnyAsync(p =>
            p.Titulo == publicacion.Titulo &&
            p.Revista == publicacion.Revista &&
            p.Volumen == publicacion.Volumen &&
            p.Anio == publicacion.Anio &&
            p.Tipo == publicacion.Tipo &&
            p.DocenteId == publicacion.DocenteId &&
            p.Externo);

        if (yaExiste)
            return Conflict("La publicación ya fue importada previamente.");

        var entidad = new PublicacionAcademica
        {
            Titulo = publicacion.Titulo,
            Revista = publicacion.Revista,
            Volumen = publicacion.Volumen,
            Anio = publicacion.Anio,
            Tipo = publicacion.Tipo,
            DocenteId = publicacion.DocenteId,
            Archivo = publicacion.Archivo,
            Externo = true
        };
        _context.PublicacionAcademica.Add(entidad);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("archivo/{id}")]
    public async Task<IActionResult> GetArchivo(int id)
    {
        var publicacion = await _context.PublicacionAcademica.FirstOrDefaultAsync(p => p.Id == id);
        if (publicacion == null || publicacion.Archivo == null)
            return NotFound();
        Response.Headers["Content-Disposition"] = "inline; filename=publicacion.pdf";
        return File(publicacion.Archivo, "application/pdf");
    }

    [HttpGet("docente/{docenteId}")]
    public async Task<ActionResult<IEnumerable<PublicacionAcademica>>> GetByDocente(int docenteId)
    {
        try
        {
            var publicaciones = await _context.PublicacionAcademica
                .Where(p => p.DocenteId == docenteId)
                .OrderByDescending(p => p.Anio)
                .ToListAsync();

            return Ok(publicaciones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}