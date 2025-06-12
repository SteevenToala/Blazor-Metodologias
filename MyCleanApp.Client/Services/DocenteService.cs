using System.Net.Http.Json;

public class DocenteService
{
    private readonly HttpClient _http;
    private readonly LocalStorageService _localStorageService;

    public DocenteService(HttpClient http, LocalStorageService localStorageService)
    {
        _http = http;
        _localStorageService = localStorageService;
    }

    public async Task<DashBoardDocente?> getDataDashBoard()
    {
        var loginResponse = await _localStorageService.ObtenerObjetoAsync<LoginResponse>("loginResponse");
        if (loginResponse == null || loginResponse.usuario == null)
        {
            return null;
        }

        try
        {
            var cedula = loginResponse.usuario.Cedula;
            Console.WriteLine("su id es : "+loginResponse.usuario.Id);
            var response = await _http.GetAsync($"http://localhost:5015/api/Docente/info/{loginResponse.usuario.Id}");

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<DashBoardDocente>();
                if (data == null)
                {
                    Console.WriteLine("No se pudo deserializar la respuesta a DashBoardDocente.");
                    return null;
                }
                var requisitos = await _http.GetAsync($"http://localhost:5015/api/NivelAcademico/{data.NivelAcademico}/requisitos");
                var requisitosArray = await requisitos.Content.ReadFromJsonAsync<RequisitoNivel[]>();
                data.requisitoNivel = requisitosArray ?? Array.Empty<RequisitoNivel>();
                return data;
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
    public async Task<PublicacionAcademicaDto[]?> getPublicacionesAcademicas()
    {
        var loginResponse = await _localStorageService.ObtenerObjetoAsync<LoginResponse>("loginResponse");
        if (loginResponse == null || loginResponse.usuario == null)
        {
            return null;

        }
        try
        {
            var response = await _http.GetAsync($"http://localhost:5015/api/PublicacionAcademica/usuario/{loginResponse.usuario.Id}");
            var data = await response.Content.ReadFromJsonAsync<PublicacionAcademicaDto[]>();
            if (data == null)
            {
                Console.WriteLine("No se pudo deserializar la respuesta a PublicacionAcademicaDto[].");
                return Array.Empty<PublicacionAcademicaDto>(); // or handle as appropriate
            }
            return data;
        }
        catch (Exception ex)
        {

            Console.WriteLine($"Excepción en la petición: {ex.Message}");
            return null;
        }
    }
    public async Task<bool> SubirPublicacionAcademicaAsync(PublicacionAcademicaRequest data)
    {
        var loginResponse = await _localStorageService.ObtenerObjetoAsync<LoginResponse>("loginResponse");
        if (loginResponse == null || loginResponse.usuario == null)
        {
            return false;

        }
        var httpClient = new HttpClient();
        var response = await httpClient.PostAsJsonAsync("http://localhost:5015/api/PublicacionAcademica", data);
        return response.IsSuccessStatusCode;
    }
}
