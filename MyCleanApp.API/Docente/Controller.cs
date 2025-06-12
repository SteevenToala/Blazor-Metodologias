using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.Docente
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocenteController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DocenteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var docentes = await _context.Docente.Include(d => d.Usuario).Include(d => d.NivelAcademico).ToListAsync();
            return Ok(docentes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var docente = await _context.Docente.Include(d => d.Usuario).Include(d => d.NivelAcademico).FirstOrDefaultAsync(d => d.Id == id);
            if (docente == null) return NotFound();
            return Ok(docente);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyCleanApp.Domain.Entities.Docente docente)
        {
            _context.Docente.Add(docente);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = docente.Id }, docente);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MyCleanApp.Domain.Entities.Docente docente)
        {
            if (id != docente.Id) return BadRequest();
            _context.Entry(docente).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var docente = await _context.Docente.FindAsync(id);
            if (docente == null) return NotFound();
            _context.Docente.Remove(docente);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
