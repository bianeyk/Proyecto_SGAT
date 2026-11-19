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

        public int Total => Categorias.Count();
        public int TotalActivas => Categorias.Count(c => c.Activo);
        public int TotalInactivas => Categorias.Count(c => !c.Activo);

        public bool HayBusqueda => !string.IsNullOrWhiteSpace(Termino);
    }
}
