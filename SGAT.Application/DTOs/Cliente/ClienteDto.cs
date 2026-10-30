using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.Cliente
{
    public class ClienteDto
    {
        public int IdCliente { get; set; }
        public string NumDocumento { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public bool EsColaborador { get; set; } = false;

        public bool Activo { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
