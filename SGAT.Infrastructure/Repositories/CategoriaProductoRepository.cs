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

        public async Task<IEnumerable<CategoriaProducto>> GetAllOrdenadasAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
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

        public async Task<IEnumerable<CategoriaProducto>> BuscarAsync(string termino)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.Nombre.Contains(termino)
                         || (c.Descripcion != null && c.Descripcion.Contains(termino)))
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<PagedResult<CategoriaProducto>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            // Se valida antes de calcular Skip para mantener el desplazamiento seguro.
            pagina = Math.Clamp(pagina, 1, 100_000);
            tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);

            var query = _dbSet.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim();
                query = query.Where(c => c.Nombre.Contains(termino)
                    || (c.Descripcion != null && c.Descripcion.Contains(termino)));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.IdCategoriaProducto)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new PagedResult<CategoriaProducto> { Items = items, TotalCount = total };
        }
    }
}
