using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories
{
    public class MarcaRepository : GenericRepository<Marca>, IMarcaRepository
    {
        public MarcaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Marca>> GetAllOrdenadasAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<Marca?> GetByNombreAsync(string nombre)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Nombre == nombre);
        }

        public async Task<bool> NombreExisteAsync(string nombre, int? excluirId = null)
        {
            var consulta = _dbSet.AsNoTracking().Where(m => m.Nombre == nombre);

            if (excluirId.HasValue)
                consulta = consulta.Where(m => m.IdMarca != excluirId.Value);

            return await consulta.AnyAsync();
        }

        public async Task<IEnumerable<Marca>> BuscarAsync(string termino)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(m => m.Nombre.Contains(termino)
                         || (m.Descripcion != null && m.Descripcion.Contains(termino)))
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<PagedResult<Marca>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            // El límite protege al servidor frente a solicitudes con tamaños arbitrarios.
            pagina = Math.Clamp(pagina, 1, 100_000);
            tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);

            var query = _dbSet.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim();
                query = query.Where(m => m.Nombre.Contains(termino)
                    || (m.Descripcion != null && m.Descripcion.Contains(termino)));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(m => m.Nombre)
                .ThenBy(m => m.IdMarca)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new PagedResult<Marca> { Items = items, TotalCount = total };
        }
    }
}
