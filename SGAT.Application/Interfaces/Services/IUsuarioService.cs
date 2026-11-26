using Microsoft.AspNetCore.Identity;
using SGAT.Application.DTOs.Usuario;
using SGAT.Application.DTOs.Paginacion;

namespace SGAT.Application.Interfaces.Services
{
    public interface IUsuarioService
    {
        Task<PagedResult<UsuarioListDto>> ObtenerPaginadosAsync(UsuarioFiltroDto filtro);
        Task<UsuarioDetalleDto?> ObtenerPorIdAsync(string id);

        Task<IdentityResult> CrearAsync(UsuarioCreateDto dto);

        Task<IdentityResult> ActualizarAsync(UsuarioEditDto dto);

        Task<IdentityResult> ActivarAsync(string id);

        Task<IdentityResult> InactivarAsync(string id);

    }
}
