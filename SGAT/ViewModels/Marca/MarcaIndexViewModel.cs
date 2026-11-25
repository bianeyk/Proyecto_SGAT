using SGAT.Application.DTOs.Producto;

namespace SGAT.Web.ViewModels.Marca
{
    /// <summary>
    /// ViewModel para el listado y la búsqueda de marcas.
    /// </summary>
    public class MarcaIndexViewModel
    {
        public IEnumerable<MarcaDto> Marcas { get; set; } = Enumerable.Empty<MarcaDto>();

        public string? Termino { get; set; }

        // Paginación
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public bool HayPaginaAnterior => PaginaActual > 1;
        public bool HayPaginaSiguiente => PaginaActual < TotalPaginas;

        public int Total => Marcas.Count();
        public int TotalActivas => Marcas.Count(m => m.Activo);
        public int TotalInactivas => Marcas.Count(m => !m.Activo);

        public bool HayBusqueda => !string.IsNullOrWhiteSpace(Termino);
    }
}
