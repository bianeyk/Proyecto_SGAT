using SGAT.Application.DTOs.Producto;
using SGAT.Application.DTOs.Paginacion;

namespace SGAT.Application.Interfaces.Services
{
    public interface ICategoriaProductoService
    {
        Task<IEnumerable<CategoriaProductoDto>> GetTodasAsync();

        Task<CategoriaProductoDto?> GetByIdAsync(int id);

        Task<IEnumerable<CategoriaProductoDto>> BuscarAsync(string termino);
        Task<PagedResult<CategoriaProductoDto>> GetPageAsync(string? termino, int pagina, int tamanoPagina);

        Task<CategoriaProductoDto> CrearAsync(CategoriaProductoDto dto);

        Task<CategoriaProductoDto> ActualizarAsync(int id, CategoriaProductoDto dto);

        Task<bool> ActivarDesactivarAsync(int id);
    }
}
