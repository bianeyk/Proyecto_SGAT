using Microsoft.AspNetCore.Identity;
using SGAT.Application.DTOs.Rol;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Services
{
    public interface IRolService
    {
        Task<IReadOnlyList<RolDto>> ObtenerTodosAsync();
        Task<IdentityResult> CrearAsync(RolCreateDto dto);
        Task<IdentityResult> ActivarAsync(string id);
        Task<IdentityResult> InactivarAsync(string id);
    }
}
