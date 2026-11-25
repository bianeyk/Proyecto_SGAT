using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories
{
    public class CategoriaProductoRepository : GenericRepository<CategoriaProducto>, ICategoriaProductoRepository
    {
        public CategoriaProductoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<CategoriaProducto?> GetByNombreAsync(string nombre)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Nombre == nombre);
        }

        public async Task<bool> NombreExisteAsync(string nombre, int? excluirId = null)
        {
            var consulta = _dbSet.AsNoTracking().Where(c => c.Nombre == nombre);

            if (excluirId.HasValue)
                consulta = consulta.Where(c => c.IdCategoriaProducto != excluirId.Value);

            return await consulta.AnyAsync();
        }

        public async Task<PagedResult<CategoriaProducto>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            // Se valida antes de calcular Skip para mantener el desplazamiento seguro.
            pagina = Math.Clamp(pagina, 1, 10_000);
            tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);

            var query = _dbSet.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim();
                if (termino.Length < 2)
                    throw new ArgumentException(
                        "El término de búsqueda debe tener al menos 2 caracteres.");

                if (termino.Length > 100)
                    throw new ArgumentException("El término de búsqueda no puede superar los 100 caracteres.");

                query = query.Where(c => c.Nombre.StartsWith(termino));
            }

            // Los contadores se calculan en un solo agregado SQL.
            var resumen = await query
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Activas = g.Count(c => c.Activo)
                })
                .FirstOrDefaultAsync();

            var total = resumen?.Total ?? 0;
            var activas = resumen?.Activas ?? 0;
            var items = await query
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.IdCategoriaProducto)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new PagedResult<CategoriaProducto>
            {
                Items = items,
                TotalCount = total,
                ActiveCount = activas,
                InactiveCount = total - activas
            };
        }
    }
}
