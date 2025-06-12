using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class ResearchProjectService
    {
        private readonly HttpClient _http;
        public ResearchProjectService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ProyectoInvestigacionDto>?> GetAllProjectsAsync()
        {
            return await _http.GetFromJsonAsync<List<ProyectoInvestigacionDto>>("http://localhost:5015/api/ProyectoInvestigacion");
        }

        public async Task<ProyectoInvestigacionDto?> GetProjectByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<ProyectoInvestigacionDto>($"http://localhost:5015/api/ProyectoInvestigacion/{id}");
        }

        public async Task<HttpResponseMessage> CreateProjectAsync(ProyectoInvestigacionDto proyecto)
        {
            return await _http.PostAsJsonAsync("http://localhost:5015/api/ProyectoInvestigacion", proyecto);
        }

        public async Task<HttpResponseMessage> UpdateProjectAsync(int id, ProyectoInvestigacionDto proyecto)
        {
            return await _http.PutAsJsonAsync($"http://localhost:5015/api/ProyectoInvestigacion/{id}", proyecto);
        }

        public async Task<HttpResponseMessage> DeleteProjectAsync(int id)
        {
            return await _http.DeleteAsync($"http://localhost:5015/api/ProyectoInvestigacion/{id}");
        }
    }

    public class ProyectoInvestigacionDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string RolEnProyecto { get; set; } = string.Empty;
        public int DocenteId { get; set; }
        // Puedes agregar más propiedades según el backend
    }
}
