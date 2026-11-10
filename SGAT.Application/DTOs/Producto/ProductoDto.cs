using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.Producto
{
    public class ProductoDto
    {
        public int IdProducto { get; set; }
        public int IdCategoriaProducto { get; set; }
        public int IdMarca { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Condicion { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
        public decimal? PrecioDescuento { get; set; }
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal StockReservado { get; set; }
        public bool Activo { get; set; }
        public DateTime CreatedAt { get; set; }

        // Alerta cuando el stock está por debajo del mínimo configurado.
        public bool StockBajo => StockMinimo > 0 && StockActual <= StockMinimo;
    }
}
