using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class CumplimientoRequisitoService
    {
        private readonly HttpClient _http;
        public CumplimientoRequisitoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CumplimientoRequisitoDto>?> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<CumplimientoRequisitoDto>>("http://localhost:5015/api/CumplimientoRequisito");
        }

        public async Task<CumplimientoRequisitoDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<CumplimientoRequisitoDto>($"http://localhost:5015/api/CumplimientoRequisito/{id}");
        }
    }

    // Elimina la clase duplicada para evitar el error CS0101
}
