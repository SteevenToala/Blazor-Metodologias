using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.API.DTOs;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class ProyectoInvestigacionController : ControllerBase
{
    private readonly AppDbContext _context;
    public ProyectoInvestigacionController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<ProyectoInvestigacion>> Get()
        => await _context.ProyectoInvestigacion.Include(p => p.Docente).ToListAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<ProyectoInvestigacion>> Get(int id)
    {
        var proyecto = await _context.ProyectoInvestigacion.Include(p => p.Docente).FirstOrDefaultAsync(p => p.Id == id);
        return proyecto == null ? NotFound() : Ok(proyecto);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] ProyectoInvestigacionCreateDto dto)
    {
        var proyecto = new ProyectoInvestigacion
        {
            Titulo = dto.Titulo,
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            RolEnProyecto = dto.RolEnProyecto,
            DocenteId = dto.DocenteId,
            Documento = dto.Documento
        };
        _context.ProyectoInvestigacion.Add(proyecto);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = proyecto.Id }, proyecto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] ProyectoInvestigacion proyecto)
    {
        if (id != proyecto.Id) return BadRequest();
        _context.Entry(proyecto).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var proyecto = await _context.ProyectoInvestigacion.FindAsync(id);
        if (proyecto == null) return NotFound();
        _context.ProyectoInvestigacion.Remove(proyecto);
        await _context.SaveChangesAsync();
        return NoContent();
    }


    [HttpPost("importar")]
    public async Task<IActionResult> ImportarProyectoExterno([FromBody] ProyectoInvestigacionDto proyecto)
    {
        // Validación para evitar duplicados
        bool yaExiste = await _context.ProyectoInvestigacion.AnyAsync(p =>
            p.Titulo == proyecto.Titulo &&
            p.FechaInicio == proyecto.FechaInicio &&
            p.FechaFin == proyecto.FechaFin &&
            p.DocenteId == proyecto.DocenteId &&
            p.Externo);

        if (yaExiste)
            return Conflict("El proyecto ya fue importado previamente.");

        var entidad = new ProyectoInvestigacion
        {
            Titulo = proyecto.Titulo,
            FechaInicio = proyecto.FechaInicio,
            FechaFin = proyecto.FechaFin,
            RolEnProyecto = proyecto.RolEnProyecto,
            DocenteId = proyecto.DocenteId,
            Documento = proyecto.Documento,
            Externo = true
        };
        _context.ProyectoInvestigacion.Add(entidad);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("usuario/{usuarioId}")]
    public async Task<ActionResult<IEnumerable<ProyectoInvestigacionDto>>> GetProyectosByUsuarioId(int usuarioId)
    {

        var docente = await _context.Docente.FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);
        if (docente == null)
            return NotFound("Docente no encontrado para el usuario dado.");

        var proyectos = await _context.ProyectoInvestigacion
            .Where(p => p.DocenteId == docente.Id)
            .Select(p => new ProyectoInvestigacionDto
            {

                Id = p.Id,

                Titulo = p.Titulo,
                FechaInicio = p.FechaInicio,
                FechaFin = p.FechaFin,
                RolEnProyecto = p.RolEnProyecto,

                DocenteId = p.DocenteId,
                Documento = p.Documento,
                Externo = p.Externo // <-- AGREGADO

            })
            .ToListAsync();

        return Ok(proyectos);
    }


    [HttpGet("documento/{id}")]
    public async Task<IActionResult> GetDocumento(int id)
    {
        var proyecto = await _context.ProyectoInvestigacion.FirstOrDefaultAsync(p => p.Id == id);
        if (proyecto == null || proyecto.Documento == null)
            return NotFound();
        Response.Headers["Content-Disposition"] = "inline; filename=documento.pdf";
        return File(proyecto.Documento, "application/pdf");
    }

    [HttpGet("docente/{docenteId}")]
    public async Task<ActionResult<IEnumerable<ProyectoInvestigacion>>> GetByDocente(int docenteId)
    {
        try
        {
            var proyectos = await _context.ProyectoInvestigacion
                .Where(p => p.DocenteId == docenteId)
                .OrderByDescending(p => p.FechaInicio)
                .ToListAsync();

            return Ok(proyectos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

}