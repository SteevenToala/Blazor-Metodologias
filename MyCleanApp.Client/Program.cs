using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyCleanApp.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:3000/") });
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.TeacherService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.CourseService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.ResearchProjectService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.AcademicPaperService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.PromotionRequirementService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.TeacherEvaluationService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.PromotionProgressService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.AdminDashboardService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.PromotionRequestService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.ReportService>();
builder.Services.AddScoped<MyCleanApp.Client.Services.UserService>();

await builder.Build().RunAsync();
