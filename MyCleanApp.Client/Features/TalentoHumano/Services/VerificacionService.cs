using System.Text.Json;
using System.Text;
using MyCleanApp.Client.DTOs;

namespace MyCleanApp.Client.Features.TalentoHumano.Services
{
    public class VerificacionService
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _jsonOptions;

        public VerificacionService(HttpClient http)
        {
            _http = http;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<List<SolicitudVerificacionDto>> GetSolicitudesPendientes()
        {
            try
            {
                var response = await _http.GetAsync("http://localhost:5015/api/ListaVerificacion/pendientes");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var solicitudes = JsonSerializer.Deserialize<List<JsonElement>>(json, _jsonOptions);
                    
                    return solicitudes?.Select(s => new SolicitudVerificacionDto
                    {
                        Id = s.GetProperty("id").GetInt32(),
                        DocenteId = s.GetProperty("docenteId").GetInt32(),
                        DocenteNombre = s.GetProperty("docenteNombre").GetString() ?? "",
                        NivelActual = s.GetProperty("nivelActual").GetString() ?? "",
                        NivelSolicitado = s.GetProperty("nivelSolicitado").GetString() ?? "",
                        FechaSolicitud = s.GetProperty("fechaSolicitud").GetDateTime(),
                        Estado = s.GetProperty("estado").GetString() ?? "",
                        FechaRespuesta = s.TryGetProperty("fechaRespuesta", out JsonElement fechaResp) && fechaResp.ValueKind != JsonValueKind.Null ? fechaResp.GetDateTime() : null,
                        Observaciones = s.TryGetProperty("observaciones", out JsonElement obs) ? obs.GetString() ?? "" : ""
                    }).ToList() ?? new List<SolicitudVerificacionDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener solicitudes pendientes: {ex.Message}");
            }
            return new List<SolicitudVerificacionDto>();
        }

        public async Task<SolicitudVerificacionDto?> GetSolicitudPorId(int solicitudId)
        {
            try
            {
                var solicitudes = await GetSolicitudesPendientes();
                return solicitudes.FirstOrDefault(s => s.Id == solicitudId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener solicitud por ID: {ex.Message}");
                return null;
            }
        }

        public async Task<List<VerificacionDocumentosDto>> GetVerificacionesSolicitud(int solicitudId)
        {
            try
            {
                var response = await _http.GetAsync($"http://localhost:5015/api/ListaVerificacion/{solicitudId}/verificaciones");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var verificaciones = JsonSerializer.Deserialize<List<JsonElement>>(json, _jsonOptions);
                    
                    return verificaciones?.Select(v => new VerificacionDocumentosDto
                    {
                        Id = v.GetProperty("id").GetInt32(),
                        SolicitudId = solicitudId,
                        NombreDocumento = v.GetProperty("tipoDocumento").GetString() ?? "",
                        Verificado = v.GetProperty("verificado").GetBoolean(),
                        FechaVerificacion = v.TryGetProperty("fechaVerificacion", out JsonElement fecha) && fecha.ValueKind != JsonValueKind.Null ? fecha.GetDateTime() : null,
                        VerificadoPor = v.TryGetProperty("verificadoPor", out JsonElement verificador) ? verificador.GetString() ?? "" : "",
                        Observaciones = v.TryGetProperty("observaciones", out JsonElement obs) ? obs.GetString() ?? "" : ""
                    }).ToList() ?? new List<VerificacionDocumentosDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener verificaciones: {ex.Message}");
            }
            return new List<VerificacionDocumentosDto>();
        }

        public async Task<List<RequisitoVerificacionDto>> GetRequisitosPorNivel(int nivelAcademicoId)
        {
            try
            {
                var response = await _http.GetAsync($"http://localhost:5015/api/RequisitoNivelAcademico/nivel/{nivelAcademicoId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var requisitos = JsonSerializer.Deserialize<List<JsonElement>>(json, _jsonOptions);
                    
                    return requisitos?.Select(r => new RequisitoVerificacionDto
                    {
                        Id = r.GetProperty("id").GetInt32(),
                        Nombre = r.GetProperty("tipoRequisito").GetProperty("nombre").GetString() ?? "",
                        Descripcion = $"Valor requerido: {r.GetProperty("valorRequerido").GetDouble()}",
                        Verificado = false
                    }).ToList() ?? new List<RequisitoVerificacionDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener requisitos por nivel: {ex.Message}");
            }
            return new List<RequisitoVerificacionDto>();
        }

        public async Task<List<ListaVerificacionDto>> GetListaVerificacionPorNivel(int nivelAcademicoId)
        {
            try
            {
                var response = await _http.GetAsync($"http://localhost:5015/api/ListaVerificacion/nivel/{nivelAcademicoId}");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<ListaVerificacionDto>>(json, _jsonOptions) ?? new List<ListaVerificacionDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener lista de verificación: {ex.Message}");
            }
            return new List<ListaVerificacionDto>();
        }

        public async Task<bool> RegistrarVerificacion(int solicitudId, VerificacionRequest verificacion)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(verificacion), Encoding.UTF8, "application/json");
                var response = await _http.PostAsync($"http://localhost:5015/api/ListaVerificacion/{solicitudId}/verificacion", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al registrar verificación: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarEstadoSolicitud(int solicitudId, EstadoSolicitudRequest estado)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(estado), Encoding.UTF8, "application/json");
                var response = await _http.PutAsync($"http://localhost:5015/api/ListaVerificacion/{solicitudId}/estado", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar estado: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EnviarAComision(int solicitudId)
        {
            try
            {
                var response = await _http.PostAsync($"http://localhost:5015/api/ListaVerificacion/{solicitudId}/enviar-comision", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar a comisión: {ex.Message}");
                return false;
            }
        }

        public async Task<List<HistorialVerificacionDto>> GetHistorialVerificacion(int solicitudId)
        {
            try
            {
                // Este endpoint no existe aún, devolvemos lista vacía por ahora
                // En el futuro se puede implementar un endpoint para obtener el historial
                await Task.CompletedTask;
                return new List<HistorialVerificacionDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener historial: {ex.Message}");
                return new List<HistorialVerificacionDto>();
            }
        }
    }
}
