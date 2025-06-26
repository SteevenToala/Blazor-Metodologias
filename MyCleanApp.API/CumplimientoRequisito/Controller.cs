using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class CumplimientoRequisitoController : ControllerBase
{
    private readonly AppDbContext _context;
    public CumplimientoRequisitoController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Get()
    {
        try
        {
            // Simplificamos para evitar problemas con navigation properties
            var cumplimientos = await _context.CumplimientoRequisito
                .Select(c => new
                {
                    c.Id,
                    c.DocenteId,
                    c.RequisitoId,
                    c.Cumplido,
                    c.FechaCumplimiento,
                    RequisitoNombre = "Requisito",
                    PorcentajeAsignado = 100
                })
                .ToListAsync();

            return Ok(cumplimientos);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> Get(int id)
    {
        try
        {
            var cumplimiento = await _context.CumplimientoRequisito
                .Where(c => c.Id == id)
                .Select(c => new
                {
                    c.Id,
                    c.DocenteId,
                    c.RequisitoId,
                    c.Cumplido,
                    c.FechaCumplimiento,
                    RequisitoNombre = "Requisito",
                    PorcentajeAsignado = 100
                })
                .FirstOrDefaultAsync();

            return cumplimiento == null ? NotFound() : Ok(cumplimiento);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CumplimientoRequisito cumplimiento)
    {
        try
        {
            if (cumplimiento == null)
            {
                return BadRequest(new { error = "Los datos del cumplimiento son requeridos" });
            }

            // Verificar que el docente existe
            var docenteExists = await _context.Docente.AnyAsync(d => d.Id == cumplimiento.DocenteId);
            if (!docenteExists)
            {
                return BadRequest(new { error = "El docente especificado no existe" });
            }

            // Verificar que el requisito existe
            var requisitoExists = await _context.RequisitoPromocion.AnyAsync(r => r.Id == cumplimiento.RequisitoId);
            if (!requisitoExists)
            {
                return BadRequest(new { error = "El requisito especificado no existe" });
            }

            _context.CumplimientoRequisito.Add(cumplimiento);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction(nameof(Get), new { id = cumplimiento.Id }, cumplimiento);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] CumplimientoRequisito cumplimiento)
    {
        try
        {
            if (id != cumplimiento.Id) 
                return BadRequest(new { error = "El ID no coincide" });

            var existingCumplimiento = await _context.CumplimientoRequisito.FindAsync(id);
            if (existingCumplimiento == null)
                return NotFound(new { error = "Cumplimiento de requisito no encontrado" });

            // Verificar que el docente existe
            var docenteExists = await _context.Docente.AnyAsync(d => d.Id == cumplimiento.DocenteId);
            if (!docenteExists)
            {
                return BadRequest(new { error = "El docente especificado no existe" });
            }

            // Verificar que el requisito existe
            var requisitoExists = await _context.RequisitoPromocion.AnyAsync(r => r.Id == cumplimiento.RequisitoId);
            if (!requisitoExists)
            {
                return BadRequest(new { error = "El requisito especificado no existe" });
            }

            existingCumplimiento.DocenteId = cumplimiento.DocenteId;
            existingCumplimiento.RequisitoId = cumplimiento.RequisitoId;
            existingCumplimiento.Cumplido = cumplimiento.Cumplido;
            existingCumplimiento.FechaCumplimiento = cumplimiento.FechaCumplimiento;

            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var cumplimiento = await _context.CumplimientoRequisito.FindAsync(id);
            if (cumplimiento == null) 
                return NotFound(new { error = "Cumplimiento de requisito no encontrado" });

            _context.CumplimientoRequisito.Remove(cumplimiento);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }
}