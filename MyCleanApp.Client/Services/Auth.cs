using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class LoginRequest
{
    public string correo { get; set; } = string.Empty;
    public string contraseña { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public Usuario? usuario { get; set; } = null;

}
public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http)
    {
        _http = http;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest loginData)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("http://localhost:5015/api/Auth/login", loginData);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponse>();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error en login: {error}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al hacer POST: {ex.Message}");
            return null;
        }
    }
}
