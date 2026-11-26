using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Infrastructure.Data;
using SGAT.Infrastructure.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Infrastructure.Services;
using SGAT.Infrastructure.Settings;
using SGAT.Application.Interfaces.Transactions;
using SGAT.Infrastructure.Data.Transactions;

namespace SGAT.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            // Registro de servicios de transacciones.
            services.AddScoped<ITransactionManager, EfTransactionManager>();

            // Gestión de bloqueos de SQL Server.
            services.AddScoped<IAppLockManager, EfAppLockManager>();

            // Configuración y validación del correo electrónico.
            services.AddOptions<EmailSettings>().Bind(configuration.GetSection("EmailSettings"))
                .ValidateDataAnnotations()
                .Validate(
                    settings => Enum.IsDefined(settings.SecurityOption),
                    "La opción de seguridad SMTP no es válida.")
                .ValidateOnStart();

            // Servicio de envío de correos.
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IMarcaRepository, MarcaRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IRolRepository, RolRepository>();
            services.AddScoped<ICategoriaProductoRepository, CategoriaProductoRepository>();
            services.AddScoped<IOrdenServicioRepository, OrdenServicioRepository>();
            services.AddScoped<IFacturaRepository, FacturaRepository>();
            services.AddScoped<ITecnicoRepository, TecnicoRepository>();

            return services;
        }
    }
}
