using System.ComponentModel.DataAnnotations;

namespace SGAT.Web.ViewModels.CategoriaProducto
{
    /// <summary>
    /// ViewModel para crear y editar categorías de producto.
    /// </summary>
    public class CategoriaProductoViewModel
    {
        public int IdCategoriaProducto { get; set; }

        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-.,()/&'+]+$",
            ErrorMessage = "El nombre contiene caracteres no permitidos (no use * ni símbolos extraños).")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede superar los 500 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-.,()/&'+]+$",
            ErrorMessage = "La descripción contiene caracteres no permitidos.")]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Activa")]
        public bool Activo { get; set; } = true;
    }
}
