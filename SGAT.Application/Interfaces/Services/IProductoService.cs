using SGAT.Application.DTOs.Producto;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Services
{
    public interface IProductoService
    {
        Task<IEnumerable<ProductoDto>> GetTodosAsync();
        Task<ProductoDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductoDto>> GetStockBajoAsync();
        Task<ProductoDto> CrearAsync(CrearProductoDto dto);
        Task<ProductoDto> ActualizarAsync(int id, CrearProductoDto dto);
        Task<bool> ActivarDesactivarAsync(int id);
    }
}
