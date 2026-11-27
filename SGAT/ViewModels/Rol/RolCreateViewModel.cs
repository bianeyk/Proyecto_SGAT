using System.ComponentModel.DataAnnotations;
using SGAT.Domain.Constants;

namespace SGAT.Web.ViewModels.Rol
{
    public class RolCreateViewModel
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(
            LimitesValidacion.NombreRolMaximo,
            ErrorMessage = "El nombre del rol no puede superar los {1} caracteres.")]
        [RegularExpression(
            @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9 _-]+$",
            ErrorMessage = "El nombre del rol contiene caracteres no permitidos.")]
        [Display(Name = "Nombre del rol")]
        public string Nombre { get; set; } = string.Empty;
    }
}