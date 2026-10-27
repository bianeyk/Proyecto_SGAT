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
    UserManager<ApplicationUser> userManager)
        {
            var email = "admin@sgat.com";

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Activo = true
                };

                var result = await userManager.CreateAsync(user, "Admin123*");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Administrador");
                }
            }
        }
    }
}
