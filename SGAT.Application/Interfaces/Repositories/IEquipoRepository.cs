using SGAT.Application.DTOs.Equipo;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Domain.Entities;

namespace SGAT.Application.Interfaces.Repositories;

public interface IEquipoRepository : IGenericRepository<Equipo>
{
    Task<PagedResult<EquipoDto>> GetPageAsync(string? termino, string estado, int pagina, int tamano);
    Task<EquipoDto?> GetDetalleAsync(int id);
    Task<bool> SerialExisteAsync(string serial, int? exceptoId);
    Task<bool> TieneOrdenesAsync(int id);
    Task<bool> RelacionesValidasAsync(ValidarRelacionesEquipoDto relaciones);
    Task<IReadOnlyList<OpcionEquipoDto>> BuscarClientesAsync(string termino);
    Task<IReadOnlyList<OpcionEquipoDto>> GetMarcasAsync(int? actual);
    Task<IReadOnlyList<OpcionEquipoDto>> GetCategoriasAsync(int? actual);
}
