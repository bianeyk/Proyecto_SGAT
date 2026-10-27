using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.Factura
{
    public class CrearFacturaDetalleDto
    {
        public string Tipo { get; set; } = string.Empty; //"PRODUCTO" o "SERVICIO"
        public int? IdProducto { get; set; }
        public int? IdServicio { get; set; }
        public int? IdLoteProducto { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
