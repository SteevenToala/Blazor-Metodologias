using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using MyCleanApp.Infrastructure.Persistence;
using MyCleanApp.Infrastructure.Services;

namespace MyCleanApp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Registrar servicios del workflow de promoción
            services.AddScoped<IPromocionWorkflowService, PromocionWorkflowService>();
            services.AddScoped<INotificacionService, NotificacionService>();
            
            // Registrar servicio de correo electrónico
            services.AddHttpClient<IEmailService, EmailService>();

            return services;
        }
    }
}
