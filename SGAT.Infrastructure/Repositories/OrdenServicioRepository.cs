using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories
{
    public class OrdenServicioRepository : GenericRepository<OrdenServicio>, IOrdenServicioRepository
    {
        public OrdenServicioRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<OrdenServicio?> GetByCodigoAsync(string codigo)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.Codigo == codigo);
        }

        public async Task<OrdenServicio?> GetConDetallesAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Equipo)
                .Include(o => o.UsuarioCrea)
                .Include(o => o.EstadoServicio)
                .Include(o => o.Detalles)
                .Include(o => o.DetallesProducto)
                    .ThenInclude(dp => dp.Producto)
                .Include(o => o.Facturas)
                .Include(o => o.HistorialesServicio)
                .Include(o => o.PagosCartera)
                .FirstOrDefaultAsync(o => o.IdOrdenServicio == id);
        }

        public async Task<IEnumerable<OrdenServicio>> GetByClienteAsync(int idCliente)
        {
            return await _dbSet
                .Where(o => o.Equipo.IdCliente == idCliente)
                .Include(o => o.Equipo)
                .Include(o => o.EstadoServicio)
                .OrderByDescending(o => o.FechaIngreso)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrdenServicio>> GetByEstadoAsync(int idEstado)
        {
            return await _dbSet
                .Where(o => o.IdEstadoServicio == idEstado)
                .Include(o => o.Equipo)
                .Include(o => o.UsuarioCrea)
                .OrderByDescending(o => o.FechaIngreso)
                .ToListAsync();
        }

        public async Task<IEnumerable<OrdenServicio>> GetByTecnicoAsync(string idUsuario)
        {
            return await _dbSet
                .Where(o => o.IdUsuarioCrea == idUsuario)
                .Include(o => o.Equipo)
                .Include(o => o.EstadoServicio)
                .OrderByDescending(o => o.FechaIngreso)
                .ToListAsync();
        }

        public async Task<string> GenerarCodigoAsync()
        {
            var count = await _dbSet.CountAsync();
            return $"OS-{(count + 1):D6}";
        }
    }
}