using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyCleanApp.Infrastructure.Persistence;

[ApiController]
[Route("api/[controller]")]
public class ReportesController : ControllerBase
{
    private readonly AppDbContext _context;
    
    public ReportesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("resumen-ejecutivo")]
    public async Task<ActionResult<ReporteResumen>> GetResumenEjecutivo()
    {
        try
        {
            var solicitudes = await _context.SolicitudAvanceRango
                .ToListAsync();

            var totalSolicitudes = solicitudes.Count;
            var pendientes = solicitudes.Count(s => s.Estado == "PENDIENTE");
            var aprobadas = solicitudes.Count(s => s.Estado == "APROBADA");
            var rechazadas = solicitudes.Count(s => s.Estado == "RECHAZADA");
            
            // Calcular tiempo promedio de revisión (en días)
            var solicitudesConRespuesta = solicitudes.Where(s => s.FechaRespuesta.HasValue).ToList();
            var tiempoPromedioRevision = solicitudesConRespuesta.Any() 
                ? solicitudesConRespuesta.Average(s => (s.FechaRespuesta!.Value - s.FechaSolicitud).TotalDays)
                : 0;

            var porcentajeAprobacion = totalSolicitudes > 0 
                ? Math.Round((double)aprobadas / totalSolicitudes * 100, 2)
                : 0;

            var resumen = new ReporteResumen
            {
                TotalSolicitudes = totalSolicitudes,
                SolicitudesPendientes = pendientes,
                SolicitudesAprobadas = aprobadas,
                SolicitudesRechazadas = rechazadas,
                TiempoPromedioRevision = Math.Round(tiempoPromedioRevision, 1),
                PorcentajeAprobacion = porcentajeAprobacion
            };

            return Ok(resumen);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("promociones-por-periodo")]
    public async Task<ActionResult<List<PromocionPorPeriodo>>> GetPromocionesPorPeriodo()
    {
        try
        {
            var solicitudes = await _context.SolicitudAvanceRango
                .Include(s => s.Docente)
                    .ThenInclude(d => d!.NivelAcademico)
                .Include(s => s.NuevoNivelAcademico)
                .Where(s => s.Estado == "APROBADA")
                .ToListAsync();

            // Agrupar por período (últimos 12 meses)
            var promociones = solicitudes
                .Where(s => s.FechaRespuesta.HasValue && s.FechaRespuesta.Value >= DateTime.Now.AddMonths(-12))
                .GroupBy(s => s.FechaRespuesta!.Value.ToString("yyyy-MM"))
                .OrderByDescending(g => g.Key)
                .Select(g => new PromocionPorPeriodo
                {
                    Periodo = g.Key,
                    AuxiliarAAgregado = g.Count(s => 
                        s.Docente?.NivelAcademico?.nombre?.Contains("Auxiliar") == true &&
                        s.NuevoNivelAcademico?.nombre?.Contains("Agregado") == true),
                    AgregadoAPrincipal = g.Count(s => 
                        s.Docente?.NivelAcademico?.nombre?.Contains("Agregado") == true &&
                        s.NuevoNivelAcademico?.nombre?.Contains("Principal") == true),
                    PrincipalAInvestigador = g.Count(s => 
                        s.Docente?.NivelAcademico?.nombre?.Contains("Principal") == true &&
                        s.NuevoNivelAcademico?.nombre?.Contains("Investigador") == true)
                })
                .ToList();

            return Ok(promociones);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("tiempos-proceso")]
    public async Task<ActionResult<List<TiempoProceso>>> GetTiemposProceso()
    {
        try
        {
            var solicitudes = await _context.SolicitudAvanceRango
                .Where(s => s.FechaRespuesta.HasValue)
                .ToListAsync();

            if (!solicitudes.Any())
            {
                return Ok(new List<TiempoProceso>());
            }

            // Calcular tiempos por etapa
            var tiemposProceso = new List<TiempoProceso>();

            // Etapa de revisión inicial (PENDIENTE -> EN_VERIFICACION o VERIFICADA)
            var tiemposRevisionInicial = solicitudes
                .Select(s => (s.FechaRespuesta!.Value - s.FechaSolicitud).TotalDays)
                .Where(t => t > 0)
                .ToList();

            if (tiemposRevisionInicial.Any())
            {
                tiemposProceso.Add(new TiempoProceso
                {
                    Etapa = "Revisión Inicial",
                    TiempoPromedio = Math.Round(tiemposRevisionInicial.Average(), 1),
                    TiempoMinimo = (int)tiemposRevisionInicial.Min(),
                    TiempoMaximo = (int)tiemposRevisionInicial.Max()
                });
            }

            // Proceso completo
            var tiemposCompletos = solicitudes
                .Where(s => s.Estado == "APROBADA" || s.Estado == "RECHAZADA")
                .Select(s => (s.FechaRespuesta!.Value - s.FechaSolicitud).TotalDays)
                .Where(t => t > 0)
                .ToList();

            if (tiemposCompletos.Any())
            {
                tiemposProceso.Add(new TiempoProceso
                {
                    Etapa = "Proceso Completo",
                    TiempoPromedio = Math.Round(tiemposCompletos.Average(), 1),
                    TiempoMinimo = (int)tiemposCompletos.Min(),
                    TiempoMaximo = (int)tiemposCompletos.Max()
                });
            }

            return Ok(tiemposProceso);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("metricas-calidad")]
    public async Task<ActionResult<MetricasCalidad>> GetMetricasCalidad()
    {
        try
        {
            var solicitudes = await _context.SolicitudAvanceRango
                .ToListAsync();

            var totalSolicitudes = solicitudes.Count;
            var solicitudesDevueltas = solicitudes.Count(s => s.Estado == "RECHAZADA");
            var solicitudesAprobadas = solicitudes.Count(s => s.Estado == "APROBADA");

            // Estimación de documentos con errores basada en solicitudes rechazadas
            var documentosConErrores = solicitudesDevueltas;
            var documentosCorrectos = solicitudesAprobadas;
            
            var porcentajeCalidad = totalSolicitudes > 0 
                ? Math.Round((double)documentosCorrectos / totalSolicitudes * 100, 2)
                : 100;

            // Estimación de revisiones repetidas (solicitudes que han estado en múltiples estados)
            var revisionesRepetidas = solicitudes.Count(s => 
                !string.IsNullOrEmpty(s.Observaciones) && s.Observaciones.Length > 50);

            var metricas = new MetricasCalidad
            {
                DocumentosConErrores = documentosConErrores,
                DocumentosCorrectos = documentosCorrectos,
                PorcentajeCalidad = porcentajeCalidad,
                RevisionesRepetidas = revisionesRepetidas,
                SolicitudesDevueltas = solicitudesDevueltas
            };

            return Ok(metricas);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("errores-documentales")]
    public async Task<ActionResult<List<ErrorDocumental>>> GetErroresDocumentales()
    {
        try
        {
            var solicitudesRechazadas = await _context.SolicitudAvanceRango
                .Where(s => s.Estado == "RECHAZADA" && !string.IsNullOrEmpty(s.Observaciones))
                .ToListAsync();

            var totalErrores = solicitudesRechazadas.Count;
            
            if (totalErrores == 0)
            {
                return Ok(new List<ErrorDocumental>());
            }

            // Análisis básico de tipos de errores comunes
            var errores = new List<ErrorDocumental>();

            var errorEvaluaciones = solicitudesRechazadas.Count(s => 
                s.Observaciones!.ToLower().Contains("evaluacion") || 
                s.Observaciones!.ToLower().Contains("puntaje"));
            
            var errorCapacitaciones = solicitudesRechazadas.Count(s => 
                s.Observaciones!.ToLower().Contains("capacitacion") || 
                s.Observaciones!.ToLower().Contains("curso"));
            
            var errorPublicaciones = solicitudesRechazadas.Count(s => 
                s.Observaciones!.ToLower().Contains("publicacion") || 
                s.Observaciones!.ToLower().Contains("articulo"));
            
            var errorProyectos = solicitudesRechazadas.Count(s => 
                s.Observaciones!.ToLower().Contains("proyecto") || 
                s.Observaciones!.ToLower().Contains("investigacion"));

            if (errorEvaluaciones > 0)
            {
                errores.Add(new ErrorDocumental
                {
                    TipoError = "Evaluaciones Docentes",
                    Frecuencia = errorEvaluaciones,
                    Porcentaje = Math.Round((double)errorEvaluaciones / totalErrores * 100, 1),
                    Impacto = "Alto"
                });
            }

            if (errorCapacitaciones > 0)
            {
                errores.Add(new ErrorDocumental
                {
                    TipoError = "Capacitaciones",
                    Frecuencia = errorCapacitaciones,
                    Porcentaje = Math.Round((double)errorCapacitaciones / totalErrores * 100, 1),
                    Impacto = "Medio"
                });
            }

            if (errorPublicaciones > 0)
            {
                errores.Add(new ErrorDocumental
                {
                    TipoError = "Publicaciones",
                    Frecuencia = errorPublicaciones,
                    Porcentaje = Math.Round((double)errorPublicaciones / totalErrores * 100, 1),
                    Impacto = "Alto"
                });
            }

            if (errorProyectos > 0)
            {
                errores.Add(new ErrorDocumental
                {
                    TipoError = "Proyectos de Investigación",
                    Frecuencia = errorProyectos,
                    Porcentaje = Math.Round((double)errorProyectos / totalErrores * 100, 1),
                    Impacto = "Alto"
                });
            }

            // Error genérico para otros casos
            var otrosErrores = totalErrores - errorEvaluaciones - errorCapacitaciones - errorPublicaciones - errorProyectos;
            if (otrosErrores > 0)
            {
                errores.Add(new ErrorDocumental
                {
                    TipoError = "Documentación General",
                    Frecuencia = otrosErrores,
                    Porcentaje = Math.Round((double)otrosErrores / totalErrores * 100, 1),
                    Impacto = "Bajo"
                });
            }

            return Ok(errores.OrderByDescending(e => e.Frecuencia).ToList());
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}

// Modelos de datos para los reportes
public class ReporteResumen
{
    public int TotalSolicitudes { get; set; }
    public int SolicitudesPendientes { get; set; }
    public int SolicitudesAprobadas { get; set; }
    public int SolicitudesRechazadas { get; set; }
    public double TiempoPromedioRevision { get; set; }
    public double PorcentajeAprobacion { get; set; }
}

public class PromocionPorPeriodo
{
    public string Periodo { get; set; } = string.Empty;
    public int AuxiliarAAgregado { get; set; }
    public int AgregadoAPrincipal { get; set; }
    public int PrincipalAInvestigador { get; set; }
}

public class TiempoProceso
{
    public string Etapa { get; set; } = string.Empty;
    public double TiempoPromedio { get; set; }
    public int TiempoMinimo { get; set; }
    public int TiempoMaximo { get; set; }
}

public class MetricasCalidad
{
    public int DocumentosConErrores { get; set; }
    public int DocumentosCorrectos { get; set; }
    public double PorcentajeCalidad { get; set; }
    public int RevisionesRepetidas { get; set; }
    public int SolicitudesDevueltas { get; set; }
}

public class ErrorDocumental
{
    public string TipoError { get; set; } = string.Empty;
    public int Frecuencia { get; set; }
    public double Porcentaje { get; set; }
    public string Impacto { get; set; } = string.Empty;
}
