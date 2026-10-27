using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.Producto
{
    public class ProductoDto
    {
        public int IdProducto { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Condicion { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public bool  Activo { get; set; }

        //Alerta cuando el stock está abajo
        public bool StockBajo => StockActual <= StockMinimo;
    }
}
