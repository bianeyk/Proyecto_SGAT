using SGAT.Domain.Entities;
using SGAT.Application.DTOs.Paginacion;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IMarcaRepository : IGenericRepository<Marca>
    {
        Task<Marca?> GetByNombreAsync(string nombre);

        Task<bool> NombreExisteAsync(string nombre, int? excluirId = null);
        Task<PagedResult<Marca>> GetPageAsync(string? termino, int pagina, int tamanoPagina);
    }
}
