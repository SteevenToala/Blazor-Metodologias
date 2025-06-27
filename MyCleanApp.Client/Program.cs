using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyCleanApp.Client;
using MyCleanApp.Client.Services;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:3000/") });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<EvaluacionService>();
builder.Services.AddScoped<DocenteService>();
builder.Services.AddScoped<VerificacionService>();
builder.Services.AddScoped<SolicitudPromocionService>();

// Agregar MudBlazor
builder.Services.AddMudServices();

await builder.Build().RunAsync();
