namespace SGAT.Web.ViewModels.Producto
{
    /// <summary>
    /// ViewModel con la información completa de un producto para la vista de detalle.
    /// </summary>
    public class ProductoDetalleViewModel
    {
        public int IdProducto { get; set; }

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

        public decimal StockDisponible => StockActual - StockReservado;
        public bool StockBajo => StockMinimo > 0 && StockActual <= StockMinimo;
    }
}
