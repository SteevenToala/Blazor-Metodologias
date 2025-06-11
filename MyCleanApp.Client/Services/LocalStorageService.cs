using System.Text.Json;
using Microsoft.JSInterop;
using System.Threading.Tasks;

public class LocalStorageService
{
    private readonly IJSRuntime _js;

    public LocalStorageService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task GuardarObjetoAsync<T>(string clave, T valor)
    {
        var json = JsonSerializer.Serialize(valor);
        await _js.InvokeVoidAsync("localStorage.setItem", clave, json);
    }

    public async Task<T?> ObtenerObjetoAsync<T>(string clave)
    {
        var json = await _js.InvokeAsync<string>("localStorage.getItem", clave);
        return string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task EliminarAsync(string clave)
    {
        await _js.InvokeVoidAsync("localStorage.removeItem", clave);
    }

    public async Task LimpiarAsync()
    {
        await _js.InvokeVoidAsync("localStorage.clear");
    }
}
