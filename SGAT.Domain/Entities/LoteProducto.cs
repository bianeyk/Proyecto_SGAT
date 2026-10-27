using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class LoteProducto
    {
        public int IdLoteProducto { get; set; }

        public int IdCompraDetalle { get; set; }
        public int IdProducto { get; set; }

        public string NumeroLote { get; set; } = string.Empty;
        public string? Serial { get; set; }
        public decimal CantidadInicial { get; set; }
        public decimal CantidadDisponible { get; set; }
        public decimal CostoUnitario { get; set; }
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
        public int GarantiaProveedorDias { get; set; } = 0;
        public string? Observaciones { get; set; }

        public DateTime FechaFinGarantiaProveedor { get; set; } // columna calculada

        // Navegacion
        public CompraDetalle CompraDetalle { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
        public ICollection<OrdenServicioDetalleProducto> DetallesOrdenServicio { get; set; } = new List<OrdenServicioDetalleProducto>();
        public ICollection<FacturaDetalle> FacturaDetalles { get; set; } = new List<FacturaDetalle>();
        public ICollection<GarantiaProveedor> GarantiasProveedor { get; set; } = new List<GarantiaProveedor>();
    }
}
