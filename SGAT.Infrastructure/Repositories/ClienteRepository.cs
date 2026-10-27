using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
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
    }
}