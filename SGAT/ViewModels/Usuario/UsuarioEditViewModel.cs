using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SGAT.Application.DTOs.Rol;
using SGAT.Domain.Constants;

namespace SGAT.Web.ViewModels.Usuario
{
    public class UsuarioEditViewModel
    {
        [Required(ErrorMessage = "El identificador es obligatorio.")]
        [StringLength(
            LimitesValidacion.IdentityIdMaximo,
            ErrorMessage = "El identificador no es válido.")]
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(
            LimitesValidacion.NombreUsuarioMaximo,
            ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        [RegularExpression(
            @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ '’-]+$",
            ErrorMessage = "El nombre contiene caracteres no permitidos.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [BindRequired]
        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        [StringLength(
            LimitesValidacion.NombreRolMaximo,
            ErrorMessage = "El rol no puede superar los {1} caracteres.")]
        [Display(Name = "Rol")]
        public string Rol { get; set; } = string.Empty;

        [BindNever]
        public IReadOnlyList<RolDto> RolesDisponibles { get; set; }
            = new List<RolDto>();
    }
}