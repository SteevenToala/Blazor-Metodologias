using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class ComisionAcademicaController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public ComisionAcademicaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ComisionAcademica>>> GetComisionesAcademicas()
    {
        try
        {
            var comisiones = await _context.ComisionAcademica
                .Include(c => c.Usuario)
                .ThenInclude(u => u.Persona)
                .Where(c => c.Activo)
                .ToListAsync();

            return Ok(comisiones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ComisionAcademica>> GetComisionAcademica(int id)
    {
        try
        {
            var comision = await _context.ComisionAcademica
                .Include(c => c.Usuario)
                .ThenInclude(u => u.Persona)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (comision == null)
            {
                return NotFound(new { error = "Comisión académica no encontrada" });
            }

            return Ok(comision);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("solicitudes-pendientes")]
    public async Task<ActionResult<IEnumerable<object>>> GetSolicitudesPendientes()
    {
        try
        {
            // Since we don't have ComisionEvaluacion table, we'll simulate with SolicitudAvanceRango data
            var solicitudesPendientes = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                .ThenInclude(d => d.Usuario)
                .ThenInclude(u => u.Persona)
                .Include(s => s.Docente.NivelAcademico)
                .Include(s => s.NuevoNivelAcademico)
                .Where(s => s.Estado == "PENDIENTE" || s.Estado == "EN_REVISION")
                .Select(s => new
                {
                    Id = s.Id,
                    SolicitudId = s.Id,
                    DocenteNombre = s.Docente != null && 
                                   s.Docente.Usuario != null && 
                                   s.Docente.Usuario.Persona != null
                        ? (s.Docente.Usuario.Persona.Nombres ?? "") + " " + 
                          (s.Docente.Usuario.Persona.Apellidos ?? "")
                        : "",
                    NivelActual = s.Docente != null && s.Docente.NivelAcademico != null
                        ? s.Docente.NivelAcademico.nombre ?? ""
                        : "",
                    NivelSolicitado = s.NuevoNivelAcademico != null
                        ? s.NuevoNivelAcademico.nombre ?? ""
                        : "",
                    FechaSolicitud = s.FechaSolicitud,
                    FechaRecepcion = s.FechaSolicitud,
                    FechaLimite = s.FechaSolicitud.AddDays(30), // 30 days limit
                    Estado = "PENDIENTE_EVALUACION",
                    DiasEnComision = EF.Functions.DateDiffDay(s.FechaSolicitud, DateTime.Now),
                    RequiereAtencionUrgente = EF.Functions.DateDiffDay(DateTime.Now, s.FechaSolicitud.AddDays(30)) <= 2
                })
                .ToListAsync();

            return Ok(solicitudesPendientes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("solicitudes-en-evaluacion")]
    public async Task<ActionResult<IEnumerable<object>>> GetSolicitudesEnEvaluacion()
    {
        try
        {
            var solicitudesEnEvaluacion = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                .ThenInclude(d => d.Usuario)
                .ThenInclude(u => u.Persona)
                .Include(s => s.Docente.NivelAcademico)
                .Include(s => s.NuevoNivelAcademico)
                .Where(s => s.Estado == "EN_EVALUACION")
                .Select(s => new
                {
                    Id = s.Id,
                    SolicitudId = s.Id,
                    DocenteNombre = s.Docente != null && 
                                   s.Docente.Usuario != null && 
                                   s.Docente.Usuario.Persona != null
                        ? (s.Docente.Usuario.Persona.Nombres ?? "") + " " + 
                          (s.Docente.Usuario.Persona.Apellidos ?? "")
                        : "",
                    NivelActual = s.Docente != null && s.Docente.NivelAcademico != null
                        ? s.Docente.NivelAcademico.nombre ?? ""
                        : "",
                    NivelSolicitado = s.NuevoNivelAcademico != null
                        ? s.NuevoNivelAcademico.nombre ?? ""
                        : "",
                    FechaRecepcion = s.FechaSolicitud,
                    FechaLimite = s.FechaSolicitud.AddDays(30),
                    Estado = "EN_EVALUACION",
                    PuntajeTotal = (decimal?)85.5, // Simulated score
                    ComentarioComision = "En proceso de evaluación"
                })
                .ToListAsync();

            return Ok(solicitudesEnEvaluacion);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("votaciones-activas")]
    public ActionResult<IEnumerable<object>> GetVotacionesActivas()
    {
        try
        {
            // Simulated active votes data
            var votacionesActivas = new object[]
            {
                new { 
                    ComisionId = 1, 
                    TotalVotos = 3,
                    VotosAprobados = 2,
                    VotosRechazados = 1,
                    VotosAbstencion = 0,
                    DocenteNombre = "Dr. Juan Pérez",
                    NivelSolicitado = "Profesor Principal"
                },
                new { 
                    ComisionId = 2, 
                    TotalVotos = 3,
                    VotosAprobados = 3,
                    VotosRechazados = 0,
                    VotosAbstencion = 0,
                    DocenteNombre = "Dra. María García",
                    NivelSolicitado = "Profesor Asociado"
                }
            };

            return Ok(votacionesActivas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("actas")]
    public ActionResult<IEnumerable<object>> GetActas()
    {
        try
        {
            // Simulated meeting minutes data
            var actas = new object[]
            {
                new { 
                    Id = 1,
                    FechaEvaluacion = DateTime.Now.AddDays(-5),
                    PuntajeTotal = 87.5m,
                    ComentarioComision = "Candidato calificado para promoción",
                    DocenteNombre = "Dr. Carlos López",
                    Decision = "APROBADO"
                },
                new { 
                    Id = 2,
                    FechaEvaluacion = DateTime.Now.AddDays(-10),
                    PuntajeTotal = 65.0m,
                    ComentarioComision = "Requiere más experiencia en investigación",
                    DocenteNombre = "Dra. Ana Rodríguez",
                    Decision = "RECHAZADO"
                }
            };

            return Ok(actas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ComisionAcademica>> CreateComisionAcademica([FromBody] ComisionAcademica comision)
    {
        try
        {
            if (comision == null)
            {
                return BadRequest(new { error = "Los datos de la comisión son requeridos" });
            }

            // Verificar que el usuario existe
            var usuarioExists = await _context.Usuario.AnyAsync(u => u.Id == comision.UsuarioId);
            if (!usuarioExists)
            {
                return BadRequest(new { error = "El usuario especificado no existe" });
            }

            _context.ComisionAcademica.Add(comision);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComisionAcademica), new { id = comision.Id }, comision);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateComisionAcademica(int id, [FromBody] ComisionAcademica comision)
    {
        try
        {
            if (id != comision.Id)
            {
                return BadRequest(new { error = "El ID no coincide" });
            }

            var existingComision = await _context.ComisionAcademica.FindAsync(id);
            if (existingComision == null)
            {
                return NotFound(new { error = "Comisión académica no encontrada" });
            }

            // Verificar que el usuario existe
            var usuarioExists = await _context.Usuario.AnyAsync(u => u.Id == comision.UsuarioId);
            if (!usuarioExists)
            {
                return BadRequest(new { error = "El usuario especificado no existe" });
            }

            existingComision.Nombre = comision.Nombre;
            existingComision.Cargo = comision.Cargo;
            existingComision.UsuarioId = comision.UsuarioId;
            existingComision.Activo = comision.Activo;
            existingComision.FechaDesignacion = comision.FechaDesignacion;
            existingComision.FechaFinPeriodo = comision.FechaFinPeriodo;

            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteComisionAcademica(int id)
    {
        try
        {
            var comision = await _context.ComisionAcademica.FindAsync(id);
            if (comision == null)
            {
                return NotFound(new { error = "Comisión académica no encontrada" });
            }

            // En lugar de eliminar, marcar como inactivo
            comision.Activo = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }
}
