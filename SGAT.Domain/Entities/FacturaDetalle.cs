using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class FacturaDetalle
    {
        public int IdFacturaDetalle { get; set; }

        public int IdFactura { get; set; }
        public int? IdLoteProducto { get; set; }
        public int? IdProducto { get; set; }
        public int? IdOrdenServicioDetalle { get; set; }

        public string Tipo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; } = 1;
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; } // columna calculada

        // Navegacion
        public Factura Factura { get; set; } = null!;
        public Producto? Producto { get; set; }
        public LoteProducto? LoteProducto { get; set; }
        public OrdenServicioDetalle? OrdenServicioDetalle { get; set; }
        public ICollection<GarantiaCliente> GarantiasCliente { get; set; } = new List<GarantiaCliente>();
    }
}
