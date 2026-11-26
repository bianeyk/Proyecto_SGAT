using Microsoft.EntityFrameworkCore;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.DTOs.Rol;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories
{
    public class RolRepository : IRolRepository
    {
        private readonly ApplicationDbContext _context;

        public RolRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<RolDto>>
            ObtenerPaginadosAsync(RolFiltroDto filtro)
        {
            ArgumentNullException.ThrowIfNull(filtro);

            // 1. Normalizar la paginación.
            var pagina = PaginacionHelper.NormalizarPagina(
                filtro.Pagina);

            var tamanoPagina = PaginacionHelper.NormalizarTamano(
                filtro.TamanoPagina);

            // 2. Consulta base de solo lectura.
            var query = _context.Roles.AsNoTracking();

            // 3. Obtener los contadores generales.
            var resumen = await query
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Activos = g.Count(r => r.Activo)
                })
                .FirstOrDefaultAsync();

            var totalGeneral = resumen?.Total ?? 0;
            var totalActivos = resumen?.Activos ?? 0;

            // 4. Aplicar búsqueda por nombre.
            if (!string.IsNullOrWhiteSpace(filtro.Termino))
            {
                var termino = filtro.Termino.Trim();

                if (termino.Length < 2 || termino.Length > 50)
                {
                    throw new ArgumentException(
                        "La búsqueda debe contener entre 2 y 50 caracteres.");
                }

                query = query.Where(r =>
                    r.Name != null &&
                    r.Name.StartsWith(termino));
            }

            // 5. Aplicar filtro por estado.
            if (filtro.Activo.HasValue)
            {
                query = query.Where(r =>
                    r.Activo == filtro.Activo.Value);
            }

            // 6. Contar los registros filtrados.
            var totalFiltrado = await query.CountAsync();

            // 7. Obtener la página solicitada.
            var roles = await query
                .OrderBy(r => r.Name)
                .ThenBy(r => r.Id)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(r => new RolDto
                {
                    Id = r.Id,
                    Nombre = r.Name ?? string.Empty,
                    Activo = r.Activo
                })
                .ToListAsync();

            // 8. Retornar el resultado.
            return new PagedResult<RolDto>
            {
                Items = roles,
                TotalCount = totalFiltrado,
                ActiveCount = totalActivos,
                InactiveCount = totalGeneral - totalActivos
            };
        }
    }
}
