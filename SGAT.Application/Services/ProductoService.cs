using SGAT.Application.DTOs.Producto;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using SGAT.SGAT.Domain.Enums;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;

namespace SGAT.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _productoRepo;
        private const decimal PrecioMaximo = 99_999_999.99m;

        private static readonly Regex CodigoValido =
            new(@"^[A-Za-z0-9\-_.]+$", RegexOptions.Compiled);

        private static readonly Regex TextoProductoValido =
            new(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-.,()/]+$", RegexOptions.Compiled);

        private static readonly Regex DescripcionValida =
            new(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-.,()/&'+]+$", RegexOptions.Compiled);

        public ProductoService(IProductoRepository productoRepo)
        {
            _productoRepo = productoRepo;
        }

        public async Task<ProductoDto?> GetByIdAsync(int id)
        {
            var p = await _productoRepo.GetByIdConDetalleAsync(id);
            return p is null ? null : MapToDto(p);
        }

        public async Task<PagedResult<ProductoDto>> GetPageAsync(
            string? termino, string filtro, int pagina, int tamanoPagina)
        {
            var result = await _productoRepo.GetPageConDetalleAsync(
                termino, filtro, pagina, tamanoPagina);
            return new PagedResult<ProductoDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                ActiveCount = result.ActiveCount,
                InactiveCount = result.InactiveCount,
                LowStockCount = result.LowStockCount
            };
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
                Nombre = TextNormalization.CollapseWhitespace(dto.Nombre),
                Descripcion = NormalizarDescripcion(dto.Descripcion),
                Condicion = ParseCondicion(dto.Condicion),
                PrecioVenta = dto.PrecioVenta,
                PrecioDescuento = dto.PrecioDescuento,
                StockMinimo = dto.StockMinimo,
                Activo = dto.Activo
            };

            await _productoRepo.AddAsync(producto);
            try
            {
                await _productoRepo.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DatabaseErrorHelper.IsUniqueConstraintViolation(ex))
            {
                throw new InvalidOperationException(
                    $"El código {codigo} ya está en uso.");
            }

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
            producto.Nombre = TextNormalization.CollapseWhitespace(dto.Nombre);
            producto.Descripcion = NormalizarDescripcion(dto.Descripcion);
            producto.Condicion = ParseCondicion(dto.Condicion);
            producto.PrecioVenta = dto.PrecioVenta;
            producto.PrecioDescuento = dto.PrecioDescuento;
            producto.StockMinimo = dto.StockMinimo;
            producto.Activo = dto.Activo;

            _productoRepo.Update(producto);
            try
            {
                await _productoRepo.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DatabaseErrorHelper.IsUniqueConstraintViolation(ex))
            {
                throw new InvalidOperationException(
                    $"El código {codigo} ya está en uso.");
            }

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

            var nombre = TextNormalization.CollapseWhitespace(dto.Nombre);
            if (nombre.Length > 150)
                throw new ArgumentException("El nombre del producto no puede superar los 150 caracteres.");

            if (!TextoProductoValido.IsMatch(nombre))
                throw new ArgumentException("El nombre del producto contiene caracteres no permitidos.");

            var codigo = Normalizar(dto.Codigo);
            if (codigo is not null)
            {
                if (codigo.Length > 100)
                    throw new ArgumentException("El código no puede superar los 100 caracteres.");

                if (!CodigoValido.IsMatch(codigo))
                    throw new ArgumentException(
                        "El código solo puede contener letras, números, guiones, puntos y guiones bajos.");
            }

            if (dto.Descripcion is not null)
            {
                var descripcion = TextNormalization.CollapseWhitespace(dto.Descripcion);
                if (descripcion.Length > 500)
                    throw new ArgumentException("La descripción no puede superar los 500 caracteres.");

                if (descripcion.Length > 0 && !DescripcionValida.IsMatch(descripcion))
                    throw new ArgumentException("La descripción contiene caracteres no permitidos.");
            }

            if (dto.PrecioVenta <= 0 || dto.PrecioVenta > PrecioMaximo)
                throw new ArgumentException(
                    "El precio de venta debe ser mayor que cero y no superar 99.999.999,99.");

            if (dto.PrecioDescuento.HasValue)
            {
                if (dto.PrecioDescuento.Value < 0
                    || dto.PrecioDescuento.Value > PrecioMaximo)
                {
                    throw new ArgumentException(
                        "El precio con descuento debe estar entre cero y 99.999.999,99.");
                }

                if (dto.PrecioDescuento.Value >= dto.PrecioVenta)
                    throw new ArgumentException(
                        "El precio de descuento debe ser menor que el precio de venta.");
            }

            if (dto.StockMinimo < 0)
                throw new ArgumentException("El stock mínimo no puede ser negativo.");

            ParseCondicion(dto.Condicion);
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

        private static string? NormalizarDescripcion(string? descripcion)
            => TextNormalization.CollapseOptional(descripcion);

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
