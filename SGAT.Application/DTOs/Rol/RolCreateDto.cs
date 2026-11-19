using System.ComponentModel.DataAnnotations;

namespace SGAT.Application.DTOs.Rol
{
    public class RolCreateDto
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(
            50,
            ErrorMessage = "El nombre del rol no puede superar los 50 caracteres.")]
        public string Nombre { get; set; } = string.Empty;
    }
}