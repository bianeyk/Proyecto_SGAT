using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SGAT.Application.DTOs.Usuario
{
    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ]+([ '-][a-zA-ZáéíóúÁÉÍÓÚüÜñÑ]+)*$",ErrorMessage = "El nombre solo puede contener letras, espacios, guiones y apóstrofes.")]
        [StringLength(100, ErrorMessage = "El nombre completo no puede superar los 100 caracteres.")]
        public string NombreCompleto { get; set; } = string.Empty;

        
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(256,ErrorMessage = "El correo electrónico no puede superar los 256 caracteres.")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$",ErrorMessage = "Ingrese un correo con una estructura válida. Ejemplo: usuario@gmail.com.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6,
     ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña.")]
        [DataType(DataType.Password)]
        [Compare("Password",
            ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        public string Rol { get; set; } = string.Empty;
    }
}
