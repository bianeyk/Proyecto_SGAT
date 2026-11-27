using SGAT.Application.DTOs.Equipo;
using SGAT.Application.DTOs.Paginacion;

namespace SGAT.Application.Interfaces.Services;

public interface IEquipoService
{
    Task<PagedResult<EquipoDto>> GetPageAsync(string? termino, string estado, int pagina, int tamano);
    Task<EquipoDto?> GetByIdAsync(int id);
    Task<int> CrearAsync(GuardarEquipoDto dto, string idUsuario);
    Task ActualizarAsync(int id, GuardarEquipoDto dto);
    Task CambiarEstadoAsync(int id, bool activo, string version);
    Task<IReadOnlyList<OpcionEquipoDto>> BuscarClientesAsync(string termino);
    Task<IReadOnlyList<OpcionEquipoDto>> GetMarcasAsync(int? actual = null);
    Task<IReadOnlyList<OpcionEquipoDto>> GetCategoriasAsync(int? actual = null);
}
