using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.PublicacionAcademica
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicacionAcademicaController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PublicacionAcademicaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var publicaciones = await _context.PublicacionAcademica.Include(p => p.Docente).ToListAsync();
            return Ok(publicaciones);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var publicacion = await _context.PublicacionAcademica.Include(p => p.Docente).FirstOrDefaultAsync(p => p.Id == id);
            if (publicacion == null) return NotFound();
            return Ok(publicacion);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyCleanApp.Domain.Entities.PublicacionAcademica publicacion)
        {
            _context.PublicacionAcademica.Add(publicacion);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = publicacion.Id }, publicacion);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MyCleanApp.Domain.Entities.PublicacionAcademica publicacion)
        {
            if (id != publicacion.Id) return BadRequest();
            _context.Entry(publicacion).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var publicacion = await _context.PublicacionAcademica.FindAsync(id);
            if (publicacion == null) return NotFound();
            _context.PublicacionAcademica.Remove(publicacion);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
