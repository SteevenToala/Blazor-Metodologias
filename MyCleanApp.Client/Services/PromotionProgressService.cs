using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class PromotionProgressService
    {
        private readonly HttpClient _http;
        public PromotionProgressService(HttpClient http)
        {
            _http = http;
        }

        public async Task<PromotionProgressDto?> GetProgressByTeacherIdAsync(int docenteId)
        {
            return await _http.GetFromJsonAsync<PromotionProgressDto>($"http://localhost:5015/api/Administrador/progreso-promocion/{docenteId}");
        }
    }

    public class PromotionProgressDto
    {
        public int Total { get; set; }
        public int Cumplidos { get; set; }
        public List<CumplimientoRequisitoDto> Detalle { get; set; } = new();
    }
    public class CumplimientoRequisitoDto
    {
        public int Id { get; set; }
        public int DocenteId { get; set; }
        public int RequisitoId { get; set; }
        public bool Cumplido { get; set; }
        public DateTime FechaCumplimiento { get; set; }
    }
}
