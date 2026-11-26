using SGAT.Application.DTOs.Paginacion;

namespace SGAT.Application.DTOs.Rol
{
    public sealed class RolFiltroDto
    {
        public string? Termino { get; set; }

        public bool? Activo { get; set; }

        public int Pagina { get; set; } = PaginacionHelper.PaginaMinima;

        public int TamanoPagina { get; set; } = PaginacionHelper.TamanoPredeterminado;
    }
}
