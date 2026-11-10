using SGAT.Application.DTOs.Producto;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using SGAT.SGAT.Domain.Enums;

namespace SGAT.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepo;

        public ProductoService(IProductoRepository productoRepo)
        {
            _productoRepo = productoRepo;
        }

        public async Task<IEnumerable<ProductoDto>> GetTodosAsync()
        {
            var productos = await _productoRepo.GetAllConDetalleAsync();
            return productos.Select(MapToDto);
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var p = await _productoRepo.GetByIdConDetalleAsync(id);
            return p is null ? null : MapToDto(p);
        }

        public async Task<IEnumerable<ProductoDto>> GetStockBajoAsync()
        {
            var productos = await _productoRepo.GetStockBajoAsync();
            return productos.Select(MapToDto);
        }

        public async Task<IEnumerable<CategoriaProductoDto>> GetCategoriasAsync()
        {
            var categorias = await _productoRepo.GetCategoriasActivasAsync();
            return categorias.Select(c => new CategoriaProductoDto
            {
                IdCategoriaProducto = c.IdCategoriaProducto,
                Nombre = c.Nombre
            });
        }

        public async Task<IEnumerable<MarcaDto>> GetMarcasAsync()
        {
            var marcas = await _productoRepo.GetMarcasActivasAsync();
            return marcas.Select(m => new MarcaDto
            {
                IdMarca = m.IdMarca,
                Nombre = m.Nombre
            });
        }

        public async Task<ProductoDto> CrearAsync(CrearProductoDto dto)
        {
            Validar(dto);
            await ValidarCatalogosAsync(dto);

            var codigo = Normalizar(dto.Codigo);
            if (codigo is not null && await _productoRepo.CodigoExisteAsync(codigo))
                throw new InvalidOperationException($"El código {codigo} ya está en uso.");

            var producto = new Producto
            {
                IdCategoriaProducto = dto.IdCategoriaProducto,
                IdMarca = dto.IdMarca,
                Codigo = codigo,
                Nombre = dto.Nombre.Trim(),
                Descripcion = dto.Descripcion,
                Condicion = ParseCondicion(dto.Condicion),
                PrecioVenta = dto.PrecioVenta,
                PrecioDescuento = dto.PrecioDescuento,
                StockMinimo = dto.StockMinimo,
                Activo = dto.Activo
            };

            await _productoRepo.AddAsync(producto);
            await _productoRepo.SaveChangesAsync();
            return MapToDto(producto);
        }

        public async Task<ProductoDto> ActualizarAsync(int id, CrearProductoDto dto)
        {
            Validar(dto);
            await ValidarCatalogosAsync(dto);

            var producto = await _productoRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Producto {id} no encontrado.");

            var codigo = Normalizar(dto.Codigo);
            if (codigo is not null
                && !string.Equals(producto.Codigo, codigo, StringComparison.OrdinalIgnoreCase)
                && await _productoRepo.CodigoExisteAsync(codigo))
            {
                throw new InvalidOperationException($"El código {codigo} ya está en uso.");
            }

            producto.IdCategoriaProducto = dto.IdCategoriaProducto;
            producto.IdMarca = dto.IdMarca;
            producto.Codigo = codigo;
            producto.Nombre = dto.Nombre.Trim();
            producto.Descripcion = dto.Descripcion;
            producto.Condicion = ParseCondicion(dto.Condicion);
            producto.PrecioVenta = dto.PrecioVenta;
            producto.PrecioDescuento = dto.PrecioDescuento;
            producto.StockMinimo = dto.StockMinimo;
            producto.Activo = dto.Activo;

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

        private static void Validar(CrearProductoDto dto)
        {
            if (dto.IdCategoriaProducto <= 0)
                throw new ArgumentException("Selecciona una categoría válida.");

            if (dto.IdMarca <= 0)
                throw new ArgumentException("Selecciona una marca válida.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new ArgumentException("El nombre del producto es obligatorio.");

            if (dto.PrecioVenta <= 0)
                throw new ArgumentException("El precio de venta debe ser mayor que cero.");

            if (dto.PrecioDescuento.HasValue && dto.PrecioDescuento.Value >= dto.PrecioVenta)
                throw new ArgumentException("El precio de descuento debe ser menor que el precio de venta.");

            if (dto.StockMinimo < 0)
                throw new ArgumentException("El stock mínimo no puede ser negativo.");
        }

        private async Task ValidarCatalogosAsync(CrearProductoDto dto)
        {
            if (!await _productoRepo.CategoriaExisteAsync(dto.IdCategoriaProducto))
                throw new ArgumentException("La categoría seleccionada no existe o está inactiva.");

            if (!await _productoRepo.MarcaExisteAsync(dto.IdMarca))
                throw new ArgumentException("La marca seleccionada no existe o está inactiva.");
        }

        private static CondicionProductoEnum ParseCondicion(string valor)
        {
            if (!Enum.TryParse<CondicionProductoEnum>(valor, ignoreCase: true, out var condicion)
                || !Enum.IsDefined(condicion))
            {
                throw new ArgumentException("La condición del producto no es válida.");
            }

            return condicion;
        }

        private static string? Normalizar(string? codigo)
            => string.IsNullOrWhiteSpace(codigo) ? null : codigo.Trim();

        private static ProductoDto MapToDto(Producto p) => new()
        {
            IdProducto = p.IdProducto,
            IdCategoriaProducto = p.IdCategoriaProducto,
            IdMarca = p.IdMarca,
            Codigo = p.Codigo ?? string.Empty,
            Nombre = p.Nombre,
            Descripcion = p.Descripcion,
            Categoria = p.CategoriaProducto?.Nombre ?? string.Empty,
            Marca = p.Marca?.Nombre ?? string.Empty,
            Condicion = p.Condicion.ToString(),
            PrecioVenta = p.PrecioVenta,
            PrecioDescuento = p.PrecioDescuento,
            StockActual = p.StockActual,
            StockMinimo = p.StockMinimo,
            StockReservado = p.StockReservado,
            Activo = p.Activo,
            CreatedAt = p.CreatedAt
        };
    }
}
