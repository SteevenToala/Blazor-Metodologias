using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace MyCleanApp.Client.Services
{
    public class UserService
    {
        private readonly HttpClient _http;
        public UserService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<UsuarioDto>?> GetAllUsersAsync()
        {
            // Cambia la URL para que coincida con el nombre correcto del controlador
            return await _http.GetFromJsonAsync<List<UsuarioDto>>("http://localhost:5015/api/Usuarios");
        }

        public async Task<UsuarioDto?> GetUserByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<UsuarioDto>($"http://localhost:5015/api/Usuarios/{id}");
        }

        public async Task<HttpResponseMessage> CreateUserAsync(CrearUsuarioDto usuario)
        {
            return await _http.PostAsJsonAsync("http://localhost:5015/api/Usuarios", usuario);
        }

        public async Task<HttpResponseMessage> UpdateUserAsync(int id, CrearUsuarioDto usuario)
        {
            return await _http.PutAsJsonAsync($"http://localhost:5015/api/Usuarios/{id}", usuario);
        }

        public async Task<HttpResponseMessage> DeleteUserAsync(int id)
        {
            return await _http.DeleteAsync($"http://localhost:5015/api/Usuarios/{id}");
        }
    }

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Correo { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }
    public class CrearUsuarioDto
    {
        public string Correo { get; set; } = string.Empty;
        public string Contraseña { get; set; } = string.Empty;
        public int RolId { get; set; }
        public int PersonaId { get; set; }
    }
}
