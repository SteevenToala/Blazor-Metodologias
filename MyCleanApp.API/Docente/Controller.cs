using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;
using MyCleanApp.API.DTOs;

[ApiController]
[Route("api/[controller]")]
public class DocenteController : ControllerBase
{
    private readonly AppDbContext _context;
    public DocenteController(AppDbContext context) => _context = context;

    [HttpGet]
    public async Task<IEnumerable<Docente>> Get()
    {
        return await _context.Docente
            .Include(d => d.NivelAcademico)
            .Include(d => d.Usuario)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Docente>> Get(int id)
    {
        var docente = await _context.Docente
            .Include(d => d.NivelAcademico)
            .Include(d => d.Usuario)
            .FirstOrDefaultAsync(d => d.Id == id);

        return docente == null ? NotFound() : Ok(docente);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] Docente docente)
    {
        _context.Docente.Add(docente);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = docente.Id }, docente);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Docente docente)
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
    [HttpGet("info/{usuarioId}")]
    public async Task<ActionResult<object>> GetInfoDocentePorUsuarioId(int usuarioId)
    {
        var docente = await _context.Docente
            .Include(d => d.NivelAcademico)
            .FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);

        if (docente == null)
            return NotFound("Docente no encontrado para ese usuario");

        // Años en el nivel actual
        var aniosEnNivel = (DateTime.Now - docente.FechaInicioNivel).TotalDays / 365.25;

        // Publicaciones académicas
        var publicaciones = await _context.PublicacionAcademica
            .CountAsync(p => p.DocenteId == docente.Id);

        // Puntaje promedio de evaluación
        var puntajeEvaluacion = await _context.EvaluacionDocente
            .Where(e => e.DocenteId == docente.Id)
            .AverageAsync(e => (float?)e.Puntaje) ?? 0;

        // Horas totales de capacitación
        var horasCapacitacion = await _context.CursoCapacitacion
            .Where(c => c.DocenteId == docente.Id)
            .SumAsync(c => (int?)c.Horas) ?? 0;

        return Ok(new
        {
            NivelAcademico = docente.NivelAcademico.nombre,
            AniosEnNivel = Math.Floor(aniosEnNivel),
            PublicacionesAcademicas = publicaciones,
            PuntajeEvaluacion = puntajeEvaluacion,
            HorasCapacitacion = horasCapacitacion
        });
    }

    [HttpGet("total")]
    public async Task<ActionResult<object>> GetTotalDocentes()
    {
        try
        {
            var total = await _context.Docente.CountAsync();
            return Ok(new { totalDocentes = total });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("requisitos-promocion/{usuarioId}")]
    public async Task<ActionResult<object>> GetRequisitosPromocion(int usuarioId)
    {
        try
        {
            var docente = await _context.Docente
                .Include(d => d.NivelAcademico)
                .FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);

            if (docente == null)
                return NotFound("Docente no encontrado para ese usuario");

            // Determinar el próximo nivel académico
            var nivelActual = docente.NivelAcademico.nombre;
            var proximoNivelId = nivelActual switch
            {
                "DT1" => 1, // DT2
                "DT2" => 2, // DT3
                "DT3" => 3, // DT4
                "DT4" => 4, // DT5
                _ => 0
            };

            if (proximoNivelId == 0)
                return Ok(new { message = "El docente está en el nivel máximo", puedePromoverse = false });

            // Obtener requisitos para el próximo nivel
            var requisitosProximoNivel = await _context.RequisitoNivelAcademico
                .Include(r => r.TipoRequisito)
                .Where(r => r.NivelAcademicoId == proximoNivelId)
                .ToListAsync();

            // Calcular estado actual del docente
            var aniosEnNivel = (DateTime.Now - docente.FechaInicioNivel).TotalDays / 365.25;
            
            var publicaciones = await _context.PublicacionAcademica
                .CountAsync(p => p.DocenteId == docente.Id);

            var puntajeEvaluacion = await _context.EvaluacionDocente
                .Where(e => e.DocenteId == docente.Id)
                .AverageAsync(e => (float?)e.Puntaje) ?? 0;

            var horasCapacitacion = await _context.CursoCapacitacion
                .Where(c => c.DocenteId == docente.Id)
                .SumAsync(c => (int?)c.Horas) ?? 0;

            // Calcular meses de investigación
            var proyectosInvestigacion = await _context.ProyectoInvestigacion
                .Where(p => p.DocenteId == docente.Id)
                .ToListAsync();

            var mesesInvestigacion = proyectosInvestigacion
                .Sum(p => {
                    var inicio = p.FechaInicio;
                    var fin = p.FechaFin;
                    return ((fin - inicio).TotalDays / 30.44); // Promedio de días por mes
                });

            // Evaluar cada requisito
            var evaluacionRequisitos = requisitosProximoNivel.Select(req =>
            {
                var valorActual = req.TipoRequisito.Nombre switch
                {
                    "Años en el rango" => (float)Math.Floor(aniosEnNivel),
                    "Papers" => (float)publicaciones,
                    "Puntaje Evaluación" => puntajeEvaluacion,
                    "Horas Capacitación" => (float)horasCapacitacion,
                    "Investigaciones" => (float)Math.Floor(mesesInvestigacion),
                    _ => 0f
                };

                var cumple = valorActual >= req.ValorRequerido;

                return new
                {
                    TipoRequisito = req.TipoRequisito.Nombre,
                    ValorRequerido = req.ValorRequerido,
                    ValorActual = valorActual,
                    Cumple = cumple,
                    Porcentaje = req.ValorRequerido > 0 ? (valorActual / req.ValorRequerido * 100) : 100
                };
            }).ToList();

            var puedePromoverse = evaluacionRequisitos.All(r => r.Cumple);

            var proximoNivel = await _context.NivelAcademico.FindAsync(proximoNivelId);

            return Ok(new
            {
                NivelActual = nivelActual,
                ProximoNivel = proximoNivel?.nombre ?? "N/A",
                PuedePromoverse = puedePromoverse,
                Requisitos = evaluacionRequisitos,
                ResumenCumplimiento = new
                {
                    RequisitosCumplidos = evaluacionRequisitos.Count(r => r.Cumple),
                    TotalRequisitos = evaluacionRequisitos.Count,
                    PorcentajeGeneral = evaluacionRequisitos.Count > 0 
                        ? evaluacionRequisitos.Average(r => r.Porcentaje) 
                        : 0
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPost("solicitar-promocion/{usuarioId}")]
    public async Task<ActionResult> SolicitarPromocion(int usuarioId)
    {
        try
        {
            var docente = await _context.Docente
                .Include(d => d.NivelAcademico)
                .FirstOrDefaultAsync(d => d.UsuarioId == usuarioId);

            if (docente == null)
                return NotFound("Docente no encontrado para ese usuario");

            // Verificar si ya tiene una solicitud pendiente
            var solicitudPendiente = await _context.SolicitudAvanceRango
                .AnyAsync(s => s.DocenteId == docente.Id && s.Estado == "PENDIENTE");

            if (solicitudPendiente)
                return BadRequest(new { error = "Ya tiene una solicitud de promoción pendiente" });

            // Determinar el próximo nivel
            var nivelActual = docente.NivelAcademico.nombre;
            var proximoNivelId = nivelActual switch
            {
                "DT1" => 1, // DT2
                "DT2" => 2, // DT3
                "DT3" => 3, // DT4
                "DT4" => 4, // DT5
                _ => 0
            };

            if (proximoNivelId == 0)
                return BadRequest(new { error = "El docente está en el nivel máximo" });

            // Crear la solicitud
            var nuevaSolicitud = new SolicitudAvanceRango
            {
                DocenteId = docente.Id,
                FechaSolicitud = DateTime.Now,
                Estado = "PENDIENTE",
                NuevoNivelAcademicoId = proximoNivelId,
                Observaciones = "Solicitud generada automáticamente"
            };

            _context.SolicitudAvanceRango.Add(nuevaSolicitud);
            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                message = "Solicitud de promoción creada exitosamente",
                solicitudId = nuevaSolicitud.Id,
                proximoNivel = proximoNivelId
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("detallados")]
    public async Task<ActionResult<IEnumerable<DocenteDetalladoDto>>> GetDocentesDetallados()
    {
        try
        {
            var docentes = await _context.Docente
                .Include(d => d.Usuario)
                    .ThenInclude(u => u.Persona)
                .Include(d => d.NivelAcademico)
                .Select(d => new DocenteDetalladoDto
                {
                    Id = d.Id,
                    NombreCompleto = (d.Usuario!.Persona!.Nombres ?? "") + " " + (d.Usuario.Persona.Apellidos ?? ""),
                    Correo = d.Usuario.Correo ?? "",
                    Cedula = d.Usuario.Persona.Cedula ?? "",
                    NivelAcademico = d.NivelAcademico!.nombre ?? "",
                    FechaInicioNivel = d.FechaInicioNivel,
                    UsuarioId = d.UsuarioId,
                    NivelAcademicoId = d.NivelAcademicoId,
                    UltimaEvaluacion = _context.EvaluacionDocente
                        .Where(e => e.DocenteId == d.Id)
                        .OrderByDescending(e => e.Periodo)
                        .Select(e => (double?)e.Puntaje)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(docentes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}