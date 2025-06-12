using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class ReportService
    {
        private readonly HttpClient _http;
        public ReportService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<ReporteAvanceDto>?> GetAllReportsAsync()
        {
            return await _http.GetFromJsonAsync<List<ReporteAvanceDto>>("http://localhost:5015/api/ReporteAvance");
        }

        public async Task<ReporteAvanceDto?> GetReportByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<ReporteAvanceDto>($"http://localhost:5015/api/ReporteAvance/{id}");
        }

        public async Task<HttpResponseMessage> CreateReportAsync(ReporteAvanceDto reporte)
        {
            return await _http.PostAsJsonAsync("http://localhost:5015/api/ReporteAvance", reporte);
        }

        public async Task UpdateReportAsync(int id, ReporteAvanceDto reporte)
        {
            await _http.PutAsJsonAsync($"http://localhost:5015/api/ReporteAvance/{id}", reporte);
        }

        public async Task DeleteReportAsync(int id)
        {
            await _http.DeleteAsync($"http://localhost:5015/api/ReporteAvance/{id}");
        }

        public async Task<List<ReporteAvanceDto>?> GetInstitutionalReportAsync(int? nivelId = null, string? periodo = null)
        {
            string url = "http://localhost:5015/api/Administrador/reporte-institucional";
            if (nivelId.HasValue || !string.IsNullOrEmpty(periodo))
            {
                url += "?";
                if (nivelId.HasValue) url += $"nivelId={nivelId.Value}&";
                if (!string.IsNullOrEmpty(periodo)) url += $"periodo={periodo}";
            }
            return await _http.GetFromJsonAsync<List<ReporteAvanceDto>>(url);
        }

        public async Task<string> UploadReportFileAsync(byte[] fileBytes, string fileName)
        {
            using var content = new MultipartFormDataContent();
            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            content.Add(fileContent, "file", fileName);
            var response = await _http.PostAsync("http://localhost:5015/api/ReporteAvance/upload", content);
            response.EnsureSuccessStatusCode();
            // Suponiendo que el backend responde con la URL del archivo
            var url = await response.Content.ReadAsStringAsync();
            return url.Trim('"'); // Por si viene como string JSON
        }
    }

    public class ReporteAvanceDto
    {
        public int Id { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public int DocenteId { get; set; }
        public string? ArchivoUrl { get; set; } // Para soporte de archivos
        // Puedes agregar más propiedades según el backend
    }
}
