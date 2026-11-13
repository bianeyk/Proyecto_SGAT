using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Infrastructure.Data;
using SGAT.Infrastructure.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Infrastructure.Services;
using SGAT.Infrastructure.Settings;

namespace SGAT.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            // Configuración del correo electrónico
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            // Servicio de envío de correos
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IOrdenServicioRepository, OrdenServicioRepository>();
            services.AddScoped<IFacturaRepository, FacturaRepository>();
            services.AddScoped<ITecnicoRepository, TecnicoRepository>();

            return services;
        }
    }
}