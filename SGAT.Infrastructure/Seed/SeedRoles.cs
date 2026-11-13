using Microsoft.AspNetCore.Identity;
using SGAT.Domain.Entities;

namespace SGAT.Infrastructure.Seed
{
    public class SeedRoles
    {
        //Creacion de Roles Fijos
        public static async Task SeedAsync(
        RoleManager<ApplicationRole> roleManager)
        {
            string[] roles =
            {
            "Administrador",
            "Gerente",
            "Tecnico"
        };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new ApplicationRole
                        {
                            Name = role
                        });
                }
            }
        }
        //Creacion de Rol Administrador Inicial


        public static async Task SeedAdminAsync(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            string? email,
            string? password)
        {
            const string nombreRol = "Administrador";

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidOperationException(
                    "No se configuró el correo del administrador.");
            }

            var rol = await roleManager.FindByNameAsync(nombreRol);

            if (rol == null || !rol.Activo)
            {
                throw new InvalidOperationException(
                    "El rol Administrador no existe o está inactivo.");
            }

            email = email.Trim();

            var usuario = await userManager.FindByEmailAsync(email);

            if (usuario == null)
            {
                if (string.IsNullOrWhiteSpace(password))
                {
                    throw new InvalidOperationException(
                        "No se configuró la contraseña del administrador.");
                }

                usuario = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Activo = true
                };

                var resultado = await userManager.CreateAsync(
                    usuario,
                    password);

                if (!resultado.Succeeded)
                {
                    var errores = string.Join(
                        ", ",
                        resultado.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Error creando administrador: {errores}");
                }
            }

            // No modificar usuarios inactivos.
            if (!usuario.Activo)
            {
                return;
            }

            var rolesUsuario = await userManager.GetRolesAsync(usuario);

            if (rolesUsuario.Count == 0)
            {
                var resultadoRol = await userManager.AddToRoleAsync(
                    usuario,
                    nombreRol);

                if (!resultadoRol.Succeeded)
                {
                    throw new InvalidOperationException(
                        "No se pudo asignar el rol Administrador.");
                }

                return;
            }

            if (rolesUsuario.Count == 1 &&
                string.Equals(
                    rolesUsuario[0],
                    nombreRol,
                    StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            throw new InvalidOperationException(
                "El usuario inicial tiene una asignación de roles incompatible. " +
                "Debe revisarse manualmente.");
        }


    }
}