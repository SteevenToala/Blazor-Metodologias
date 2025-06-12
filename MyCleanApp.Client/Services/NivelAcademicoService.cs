using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class NivelAcademicoService
    {
        private readonly HttpClient _http;
        public NivelAcademicoService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<NivelAcademicoDto>?> GetAllAsync()
        {
            return await _http.GetFromJsonAsync<List<NivelAcademicoDto>>("http://localhost:5015/api/NivelAcademico");
        }

        public async Task<NivelAcademicoDto?> GetByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<NivelAcademicoDto>($"http://localhost:5015/api/NivelAcademico/{id}");
        }
    }

    public class NivelAcademicoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
    }
}
