using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SGAT.Application.DTOs.Rol;
using SGAT.Domain.Constants;

namespace SGAT.Web.ViewModels.Usuario
{
    public class UsuarioCreateViewModel
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [StringLength(
            LimitesValidacion.NombreUsuarioMaximo,
            ErrorMessage = "El nombre no puede superar los {1} caracteres.")]
        [RegularExpression(
            @"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ '’-]+$",
            ErrorMessage = "El nombre contiene caracteres no permitidos.")]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(
            LimitesValidacion.EmailMaximo,
            ErrorMessage = "El correo no puede superar los {1} caracteres.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(
            LimitesValidacion.PasswordMaximo,
            MinimumLength = LimitesValidacion.PasswordMinimo,
            ErrorMessage = "La contraseña debe tener entre {2} y {1} caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [StringLength(
            LimitesValidacion.PasswordMaximo,
            ErrorMessage = "La contraseña no puede superar los {1} caracteres.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password),
            ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

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