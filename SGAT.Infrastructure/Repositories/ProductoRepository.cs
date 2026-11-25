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

        public async Task<PagedResult<Producto>> GetPageConDetalleAsync(
            string? termino, string filtro, int pagina, int tamanoPagina)
        {
            // El filtrado y la paginación se ejecutan en SQL antes de materializar
            // los productos, evitando cargar todo el inventario en memoria.
            pagina = Math.Clamp(pagina, 1, 10_000);
            tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);

            var query = _dbSet
                .Include(p => p.CategoriaProducto)
                .Include(p => p.Marca)
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim();
                if (termino.Length < 2)
                    throw new ArgumentException(
                        "El término de búsqueda debe tener al menos 2 caracteres.");

                if (termino.Length > 100)
                    throw new ArgumentException("El término de búsqueda no puede superar los 100 caracteres.");

                query = query.Where(p =>
                    p.Nombre.StartsWith(termino)
                    || (p.Codigo != null && p.Codigo.StartsWith(termino))
                    || p.CategoriaProducto.Nombre.StartsWith(termino)
                    || p.Marca.Nombre.StartsWith(termino));
            }

            // Un único agregado obtiene los contadores sobre toda la búsqueda,
            // antes de aplicar el filtro y la paginación.
            var resumen = await query
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Activos = g.Count(p => p.Activo),
                    StockBajo = g.Count(p =>
                        p.Activo && p.StockMinimo > 0 && p.StockActual <= p.StockMinimo)
                })
                .FirstOrDefaultAsync();

            var totalTodos = resumen?.Total ?? 0;
            var activos = resumen?.Activos ?? 0;
            var stockBajo = resumen?.StockBajo ?? 0;
            var inactivos = totalTodos - activos;

            query = (filtro ?? "todos").ToLowerInvariant() switch
            {
                "activo" => query.Where(p => p.Activo),
                "inactivo" => query.Where(p => !p.Activo),
                "stockbajo" => query.Where(p =>
                    p.Activo && p.StockMinimo > 0 && p.StockActual <= p.StockMinimo),
                _ => query
            };

            // El filtro es un subconjunto de los contadores ya calculados.
            var total = (filtro ?? "todos").ToLowerInvariant() switch
            {
                "activo" => activos,
                "inactivo" => inactivos,
                "stockbajo" => stockBajo,
                _ => totalTodos
            };
            var items = await query
                .OrderBy(p => p.Nombre)
                .ThenBy(p => p.IdProducto)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new PagedResult<Producto>
            {
                Items = items,
                TotalCount = total,
                ActiveCount = activos,
                InactiveCount = inactivos,
                LowStockCount = stockBajo
            };
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
