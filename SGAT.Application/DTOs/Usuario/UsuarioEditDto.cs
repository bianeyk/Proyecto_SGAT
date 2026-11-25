using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SGAT.Application.DTOs.Usuario
{
    public class UsuarioEditDto
    {
        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ]+([ '-][a-zA-ZáéíóúÁÉÍÓÚüÜñÑ]+)*$", ErrorMessage = "El nombre solo puede contener letras, espacios, guiones y apóstrofes.")]
        [StringLength(100, ErrorMessage = "El nombre completo no puede superar los 100 caracteres.")]
        public string NombreCompleto { get; set; } = string.Empty;

        public bool Activo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        public string Rol { get; set; } = string.Empty;
    }
}
