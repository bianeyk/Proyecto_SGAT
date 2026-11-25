using SGAT.Application.DTOs.Producto;

namespace SGAT.Web.ViewModels.CategoriaProducto
{
    /// <summary>
    /// ViewModel para el listado y la búsqueda de categorías de producto.
    /// </summary>
    public class CategoriaProductoIndexViewModel
    {
        public IEnumerable<CategoriaProductoDto> Categorias { get; set; } = Enumerable.Empty<CategoriaProductoDto>();

        public string? Termino { get; set; }

        // Paginación
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalRegistros { get; set; }
        public bool HayPaginaAnterior => PaginaActual > 1;
        public bool HayPaginaSiguiente => PaginaActual < TotalPaginas;

        public int Total => TotalRegistros;
        public int TotalActivas => Categorias.Count(c => c.Activo);
        public int TotalInactivas => Categorias.Count(c => !c.Activo);

        public bool HayBusqueda => !string.IsNullOrWhiteSpace(Termino);
    }
}
