using System.ComponentModel.DataAnnotations;
using SGAT.SGAT.Domain.Enums;

namespace SGAT.Web.ViewModels.Producto
{
    /// <summary>
    /// ViewModel para crear y editar productos.
    /// </summary>
    public class ProductoViewModel : IValidatableObject
    {
        public int IdProducto { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría válida.")]
        [Display(Name = "Categoría")]
        public int IdCategoriaProducto { get; set; }

        [Required(ErrorMessage = "La marca es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una marca válida.")]
        [Display(Name = "Marca")]
        public int IdMarca { get; set; }

        [StringLength(100, ErrorMessage = "El código no puede superar los 100 caracteres.")]
        [RegularExpression(@"^[A-Za-z0-9\-_.]+$", ErrorMessage = "El código solo puede contener letras, números, guiones, puntos y guiones bajos.")]
        [Display(Name = "Código")]
        public string? Codigo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-.,()/]+$", ErrorMessage = "El nombre contiene caracteres no permitidos.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Condición")]
        public CondicionProductoEnum Condicion { get; set; } = CondicionProductoEnum.NUEVO;

        [Required(ErrorMessage = "El precio de venta es obligatorio.")]
        [Range(typeof(decimal), "0.01", "99999999.99", ErrorMessage = "El precio de venta debe ser mayor que cero.")]
        [Display(Name = "Precio de venta")]
        [DataType(DataType.Currency)]
        public decimal PrecioVenta { get; set; }

        [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "El precio con descuento no puede ser negativo.")]
        [Display(Name = "Precio con descuento")]
        [DataType(DataType.Currency)]
        public decimal? PrecioDescuento { get; set; }

        [Range(typeof(decimal), "0", "99999999.99", ErrorMessage = "El stock mínimo no puede ser negativo.")]
        [Display(Name = "Stock mínimo")]
        public decimal StockMinimo { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;

        // Catálogos disponibles para los desplegables del formulario.
        public IReadOnlyList<CategoriaProductoOptionViewModel> Categorias { get; set; }
            = new List<CategoriaProductoOptionViewModel>();

        public IReadOnlyList<MarcaOptionViewModel> Marcas { get; set; }
            = new List<MarcaOptionViewModel>();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PrecioDescuento.HasValue && PrecioDescuento.Value >= PrecioVenta)
            {
                yield return new ValidationResult(
                    "El precio con descuento debe ser menor que el precio de venta.",
                    new[] { nameof(PrecioDescuento) });
            }
        }
    }
}
