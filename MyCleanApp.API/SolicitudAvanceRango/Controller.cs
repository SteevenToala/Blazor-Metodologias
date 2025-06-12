using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.SolicitudAvanceRango
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudAvanceRangoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SolicitudAvanceRangoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.SolicitudAvanceRango.Include(s => s.Docente).Include(s => s.Administrador).Include(s => s.NuevoNivelAcademico).ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.SolicitudAvanceRango.Include(s => s.Docente).Include(s => s.Administrador).Include(s => s.NuevoNivelAcademico).FirstOrDefaultAsync(s => s.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyCleanApp.Domain.Entities.SolicitudAvanceRango item)
        {
            _context.SolicitudAvanceRango.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MyCleanApp.Domain.Entities.SolicitudAvanceRango item)
        {
            if (id != item.Id) return BadRequest();
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.SolicitudAvanceRango.FindAsync(id);
            if (item == null) return NotFound();
            _context.SolicitudAvanceRango.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
