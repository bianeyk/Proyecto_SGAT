using Microsoft.Extensions.DependencyInjection;
using SGAT.Application.Interfaces.Services;
using SGAT.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IProductoService, ProductoService>();
            //services.AddScoped<IOrdenServicioService, OrdenServicioService>();// Falta implementación
            // services.AddScoped<IFacturaService, FacturaService>(); // Falta implementación
            // services.AddScoped<IAuthService, AuthService>(); // Falta implementación
            // services.AddScoped<IPasswordHasher, PasswordHasher>(); // Falta implementación

            return services;
        }
    }
}