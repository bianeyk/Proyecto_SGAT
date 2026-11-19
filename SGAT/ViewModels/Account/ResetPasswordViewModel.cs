using System.ComponentModel.DataAnnotations;

namespace SGAT.Web.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(256, ErrorMessage = "El correo no puede superar los 256 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(2048, ErrorMessage = "El token no puede superar los 256 caracteres.")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [StringLength(100, MinimumLength = 8,ErrorMessage = "La contraseña debe tener entre 8 y 100 caracteres.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare(nameof(NewPassword),
            ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}