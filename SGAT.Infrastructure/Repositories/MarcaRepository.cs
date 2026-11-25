using Microsoft.EntityFrameworkCore;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Domain.Entities;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories
{
    public class MarcaRepository : GenericRepository<Marca>, IMarcaRepository
    {
        public MarcaRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Marca?> GetByNombreAsync(string nombre)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Nombre == nombre);
        }

        public async Task<bool> NombreExisteAsync(string nombre, int? excluirId = null)
        {
            var consulta = _dbSet.AsNoTracking().Where(m => m.Nombre == nombre);

            if (excluirId.HasValue)
                consulta = consulta.Where(m => m.IdMarca != excluirId.Value);

            return await consulta.AnyAsync();
        }

        public async Task<PagedResult<Marca>> GetPageAsync(
            string? termino, int pagina, int tamanoPagina)
        {
            // El límite protege al servidor frente a solicitudes con tamaños arbitrarios.
            pagina = Math.Clamp(pagina, 1, 10_000);
            tamanoPagina = Math.Clamp(tamanoPagina, 1, 100);

            var query = _dbSet.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(termino))
            {
                termino = termino.Trim();
                if (termino.Length < 2)
                    throw new ArgumentException(
                        "El término de búsqueda debe tener al menos 2 caracteres.");

                if (termino.Length > 100)
                    throw new ArgumentException("El término de búsqueda no puede superar los 100 caracteres.");

                query = query.Where(m => m.Nombre.StartsWith(termino));
            }

            // Un único agregado evita ejecutar un CountAsync por cada contador.
            var resumen = await query
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Activas = g.Count(m => m.Activo)
                })
                .FirstOrDefaultAsync();

            var total = resumen?.Total ?? 0;
            var activas = resumen?.Activas ?? 0;
            var items = await query
                .OrderBy(m => m.Nombre)
                .ThenBy(m => m.IdMarca)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToListAsync();

            return new PagedResult<Marca>
            {
                Items = items,
                TotalCount = total,
                ActiveCount = activas,
                InactiveCount = total - activas
            };
        }
    }
}
