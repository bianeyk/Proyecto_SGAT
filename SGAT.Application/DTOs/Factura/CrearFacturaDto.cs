using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.Factura
{
    public class CrearFacturaDto
    {
        public int IdCliente { get; set; }
        public int? IdOrdenServicio { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public int GarantiaDias { get; set; } = 0;
        public List<CrearFacturaDetalleDto> Detalles { get; set; } = new();
    }
}
