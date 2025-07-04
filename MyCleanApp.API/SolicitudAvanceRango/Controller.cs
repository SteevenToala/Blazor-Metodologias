using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Domain.Entities;
using MyCleanApp.Infrastructure.Persistence;
using MyCleanApp.Infrastructure.Services;
using MyCleanApp.API.DTOs;
using System.Data.SqlClient;

[ApiController]
[Route("api/[controller]")]
public class SolicitudAvanceRangoController : ControllerBase
{
    private readonly AppDbContext _context;
    public SolicitudAvanceRangoController(AppDbContext context) => _context = context;

[HttpGet("PendientesPorUsuario")]
public async Task<ActionResult<IEnumerable<object>>> GetSolicitudesPendientesPorUsuario([FromQuery] string correo)
{
    try
    {
        string connectionString = _context.Database.GetConnectionString();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        // 1. Obtener el usuarioId por el correo
        int? usuarioId = null;
        string getUserIdQuery = @"
            SELECT id 
            FROM Usuario 
            WHERE LTRIM(RTRIM(LOWER(correo))) = LTRIM(RTRIM(LOWER(@correo)))";

        using (var getUserIdCommand = new SqlCommand(getUserIdQuery, connection))
        {
            getUserIdCommand.Parameters.AddWithValue("@correo", correo);
            var result = await getUserIdCommand.ExecuteScalarAsync();
            if (result != null && int.TryParse(result.ToString(), out int parsedId))
            {
                usuarioId = parsedId;
            }
        }

        if (usuarioId == null)
        {
            return NotFound(new { error = "Usuario no encontrado con ese correo" });
        }

        // 2. Consulta de solicitudes que NO han sido aprobadas por ese usuario
        string query = @"
            SELECT s.id, s.docenteId, s.fechaSolicitud, ISNULL(s.estado, 'PENDIENTE') AS estado, 
                   s.fechaRespuesta, ISNULL(s.observaciones, '') AS observaciones,
                   s.nuevoNivelAcademicoId,
                   p.nombres + ' ' + p.apellidos AS docenteNombre,
                   naActual.nombre AS nivelActual,
                   naNuevo.nombre AS nuevoNivel
            FROM SolicitudAvanceRango s
            INNER JOIN Docente d ON s.docenteId = d.id
            INNER JOIN Usuario uDocente ON d.usuarioId = uDocente.id
            INNER JOIN Persona p ON uDocente.personaId = p.id
            LEFT JOIN NivelAcademico naActual ON d.nivelAcademicoId = naActual.id
            LEFT JOIN NivelAcademico naNuevo ON s.nuevoNivelAcademicoId = naNuevo.id
            WHERE NOT EXISTS (
                SELECT 1 
                FROM AprobacionSolicitudRango apr 
                WHERE apr.solicitudId = s.id 
                  AND apr.usuarioId = @usuarioId
            )";

        using var command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@usuarioId", usuarioId);

        var solicitudes = new List<object>();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            solicitudes.Add(new
            {
                Id = reader.GetInt32(0),
                DocenteId = reader.GetInt32(1),
                FechaSolicitud = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                Estado = reader.GetString(3),
                FechaRespuesta = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                Observaciones = reader.GetString(5),
                NuevoNivelAcademicoId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                DocenteNombre = reader.GetString(7),
                NivelActual = reader.IsDBNull(8) ? null : reader.GetString(8),
                NuevoNivel = reader.IsDBNull(9) ? null : reader.GetString(9)
            });
        }

        return Ok(solicitudes);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
    }
}
[HttpGet("SolicitudesAprobadasComision")]
public async Task<ActionResult<IEnumerable<object>>> GetSolicitudesAprobadasComision()
{
    try
    {
        string connectionString = _context.Database.GetConnectionString();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        string query = @"
            SELECT s.id, s.docenteId, s.fechaSolicitud, ISNULL(s.estado, 'PENDIENTE') AS estado, 
                   s.fechaRespuesta, ISNULL(s.observaciones, '') AS observaciones,
                   s.nuevoNivelAcademicoId,
                   p.nombres + ' ' + p.apellidos AS docenteNombre,
                   naActual.nombre AS nivelActual,
                   naNuevo.nombre AS nuevoNivel
            FROM SolicitudAvanceRango s
            INNER JOIN Docente d ON s.docenteId = d.id
            INNER JOIN Usuario uDocente ON d.usuarioId = uDocente.id
            INNER JOIN Persona p ON uDocente.personaId = p.id
            LEFT JOIN NivelAcademico naActual ON d.nivelAcademicoId = naActual.id
            LEFT JOIN NivelAcademico naNuevo ON s.nuevoNivelAcademicoId = naNuevo.id
            WHERE s.estado = 'APROBADA_COMICION'";

        using var command = new SqlCommand(query, connection);
        var solicitudes = new List<object>();

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            solicitudes.Add(new
            {
                Id = reader.GetInt32(0),
                DocenteId = reader.GetInt32(1),
                FechaSolicitud = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                Estado = reader.GetString(3),
                FechaRespuesta = reader.IsDBNull(4) ? (DateTime?)null : reader.GetDateTime(4),
                Observaciones = reader.GetString(5),
                NuevoNivelAcademicoId = reader.IsDBNull(6) ? (int?)null : reader.GetInt32(6),
                DocenteNombre = reader.GetString(7),
                NivelActual = reader.IsDBNull(8) ? null : reader.GetString(8),
                NuevoNivel = reader.IsDBNull(9) ? null : reader.GetString(9)
            });
        }

        return Ok(solicitudes);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
    }
}

