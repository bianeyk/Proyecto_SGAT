using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.Cliente
{
    public class CrearClienteDto
    {
        public string NumDocumento { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Direccion { get; set; }
    }
}
