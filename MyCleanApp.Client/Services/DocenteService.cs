using System.Net.Http.Json;
using MyCleanApp.Application.DTOs;
using MyCleanApp.Client.DTOs;
using System.Text;
using System.Text.Json;

public class DocenteService
{
    private readonly HttpClient _http;
    private readonly LocalStorageService _localStorageService;
    private readonly JsonSerializerOptions _jsonOptions;

    public DocenteService(HttpClient http, LocalStorageService localStorageService)
    {
        _http = http;
        _localStorageService = localStorageService;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
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
            Console.WriteLine("su id es : " + loginResponse.usuario.Id);
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

    public async Task<CursoCapacitacionDto[]> getCursosCapacitaciones()
    {
        var loginResponse = await _localStorageService.ObtenerObjetoAsync<LoginResponse>("loginResponse");
        if (loginResponse == null || loginResponse.usuario == null)
        {
            return null;
        }

        try
        {
            var requisitos = await _http.GetAsync($"http://localhost:5015/api/CursoCapacitacion/usuario/{loginResponse.usuario.Id}");
            var cursoCapacitacions = await requisitos.Content.ReadFromJsonAsync<CursoCapacitacionDto[]>();
            return cursoCapacitacions ?? Array.Empty<CursoCapacitacionDto>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Excepción en la petición: {ex.Message}");
            return null;
        }

    }


    public async Task<bool> SubirCapacigtaciones(CursoCapacitacionCreateReques data)
    {
        var loginResponse = await _localStorageService.ObtenerObjetoAsync<LoginResponse>("loginResponse");
        if (loginResponse == null || loginResponse.usuario == null)
        {
            return false;

        }
        var httpClient = new HttpClient();
        var response = await httpClient.PostAsJsonAsync($"http://localhost:5015/api/CursoCapacitacion/usuario/{loginResponse.usuario.Id}", data);
        return response.IsSuccessStatusCode;
    }

    public async Task<int?> ObtenerDocenteIdPorUsuarioId(int usuarioId)
    {
        var response = await _http.GetAsync($"http://localhost:5015/api/Docente");
        if (response.IsSuccessStatusCode)
        {
            var docentes = await response.Content.ReadFromJsonAsync<List<DocenteDto>>();
            var docente = docentes?.FirstOrDefault(d => d.UsuarioId == usuarioId);
            return docente?.Id;
        }
        return null;
    }

    public async Task<bool> RegistrarProyectoInvestigacionAsync(ProyectoInvestigacionRequest data)
    {
        var loginResponse = await _localStorageService.ObtenerObjetoAsync<LoginResponse>("loginResponse");
        if (loginResponse == null || loginResponse.usuario == null)
        {
            return false;
        }
        var docenteId = await ObtenerDocenteIdPorUsuarioId(loginResponse.usuario.Id);
        if (docenteId == null)
            return false;
        data.DocenteId = docenteId.Value;
        var response = await _http.PostAsJsonAsync("http://localhost:5015/api/ProyectoInvestigacion", data);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ProyectoInvestigacionDto>> GetProyectosInvestigacionAsync()
    {
        var loginResponse = await _localStorageService.ObtenerObjetoAsync<LoginResponse>("loginResponse");
        if (loginResponse == null || loginResponse.usuario == null)
        {
            return new List<ProyectoInvestigacionDto>();
        }
        var docenteId = await ObtenerDocenteIdPorUsuarioId(loginResponse.usuario.Id);
        if (docenteId == null)
            return new List<ProyectoInvestigacionDto>();
        var response = await _http.GetAsync($"http://localhost:5015/api/ProyectoInvestigacion");
        if (!response.IsSuccessStatusCode)
            return new List<ProyectoInvestigacionDto>();
        var proyectos = await response.Content.ReadFromJsonAsync<List<ProyectoInvestigacionDto>>();
        return proyectos?.Where(p => p.DocenteId == docenteId.Value).ToList() ?? new List<ProyectoInvestigacionDto>();
    }

    public async Task<bool> ImportarCursoExternoAsync(CursoCapacitacionDto curso)
    {
        var response = await _http.PostAsJsonAsync("http://localhost:5015/api/CursoCapacitacion/importar", curso);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ImportarProyectoExternoAsync(ProyectoInvestigacionDto proyecto)
    {
        var response = await _http.PostAsJsonAsync("http://localhost:5015/api/ProyectoInvestigacion/importar", proyecto);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<PublicacionAcademicaDto>> GetPublicacionesAcademicasAsync()
    {
        var loginResponse = await _localStorageService.ObtenerObjetoAsync<LoginResponse>("loginResponse");
        if (loginResponse == null || loginResponse.usuario == null)
        {
            return new List<PublicacionAcademicaDto>();
        }
        var docenteId = await ObtenerDocenteIdPorUsuarioId(loginResponse.usuario.Id);
        if (docenteId == null)
            return new List<PublicacionAcademicaDto>();
        var response = await _http.GetAsync($"http://localhost:5015/api/PublicacionAcademica");
        if (!response.IsSuccessStatusCode)
            return new List<PublicacionAcademicaDto>();
        var publicaciones = await response.Content.ReadFromJsonAsync<List<PublicacionAcademicaDto>>();
        return publicaciones?.Where(p => p.DocenteId == docenteId.Value).ToList() ?? new List<PublicacionAcademicaDto>();
    }

    public async Task<bool> ImportarPublicacionExternaAsync(PublicacionAcademicaDto publicacion)
    {
        var response = await _http.PostAsJsonAsync("http://localhost:5015/api/PublicacionAcademica/importar", publicacion);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EditarCursoAsync(CursoCapacitacionDto curso)
    {
        var response = await _http.PutAsJsonAsync($"http://localhost:5015/api/CursoCapacitacion/{curso.Id}", curso);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarCursoAsync(int cursoId)
    {
        var response = await _http.DeleteAsync($"http://localhost:5015/api/CursoCapacitacion/{cursoId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EditarProyectoAsync(ProyectoInvestigacionDto proyecto)
    {
        var response = await _http.PutAsJsonAsync($"http://localhost:5015/api/ProyectoInvestigacion/{proyecto.Id}", proyecto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarProyectoAsync(int proyectoId)
    {
        var response = await _http.DeleteAsync($"http://localhost:5015/api/ProyectoInvestigacion/{proyectoId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EditarPublicacionAsync(PublicacionAcademicaDto publicacion)
    {
        var response = await _http.PutAsJsonAsync($"http://localhost:5015/api/PublicacionAcademica/{publicacion.Id}", publicacion);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> EliminarPublicacionAsync(int publicacionId)
    {
        var response = await _http.DeleteAsync($"http://localhost:5015/api/PublicacionAcademica/{publicacionId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SolicitarAvanceRangoAsync(SolicitudAvanceRangoDto solicitud)
    {
        var response = await _http.PostAsJsonAsync("http://localhost:5015/api/SolicitudAvanceRango", solicitud);
        return response.IsSuccessStatusCode;
    }

    public async Task<LoginResponse?> GetLoginResponseAsync()
    {
        return await _localStorageService.ObtenerObjetoAsync<LoginResponse>("loginResponse");
    }

    public async Task<int?> ObtenerNivelAcademicoIdPorNombreAsync(string nombre)
    {
        var response = await _http.GetAsync("http://localhost:5015/api/NivelAcademico");
        if (!response.IsSuccessStatusCode)
            return null;
        var niveles = await response.Content.ReadFromJsonAsync<List<NivelAcademicoDto>>();
        var nivel = niveles?.FirstOrDefault(n => n.Nombre.Trim().Equals(nombre.Trim(), StringComparison.OrdinalIgnoreCase));
        return nivel?.Id;
    }

    public async Task<List<SolicitudAvanceRangoDto>?> ObtenerSolicitudesAvancePorDocenteIdAsync(int docenteId)
    {
        var response = await _http.GetAsync($"http://localhost:5015/api/SolicitudAvanceRango");
        if (!response.IsSuccessStatusCode)
            return null;
        var solicitudes = await response.Content.ReadFromJsonAsync<List<SolicitudAvanceRangoDto>>();
        return solicitudes?.Where(s => s.DocenteId == docenteId).ToList();
    }

    // Métodos para Lista de Verificación
    public async Task<List<ListaVerificacionDto>> GetListaVerificacionPorNivel(int nivelAcademicoId)
    {
        try
        {
            var response = await _http.GetAsync($"http://localhost:5015/api/ListaVerificacion/nivel/{nivelAcademicoId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ListaVerificacionDto>>(json, _jsonOptions) ?? new List<ListaVerificacionDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener lista de verificación: {ex.Message}");
        }
        return new List<ListaVerificacionDto>();
    }

    public async Task<List<VerificacionDocumentosDto>> GetVerificacionDocumentos(int solicitudId)
    {
        try
        {
            var response = await _http.GetAsync($"http://localhost:5015/api/VerificacionDocumentos/solicitud/{solicitudId}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<VerificacionDocumentosDto>>(json, _jsonOptions) ?? new List<VerificacionDocumentosDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener verificación de documentos: {ex.Message}");
        }
        return new List<VerificacionDocumentosDto>();
    }

    public async Task<bool> ActualizarVerificacionDocumento(int verificacionId, bool verificado, string observaciones)
    {
        try
        {
            var data = new { verificado, observaciones, fechaVerificacion = DateTime.Now };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"http://localhost:5015/api/VerificacionDocumentos/{verificacionId}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al actualizar verificación: {ex.Message}");
            return false;
        }
    }

    // Métodos para Apelaciones
    public async Task<bool> CrearApelacion(CrearApelacionDto apelacion)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(apelacion), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("http://localhost:5015/api/ApelacionPromocion", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al crear apelación: {ex.Message}");
            return false;
        }
    }

    public async Task<List<ApelacionPromocionDto>> GetApelacionesPendientes()
    {
        try
        {
            var response = await _http.GetAsync("http://localhost:5015/api/ApelacionPromocion/pendientes");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ApelacionPromocionDto>>(json, _jsonOptions) ?? new List<ApelacionPromocionDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener apelaciones pendientes: {ex.Message}");
        }
        return new List<ApelacionPromocionDto>();
    }

    public async Task<bool> ResolverApelacion(int apelacionId, string respuesta, bool aprobada)
    {
        try
        {
            var data = new { respuestaComision = respuesta, estado = aprobada ? "APROBADA" : "RECHAZADA", fechaRespuesta = DateTime.Now, resuelto = true };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"http://localhost:5015/api/ApelacionPromocion/{apelacionId}", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al resolver apelación: {ex.Message}");
            return false;
        }
    }

    // Métodos para Seguimiento de Plazos
    public async Task<List<SeguimientoPlazoDto>> GetPlazosVencidos()
    {
        try
        {
            var response = await _http.GetAsync("http://localhost:5015/api/SeguimientoPlazos/vencidos");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SeguimientoPlazoDto>>(json, _jsonOptions) ?? new List<SeguimientoPlazoDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener plazos vencidos: {ex.Message}");
        }
        return new List<SeguimientoPlazoDto>();
    }

    public async Task<List<SeguimientoPlazoDto>> GetPlazosPorVencer()
    {
        try
        {
            var response = await _http.GetAsync("http://localhost:5015/api/SeguimientoPlazos/por-vencer");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<SeguimientoPlazoDto>>(json, _jsonOptions) ?? new List<SeguimientoPlazoDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener plazos por vencer: {ex.Message}");
        }
        return new List<SeguimientoPlazoDto>();
    }

    // Métodos para Comisión Académica
    public async Task<List<ComisionAcademicaDto>> GetMiembrosComision()
    {
        try
        {
            var response = await _http.GetAsync("http://localhost:5015/api/ComisionAcademica");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<ComisionAcademicaDto>>(json, _jsonOptions) ?? new List<ComisionAcademicaDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener miembros de comisión: {ex.Message}");
        }
        return new List<ComisionAcademicaDto>();
    }

    // Métodos para Informes Finales
    public async Task<bool> GenerarInformeFinal(int solicitudId)
    {
        try
        {
            var data = new { solicitudId };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PostAsync("http://localhost:5015/api/InformeFinalPromocion/generar", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al generar informe final: {ex.Message}");
            return false;
        }
    }

    public async Task<List<InformeFinalPromocionDto>> GetInformesFinales()
    {
        try
        {
            var response = await _http.GetAsync("http://localhost:5015/api/InformeFinalPromocion");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<InformeFinalPromocionDto>>(json, _jsonOptions) ?? new List<InformeFinalPromocionDto>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener informes finales: {ex.Message}");
        }
        return new List<InformeFinalPromocionDto>();
    }

    public async Task<bool> EnviarInformeAConsejo(int informeId)
    {
        try
        {
            var data = new { estado = "ENVIADO_CONSEJO", fechaEnvioConsejo = DateTime.Now };
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var response = await _http.PutAsync($"http://localhost:5015/api/InformeFinalPromocion/{informeId}/enviar-consejo", content);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al enviar informe al consejo: {ex.Message}");
            return false;
        }
    }

}
