using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SGAT.Application.DTOs.Equipo;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;

namespace SGAT.Application.Services;

public sealed class CategoriaDispositivoService(ICategoriaDispositivoRepository repository)
    : ICategoriaDispositivoService
{
    private static readonly Regex TextoValido =
        new(@"^[\p{L}\p{M}\p{N}\s\-.,()/&'+]+$", RegexOptions.Compiled);

    public async Task<CategoriaDispositivoDto?> GetByIdAsync(int id)
    {
        var categoria = await repository.GetByIdAsync(id);
        return categoria is null ? null : Mapear(categoria);
    }

    public async Task<PagedResult<CategoriaDispositivoDto>> GetPageAsync(
        string? termino, int pagina, int tamanoPagina)
    {
        var resultado = await repository.GetPageAsync(
            NormalizarBusqueda(termino), pagina, tamanoPagina);

        return new PagedResult<CategoriaDispositivoDto>
        {
            Items = resultado.Items.Select(Mapear).ToList(),
            TotalCount = resultado.TotalCount,
            ActiveCount = resultado.ActiveCount,
            InactiveCount = resultado.InactiveCount
        };
    }

    public async Task<CategoriaDispositivoDto> CrearAsync(CategoriaDispositivoDto dto)
    {
        var datos = Validar(dto);
        if (await repository.NombreExisteAsync(datos.Nombre))
            throw new InvalidOperationException(
                $"Ya existe una categoría de dispositivo con el nombre {datos.Nombre}.");

        var categoria = new CategoriaDispositivo
        {
            Nombre = datos.Nombre,
            Descripcion = datos.Descripcion,
            Activo = dto.Activo
        };

        await repository.AddAsync(categoria);
        await GuardarAsync(datos.Nombre);
        return Mapear(categoria);
    }

    public async Task<CategoriaDispositivoDto> ActualizarAsync(
        int id, CategoriaDispositivoDto dto)
    {
        var datos = Validar(dto);
        var categoria = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Categoría de dispositivo {id} no encontrada.");

        if (await repository.NombreExisteAsync(datos.Nombre, id))
            throw new InvalidOperationException(
                $"Ya existe una categoría de dispositivo con el nombre {datos.Nombre}.");

        categoria.Nombre = datos.Nombre;
        categoria.Descripcion = datos.Descripcion;
        categoria.Activo = dto.Activo;
        repository.Update(categoria);
        await GuardarAsync(datos.Nombre);
        return Mapear(categoria);
    }

    public async Task<bool> ActivarDesactivarAsync(int id)
    {
        var categoria = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Categoría de dispositivo {id} no encontrada.");
        categoria.Activo = !categoria.Activo;
        repository.Update(categoria);
        return await repository.SaveChangesAsync();
    }

    private async Task GuardarAsync(string nombre)
    {
        try
        {
            await repository.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (DatabaseErrorHelper.IsUniqueConstraintViolation(ex))
        {
            throw new InvalidOperationException(
                $"Ya existe una categoría de dispositivo con el nombre {nombre}.");
        }
    }

    private static (string Nombre, string? Descripcion) Validar(
        CategoriaDispositivoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre))
            throw new ArgumentException("El nombre de la categoría de dispositivo es obligatorio.");

        var nombre = TextNormalization.CollapseWhitespace(dto.Nombre);
        if (nombre.Length > 100 || !TextoValido.IsMatch(nombre))
            throw new ArgumentException(
                "El nombre de la categoría de dispositivo no es válido.");

        var descripcion = TextNormalization.CollapseOptional(dto.Descripcion);
        if (descripcion is not null &&
            (descripcion.Length > 255 || !TextoValido.IsMatch(descripcion)))
            throw new ArgumentException("La descripción no es válida.");

        return (nombre, descripcion);
    }

    private static string? NormalizarBusqueda(string? termino)
    {
        var limpio = TextNormalization.CollapseOptional(termino);
        if (limpio is not null && (limpio.Length < 2 || limpio.Length > 100))
            throw new ArgumentException(
                "El término de búsqueda debe tener entre 2 y 100 caracteres.");
        return limpio;
    }

    private static CategoriaDispositivoDto Mapear(CategoriaDispositivo categoria) =>
        new(categoria.IdCategoriaDispositivo, categoria.Nombre,
            categoria.Descripcion, categoria.Activo);
}
