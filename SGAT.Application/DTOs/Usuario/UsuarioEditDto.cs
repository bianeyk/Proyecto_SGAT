using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SGAT.Application.DTOs.Usuario
{
    public class UsuarioEditDto
    {
        public string Id { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public bool Activo { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        public string Rol { get; set; } = string.Empty;
    }
}
