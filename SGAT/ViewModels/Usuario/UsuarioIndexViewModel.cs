using SGAT.Application.DTOs.Paginacion;
using SGAT.Application.DTOs.Rol;
using SGAT.Application.DTOs.Usuario;

namespace SGAT.Web.ViewModels.Usuario
{
    public sealed class UsuarioIndexViewModel
    {
        public UsuarioFiltroDto Filtro { get; set; } = new();

        public PagedResult<UsuarioListDto> Resultado { get; set; }
            = new();

        public IReadOnlyList<RolDto> RolesDisponibles { get; set; }
            = Array.Empty<RolDto>();

        public int TotalUsuarios =>
            Resultado.ActiveCount + Resultado.InactiveCount;

        public int TotalPaginas =>
            Math.Clamp(
                (int)Math.Ceiling(
            Resultado.TotalCount /
            (double)Filtro.TamanoPagina),
        PaginacionHelper.PaginaMinima,
        PaginacionHelper.PaginaMaxima);

        public bool TienePaginaAnterior =>
            Filtro.Pagina > 1;

        public bool TienePaginaSiguiente =>
            Filtro.Pagina < TotalPaginas;
    }
}
