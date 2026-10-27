using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SGAT.Application.DTOs.Rol;
using SGAT.Application.DTOs.Rol.SGAT.Application.DTOs.Rol;
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
                    Nombre = rol.Name!
                });
            }

            return lista;
        }
    }
}
