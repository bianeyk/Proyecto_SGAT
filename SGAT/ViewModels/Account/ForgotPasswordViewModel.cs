
using System.ComponentModel.DataAnnotations;

namespace SGAT.Web.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El correo es obligatorio.")]
        [StringLength(256, ErrorMessage = "El correo no puede superar los 256 caracteres.")]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        public string Email { get; set; } = string.Empty;
    }
}
