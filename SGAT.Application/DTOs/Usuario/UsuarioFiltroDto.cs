using SGAT.Application.DTOs.Paginacion;

namespace SGAT.Application.DTOs.Usuario
{
    public sealed class UsuarioFiltroDto
    {
        public string? Termino { get; set; }

        public bool? Activo { get; set; }

        public string? Rol { get; set; }

        public int Pagina { get; set; } = PaginacionHelper.PaginaMinima;

        public int TamanoPagina { get; set; } = PaginacionHelper.TamanoPredeterminado;
    }
}