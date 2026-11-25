using SGAT.Application.DTOs.Producto;
using SGAT.Application.DTOs.Paginacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> GetTodosAsync();
        Task<PagedResult<ProductoDto>> GetPageAsync(
            string? termino, string filtro, int pagina, int tamanoPagina);
        Task<ProductoDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductoDto>> GetStockBajoAsync();
        Task<IEnumerable<CategoriaProductoDto>> GetCategoriasAsync();
        Task<IEnumerable<MarcaDto>> GetMarcasAsync();
        Task<ProductoDto> CrearAsync(CrearProductoDto dto);
        Task<ProductoDto> ActualizarAsync(int id, CrearProductoDto dto);
        Task<bool> ActivarDesactivarAsync(int id);
    }
}
