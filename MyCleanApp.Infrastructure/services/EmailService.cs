using System.Text;
using System.Text.Json;

namespace MyCleanApp.Infrastructure.Services
{
    public interface IEmailService
    {
        Task<bool> EnviarCorreoDecisionComision(string destinatario, string decision, string observaciones, string nombreDocente, string nivelSolicitado);
    }

    public class EmailService : IEmailService
    {
        private readonly HttpClient _httpClient;
        private readonly string _emailApiUrl = "https://api-b7rtqstgmq-uc.a.run.app/enviarCorreo";

        public EmailService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> EnviarCorreoDecisionComision(string destinatario, string decision, string observaciones, string nombreDocente, string nivelSolicitado)
        {
            try
            {
                var asunto = $"Decisión de la Comisión Académica - Solicitud de Promoción a {nivelSolicitado}";
                
                var cuerpoCorreo = GenerarCuerpoCorreo(decision, observaciones, nombreDocente, nivelSolicitado);

                var emailRequest = new
                {
                    to = destinatario,
                    subject = asunto,
                    text = cuerpoCorreo
                };

                var jsonContent = JsonSerializer.Serialize(emailRequest);
                Console.WriteLine($"Enviando correo a: {destinatario}");
                Console.WriteLine($"JSON a enviar: {jsonContent}");
                
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_emailApiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                Console.WriteLine($"Respuesta del API: Status={response.StatusCode}, Content={responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Correo enviado exitosamente a {destinatario}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Error al enviar correo: {response.StatusCode} - {responseContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al enviar correo: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }

        private string GenerarCuerpoCorreo(string decision, string observaciones, string nombreDocente, string nivelSolicitado)
        {
            var esAprobado = decision.ToUpper() == "APROBADO";
            var resultadoTexto = esAprobado ? "APROBADA" : "RECHAZADA";

            var cuerpo = $@"UNIVERSIDAD - SISTEMA DE PROMOCIÓN ACADÉMICA
============================================

DECISIÓN DE LA COMISIÓN ACADÉMICA

Estimado/a {nombreDocente},

La Comisión Académica ha emitido una decisión sobre su solicitud de promoción al nivel académico {nivelSolicitado}.

*** SOLICITUD {resultadoTexto} ***

DETALLES DE LA DECISIÓN:
------------------------
• Resultado: {decision}
• Nivel Solicitado: {nivelSolicitado}
• Fecha de Decisión: {DateTime.Now:dd/MM/yyyy HH:mm}";

            if (!string.IsNullOrEmpty(observaciones))
            {
                cuerpo += $@"
• Observaciones: {observaciones}";
            }

            if (esAprobado)
            {
                cuerpo += @"

Su solicitud ha sido APROBADA por la Comisión Académica. El expediente será enviado al Consejo Universitario para la aprobación final.

Le notificaremos cuando el Consejo Universitario emita su decisión final.";
            }
            else
            {
                cuerpo += @"

Lamentamos informarle que su solicitud ha sido RECHAZADA por la Comisión Académica.

Si no está de acuerdo con esta decisión, puede presentar una apelación según el reglamento universitario.";
            }

            cuerpo += @"

Para más información o consultas, puede comunicarse con la Secretaría Académica.

Cordialmente,
Comisión Académica
Universidad

---
Este es un correo electrónico automático del Sistema de Promoción Académica. 
Por favor, no responda a este mensaje.
Fecha de envío: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            return cuerpo;
        }
    }
}
