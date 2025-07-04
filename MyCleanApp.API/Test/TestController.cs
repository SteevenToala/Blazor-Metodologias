using Microsoft.AspNetCore.Mvc;
using MyCleanApp.Infrastructure.Services;

namespace MyCleanApp.API.Test
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public TestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("enviar-correo")]
        public async Task<ActionResult> TestEnviarCorreo([FromBody] TestEmailRequest request)
        {
            try
            {
                var resultado = await _emailService.EnviarCorreoDecisionComision(
                    request.To,
                    request.Decision,
                    request.Observaciones,
                    request.NombreDocente,
                    request.NivelSolicitado
                );

                return Ok(new { 
                    exito = resultado,
                    mensaje = resultado ? "Correo enviado exitosamente" : "Error al enviar correo",
                    destinatario = request.To,
                    decision = request.Decision
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }

    public class TestEmailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Decision { get; set; } = string.Empty;
        public string Observaciones { get; set; } = string.Empty;
        public string NombreDocente { get; set; } = string.Empty;
        public string NivelSolicitado { get; set; } = string.Empty;
    }
}
