using SGAT.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginDto dto);
        Task LogoutAsync(int idUsuario);
    }
}
