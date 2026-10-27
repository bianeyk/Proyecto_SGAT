using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class MovimientoInventario
    {
        public int IdMovimientoInventario { get; set; }

        public int IdProducto { get; set; }
        public int? IdProveedor { get; set; }

        public TipoMovimientoEnum Tipo { get; set; }
        public decimal Cantidad { get; set; }
        public decimal? StockAnterior { get; set; }
        public decimal? StockNuevo { get; set; }
        public string? Observacion { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navegacion
        public Producto Producto { get; set; } = null!;
        public Proveedor? Proveedor { get; set; }
    }
}
