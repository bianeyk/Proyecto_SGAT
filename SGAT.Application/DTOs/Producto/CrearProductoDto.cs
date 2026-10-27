using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.Producto
{
    public class CrearProductoDto
    {
        public int IdCategoriaProducto { get; set; }
        public string? Codigo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Condicion { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public decimal? PrecioDescuento { get; set; }
        public decimal StockMinimo { get; set; } = 0;
    }
}
