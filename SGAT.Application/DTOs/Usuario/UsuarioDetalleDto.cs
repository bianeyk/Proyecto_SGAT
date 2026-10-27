using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.Usuario
{
    public class UsuarioDetalleDto
    {
        public string Id { get; set; } = string.Empty;

        public string NombreCompleto { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public bool Activo { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
