using SGAT.Domain.Entities;
using SGAT.Application.DTOs.Paginacion;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface ICategoriaProductoRepository : IGenericRepository<CategoriaProducto>
    {
        Task<CategoriaProducto?> GetByNombreAsync(string nombre);

        Task<bool> NombreExisteAsync(string nombre, int? excluirId = null);
        Task<PagedResult<CategoriaProducto>> GetPageAsync(string? termino, int pagina, int tamanoPagina);
    }
}
