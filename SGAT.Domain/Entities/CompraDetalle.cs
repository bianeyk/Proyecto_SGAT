using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class CompraDetalle
    {
        public int IdCompraDetalle { get; set; }

        public int IdCompra { get; set; }
        public int IdProducto { get; set; }

        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }

        public decimal Subtotal { get; set; } // columna calculada (persisted)

        // Navegacion
        public Compra Compra { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
        public ICollection<LoteProducto> Lotes { get; set; } = new List<LoteProducto>();
    }
}
