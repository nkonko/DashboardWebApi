using Microsoft.Extensions.DependencyInjection;
using WebApi.Infrastructure.Services;

namespace WebApi.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Registro automático de servicios usando Scrutor
            services.Scan(scan => scan
                .FromAssemblyOf<DashboardService>()
                .AddClasses(classes => classes.InNamespaceOf<DashboardService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            );
        }
    }
}
