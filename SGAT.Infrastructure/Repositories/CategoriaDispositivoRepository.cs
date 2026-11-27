using Microsoft.EntityFrameworkCore;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories;

public sealed class CategoriaDispositivoRepository(ApplicationDbContext context)
    : GenericRepository<CategoriaDispositivo>(context), ICategoriaDispositivoRepository
{
    public Task<bool> NombreExisteAsync(string nombre, int? excluirId = null) =>
        _dbSet.AsNoTracking().AnyAsync(c =>
            c.Nombre == nombre && (!excluirId.HasValue || c.IdCategoriaDispositivo != excluirId));

    public async Task<PagedResult<CategoriaDispositivo>> GetPageAsync(
        string? termino, int pagina, int tamanoPagina)
    {
        pagina = Math.Clamp(pagina, 1, 10_000);
        tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);
        var consulta = _dbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(termino))
            consulta = consulta.Where(c => c.Nombre.StartsWith(termino));

        var resumen = await consulta.GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Activas = g.Count(c => c.Activo)
            })
            .FirstOrDefaultAsync();

        var total = resumen?.Total ?? 0;
        var activas = resumen?.Activas ?? 0;
        return new PagedResult<CategoriaDispositivo>
        {
            Items = await consulta.OrderBy(c => c.Nombre)
                .ThenBy(c => c.IdCategoriaDispositivo)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync(),
            TotalCount = total,
            ActiveCount = activas,
            InactiveCount = total - activas
        };
    }
}
