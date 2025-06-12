using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class PromotionRequirementService
    {
        private readonly HttpClient _http;
        public PromotionRequirementService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<RequisitoPromocionDto>?> GetAllRequirementsAsync()
        {
            return await _http.GetFromJsonAsync<List<RequisitoPromocionDto>>("http://localhost:5015/api/RequisitoPromocion");
        }

        public async Task<RequisitoPromocionDto?> GetRequirementByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<RequisitoPromocionDto>($"http://localhost:5015/api/RequisitoPromocion/{id}");
        }

        public async Task<HttpResponseMessage> CreateRequirementAsync(RequisitoPromocionDto req)
        {
            return await _http.PostAsJsonAsync("http://localhost:5015/api/RequisitoPromocion", req);
        }

        public async Task<HttpResponseMessage> UpdateRequirementAsync(int id, RequisitoPromocionDto req)
        {
            return await _http.PutAsJsonAsync($"http://localhost:5015/api/RequisitoPromocion/{id}", req);
        }

        public async Task<HttpResponseMessage> DeleteRequirementAsync(int id)
        {
            return await _http.DeleteAsync($"http://localhost:5015/api/RequisitoPromocion/{id}");
        }
    }

    public class RequisitoPromocionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int PorcentajeAsignado { get; set; }
    }
}
