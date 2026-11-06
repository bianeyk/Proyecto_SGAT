using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGAT.Application.DTOs.Rol;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;

namespace SGAT.Application.Services
{
    public class RolService : IRolService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IReadOnlyList<RolDto>> ObtenerTodosAsync()
        {
            var roles = await _roleManager.Roles
                .OrderBy(r => r.Name)
                .ToListAsync();

            var lista = new List<RolDto>();

            foreach (var rol in roles)
            {
                lista.Add(new RolDto
                {
                    Id = rol.Id,
                    Nombre = rol.Name!,
                    Activo = rol.Activo
                });
            }

            return lista;
        }
        public async Task<IdentityResult> CrearAsync(RolCreateDto dto)
        {
            // 1. Limpiar espacios al inicio y al final.
            var nombre = dto.Nombre.Trim();

            // 2. Comprobar si ya existe un rol con ese nombre.
            var rolExistente = await _roleManager.FindByNameAsync(nombre);

            if (rolExistente != null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "Ya existe un rol con ese nombre."
                    });
            }

            // 3. Crear la entidad ApplicationRole.
            var nuevoRol = new ApplicationRole
            {
                Name = nombre,
                Activo = true
            };

            // 4. Guardar el rol mediante Identity.
            return await _roleManager.CreateAsync(nuevoRol);
        }

        public async Task<IdentityResult> ActivarAsync(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);

            if (rol == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "El rol no existe."
                    });
            }

            rol.Activo = true;

            return await _roleManager.UpdateAsync(rol);
        }

        public async Task<IdentityResult> InactivarAsync(string id)
        {
            var rol = await _roleManager.FindByIdAsync(id);

            if (rol == null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "El rol no existe."
                    });
            }

            if (rol.Name == "Administrador")
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description = "El rol Administrador no puede inactivarse."
                    });
            }

            rol.Activo = false;

            return await _roleManager.UpdateAsync(rol);
        }
    }
}
