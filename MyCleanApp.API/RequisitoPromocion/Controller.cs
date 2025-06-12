using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class RequisitoPromocionController : ControllerBase
{
    private readonly AppDbContext _context;
    public RequisitoPromocionController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<RequisitoPromocion>> Get()
    {
        return await _context.RequisitoPromocion.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RequisitoPromocion>> Get(int id)
    {
        var requisito = await _context.RequisitoPromocion.FindAsync(id);
        return requisito == null ? NotFound() : Ok(requisito);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] RequisitoPromocion requisito)
    {
        _context.RequisitoPromocion.Add(requisito);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = requisito.Id }, requisito);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] RequisitoPromocion requisito)
    {
        if (id != requisito.Id) return BadRequest();

        _context.Entry(requisito).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var requisito = await _context.RequisitoPromocion.FindAsync(id);
        if (requisito == null) return NotFound();

        _context.RequisitoPromocion.Remove(requisito);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}