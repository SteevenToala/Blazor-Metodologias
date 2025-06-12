using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class AcademicPaperService
    {
        private readonly HttpClient _http;
        public AcademicPaperService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<PublicacionAcademicaDto>?> GetAllPapersAsync()
        {
            return await _http.GetFromJsonAsync<List<PublicacionAcademicaDto>>("http://localhost:5015/api/PublicacionAcademica");
        }

        public async Task<PublicacionAcademicaDto?> GetPaperByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<PublicacionAcademicaDto>($"http://localhost:5015/api/PublicacionAcademica/{id}");
        }

        public async Task<HttpResponseMessage> CreatePaperAsync(PublicacionAcademicaDto paper)
        {
            return await _http.PostAsJsonAsync("http://localhost:5015/api/PublicacionAcademica", paper);
        }

        public async Task<HttpResponseMessage> UpdatePaperAsync(int id, PublicacionAcademicaDto paper)
        {
            return await _http.PutAsJsonAsync($"http://localhost:5015/api/PublicacionAcademica/{id}", paper);
        }

        public async Task<HttpResponseMessage> DeletePaperAsync(int id)
        {
            return await _http.DeleteAsync($"http://localhost:5015/api/PublicacionAcademica/{id}");
        }
    }

    public class PublicacionAcademicaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Revista { get; set; } = string.Empty;
        public string Volumen { get; set; } = string.Empty;
        public int Anio { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public int DocenteId { get; set; }
        // Puedes agregar más propiedades según el backend
    }
}