[HttpGet("SolicitudesAprobadasTotalConsejo")]
public async Task<ActionResult<IEnumerable<object>>> GetSolicitudesTotalmenteAprobadasPorConsejo()
{
    try
    {
        string connectionString = _context.Database.GetConnectionString();

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var solicitudesAprobadas = new List<object>();

        string query = @"
            WITH ConsejoCount AS (
                SELECT COUNT(*) AS totalConsejo
                FROM Usuario
                WHERE rol = 'CONSEJO_UNIVERSITARIO'
            ),
            AprobadasPorConsejo AS (
                SELECT s.id, COUNT(DISTINCT a.usuarioId) AS totalAprobaciones
                FROM SolicitudAvanceRango s
                JOIN AprobacionSolicitudRango a ON a.solicitudId = s.id
                JOIN Usuario u ON a.usuarioId = u.id
                WHERE u.rol = 'CONSEJO_UNIVERSITARIO'
                  AND s.estado = 'APROBADA_COMICION'
                GROUP BY s.id
            )
            SELECT s.id, s.docenteId, s.fechaSolicitud, s.estado,
                   p.nombres + ' ' + p.apellidos AS docenteNombre,
                   naActual.nombre AS nivelActual,
                   naNuevo.nombre AS nuevoNivel
            FROM SolicitudAvanceRango s
            JOIN AprobadasPorConsejo apc ON apc.id = s.id
            CROSS JOIN ConsejoCount cc
            INNER JOIN Docente d ON s.docenteId = d.id
            INNER JOIN Usuario uDocente ON d.usuarioId = uDocente.id
            INNER JOIN Persona p ON uDocente.personaId = p.id
            LEFT JOIN NivelAcademico naActual ON d.nivelAcademicoId = naActual.id
            LEFT JOIN NivelAcademico naNuevo ON s.nuevoNivelAcademicoId = naNuevo.id
            WHERE apc.totalAprobaciones = cc.totalConsejo
        ";

        using var command = new SqlCommand(query, connection);
        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            solicitudesAprobadas.Add(new
            {
                Id = reader.GetInt32(0),
                DocenteId = reader.GetInt32(1),
                FechaSolicitud = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                Estado = reader.GetString(3),
                DocenteNombre = reader.GetString(4),
                NivelActual = reader.IsDBNull(5) ? null : reader.GetString(5),
                NuevoNivel = reader.IsDBNull(6) ? null : reader.GetString(6)
            });
        }

        return Ok(solicitudesAprobadas);
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
    }
}

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> Get()
    {
        try
        {
            var solicitudes = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .Include(s => s.Docente)
                    .ThenInclude(d => d.NivelAcademico)
                .Include(s => s.NuevoNivelAcademico)
                .Select(s => new
                {
                    s.Id,
                    s.DocenteId,
                    s.FechaSolicitud,
                    Estado = s.Estado ?? "PENDIENTE",
                    s.FechaRespuesta,
                    Observaciones = s.Observaciones ?? "",
                    s.NuevoNivelAcademicoId,
                    DocenteNombre = s.Docente != null && s.Docente.Usuario != null && s.Docente.Usuario.Persona != null
                        ? (s.Docente.Usuario.Persona.Nombres ?? "") + " " + (s.Docente.Usuario.Persona.Apellidos ?? "")
                        : "Sin información",
                    NivelActual = s.Docente != null && s.Docente.NivelAcademico != null
                        ? s.Docente.NivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel",
                    NuevoNivel = s.NuevoNivelAcademico != null
                        ? s.NuevoNivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel"
                })
                .ToListAsync();
            
            return Ok(solicitudes);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> Get(int id)
    {
        try
        {
            var solicitud = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                    .ThenInclude(d => d.Usuario)
                        .ThenInclude(u => u.Persona)
                .Include(s => s.Docente)
                    .ThenInclude(d => d.NivelAcademico)
                .Include(s => s.NuevoNivelAcademico)
                .Where(s => s.Id == id)
                .Select(s => new
                {
                    s.Id,
                    s.DocenteId,
                    s.FechaSolicitud,
                    Estado = s.Estado ?? "PENDIENTE",
                    s.FechaRespuesta,
                    Observaciones = s.Observaciones ?? "",
                    s.NuevoNivelAcademicoId,
                    DocenteNombre = s.Docente != null && s.Docente.Usuario != null && s.Docente.Usuario.Persona != null
                        ? (s.Docente.Usuario.Persona.Nombres ?? "") + " " + (s.Docente.Usuario.Persona.Apellidos ?? "")
                        : "Sin información",
                    NivelActual = s.Docente != null && s.Docente.NivelAcademico != null
                        ? s.Docente.NivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel",
                    NuevoNivel = s.NuevoNivelAcademico != null
                        ? s.NuevoNivelAcademico.nombre ?? "Sin nivel"
                        : "Sin nivel"
                })
                .FirstOrDefaultAsync();

            return solicitud == null ? NotFound() : Ok(solicitud);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] SolicitudAvanceRango solicitud)
    {
        try
        {
            _context.SolicitudAvanceRango.Add(solicitud);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = solicitud.Id }, solicitud);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] SolicitudAvanceRango solicitud)
    {
        try
        {
            if (id != solicitud.Id) return BadRequest();
            
            _context.Entry(solicitud).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(id);
            if (solicitud == null) return NotFound();

            _context.SolicitudAvanceRango.Remove(solicitud);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPost("{id}/aprobar")]
    public async Task<IActionResult> AprobarSolicitud(int id, [FromBody] AprobacionRequest request)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Buscar la solicitud con sus relaciones
            var solicitud = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (solicitud == null)
                return NotFound(new { error = "Solicitud no encontrada" });

            if (solicitud.Estado == "APROBADA")
                return BadRequest(new { error = "La solicitud ya ha sido aprobada" });

            // Actualizar el estado de la solicitud
            solicitud.Estado = "APROBADA";
            solicitud.FechaRespuesta = DateTime.Now;
            solicitud.Observaciones = request.Observaciones ?? "Promoción aprobada";

            // Actualizar el nivel del docente
            var docente = solicitud.Docente;
            if (docente != null && solicitud.NuevoNivelAcademicoId.HasValue)
            {
                docente.NivelAcademicoId = solicitud.NuevoNivelAcademicoId.Value;
                docente.FechaInicioNivel = DateTime.Now;
                _context.Entry(docente).State = EntityState.Modified;

                // Resetear la evaluación del docente a 0 al ser promovido
                var evaluacionDocente = await _context.EvaluacionDocente
                    .FirstOrDefaultAsync(e => e.DocenteId == docente.Id);
                
                if (evaluacionDocente != null)
                {
                    evaluacionDocente.Puntaje = 0;
                    evaluacionDocente.Periodo = $"{DateTime.Now.Year}{(DateTime.Now.Month <= 6 ? "A" : "B")}";
                    _context.Entry(evaluacionDocente).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new 
            { 
                message = "Solicitud aprobada exitosamente",
                solicitudId = id,
                nuevoNivelId = solicitud.NuevoNivelAcademicoId,
                fechaPromocion = DateTime.Now
            });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    [HttpPost("{id}/rechazar")]
    public async Task<IActionResult> RechazarSolicitud(int id, [FromBody] RechazoRequest request)
    {
        try
        {
            var solicitud = await _context.SolicitudAvanceRango.FindAsync(id);
            if (solicitud == null)
                return NotFound(new { error = "Solicitud no encontrada" });

            if (solicitud.Estado == "RECHAZADA")
                return BadRequest(new { error = "La solicitud ya ha sido rechazada" });

            solicitud.Estado = "RECHAZADA";
            solicitud.FechaRespuesta = DateTime.Now;
            solicitud.Observaciones = request.Motivo ?? "Solicitud rechazada";

            await _context.SaveChangesAsync();

            return Ok(new 
            { 
                message = "Solicitud rechazada",
                solicitudId = id,
                motivo = request.Motivo
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
        }
    }

    // Nuevos endpoints para el workflow de promoción

    [HttpPost("{id}/presentar")]
    public async Task<IActionResult> PresentarSolicitud(int id)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.PresentarSolicitudAsync(id);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/recibir-talento-humano")]
    public async Task<IActionResult> RecibirEnTalentoHumano(int id, [FromBody] UsuarioRequest request)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.RecibirEnTalentoHumanoAsync(id, request.UsuarioId);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/iniciar-verificacion")]
    public async Task<IActionResult> IniciarVerificacion(int id, [FromBody] UsuarioRequest request)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.IniciarVerificacionAsync(id, request.UsuarioId);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/completar-verificacion")]
    public async Task<IActionResult> CompletarVerificacion(int id, [FromBody] CompletarVerificacionRequest request)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.CompletarVerificacionAsync(id, request.DocumentosValidos, request.Observaciones);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/enviar-comision")]
    public async Task<IActionResult> EnviarAComision(int id)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.EnviarAComisionAsync(id);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/iniciar-analisis-comision")]
    public async Task<IActionResult> IniciarAnalisisComision(int id)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.IniciarAnalisisComisionAsync(id);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/notificar-resultado")]
    public async Task<IActionResult> NotificarResultado(int id, [FromBody] ResultadoComisionRequest request)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.NotificarResultadoAsync(id, request.Aprobada, request.Observaciones);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/respuesta-docente")]
    public async Task<IActionResult> RegistrarRespuestaDocente(int id, [FromBody] RespuestaDocenteRequest request)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.RegistrarRespuestaDocenteAsync(id, request.Acepta);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/presentar-apelacion")]
    public async Task<IActionResult> PresentarApelacion(int id, [FromBody] ApelacionRequest request)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.PresentarApelacionAsync(id, request.Motivo, request.Fundamentos);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/resolver-apelacion")]
    public async Task<IActionResult> ResolverApelacion(int id, [FromBody] ResolucionApelacionRequest request)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.ResolverApelacionAsync(id, request.Aceptada, request.Resolucion);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/generar-informe-final")]
    public async Task<IActionResult> GenerarInformeFinal(int id)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.GenerarInformeFinalAsync(id);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/enviar-consejo")]
    public async Task<IActionResult> EnviarAConsejo(int id)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.EnviarAConsejoAsync(id);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/aprobar-consejo")]
    public async Task<IActionResult> AprobarEnConsejo(int id)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.AprobarEnConsejoAsync(id);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpPost("{id}/hacer-efectiva")]
    public async Task<IActionResult> HacerPromocionEfectiva(int id)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var resultado = await workflowService.HacerPromocionEfectivaAsync(id);
        
        if (resultado.Success)
            return Ok(resultado);
        else
            return BadRequest(resultado);
    }

    [HttpGet("por-estado/{estado}")]
    public async Task<IActionResult> GetSolicitudesPorEstado(string estado)
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var solicitudes = await workflowService.GetSolicitudesPorEstadoAsync(estado);
        return Ok(solicitudes);
    }

    [HttpGet("vencidas")]
    public async Task<IActionResult> GetSolicitudesVencidas()
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        var solicitudes = await workflowService.GetSolicitudesVencidasAsync();
        return Ok(solicitudes);
    }


