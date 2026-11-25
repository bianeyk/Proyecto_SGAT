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

        public async Task<IEnumerable<Cliente>> BuscarAsync(string termino)
        {
            return await _dbSet
                .Where(c => c.NumDoc.Contains(termino)
                         || c.Nombre.Contains(termino)
                         || (c.Email != null && c.Email.Contains(termino))
                         || (c.Telefono != null && c.Telefono.Contains(termino)))
                .ToListAsync();
        }

        public async Task<PagedResult<Cliente>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            // Se limitan ambos valores para evitar consultas excesivamente grandes
            // o desbordamientos al calcular el desplazamiento.
            pagina = Math.Clamp(pagina, 1, 100_000);
            tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);

            var query = _dbSet.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim();
                query = query.Where(c => c.NumDoc.Contains(termino)
                    || c.Nombre.Contains(termino)
                    || (c.Email != null && c.Email.Contains(termino))
                    || (c.Telefono != null && c.Telefono.Contains(termino)));
            }

            // El conteo se ejecuta en la base de datos, no sobre una lista en memoria.
            var total = await query.CountAsync();
            var items = await query
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.IdCliente)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new PagedResult<Cliente> { Items = items, TotalCount = total };
        }
    }
}
