using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class CourseService
    {
        private readonly HttpClient _http;
        public CourseService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<CursoCapacitacionDto>?> GetAllCoursesAsync()
        {
            return await _http.GetFromJsonAsync<List<CursoCapacitacionDto>>("http://localhost:5015/api/CursoCapacitacion");
        }

        public async Task<CursoCapacitacionDto?> GetCourseByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<CursoCapacitacionDto>($"http://localhost:5015/api/CursoCapacitacion/{id}");
        }

        public async Task<HttpResponseMessage> CreateCourseAsync(CursoCapacitacionDto curso)
        {
            return await _http.PostAsJsonAsync("http://localhost:5015/api/CursoCapacitacion", curso);
        }

        public async Task<HttpResponseMessage> UpdateCourseAsync(int id, CursoCapacitacionDto curso)
        {
            return await _http.PutAsJsonAsync($"http://localhost:5015/api/CursoCapacitacion/{id}", curso);
        }

        public async Task<HttpResponseMessage> DeleteCourseAsync(int id)
        {
            return await _http.DeleteAsync($"http://localhost:5015/api/CursoCapacitacion/{id}");
        }
    }

    public class CursoCapacitacionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Horas { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DocenteId { get; set; }
        // Puedes agregar más propiedades según el backend
    }
}
