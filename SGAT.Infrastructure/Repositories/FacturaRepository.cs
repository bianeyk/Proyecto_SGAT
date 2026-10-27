using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Domain.Entities;
using SGAT.SGAT.Domain.Enums;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories
{
    public class FacturaRepository : GenericRepository<Factura>, IFacturaRepository
    {
        public FacturaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Factura?> GetByNumeroAsync(string numeroFactura)
        {
            return await _dbSet.FirstOrDefaultAsync(f => f.NumeroFactura == numeroFactura);
        }

        public async Task<bool> NumeroExisteAsync(string numeroFactura)
        {
            return await _dbSet.AnyAsync(f => f.NumeroFactura == numeroFactura);
        }

        public async Task<IEnumerable<Factura>> GetByClienteAsync(int idCliente)
        {
            return await _dbSet
                .Where(f => f.IdCliente == idCliente)
                .Include(f => f.Cliente)
                .Include(f => f.Usuario)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        public async Task<IEnumerable<Factura>> GetPendientesAsync()
        {
            return await _dbSet
                .Where(f => f.Estado == EstadoFacturaEnum.PENDIENTE)
                .Include(f => f.Cliente)
                .Include(f => f.Usuario)
                .OrderByDescending(f => f.FechaFactura)
                .ToListAsync();
        }

        public async Task<Factura?> GetConDetallesAsync(int id)
        {
            return await _dbSet
                .Include(f => f.Cliente)
                .Include(f => f.OrdenServicio)
                .Include(f => f.Usuario)
                .Include(f => f.Detalles)
                    .ThenInclude(d => d.Producto)
                .Include(f => f.PagosCartera)
                .FirstOrDefaultAsync(f => f.IdFactura == id);
        }

        public async Task<string> GenerarNumeroFacturaAsync()
        {
            var count = await _dbSet.CountAsync();
            return $"FAC-{(count + 1):D6}";
        }
    }
}