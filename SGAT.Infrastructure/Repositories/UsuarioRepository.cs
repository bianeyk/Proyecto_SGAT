using Microsoft.EntityFrameworkCore;
using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.DTOs.Usuario;
using SGAT.Application.Interfaces.Repositories;
using SGAT.Infrastructure.Data;

namespace SGAT.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResult<UsuarioListDto>>
            ObtenerPaginadosAsync(UsuarioFiltroDto filtro)
        {
            ArgumentNullException.ThrowIfNull(filtro);

            // 1. Limitar los valores de paginación.
            var pagina = PaginacionHelper.NormalizarPagina(filtro.Pagina);

            var tamanoPagina = PaginacionHelper.NormalizarTamano(filtro.TamanoPagina);

            // 2. Consulta base de solo lectura.
            var query = _context.Users.AsNoTracking();

            // 3. Obtener los contadores generales.
            var resumen = await query
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Activos = g.Count(u => u.Activo)
                })
                .FirstOrDefaultAsync();

            var totalGeneral = resumen?.Total ?? 0;
            var totalActivos = resumen?.Activos ?? 0;

            // 4. Aplicar búsqueda por nombre o correo.
            if (!string.IsNullOrWhiteSpace(filtro.Termino))
            {
                var termino = filtro.Termino.Trim();

                if (termino.Length < 2 || termino.Length > 100)
                {
                    throw new ArgumentException(
                        "La búsqueda debe contener entre 2 y 100 caracteres.");
                }

                query = query.Where(u =>
                    u.NombreCompleto.StartsWith(termino) ||
                    (u.Email != null &&
                     u.Email.StartsWith(termino)));
            }

            // 5. Aplicar filtro por estado.
            if (filtro.Activo.HasValue)
            {
                query = query.Where(u =>
                    u.Activo == filtro.Activo.Value);
            }

            // 6. Aplicar filtro por rol.
            if (!string.IsNullOrWhiteSpace(filtro.Rol))
            {
                var rolId = filtro.Rol.Trim();

                query = query.Where(u =>
                    _context.UserRoles.Any(ur =>
                        ur.UserId == u.Id &&
                        ur.RoleId == rolId));
            }

            // 7. Contar los registros filtrados.
            var totalFiltrado = await query.CountAsync();

            // 8. Obtener únicamente la página solicitada.
            var usuarios = await query
                .OrderBy(u => u.NombreCompleto)
                .ThenBy(u => u.Id)
                .Skip((pagina - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .Select(u => new UsuarioListDto
                {
                    Id = u.Id,
                    NombreCompleto = u.NombreCompleto,
                    Email = u.Email ?? string.Empty,
                    Activo = u.Activo
                })
                .ToListAsync();

            // 9. Obtener los roles de la página.
            var usuariosIds = usuarios
                .Select(u => u.Id)
                .ToList();

            if (usuariosIds.Count > 0)
            {
                var rolesUsuarios = await (
                    from ur in _context.UserRoles.AsNoTracking()
                    join r in _context.Roles.AsNoTracking()
                        on ur.RoleId equals r.Id
                    where usuariosIds.Contains(ur.UserId)
                    select new
                    {
                        ur.UserId,
                        NombreRol = r.Name ?? string.Empty
                    })
                    .ToListAsync();

                var rolesPorUsuario = rolesUsuarios
                    .ToLookup(r => r.UserId, r => r.NombreRol);

                foreach (var usuario in usuarios)
                {
                    usuario.Roles = rolesPorUsuario[usuario.Id]
                        .ToList();
                }
            }

            // 10. Retornar el resultado paginado.
            return new PagedResult<UsuarioListDto>
            {
                Items = usuarios,
                TotalCount = totalFiltrado,
                ActiveCount = totalActivos,
                InactiveCount = totalGeneral - totalActivos
            };
        }
    }
}