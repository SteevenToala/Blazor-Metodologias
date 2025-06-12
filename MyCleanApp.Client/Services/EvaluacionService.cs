using System.Net.Http.Json;

public class EvaluacionService
{
    private readonly HttpClient _http;
    private readonly LocalStorageService _localStorageService;

    public EvaluacionService(HttpClient http, LocalStorageService localStorageService)
    {
        _http = http;
        _localStorageService = localStorageService;
    }

    public async Task<EvaluacionDocenteRequest?> EnviarEvaluacionDocenteAsync()
    {
        var loginResponse = await _localStorageService.ObtenerObjetoAsync<LoginResponse>("loginResponse");
        if (loginResponse == null || loginResponse.usuario == null)
        {
            return null;
        }

        try
        {
            var cedula = loginResponse.usuario.Cedula;
            var response = await _http.GetAsync($"http://localhost:3000/persona/{cedula}");

            if (response.IsSuccessStatusCode)
            {
                var evaluacion = await response.Content.ReadFromJsonAsync<EvaluacionDocenteRequest>();
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Respuesta del servidor: {content}");
                return evaluacion;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error en GET: {error}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción en la petición: {ex.Message}");
            return null;
        }
    }
}
