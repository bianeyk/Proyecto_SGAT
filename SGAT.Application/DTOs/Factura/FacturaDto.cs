using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.Factura
{
    public class FacturaDto
    {
        public int IdFactura { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public string  Cliente{ get; set; } = string.Empty;
        public string? OrdenServicio { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Iva { get; set; }
        public decimal Total { get; set; }
        public decimal SaldoPendiente { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaFactura { get; set; }
    }
}
