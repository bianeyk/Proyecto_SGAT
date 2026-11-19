
using Microsoft.AspNetCore.Identity;
using SGAT.Domain.Entities;

namespace SGAT.Web.Middleware
{
    public class ValidarEstadoUsuarioMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ValidarEstadoUsuarioMiddleware> _logger;

        public ValidarEstadoUsuarioMiddleware(
            RequestDelegate next,
            ILogger<ValidarEstadoUsuarioMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            SignInManager<ApplicationUser> signInManager)
        {
            // Solo revisar solicitudes de usuarios autenticados.
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var usuario = await userManager.GetUserAsync(context.User);

                // Comprobar que el usuario exista y esté activo.
                if (usuario == null || !usuario.Activo)
                {
                    _logger.LogWarning(
                        "Se rechazó una solicitud por usuario inexistente o inactivo.");

                    await signInManager.SignOutAsync();

                    context.Response.Redirect(
                        "/Account/Login?inactiva=true");

                    return;
                }

                // Obtener los roles asignados al usuario.
                var roles = await userManager.GetRolesAsync(usuario);

                // Este sistema espera exactamente un rol por usuario.
                if (roles.Count != 1)
                {
                    _logger.LogWarning(
                        "Se rechazó una solicitud del usuario {UsuarioId} por tener {CantidadRoles} roles.",
                        usuario.Id,
                        roles.Count);

                    await signInManager.SignOutAsync();

                    context.Response.Redirect(
                        "/Account/Login?permisos=true");

                    return;
                }

                // Comprobar que el rol exista y esté activo.
                var rol = await roleManager.FindByNameAsync(roles[0]);

                if (rol == null || !rol.Activo)
                {
                    _logger.LogWarning(
                        "Se rechazó una solicitud del usuario {UsuarioId} porque su rol no existe o está inactivo.",
                        usuario.Id);

                    await signInManager.SignOutAsync();

                    context.Response.Redirect(
                        "/Account/Login?permisos=true");

                    return;
                }

                // Comprobar que el rol de la sesión coincida
                // con el rol actual de la base de datos.
                if (!context.User.IsInRole(roles[0]))
                {
                    _logger.LogWarning(
                        "Se rechazó una solicitud del usuario {UsuarioId} por discrepancia entre el rol de la sesión y el rol actual.",
                        usuario.Id);

                    await signInManager.SignOutAsync();

                    context.Response.Redirect(
                        "/Account/Login?permisos=true");

                    return;
                }
            }

            // Si las comprobaciones pasan, continuar la solicitud.
            await _next(context);
        }
    }
}
