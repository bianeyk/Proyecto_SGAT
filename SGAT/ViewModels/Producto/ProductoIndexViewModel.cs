using SGAT.Application.DTOs.Producto;

namespace SGAT.Web.ViewModels.Producto
{
    /// <summary>
    /// ViewModel para el listado, búsqueda y filtrado de productos.
    /// </summary>
    public class ProductoIndexViewModel
    {
        public IEnumerable<ProductoDto> Productos { get; set; } = Enumerable.Empty<ProductoDto>();

        public string? Termino { get; set; }

        /// <summary>
        /// Filtro aplicado: todos, activos, inactivos o stockbajo.
        /// </summary>
        public string Filtro { get; set; } = "todos";

        // Paginación
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalRegistros { get; set; }
        public bool HayPaginaAnterior => PaginaActual > 1;
        public bool HayPaginaSiguiente => PaginaActual < TotalPaginas;

        public int Total => TotalRegistros;
        public int TotalActivos { get; set; }
        public int TotalInactivos { get; set; }
        public int TotalStockBajo { get; set; }

        public bool HayBusqueda => !string.IsNullOrWhiteSpace(Termino);
    }
}
