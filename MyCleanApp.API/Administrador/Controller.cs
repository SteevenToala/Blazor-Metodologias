using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Infrastructure.Persistence;

namespace MyCleanApp.API.Administrador
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministradorController : ControllerBase
    {
        private readonly AppDbContext _context;
        public AdministradorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.Administrador.Include(a => a.Usuario).ToListAsync();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.Administrador.Include(a => a.Usuario).FirstOrDefaultAsync(a => a.Id == id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(MyCleanApp.Domain.Entities.Administrador item)
        {
            _context.Administrador.Add(item);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, MyCleanApp.Domain.Entities.Administrador item)
        {
            if (id != item.Id) return BadRequest();
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.Administrador.FindAsync(id);
            if (item == null) return NotFound();
            _context.Administrador.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ENDPOINT: Listar proyectos de investigación por docente
        [HttpGet("proyectos/{docenteId}")]
        public async Task<IActionResult> GetProyectosByDocente(int docenteId)
        {
            var proyectos = await _context.ProyectoInvestigacion.Where(p => p.DocenteId == docenteId).ToListAsync();
            return Ok(proyectos);
        }

        // ENDPOINT: Listar cursos de capacitación por docente
        [HttpGet("cursos/{docenteId}")]
        public async Task<IActionResult> GetCursosByDocente(int docenteId)
        {
            var cursos = await _context.CursoCapacitacion.Where(c => c.DocenteId == docenteId).ToListAsync();
            return Ok(cursos);
        }

        // ENDPOINT: Listar publicaciones académicas por docente
        [HttpGet("publicaciones/{docenteId}")]
        public async Task<IActionResult> GetPublicacionesByDocente(int docenteId)
        {
            var publicaciones = await _context.PublicacionAcademica.Where(p => p.DocenteId == docenteId).ToListAsync();
            return Ok(publicaciones);
        }

        // ENDPOINT: Listar evaluaciones docentes por docente
        [HttpGet("evaluaciones/{docenteId}")]
        public async Task<IActionResult> GetEvaluacionesByDocente(int docenteId)
        {
            var evaluaciones = await _context.EvaluacionDocente.Where(e => e.DocenteId == docenteId).ToListAsync();
            return Ok(evaluaciones);
        }

        // ENDPOINT: Progreso de promoción de un docente
        [HttpGet("progreso-promocion/{docenteId}")]
        public async Task<IActionResult> GetProgresoPromocion(int docenteId)
        {
            // Lógica de ejemplo: cuenta requisitos cumplidos vs totales
            var requisitos = await _context.CumplimientoRequisito.Where(c => c.DocenteId == docenteId).ToListAsync();
            var total = await _context.RequisitoPromocion.CountAsync();
            var cumplidos = requisitos.Count(r => r.Cumplido);
            return Ok(new { Total = total, Cumplidos = cumplidos, Detalle = requisitos });
        }

        // ENDPOINT: Dashboard docente (resumen)
        [HttpGet("dashboard-docente/{docenteId}")]
        public async Task<IActionResult> GetDashboardDocente(int docenteId)
        {
            var docente = await _context.Docente.Include(d => d.NivelAcademico).FirstOrDefaultAsync(d => d.Id == docenteId);
            var cursos = await _context.CursoCapacitacion.CountAsync(c => c.DocenteId == docenteId);
            var proyectos = await _context.ProyectoInvestigacion.CountAsync(p => p.DocenteId == docenteId);
            var publicaciones = await _context.PublicacionAcademica.CountAsync(p => p.DocenteId == docenteId);
            var evaluaciones = await _context.EvaluacionDocente.CountAsync(e => e.DocenteId == docenteId);
            return Ok(new { docente, cursos, proyectos, publicaciones, evaluaciones });
        }

        // ENDPOINT: Dashboard admin (resumen global)
        [HttpGet("dashboard-admin")]
        public async Task<IActionResult> GetDashboardAdmin()
        {
            var totalDocentes = await _context.Docente.CountAsync();
            var totalCursos = await _context.CursoCapacitacion.CountAsync();
            var totalProyectos = await _context.ProyectoInvestigacion.CountAsync();
            var totalPublicaciones = await _context.PublicacionAcademica.CountAsync();
            var totalEvaluaciones = await _context.EvaluacionDocente.CountAsync();
            return Ok(new { totalDocentes, totalCursos, totalProyectos, totalPublicaciones, totalEvaluaciones });
        }

        // ENDPOINT: Aprobar/rechazar solicitud de avance de rango
        [HttpPost("aprobar-solicitud/{solicitudId}")]
        public async Task<IActionResult> AprobarSolicitud(int solicitudId, [FromBody] int administradorId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null) return NotFound();
            solicitud.Estado = "APROBADA";
            solicitud.AdministradorId = administradorId;
            solicitud.FechaRespuesta = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(solicitud);
        }

        [HttpPost("rechazar-solicitud/{solicitudId}")]
        public async Task<IActionResult> RechazarSolicitud(int solicitudId, [FromBody] int administradorId)
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(solicitudId);
            if (solicitud == null) return NotFound();
            solicitud.Estado = "RECHAZADA";
            solicitud.AdministradorId = administradorId;
            solicitud.FechaRespuesta = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(solicitud);
        }

        // ENDPOINT: Asignar/modificar nivel académico de un docente
        [HttpPost("asignar-nivel/{docenteId}")]
        public async Task<IActionResult> AsignarNivel(int docenteId, [FromBody] int nuevoNivelId)
        {
            var docente = await _context.Docente.FindAsync(docenteId);
            if (docente == null) return NotFound();
            docente.NivelAcademicoId = nuevoNivelId;
            await _context.SaveChangesAsync();
            return Ok(docente);
        }

        // ENDPOINT: Activar/desactivar usuario
        [HttpPost("activar-usuario/{usuarioId}")]
        public async Task<IActionResult> ActivarUsuario(int usuarioId)
        {
            var usuario = await _context.Usuario.FindAsync(usuarioId);
            if (usuario == null) return NotFound();
            usuario.Activo = true;
            await _context.SaveChangesAsync();
            return Ok(usuario);
        }

        [HttpPost("desactivar-usuario/{usuarioId}")]
        public async Task<IActionResult> DesactivarUsuario(int usuarioId)
        {
            var usuario = await _context.Usuario.FindAsync(usuarioId);
            if (usuario == null) return NotFound();
            usuario.Activo = false;
            await _context.SaveChangesAsync();
            return Ok(usuario);
        }

        // ENDPOINT: Reporte institucional filtrado
        [HttpGet("reporte-institucional")]
        public async Task<IActionResult> GetReporteInstitucional([FromQuery] int? nivelId, [FromQuery] string? periodo)
        {
            var query = _context.ReporteAvance.AsQueryable();
            if (nivelId.HasValue)
            {
                query = query.Where(r => r.Docente.NivelAcademicoId == nivelId);
            }
            if (!string.IsNullOrEmpty(periodo))
            {
                // Suponiendo que ReporteAvance tiene un campo periodo (ajustar si no)
                // query = query.Where(r => r.Periodo == periodo);
            }
            var reportes = await query.Include(r => r.Docente).ToListAsync();
            return Ok(reportes);
        }
    }
}
