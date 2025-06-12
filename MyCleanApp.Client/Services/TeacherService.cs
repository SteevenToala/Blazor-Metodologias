using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class TeacherService
    {
        private readonly HttpClient _http;
        public TeacherService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<DocenteDto>?> GetAllTeachersAsync()
        {
            return await _http.GetFromJsonAsync<List<DocenteDto>>("http://localhost:5015/api/Docente");
        }

        public async Task<DocenteDto?> GetTeacherByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<DocenteDto>($"http://localhost:5015/api/Docente/{id}");
        }

        public async Task<HttpResponseMessage> CreateTeacherAsync(DocenteDto docente)
        {
            return await _http.PostAsJsonAsync("http://localhost:5015/api/Docente", docente);
        }

        public async Task<HttpResponseMessage> UpdateTeacherAsync(int id, DocenteDto docente)
        {
            return await _http.PutAsJsonAsync($"http://localhost:5015/api/Docente/{id}", docente);
        }

        public async Task<HttpResponseMessage> DeleteTeacherAsync(int id)
        {
            return await _http.DeleteAsync($"http://localhost:5015/api/Docente/{id}");
        }
    }

    public class DocenteDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int NivelAcademicoId { get; set; }
        public DateTime FechaInicioNivel { get; set; }
        // Puedes agregar más propiedades según el backend
    }
}
