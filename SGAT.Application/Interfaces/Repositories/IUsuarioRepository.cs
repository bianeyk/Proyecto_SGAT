using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.DTOs.Usuario;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IUsuarioRepository
    {
        Task<PagedResult<UsuarioListDto>> ObtenerPaginadosAsync(
            UsuarioFiltroDto filtro);
    }
}