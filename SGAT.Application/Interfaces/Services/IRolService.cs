using SGAT.Application.DTOs.Rol.SGAT.Application.DTOs.Rol;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Services
{
    public interface IRolService
    {
        Task<IReadOnlyList<RolDto>> ObtenerTodosAsync();
    }
}
