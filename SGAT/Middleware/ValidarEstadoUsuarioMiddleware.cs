using Microsoft.AspNetCore.Identity;
using SGAT.Domain.Entities;

namespace SGAT.Web.Middleware
{
    public class ValidarEstadoUsuarioMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidarEstadoUsuarioMiddleware(RequestDelegate next)
        {
            _next = next;
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
                    await signInManager.SignOutAsync();

                    context.Response.Redirect(
                        "/Account/Login?inactiva=true");

                    return;
                }

                // Comprobar que el rol exista y esté activo.
                var rol = await roleManager.FindByNameAsync(roles[0]);

                if (rol == null || !rol.Activo)
                {
                    await signInManager.SignOutAsync();

                    context.Response.Redirect(
                        "/Account/Login?inactiva=true");

                    return;
                }
            }

            // Si las comprobaciones pasan, continuar la solicitud.
            await _next(context);
        }
    }
}