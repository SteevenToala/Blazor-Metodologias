using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.EvaluacionDocente
{
    [ApiController]
    [Route("api/[controller]")]
    public class EvaluacionDocenteController : ControllerBase
    {
        private readonly AppDbContext _context;
        public EvaluacionDocenteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var evaluaciones = await _context.EvaluacionDocente.Include(e => e.Docente).ToListAsync();
            return Ok(evaluaciones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evaluacion = await _context.EvaluacionDocente.Include(e => e.Docente).FirstOrDefaultAsync(e => e.Id == id);
            if (evaluacion == null) return NotFound();
            return Ok(evaluacion);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyCleanApp.Domain.Entities.EvaluacionDocente evaluacion)
        {
            _context.EvaluacionDocente.Add(evaluacion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = evaluacion.Id }, evaluacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MyCleanApp.Domain.Entities.EvaluacionDocente evaluacion)
        {
            if (id != evaluacion.Id) return BadRequest();
            _context.Entry(evaluacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var evaluacion = await _context.EvaluacionDocente.FindAsync(id);
            if (evaluacion == null) return NotFound();
            _context.EvaluacionDocente.Remove(evaluacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
