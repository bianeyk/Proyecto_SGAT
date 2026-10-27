using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class PagoCartera
    {
        public int IdPagoCartera { get; set; }

        public int IdFactura { get; set; }
        public int IdCliente { get; set; }
        public int? IdOrdenServicio { get; set; }

        public decimal Abono { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.Now;
        public MetodoPagoEnum MetodoPago { get; set; }
        public string? Referencia { get; set; }
        public string? Observacion { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navegacion
        public Factura Factura { get; set; } = null!;
        public Cliente Cliente { get; set; } = null!;
        public OrdenServicio? OrdenServicio { get; set; }
    }
}
