using Microsoft.AspNetCore.Mvc;
using MyCleanApp.Infrastructure.Services;

namespace MyCleanApp.API.Notificacion
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificacionController : ControllerBase
    {
        private readonly INotificacionService _notificacionService;

        public NotificacionController(INotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        [HttpGet("usuario/{usuarioId}")]
        public async Task<IActionResult> GetNotificacionesPorUsuario(int usuarioId)
        {
            try
            {
                var notificaciones = await _notificacionService.GetNotificacionesPorUsuarioAsync(usuarioId);
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpGet("pendientes")]
        public async Task<IActionResult> GetNotificacionesPendientes()
        {
            try
            {
                var notificaciones = await _notificacionService.GetNotificacionesPendientesAsync();
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
            }
        }

        [HttpPost("{id}/marcar-leida")]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            try
            {
                // Implementar lógica para marcar como leída
                return Ok(new { message = "Notificación marcada como leída" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno del servidor", details = ex.Message });
            }
        }
    }
}
