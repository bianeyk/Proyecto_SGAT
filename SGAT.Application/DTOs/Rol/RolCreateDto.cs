using System.ComponentModel.DataAnnotations;

namespace SGAT.Application.DTOs.Rol
{
    public class RolCreateDto
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(50,ErrorMessage = "El nombre del rol no puede superar los 50 caracteres.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9]+([ _-][a-zA-ZáéíóúÁÉÍÓÚüÜñÑ0-9]+)*$", ErrorMessage = "El nombre del rol contiene caracteres no permitidos.")]
        public string Nombre { get; set; } = string.Empty;
    }
}