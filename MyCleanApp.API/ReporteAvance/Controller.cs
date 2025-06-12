using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.ReporteAvance
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReporteAvanceController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReporteAvanceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.ReporteAvance.Include(r => r.Docente).ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.ReporteAvance.Include(r => r.Docente).FirstOrDefaultAsync(r => r.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyCleanApp.Domain.Entities.ReporteAvance item)
        {
            _context.ReporteAvance.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MyCleanApp.Domain.Entities.ReporteAvance item)
        {
            if (id != item.Id) return BadRequest();
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.ReporteAvance.FindAsync(id);
            if (item == null) return NotFound();
            _context.ReporteAvance.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
