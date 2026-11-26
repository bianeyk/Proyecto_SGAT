using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.DTOs.Rol;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IRolRepository
    {
        Task<PagedResult<RolDto>> ObtenerPaginadosAsync(
            RolFiltroDto filtro);
    }
}