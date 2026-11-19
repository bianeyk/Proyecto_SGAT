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

        public int Total => Marcas.Count();
        public int TotalActivas => Marcas.Count(m => m.Activo);
        public int TotalInactivas => Marcas.Count(m => !m.Activo);

        public bool HayBusqueda => !string.IsNullOrWhiteSpace(Termino);
    }
}
