using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class PromotionRequestService
    {
        private readonly HttpClient _http;
        public PromotionRequestService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<SolicitudAvanceRangoDto>?> GetAllRequestsAsync()
        {
            return await _http.GetFromJsonAsync<List<SolicitudAvanceRangoDto>>("http://localhost:5015/api/SolicitudAvanceRango");
        }

        public async Task<SolicitudAvanceRangoDto?> GetRequestByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<SolicitudAvanceRangoDto>($"http://localhost:5015/api/SolicitudAvanceRango/{id}");
        }

        public async Task<HttpResponseMessage> CreateRequestAsync(SolicitudAvanceRangoDto req)
        {
            return await _http.PostAsJsonAsync("http://localhost:5015/api/SolicitudAvanceRango", req);
        }

        public async Task<HttpResponseMessage> UpdateRequestAsync(int id, SolicitudAvanceRangoDto req)
        {
            return await _http.PutAsJsonAsync($"http://localhost:5015/api/SolicitudAvanceRango/{id}", req);
        }

        public async Task<HttpResponseMessage> DeleteRequestAsync(int id)
        {
            return await _http.DeleteAsync($"http://localhost:5015/api/SolicitudAvanceRango/{id}");
        }

        public async Task<HttpResponseMessage> ApproveRequestAsync(int solicitudId, int administradorId)
        {
            return await _http.PostAsJsonAsync($"http://localhost:5015/api/Administrador/aprobar-solicitud/{solicitudId}", administradorId);
        }

        public async Task<HttpResponseMessage> RejectRequestAsync(int solicitudId, int administradorId)
        {
            return await _http.PostAsJsonAsync($"http://localhost:5015/api/Administrador/rechazar-solicitud/{solicitudId}", administradorId);
        }
    }

    public class SolicitudAvanceRangoDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime? FechaRespuesta { get; set; }
        public int? AdministradorId { get; set; }
        public string? Observaciones { get; set; }
        public int NuevoNivelAcademicoId { get; set; }
    }
}
