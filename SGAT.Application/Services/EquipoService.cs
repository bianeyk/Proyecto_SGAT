using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SGAT.Application.DTOs.Equipo;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.Interfaces.Services;
using SGAT.Domain.Entities;

namespace SGAT.Application.Services;

public sealed class EquipoService(IEquipoRepository repository) : IEquipoService
{
    private static readonly Regex TextoValido = new(@"^[\p{L}\p{M}\p{N} .,'()/&+#:_\-]+$", RegexOptions.Compiled);
    private static readonly Regex SerialValido = new(@"^[A-Za-z0-9._/\-]+$", RegexOptions.Compiled);

    public Task<PagedResult<EquipoDto>> GetPageAsync(string? termino, string estado, int pagina, int tamano)
    {
        termino = NormalizarBusqueda(termino);
        if (estado is not ("todos" or "activo" or "inactivo"))
            throw new ArgumentException("El filtro de estado no es válido.");
        return repository.GetPageAsync(termino, estado, Math.Clamp(pagina, 1, 1_000), Math.Clamp(tamano, 1, 100));
    }

    public Task<EquipoDto?> GetByIdAsync(int id) => repository.GetDetalleAsync(id);

    public async Task<int> CrearAsync(GuardarEquipoDto dto, string idUsuario)
    {
        ArgumentNullException.ThrowIfNull(dto);
        if (string.IsNullOrWhiteSpace(idUsuario))
            throw new ArgumentException("El usuario creador es obligatorio.");
        var equipo = new Equipo { IdUsuario = idUsuario };
        await AplicarDatosAsync(equipo, dto, nuevo: true);
        await repository.AddAsync(equipo);
        await GuardarAsync();
        return equipo.IdEquipo;
    }

    public async Task ActualizarAsync(int id, GuardarEquipoDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);
        var equipo = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Equipo no encontrado.");
        VerificarVersion(equipo, dto.Version);
        if (dto.IdCliente != equipo.IdCliente && await repository.TieneOrdenesAsync(id))
            throw new ArgumentException("No se puede cambiar el cliente de un equipo con órdenes de servicio.");
        await AplicarDatosAsync(equipo, dto, nuevo: false);
        equipo.UpdatedAt = DateTime.Now;
        repository.Update(equipo);
        await GuardarAsync();
    }

    public async Task CambiarEstadoAsync(int id, bool activo, string version)
    {
        var equipo = await repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Equipo no encontrado.");
        VerificarVersion(equipo, version);
        // La baja es lógica: conserva el equipo y todas sus órdenes.
        equipo.Activo = activo;
        equipo.UpdatedAt = DateTime.Now;
        repository.Update(equipo);
        await GuardarAsync();
    }

    private async Task AplicarDatosAsync(Equipo equipo, GuardarEquipoDto dto, bool nuevo)
    {
        var modelo = Texto(dto.Modelo, 100, "modelo", obligatorio: true)!;
        var descripcion = Texto(dto.Descripcion, 255, "descripción", obligatorio: false);
        var serialAnterior = equipo.Serial;
        var serialAnteriorGenerado = equipo.SerialGenerado;
        var serial = TextNormalization.TrimOptional(dto.Serial);
        var serialGenerado = serial is null;
        if (serial is null)
        {
            serial = await GenerarSerialUnicoAsync(nuevo ? null : equipo.IdEquipo);
        }
        else
        {
            if (serial.Length > 100 || !SerialValido.IsMatch(serial))
                throw new ArgumentException("El serial admite hasta 100 letras, números, puntos, guiones, barras y guiones bajos.");
            if (await repository.SerialExisteAsync(serial, nuevo ? null : equipo.IdEquipo))
                throw new InvalidOperationException("Ya existe un equipo con ese serial.");
        }
        var relaciones = new ValidarRelacionesEquipoDto(
            dto.IdCliente,
            nuevo || dto.IdCliente != equipo.IdCliente,
            dto.IdMarca,
            nuevo || dto.IdMarca != equipo.IdMarca,
            dto.IdCategoriaDispositivo,
            nuevo || dto.IdCategoriaDispositivo != equipo.IdCategoriaDispositivo);
        if (relaciones.IdCliente <= 0 || relaciones.IdMarca <= 0 ||
            relaciones.IdCategoriaDispositivo <= 0 ||
            !await repository.RelacionesValidasAsync(relaciones))
            throw new ArgumentException("Selecciona un cliente, marca y categoría de dispositivo existentes y activos.");
        equipo.IdCliente = dto.IdCliente;
        equipo.IdMarca = dto.IdMarca;
        equipo.IdCategoriaDispositivo = dto.IdCategoriaDispositivo;
        equipo.Modelo = modelo;
        equipo.Serial = serial;
        equipo.SerialGenerado = serialGenerado
            || (!nuevo && serialAnteriorGenerado &&
                string.Equals(serialAnterior, serial, StringComparison.OrdinalIgnoreCase));
        equipo.Descripcion = descripcion;
    }

    private async Task<string> GenerarSerialUnicoAsync(int? exceptoId)
    {
        for (var intento = 0; intento < 5; intento++)
        {
            var bytes = RandomNumberGenerator.GetBytes(8);
            var serial = $"SGAT-{Convert.ToHexString(bytes)}";
            if (!await repository.SerialExisteAsync(serial, exceptoId))
                return serial;
        }

        throw new InvalidOperationException(
            "No fue posible generar un serial único. Inténtalo nuevamente.");
    }

    private static string? Texto(string? valor, int maximo, string campo, bool obligatorio)
    {
        var limpio = TextNormalization.CollapseOptional(valor);
        if (limpio is null)
        {
            if (obligatorio) throw new ArgumentException($"El {campo} es obligatorio.");
            return null;
        }
        if (limpio.Length > maximo || !TextoValido.IsMatch(limpio))
            throw new ArgumentException($"El campo {campo} admite hasta {maximo} caracteres de texto, sin etiquetas ni símbolos especiales.");
        return limpio;
    }

    private static string? NormalizarBusqueda(string? termino)
    {
        var limpio = TextNormalization.CollapseOptional(termino);
        if (limpio is not null && (limpio.Length < 2 || limpio.Length > 100))
            throw new ArgumentException("La búsqueda debe tener entre 2 y 100 caracteres.");
        return limpio;
    }

    private static void VerificarVersion(Equipo equipo, string version)
    {
        if (string.IsNullOrEmpty(version) || version != Convert.ToBase64String(equipo.Version))
            throw new InvalidOperationException("Otro usuario modificó este equipo. Recarga la página antes de guardar.");
    }

    private async Task GuardarAsync()
    {
        try { await repository.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException("Otro usuario modificó este equipo. Recarga la página antes de guardar.");
        }
        catch (DbUpdateException ex) when (DatabaseErrorHelper.IsUniqueConstraintViolation(ex))
        {
            throw new InvalidOperationException("Ya existe un equipo con ese serial.");
        }
    }

    public Task<IReadOnlyList<OpcionEquipoDto>> BuscarClientesAsync(string termino)
    {
        var limpio = NormalizarBusqueda(termino);
        if (limpio is null) throw new ArgumentException("Escribe al menos 2 caracteres para buscar clientes.");
        return repository.BuscarClientesAsync(limpio);
    }

    public Task<IReadOnlyList<OpcionEquipoDto>> GetMarcasAsync(int? actual = null) => repository.GetMarcasAsync(actual);
    public Task<IReadOnlyList<OpcionEquipoDto>> GetCategoriasAsync(int? actual = null) => repository.GetCategoriasAsync(actual);
}
