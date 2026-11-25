using SGAT.Application.DTOs.Producto;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;

namespace SGAT.Application.Services
{
    public class MarcaService : IMarcaService
    {
        private readonly IMarcaRepository _marcaRepo;

        public MarcaService(IMarcaRepository marcaRepo)
        {
            _marcaRepo = marcaRepo;
        }

        public async Task<IEnumerable<MarcaDto>> GetTodasAsync()
        {
            var marcas = await _marcaRepo.GetAllOrdenadasAsync();
            return marcas.Select(MapToDto);
        }

        public async Task<MarcaDto?> GetByIdAsync(int id)
        {
            var marca = await _marcaRepo.GetByIdAsync(id);
            return marca is null ? null : MapToDto(marca);
        }

        public async Task<IEnumerable<MarcaDto>> BuscarAsync(string termino)
        {
            var marcas = await _marcaRepo.BuscarAsync(termino);
            return marcas.Select(MapToDto);
        }

        public async Task<PagedResult<MarcaDto>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            var result = await _marcaRepo.GetPageAsync(termino, pagina, tamanoPagina);
            return new PagedResult<MarcaDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                TotalCount = result.TotalCount
            };
        }

        public async Task<MarcaDto> CrearAsync(MarcaDto dto)
        {
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
            await _marcaRepo.SaveChangesAsync();
            return MapToDto(marca);
        }

        public async Task<MarcaDto> ActualizarAsync(int id, MarcaDto dto)
        {
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
            await _marcaRepo.SaveChangesAsync();
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

            var limpio = nombre.Trim();

            if (limpio.Length > 100)
                throw new ArgumentException("El nombre de la marca no puede superar los 100 caracteres.");

            return limpio;
        }

        private static string? NormalizarDescripcion(string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(descripcion))
                return null;

            var limpia = descripcion.Trim();

            if (limpia.Length > 255)
                throw new ArgumentException("La descripción no puede superar los 255 caracteres.");

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
