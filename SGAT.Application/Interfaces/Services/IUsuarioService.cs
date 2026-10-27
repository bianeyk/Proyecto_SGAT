using Microsoft.AspNetCore.Identity;
using SGAT.Application.DTOs.Usuario;


namespace SGAT.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<IReadOnlyList<UsuarioListDto>> ObtenerTodosAsync();

        Task<UsuarioDetalleDto?> ObtenerPorIdAsync(string id);

        Task<IdentityResult> CrearAsync(UsuarioCreateDto dto);

        Task<IdentityResult> ActualizarAsync(UsuarioEditDto dto);

        Task<IdentityResult> ActivarAsync(string id);

        Task<IdentityResult> InactivarAsync(string id);
    }
}
