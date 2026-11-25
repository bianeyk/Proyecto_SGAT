using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories
{
    public class ClienteRepository : GenericRepository<Cliente>, IClienteRepository
    {
        public ClienteRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Cliente?> GetByNumDocAsync(string numDoc)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.NumDoc == numDoc);
        }

        public async Task<bool> NumDocExisteAsync(string numDoc)
        {
            return await _dbSet.AnyAsync(c => c.NumDoc == numDoc);
        }

        public async Task<PagedResult<Cliente>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            // Se limitan ambos valores para evitar consultas excesivamente grandes
            // o desbordamientos al calcular el desplazamiento.
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

                query = query.Where(c => c.NumDoc.StartsWith(termino)
                    || c.Nombre.StartsWith(termino)
                    || (c.Email != null && c.Email.StartsWith(termino))
                    || (c.Telefono != null && c.Telefono.StartsWith(termino)));
            }

            // Un único agregado obtiene los contadores necesarios para la vista.
            var resumen = await query
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Activos = g.Count(c => c.Activo)
                })
                .FirstOrDefaultAsync();

            var total = resumen?.Total ?? 0;
            var activos = resumen?.Activos ?? 0;
            var items = await query
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.IdCliente)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new PagedResult<Cliente>
            {
                Items = items,
                TotalCount = total,
                ActiveCount = activos,
                InactiveCount = total - activos
            };
        }
    }
}
