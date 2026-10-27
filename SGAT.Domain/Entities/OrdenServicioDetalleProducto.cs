using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class OrdenServicioDetalleProducto
    {
        public int IdOrdenServicioDetalleProducto { get; set; }

        public int IdOrdenServicio { get; set; }
        public int IdProducto { get; set; }
        public int IdLoteProducto { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public EstadoOrdenServicioProductoEnum Estado { get; set; } = EstadoOrdenServicioProductoEnum.RESERVADO;


        public decimal Subtotal { get; set; } // columna calculada

        // Navegacion
        public OrdenServicio OrdenServicio { get; set; } = null!;
        public Producto Producto { get; set; } = null!;
        public LoteProducto LoteProducto { get; set; } = null!;
    }
}
