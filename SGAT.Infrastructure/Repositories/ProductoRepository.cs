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
            return await _dbSet
                .Include(p => p.CategoriaProducto)
                .Include(p => p.Marca)
                .Where(p => p.Activo && p.StockMinimo > 0 && p.StockActual <= p.StockMinimo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetActivosAsync()
        {
            return await _dbSet.Where(p => p.Activo).ToListAsync();
        }

        public async Task<IEnumerable<Producto>> GetAllConDetalleAsync()
        {
            return await _dbSet
                .Include(p => p.CategoriaProducto)
                .Include(p => p.Marca)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Producto?> GetByIdConDetalleAsync(int id)
        {
            return await _dbSet
                .Include(p => p.CategoriaProducto)
                .Include(p => p.Marca)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == id);
        }

        public async Task<IEnumerable<CategoriaProducto>> GetCategoriasActivasAsync()
        {
            return await _context.CategoriasProducto
                .AsNoTracking()
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<IEnumerable<Marca>> GetMarcasActivasAsync()
        {
            return await _context.Marcas
                .AsNoTracking()
                .Where(m => m.Activo)
                .OrderBy(m => m.Nombre)
                .ToListAsync();
        }

        public async Task<bool> CategoriaExisteAsync(int idCategoria)
        {
            return await _context.CategoriasProducto
                .AnyAsync(c => c.IdCategoriaProducto == idCategoria && c.Activo);
        }

        public async Task<bool> MarcaExisteAsync(int idMarca)
        {
            return await _context.Marcas
                .AnyAsync(m => m.IdMarca == idMarca && m.Activo);
        }
    }
}
