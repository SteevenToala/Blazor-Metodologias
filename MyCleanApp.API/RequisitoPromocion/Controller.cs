using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.RequisitoPromocion
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequisitoPromocionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public RequisitoPromocionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requisitos = await _context.RequisitoPromocion.ToListAsync();
            return Ok(requisitos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var requisito = await _context.RequisitoPromocion.FindAsync(id);
            if (requisito == null) return NotFound();
            return Ok(requisito);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyCleanApp.Domain.Entities.RequisitoPromocion requisito)
        {
            _context.RequisitoPromocion.Add(requisito);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = requisito.Id }, requisito);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MyCleanApp.Domain.Entities.RequisitoPromocion requisito)
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
}
