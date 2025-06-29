using System.Net.Http.Json;
using System.Text.Json;

namespace MyCleanApp.Client.Services
{
    public class SolicitudPromocionService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public SolicitudPromocionService(HttpClient http)
        {
            _http = http;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<SolicitudPromocionDto>?> ObtenerSolicitudesAsync()
        {
            try
            {
                var response = await _http.GetAsync("http://localhost:5015/api/SolicitudAvanceRango");
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<SolicitudPromocionDto>>();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener solicitudes: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> AprobarSolicitudAsync(int solicitudId, string observaciones = "")
        {
            try
            {
                var request = new AprobacionRequest { Observaciones = observaciones };
                var response = await _http.PostAsJsonAsync($"http://localhost:5015/api/SolicitudAvanceRango/{solicitudId}/aprobar", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al aprobar solicitud: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RechazarSolicitudAsync(int solicitudId, string motivo)
        {
            try
            {
                var request = new RechazoRequest { Motivo = motivo };
                var response = await _http.PostAsJsonAsync($"http://localhost:5015/api/SolicitudAvanceRango/{solicitudId}/rechazar", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al rechazar solicitud: {ex.Message}");
                return false;
            }
        }
    }

    // DTOs
    public class SolicitudPromocionDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaRespuesta { get; set; }
        public string Observaciones { get; set; } = string.Empty;
        public int NuevoNivelAcademicoId { get; set; }
        public string DocenteNombre { get; set; } = string.Empty;
        public string NivelActual { get; set; } = string.Empty;
        public string NuevoNivel { get; set; } = string.Empty;
    }

    public class AprobacionRequest
    {
        public string? Observaciones { get; set; }
    }

    public class RechazoRequest
    {
        public string? Motivo { get; set; }
    }
}