[HttpPut("ActualizarEstado")]
public async Task<IActionResult> ActualizarEstado([FromBody] ActualizarEstadoRequest request)
{
    try
    {
        string connectionString = _context.Database.GetConnectionString();
        string? email = null;
        string? nombreCompleto = null;
        string? nivelSolicitado = null;
        DateTime fechaDecision = DateTime.Now;

        using (var connection = new SqlConnection(connectionString))
        {
            await connection.OpenAsync();

            // 1. Validar que UsuarioId existe
            using (var validarUsuarioCmd = new SqlCommand("SELECT COUNT(1) FROM Usuario WHERE id = @UsuarioId", connection))
            {
                validarUsuarioCmd.Parameters.AddWithValue("@UsuarioId", request.UsuarioId);
                var existe = (int)await validarUsuarioCmd.ExecuteScalarAsync();
                if (existe == 0)
                {
                    return BadRequest(new { error = $"UsuarioId {request.UsuarioId} no existe." });
                }
            }

            // 2. Obtener correo, nombre completo y nivel solicitado
            using (var getDatosCommand = new SqlCommand(@"
                SELECT 
                    u.correo,
                    p.nombres + ' ' + p.apellidos AS nombreCompleto,
                    na.nombre AS nivelSolicitado
                FROM SolicitudAvanceRango s
                INNER JOIN Docente d ON s.docenteId = d.id
                INNER JOIN Usuario u ON d.usuarioId = u.id
                INNER JOIN Persona p ON u.personaId = p.id
                LEFT JOIN NivelAcademico na ON s.nuevoNivelAcademicoId = na.id
                WHERE s.id = @SolicitudId", connection))
            {
                getDatosCommand.Parameters.AddWithValue("@SolicitudId", request.Id);
                using var reader = await getDatosCommand.ExecuteReaderAsync();
                if (!await reader.ReadAsync())
                    return NotFound(new { error = "Solicitud no encontrada" });

                email = reader["correo"]?.ToString();
                nombreCompleto = reader["nombreCompleto"]?.ToString();
                nivelSolicitado = reader["nivelSolicitado"]?.ToString();
            }

            // 3. Insertar nuevo registro de aprobación/rechazo
            using (var insertCmd = new SqlCommand(@"
                INSERT INTO AprobacionSolicitudRango (solicitudId, usuarioId, estado, fechaDecision)
                VALUES (@SolicitudId, @UsuarioId, @Estado, @FechaDecision)", connection))
            {
                insertCmd.Parameters.AddWithValue("@SolicitudId", request.Id);
                insertCmd.Parameters.AddWithValue("@UsuarioId", request.UsuarioId);
                insertCmd.Parameters.AddWithValue("@Estado", request.Estado);
                insertCmd.Parameters.AddWithValue("@FechaDecision", fechaDecision);

                await insertCmd.ExecuteNonQueryAsync();
            }

            // 4. Enviar correo solo si está RECHAZADO
            if (request.Estado == "RECHAZADO" && !string.IsNullOrEmpty(email))
            {
                var html = $@"
                    UNIVERSIDAD - SISTEMA DE PROMOCIÓN ACADÉMICA<br/>
                    ============================================<br/><br/>

                    <b>DECISIÓN DEL CONSEJO UNIVERSITARIO</b><br/><br/>

                    Estimado/a <b>{nombreCompleto}</b>,<br/><br/>

                    El Consejo Universitario ha emitido una decisión sobre su solicitud de promoción al nivel académico <b>{nivelSolicitado}</b>.<br/><br/>

                    <h3 style='color:red;'>*** SOLICITUD RECHAZADA ***</h3><br/>

                    <b>DETALLES DE LA DECISIÓN:</b><br/>
                    • Resultado: RECHAZADO<br/>
                    • Nivel Solicitado: {nivelSolicitado}<br/>
                    • Fecha de Decisión: {fechaDecision:dd/MM/yyyy HH:mm}<br/>
                    • Observaciones: Su solicitud ha sido rechazada por la el consejo Universitario.<br/><br/>

                    Para más información o consultas, puede comunicarse con la Secretaría Académica.<br/><br/>

                    Cordialmente,<br/>
                    Consejo Universitario<br/>
                    Universidad<br/><br/>

                    <hr/>
                    <small>Este es un correo electrónico automático del Sistema de Promoción Académica.<br/>
                    Por favor, no responda a este mensaje.<br/>
                    Fecha de envío: {fechaDecision:dd/MM/yyyy HH:mm:ss}</small>";

                using var httpClient = new HttpClient();
                var correoPayload = new
                {
                    to = email,
                    subject = "Decisión del Consejo Universitario",
                    html = html
                };
                var content = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(correoPayload),
                    System.Text.Encoding.UTF8,
                    "application/json");

                await httpClient.PostAsync("https://api-b7rtqstgmq-uc.a.run.app/enviarCorreoHtml", content);
            }

            connection.Close();
        }

        return NoContent();
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
    }
}




    [HttpPost("procesar-vencidas")]
    public async Task<IActionResult> ProcesarSolicitudesVencidas()
    {
        var workflowService = HttpContext.RequestServices.GetRequiredService<IPromocionWorkflowService>();
        await workflowService.ProcesarSolicitudesVencidasAsync();
        return Ok(new { message = "Solicitudes vencidas procesadas" });
    }

    // Clases de request adicionales
    public class UsuarioRequest
    {
        public int UsuarioId { get; set; }
    }

    public class ResultadoComisionRequest
    {
        public bool Aprobada { get; set; }
        public string Observaciones { get; set; } = "";
    }

    public class RespuestaDocenteRequest
    {
        public bool Acepta { get; set; }
    }

    public class ApelacionRequest
    {
        public string Motivo { get; set; } = "";
        public string Fundamentos { get; set; } = "";
    }

    public class ResolucionApelacionRequest
    {
        public bool Aceptada { get; set; }
        public string Resolucion { get; set; } = "";
    }
}

public class AprobacionRequest
{
    public string? Observaciones { get; set; }
}

public class RechazoRequest
{
    public string? Motivo { get; set; }
}