using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
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
    }
}
