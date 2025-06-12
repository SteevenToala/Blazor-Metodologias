using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class AdminDashboardService
    {
        private readonly HttpClient _http;
        public AdminDashboardService(HttpClient http)
        {
            _http = http;
        }

        public async Task<AdminDashboardDto?> GetAdminDashboardAsync()
        {
            return await _http.GetFromJsonAsync<AdminDashboardDto>("http://localhost:5015/api/Administrador/dashboard-admin");
        }
    }

    public class AdminDashboardDto
    {
        public int TotalDocentes { get; set; }
        public int TotalCursos { get; set; }
        public int TotalProyectos { get; set; }
        public int TotalPublicaciones { get; set; }
        public int TotalEvaluaciones { get; set; }
    }
}
