using SGAT.Application.DTOs.Producto;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace SGAT.Application.Services
{
    public class CategoriaProductoService : ICategoriaProductoService
    {
        private readonly ICategoriaProductoRepository _categoriaRepo;

        private static readonly Regex TextoValido =
            new(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-.,()/&'+]+$",
                RegexOptions.Compiled);

        public CategoriaProductoService(ICategoriaProductoRepository categoriaRepo)
        {
            _categoriaRepo = categoriaRepo;
        }

        public async Task<CategoriaProductoDto?> GetByIdAsync(int id)
        {
            var categoria = await _categoriaRepo.GetByIdAsync(id);
            return categoria is null ? null : MapToDto(categoria);
        }

        public async Task<PagedResult<CategoriaProductoDto>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            var result = await _categoriaRepo.GetPageAsync(termino, pagina, tamanoPagina);
            return new PagedResult<CategoriaProductoDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                ActiveCount = result.ActiveCount,
                InactiveCount = result.InactiveCount
            };
        }

        public async Task<CategoriaProductoDto> CrearAsync(CategoriaProductoDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var nombre = NormalizarNombre(dto.Nombre);
            var descripcion = NormalizarDescripcion(dto.Descripcion);

            if (await _categoriaRepo.NombreExisteAsync(nombre))
                throw new InvalidOperationException($"Ya existe una categoría con el nombre {nombre}.");

            var categoria = new CategoriaProducto
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Activo = dto.Activo
            };

            await _categoriaRepo.AddAsync(categoria);
            try
            {
                await _categoriaRepo.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DatabaseErrorHelper.IsUniqueConstraintViolation(ex))
            {
                throw new InvalidOperationException(
                    $"Ya existe una categoría con el nombre {nombre}.");
            }

            return MapToDto(categoria);
        }

        public async Task<CategoriaProductoDto> ActualizarAsync(int id, CategoriaProductoDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var nombre = NormalizarNombre(dto.Nombre);
            var descripcion = NormalizarDescripcion(dto.Descripcion);

            var categoria = await _categoriaRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Categoría {id} no encontrada.");

            if (await _categoriaRepo.NombreExisteAsync(nombre, excluirId: id))
                throw new InvalidOperationException($"Ya existe una categoría con el nombre {nombre}.");

            categoria.Nombre = nombre;
            categoria.Descripcion = descripcion;
            categoria.Activo = dto.Activo;

            _categoriaRepo.Update(categoria);
            try
            {
                await _categoriaRepo.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DatabaseErrorHelper.IsUniqueConstraintViolation(ex))
            {
                throw new InvalidOperationException(
                    $"Ya existe una categoría con el nombre {nombre}.");
            }

            return MapToDto(categoria);
        }

        public async Task<bool> ActivarDesactivarAsync(int id)
        {
            var categoria = await _categoriaRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Categoría {id} no encontrada.");

            categoria.Activo = !categoria.Activo;
            _categoriaRepo.Update(categoria);
            return await _categoriaRepo.SaveChangesAsync();
        }

        private static string NormalizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la categoría es obligatorio.");

            var limpio = TextNormalization.CollapseWhitespace(nombre);

            if (limpio.Length > 100)
                throw new ArgumentException("El nombre de la categoría no puede superar los 100 caracteres.");

            if (!TextoValido.IsMatch(limpio))
                throw new ArgumentException(
                    "El nombre de la categoría contiene caracteres no permitidos.");

            return limpio;
        }

        private static string? NormalizarDescripcion(string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return null;

            var limpia = TextNormalization.CollapseWhitespace(descripcion);

            if (limpia.Length > 500)
                throw new ArgumentException("La descripción no puede superar los 500 caracteres.");

            if (!TextoValido.IsMatch(limpia))
                throw new ArgumentException("La descripción contiene caracteres no permitidos.");

            return limpia;
        }

        private static CategoriaProductoDto MapToDto(CategoriaProducto c) => new()
        {
            IdCategoriaProducto = c.IdCategoriaProducto,
            Nombre = c.Nombre,
            Descripcion = c.Descripcion,
            Activo = c.Activo
        };
    }
}
