using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SGAT.Application.DTOs.Equipo;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories;

public sealed class EquipoRepository(ApplicationDbContext context) : GenericRepository<Equipo>(context), IEquipoRepository
{
    private static readonly Expression<Func<Equipo, EquipoDto>> Proyeccion = e => new EquipoDto
    {
        IdEquipo = e.IdEquipo, IdCliente = e.IdCliente, IdMarca = e.IdMarca,
        IdCategoriaDispositivo = e.IdCategoriaDispositivo,
        Cliente = e.Cliente.Nombre, DocumentoCliente = e.Cliente.NumDoc,
        Marca = e.Marca.Nombre, Categoria = e.CategoriaDispositivo.Nombre,
        Modelo = e.Modelo, Serial = e.Serial, SerialGenerado = e.SerialGenerado, Descripcion = e.Descripcion,
        Activo = e.Activo, TieneOrdenes = e.OrdenesServicio.Any(),
        CreatedAt = e.CreatedAt, UpdatedAt = e.UpdatedAt,
        Version = Convert.ToBase64String(e.Version)
    };

    public async Task<PagedResult<EquipoDto>> GetPageAsync(string? termino, string estado, int pagina, int tamano)
    {
        pagina = Math.Clamp(pagina, 1, 1_000);
        tamano = Math.Clamp(tamano, 1, 100);
        if (termino is not null && (termino.Length < 2 || termino.Length > 100))
            throw new ArgumentException("La búsqueda debe tener entre 2 y 100 caracteres.");
        if (estado is not ("todos" or "activo" or "inactivo"))
            throw new ArgumentException("Filtro inválido.");
        var query = _dbSet.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(termino))
            query = query.Where(e => e.Modelo.StartsWith(termino)
                || (e.Serial != null && e.Serial.StartsWith(termino))
                || e.Cliente.Nombre.StartsWith(termino) || e.Cliente.NumDoc.StartsWith(termino)
                || e.Marca.Nombre.StartsWith(termino) || e.CategoriaDispositivo.Nombre.StartsWith(termino));
        // Los contadores pertenecen al conjunto buscado, no solamente a la página visible.
        var resumen = await query.GroupBy(_ => 1)
            .Select(g => new { Total = g.Count(), Activos = g.Count(e => e.Activo) }).FirstOrDefaultAsync();
        var activos = resumen?.Activos ?? 0;
        var inactivos = (resumen?.Total ?? 0) - activos;
        if (estado != "todos") query = query.Where(e => e.Activo == (estado == "activo"));
        return new PagedResult<EquipoDto>
        {
            Items = await query.OrderBy(e => e.Modelo).ThenBy(e => e.IdEquipo)
                .Skip((pagina - 1) * tamano).Take(tamano).Select(Proyeccion).ToListAsync(),
            TotalCount = estado == "activo" ? activos : estado == "inactivo" ? inactivos : resumen?.Total ?? 0,
            ActiveCount = activos, InactiveCount = inactivos
        };
    }

    public Task<EquipoDto?> GetDetalleAsync(int id) =>
        _dbSet.AsNoTracking().Where(e => e.IdEquipo == id).Select(Proyeccion).SingleOrDefaultAsync();
    public Task<bool> SerialExisteAsync(string serial, int? exceptoId) =>
        _dbSet.AnyAsync(e => e.Serial == serial && (!exceptoId.HasValue || e.IdEquipo != exceptoId));
    public Task<bool> TieneOrdenesAsync(int id) => _context.OrdenesServicio.AnyAsync(o => o.IdEquipo == id);
    public async Task<bool> RelacionesValidasAsync(ValidarRelacionesEquipoDto relaciones)
    {
        var resultado = await (
            from cliente in _context.Clientes
            where cliente.IdCliente == relaciones.IdCliente
            join marca in _context.Marcas
                on relaciones.IdMarca equals marca.IdMarca into marcas
            from marca in marcas.DefaultIfEmpty()
            join categoria in _context.CategoriasDispositivo
                on relaciones.IdCategoriaDispositivo equals categoria.IdCategoriaDispositivo into categorias
            from categoria in categorias.DefaultIfEmpty()
            select new
            {
                ClienteValido = !relaciones.ClienteDebeEstarActivo || cliente.Activo,
                MarcaValida = marca != null &&
                    (!relaciones.MarcaDebeEstarActiva || marca.Activo),
                CategoriaValida = categoria != null &&
                    (!relaciones.CategoriaDebeEstarActiva || categoria.Activo)
            }).SingleOrDefaultAsync();

        return resultado is not null &&
            resultado.ClienteValido &&
            resultado.MarcaValida &&
            resultado.CategoriaValida;
    }

    public async Task<IReadOnlyList<OpcionEquipoDto>> BuscarClientesAsync(string termino)
    {
        if (string.IsNullOrWhiteSpace(termino) || termino.Length < 2 || termino.Length > 100)
            throw new ArgumentException("La búsqueda debe tener entre 2 y 100 caracteres.");
        return await _context.Clientes.AsNoTracking()
            .Where(c => c.Activo && (c.Nombre.StartsWith(termino) || c.NumDoc.StartsWith(termino)))
            .OrderBy(c => c.Nombre).ThenBy(c => c.IdCliente).Take(20)
            .Select(c => new OpcionEquipoDto(c.IdCliente, c.Nombre + " — " + c.NumDoc)).ToListAsync();
    }

    public async Task<IReadOnlyList<OpcionEquipoDto>> GetMarcasAsync(int? actual) =>
        await _context.Marcas.AsNoTracking().Where(m => m.Activo || m.IdMarca == actual)
            .OrderBy(m => m.Nombre).Select(m => new OpcionEquipoDto(m.IdMarca, m.Nombre + (m.Activo ? "" : " (inactiva)"))).ToListAsync();
    public async Task<IReadOnlyList<OpcionEquipoDto>> GetCategoriasAsync(int? actual) =>
        await _context.CategoriasDispositivo.AsNoTracking().Where(c => c.Activo || c.IdCategoriaDispositivo == actual)
            .OrderBy(c => c.Nombre).Select(c => new OpcionEquipoDto(c.IdCategoriaDispositivo, c.Nombre + (c.Activo ? "" : " (inactiva)"))).ToListAsync();
}
