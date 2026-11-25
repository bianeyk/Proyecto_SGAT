using SGAT.Application.DTOs.Producto;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace SGAT.Application.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _marcaRepo;

        private static readonly Regex TextoValido =
            new(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-.,()/&'+]+$",
                RegexOptions.Compiled);

        public MarcaService(IMarcaRepository marcaRepo)
        {
            _marcaRepo = marcaRepo;
        }

        public async Task<MarcaDto?> GetByIdAsync(int id)
        {
            var marca = await _marcaRepo.GetByIdAsync(id);
            return marca is null ? null : MapToDto(marca);
        }

        public async Task<PagedResult<MarcaDto>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            var result = await _marcaRepo.GetPageAsync(termino, pagina, tamanoPagina);
            return new PagedResult<MarcaDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount,
                ActiveCount = result.ActiveCount,
                InactiveCount = result.InactiveCount
            };
        }

        public async Task<MarcaDto> CrearAsync(MarcaDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var nombre = NormalizarNombre(dto.Nombre);
            var descripcion = NormalizarDescripcion(dto.Descripcion);

            if (await _marcaRepo.NombreExisteAsync(nombre))
                throw new InvalidOperationException($"Ya existe una marca con el nombre {nombre}.");

            var marca = new Marca
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Activo = dto.Activo
            };

            await _marcaRepo.AddAsync(marca);
            try
            {
                await _marcaRepo.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DatabaseErrorHelper.IsUniqueConstraintViolation(ex))
            {
                throw new InvalidOperationException($"Ya existe una marca con el nombre {nombre}.");
            }

            return MapToDto(marca);
        }

        public async Task<MarcaDto> ActualizarAsync(int id, MarcaDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var nombre = NormalizarNombre(dto.Nombre);
            var descripcion = NormalizarDescripcion(dto.Descripcion);

            var marca = await _marcaRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Marca {id} no encontrada.");

            if (await _marcaRepo.NombreExisteAsync(nombre, excluirId: id))
                throw new InvalidOperationException($"Ya existe una marca con el nombre {nombre}.");

            marca.Nombre = nombre;
            marca.Descripcion = descripcion;
            marca.Activo = dto.Activo;

            _marcaRepo.Update(marca);
            try
            {
                await _marcaRepo.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (DatabaseErrorHelper.IsUniqueConstraintViolation(ex))
            {
                throw new InvalidOperationException($"Ya existe una marca con el nombre {nombre}.");
            }

            return MapToDto(marca);
        }

        public async Task<bool> ActivarDesactivarAsync(int id)
        {
            var marca = await _marcaRepo.GetByIdAsync(id)
                ?? throw new KeyNotFoundException($"Marca {id} no encontrada.");

            marca.Activo = !marca.Activo;
            _marcaRepo.Update(marca);
            return await _marcaRepo.SaveChangesAsync();
        }

        private static string NormalizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la marca es obligatorio.");

            var limpio = TextNormalization.CollapseWhitespace(nombre);

            if (limpio.Length > 100)
                throw new ArgumentException("El nombre de la marca no puede superar los 100 caracteres.");

            if (!TextoValido.IsMatch(limpio))
                throw new ArgumentException("El nombre de la marca contiene caracteres no permitidos.");

            return limpio;
        }

        private static string? NormalizarDescripcion(string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return null;

            var limpia = TextNormalization.CollapseWhitespace(descripcion);

            if (limpia.Length > 255)
                throw new ArgumentException("La descripción no puede superar los 255 caracteres.");

            if (!TextoValido.IsMatch(limpia))
                throw new ArgumentException("La descripción contiene caracteres no permitidos.");

            return limpia;
        }

        private static MarcaDto MapToDto(Marca m) => new()
        {
            IdMarca = m.IdMarca,
            Nombre = m.Nombre,
            Descripcion = m.Descripcion,
            Activo = m.Activo
        };
    }
}
