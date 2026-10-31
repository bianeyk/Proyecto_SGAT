using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Domain.Entities;
using SGAT.Domain.Enums;
using SGAT.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SGAT.Infrastructure.Repositories
{
    public class TecnicoRepository : ITecnicoRepository
    {
        private readonly ApplicationDbContext _context;

        public TecnicoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrdenServicio>> GetOrdenesAsync(EstadoOrden? estado = null, string? tecnicoId = null)
        {
            var query = _context.OrdenesServicio
                .AsNoTracking()
                .Include(o => o.Equipo)
                    .ThenInclude(e => e.CategoriaDispositivo)
                .Include(o => o.Equipo)
                    .ThenInclude(e => e.Marca)
                .Include(o => o.DiagnosticoTecnico)
                .AsQueryable();

            if (estado.HasValue)
            {
                query = query.Where(o => o.EstadoOrden == estado.Value);
            }

            if (!string.IsNullOrWhiteSpace(tecnicoId))
            {
                query = query.Where(o => o.DiagnosticoTecnico != null && o.DiagnosticoTecnico.TecnicoId == tecnicoId);
            }

            return await query
                .OrderByDescending(o => o.FechaIngreso)
                .ToListAsync();
        }

        public async Task<OrdenServicio?> GetDetalleOrdenAsync(int ordenId)
        {
            return await _context.OrdenesServicio
                .Include(o => o.Equipo)
                    .ThenInclude(e => e.Cliente)
                .Include(o => o.Equipo)
                    .ThenInclude(e => e.CategoriaDispositivo)
                .Include(o => o.Equipo)
                    .ThenInclude(e => e.Marca)
                .Include(o => o.DiagnosticoTecnico)
                    .ThenInclude(d => d!.Tecnico)
                .Include(o => o.Bitacoras)
                .FirstOrDefaultAsync(o => o.IdOrdenServicio == ordenId);
        }

        public async Task<DiagnosticoTecnico?> GetDiagnosticoPorOrdenIdAsync(int ordenId)
        {
            return await _context.DiagnosticosTecnicos
                .Include(d => d.Orden)
                .FirstOrDefaultAsync(d => d.OrdenId == ordenId);
        }

        public async Task AddDiagnosticoAsync(DiagnosticoTecnico diagnostico)
        {
            await _context.DiagnosticosTecnicos.AddAsync(diagnostico);
        }

        public void UpdateDiagnostico(DiagnosticoTecnico diagnostico)
        {
            _context.DiagnosticosTecnicos.Update(diagnostico);
        }

        public async Task AddBitacoraAsync(BitacoraTecnica bitacora)
        {
            await _context.BitacorasTecnicas.AddAsync(bitacora);
        }

        public void UpdateOrden(OrdenServicio orden)
        {
            _context.OrdenesServicio.Update(orden);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
