using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.DTOs.Paginacion;
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

        public async Task<PagedResult<Producto>> GetPageConDetalleAsync(
            string? termino, string filtro, int pagina, int tamanoPagina)
        {
            // El filtrado y la paginación se ejecutan en SQL antes de materializar
            // los productos, evitando cargar todo el inventario en memoria.
            pagina = Math.Clamp(pagina, 1, 100_000);
            tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);

            var query = _dbSet
                .Include(p => p.CategoriaProducto)
                .Include(p => p.Marca)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim();
                query = query.Where(p =>
                    p.Nombre.Contains(termino)
                    || (p.Codigo != null && p.Codigo.Contains(termino))
                    || p.CategoriaProducto.Nombre.Contains(termino)
                    || p.Marca.Nombre.Contains(termino));
            }

            query = (filtro ?? "todos").ToLowerInvariant() switch
            {
                "activo" => query.Where(p => p.Activo),
                "inactivo" => query.Where(p => !p.Activo),
                "stockbajo" => query.Where(p =>
                    p.Activo && p.StockMinimo > 0 && p.StockActual <= p.StockMinimo),
                _ => query
            };

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Nombre)
                .ThenBy(p => p.IdProducto)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new PagedResult<Producto> { Items = items, TotalCount = total };
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
