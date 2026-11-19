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

            foreach (var nombreRol in roles)
            {
                // 1. Comprobar si el rol ya existe.
                var rolExistente = await roleManager.FindByNameAsync(nombreRol);

                if (rolExistente != null)
                {
                    // No modificar ni reactivar roles existentes.
                    continue;
                }

                // 2. Crear el rol inicial activo.
                var nuevoRol = new ApplicationRole
                {
                    Name = nombreRol,
                    Activo = true
                };

                var resultado = await roleManager.CreateAsync(nuevoRol);

                // 3. Comprobar el resultado de Identity.
                if (!resultado.Succeeded)
                {
                    var errores = string.Join(
                        ", ",
                        resultado.Errors.Select(e => e.Description));

                    throw new InvalidOperationException(
                        $"Error creando el rol {nombreRol}: {errores}");
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

    // 1. Validar el correo del administrador.
    if (string.IsNullOrWhiteSpace(email))
    {
        throw new InvalidOperationException(
            "No se configuró el correo del administrador.");
    }

    email = email.Trim();

    // 2. Comprobar que el rol Administrador exista y esté activo.
    var rol = await roleManager.FindByNameAsync(nombreRol);

    if (rol == null || !rol.Activo)
    {
        throw new InvalidOperationException(
            "El rol Administrador no existe o está inactivo.");
    }

    // 3. Buscar si el administrador ya existe.
    var usuario = await userManager.FindByEmailAsync(email);

    // 4. Crear el administrador únicamente si no existe.
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

        // 5. Asignar Administrador solamente a la cuenta recién creada.
        var resultadoRol = await userManager.AddToRoleAsync(
            usuario,
            nombreRol);

        if (!resultadoRol.Succeeded)
        {
            // Evitar dejar una cuenta inicial sin el rol requerido.
            var resultadoEliminar = await userManager.DeleteAsync(usuario);

            if (!resultadoEliminar.Succeeded)
            {
                throw new InvalidOperationException(
                    "No se pudo asignar el rol Administrador ni eliminar " +
                    "la cuenta inicial incompleta.");
            }

            throw new InvalidOperationException(
                "No se pudo asignar el rol Administrador.");
        }

        return;
    }

    // 6. No modificar usuarios existentes que estén inactivos.
    if (!usuario.Activo)
    {
        return;
    }

    // 7. Comprobar los roles del usuario existente.
    var rolesUsuario = await userManager.GetRolesAsync(usuario);

    // 8. Si tiene exactamente el rol Administrador, no hacer cambios.
    if (rolesUsuario.Count == 1 &&
        string.Equals(
            rolesUsuario[0],
            nombreRol,
            StringComparison.OrdinalIgnoreCase))
    {
        return;
    }

    // 9. No asignar privilegios automáticamente a cuentas existentes.
    throw new InvalidOperationException(
        "El usuario inicial existente tiene una asignación de roles " +
        "incompatible. Debe revisarse manualmente.");
}



    }
}