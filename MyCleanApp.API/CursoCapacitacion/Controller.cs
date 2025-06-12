using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.CursoCapacitacion
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursoCapacitacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public CursoCapacitacionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var cursos = await _context.CursoCapacitacion.Include(c => c.Docente).ToListAsync();
            return Ok(cursos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var curso = await _context.CursoCapacitacion.Include(c => c.Docente).FirstOrDefaultAsync(c => c.Id == id);
            if (curso == null) return NotFound();
            return Ok(curso);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyCleanApp.Domain.Entities.CursoCapacitacion curso)
        {
            _context.CursoCapacitacion.Add(curso);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = curso.Id }, curso);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MyCleanApp.Domain.Entities.CursoCapacitacion curso)
        {
            if (id != curso.Id) return BadRequest();
            _context.Entry(curso).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var curso = await _context.CursoCapacitacion.FindAsync(id);
            if (curso == null) return NotFound();
            _context.CursoCapacitacion.Remove(curso);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
