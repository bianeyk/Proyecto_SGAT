using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using SGAT.SGAT.Domain.Enums;

namespace SGAT.Web.ViewModels.Producto
{
    public abstract class ProductoFormViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría.")]
        [Display(Name = "Categoría")]
        public int IdCategoriaProducto { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una marca.")]
        [Display(Name = "Marca")]
        public int IdMarca { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Condición")]
        public CondicionProductoEnum Condicion { get; set; } = CondicionProductoEnum.NUEVO;

        [Range(typeof(decimal), "0.01", "99999999.99", ErrorMessage = "El precio de venta debe ser mayor que cero.")]
        [Display(Name = "Precio de venta")]
        public decimal PrecioVenta { get; set; }

        [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "El precio de descuento debe ser cero o mayor.")]
        [Display(Name = "Precio de descuento")]
        public decimal? PrecioDescuento { get; set; }

        [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "El stock mínimo debe ser cero o mayor.")]
        [Display(Name = "Stock mínimo")]
        public decimal StockMinimo { get; set; }

        public IReadOnlyList<SelectListItem> CategoriasDisponibles { get; set; } =
            Array.Empty<SelectListItem>();

        public IReadOnlyList<SelectListItem> MarcasDisponibles { get; set; } =
            Array.Empty<SelectListItem>();
    }
}
