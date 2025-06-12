using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.CumplimientoRequisito
{
    [ApiController]
    [Route("api/[controller]")]
    public class CumplimientoRequisitoController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CumplimientoRequisitoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.CumplimientoRequisito.Include(c => c.Docente).Include(c => c.Requisito).ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.CumplimientoRequisito.Include(c => c.Docente).Include(c => c.Requisito).FirstOrDefaultAsync(c => c.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyCleanApp.Domain.Entities.CumplimientoRequisito item)
        {
            _context.CumplimientoRequisito.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MyCleanApp.Domain.Entities.CumplimientoRequisito item)
        {
            if (id != item.Id) return BadRequest();
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.CumplimientoRequisito.FindAsync(id);
            if (item == null) return NotFound();
            _context.CumplimientoRequisito.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
