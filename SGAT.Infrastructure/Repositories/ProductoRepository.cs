using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories
{
    public class ProductoRepository : GenericRepository<Producto>, IProductoRepository
    {
        public ProductoRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Producto?> GetByCodigoAsync(string codigo)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.Codigo == codigo);
        }

        public async Task<bool> CodigoExisteAsync(string codigo)
        {
            return await _dbSet.AnyAsync(p => p.Codigo == codigo);
        }

        public async Task<IEnumerable<Producto>> GetStockBajoAsync()
        {
            return await _dbSet.Where(p => p.StockActual <= p.StockMinimo).ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetActivosAsync()
        {
            return await _dbSet.Where(p => p.Activo).ToListAsync();
        }
    }
}