using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Infrastructure.Data;
using SGAT.Infrastructure.Repositories;

namespace SGAT.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IProductoRepository, ProductoRepository>();
            services.AddScoped<IOrdenServicioRepository, OrdenServicioRepository>();
            services.AddScoped<IFacturaRepository, FacturaRepository>();
            services.AddScoped<ITecnicoRepository, TecnicoRepository>();

            return services;
        }
    }
}