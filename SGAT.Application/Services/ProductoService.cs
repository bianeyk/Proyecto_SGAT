using SGAT.Application.DTOs.Producto;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Services
{
    public class ProductoService:IProductoService
    {
        private readonly IProductoRepository _productoRepo;

        public ProductoService(IProductoRepository productoRepo)
        {
            _productoRepo = productoRepo;
        }

        public async Task<IEnumerable<ProductoDto>> GetTodosAsync()
        {
            var productos = await _productoRepo.GetAllAsync();
            return productos.Select(MapToDto);
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var p = await _productoRepo.GetByIdAsync(id);
            return p is null ? null : MapToDto(p);
        }

        public async Task<IEnumerable<ProductoDto>> GetStockBajoAsync()
        {
            var productos = await _productoRepo.GetStockBajoAsync();
            return productos.Select(MapToDto);
        }

        public async Task<ProductoDto> CrearAsync(CrearProductoDto dto)
        {
            if (dto.Codigo is not null && await _productoRepo.CodigoExisteAsync(dto.Codigo))
                throw new InvalidOperationException($"El código {dto.Codigo} ya está en uso.");

            var producto = new Producto
            {
                IdCategoriaProducto = dto.IdCategoriaProducto,
                Codigo = dto.Codigo,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Condicion = Enum.Parse<CondicionProductoEnum>(dto.Condicion),
                PrecioVenta = dto.PrecioVenta,
                PrecioDescuento = dto.PrecioDescuento,
                StockMinimo = dto.StockMinimo
            };

            await _productoRepo.AddAsync(producto);
            await _productoRepo.SaveChangesAsync();
            return MapToDto(producto);
        }

        public async Task<ProductoDto> ActualizarAsync(int id, CrearProductoDto dto)
        {
            var producto = await _productoRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Producto {id} no encontrado.");

            producto.Nombre = dto.Nombre;
            producto.Descripcion = dto.Descripcion;
            producto.Condicion = Enum.Parse<CondicionProductoEnum>(dto.Condicion);
            producto.PrecioVenta = dto.PrecioVenta;
            producto.PrecioDescuento = dto.PrecioDescuento;
            producto.StockMinimo = dto.StockMinimo;
            producto.IdCategoriaProducto = dto.IdCategoriaProducto;

            _productoRepo.Update(producto);
            await _productoRepo.SaveChangesAsync();
            return MapToDto(producto);
        }

        public async Task<bool> ActivarDesactivarAsync(int id)
        {
            var producto = await _productoRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Producto {id} no encontrado.");

            producto.Activo = !producto.Activo;
            _productoRepo.Update(producto);
            return await _productoRepo.SaveChangesAsync();
        }

        private static ProductoDto MapToDto(Producto p) => new()
        {
            IdProducto = p.IdProducto,
            Codigo = p.Codigo ?? string.Empty,
            Nombre = p.Nombre,
            Condicion = p.Condicion.ToString(),
            PrecioVenta = p.PrecioVenta,
            StockActual = p.StockActual,
            StockMinimo = p.StockMinimo,
            Activo = p.Activo
        };
    }
}
