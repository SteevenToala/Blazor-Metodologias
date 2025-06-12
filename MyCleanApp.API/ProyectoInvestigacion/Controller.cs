using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.ProyectoInvestigacion
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProyectoInvestigacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProyectoInvestigacionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var proyectos = await _context.ProyectoInvestigacion.Include(p => p.Docente).ToListAsync();
            return Ok(proyectos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var proyecto = await _context.ProyectoInvestigacion.Include(p => p.Docente).FirstOrDefaultAsync(p => p.Id == id);
            if (proyecto == null) return NotFound();
            return Ok(proyecto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyCleanApp.Domain.Entities.ProyectoInvestigacion proyecto)
        {
            _context.ProyectoInvestigacion.Add(proyecto);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = proyecto.Id }, proyecto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MyCleanApp.Domain.Entities.ProyectoInvestigacion proyecto)
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
    }
}
