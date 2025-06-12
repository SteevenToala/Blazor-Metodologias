using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class TeacherEvaluationService
    {
        private readonly HttpClient _http;
        public TeacherEvaluationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<EvaluacionDocenteDto>?> GetAllEvaluationsAsync()
        {
            return await _http.GetFromJsonAsync<List<EvaluacionDocenteDto>>("http://localhost:5015/api/EvaluacionDocente");
        }

        public async Task<EvaluacionDocenteDto?> GetEvaluationByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<EvaluacionDocenteDto>($"http://localhost:5015/api/EvaluacionDocente/{id}");
        }

        public async Task<HttpResponseMessage> CreateEvaluationAsync(EvaluacionDocenteDto eval)
        {
            return await _http.PostAsJsonAsync("http://localhost:5015/api/EvaluacionDocente", eval);
        }

        public async Task<HttpResponseMessage> UpdateEvaluationAsync(int id, EvaluacionDocenteDto eval)
        {
            return await _http.PutAsJsonAsync($"http://localhost:5015/api/EvaluacionDocente/{id}", eval);
        }

        public async Task<HttpResponseMessage> DeleteEvaluationAsync(int id)
        {
            return await _http.DeleteAsync($"http://localhost:5015/api/EvaluacionDocente/{id}");
        }
    }

    public class EvaluacionDocenteDto
    {
        public int Id { get; set; }
        public string Periodo { get; set; } = string.Empty;
        public float Puntaje { get; set; }
        public int DocenteId { get; set; }
    }
}
