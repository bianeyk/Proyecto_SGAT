using SGAT.Application.DTOs.Equipo;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Domain.Entities;

namespace SGAT.Application.Interfaces.Repositories;

public interface ICategoriaDispositivoRepository : IGenericRepository<CategoriaDispositivo>
{
    Task<PagedResult<CategoriaDispositivo>> GetPageAsync(string? termino, int pagina, int tamanoPagina);
    Task<bool> NombreExisteAsync(string nombre, int? excluirId = null);
}
