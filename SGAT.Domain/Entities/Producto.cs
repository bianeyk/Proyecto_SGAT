using SGAT.SGAT.Domain.Enums;

namespace SGAT.Domain.Entities
{
    public class Producto
    {
        public int IdProducto { get; set; }

        public int IdCategoriaProducto { get; set; }

        public string? Codigo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public CondicionProductoEnum Condicion { get; set; } = CondicionProductoEnum.NUEVO;
        public decimal PrecioVenta { get; set; }
        public decimal? PrecioDescuento { get; set; }
        public decimal StockActual { get; set; } = 0;
        public decimal StockMinimo { get; set; } = 0;
        public decimal StockReservado { get; set; } = 0;

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navegacion
        public CategoriaProducto CategoriaProducto { get; set; } = null!;
        public ICollection<CompraDetalle> CompraDetalles { get; set; } = new List<CompraDetalle>();
        public ICollection<LoteProducto> Lotes { get; set; } = new List<LoteProducto>();
        public ICollection<OrdenServicioDetalleProducto> DetallesOrdenServicio { get; set; } = new List<OrdenServicioDetalleProducto>();
        public ICollection<FacturaDetalle> FacturaDetalles { get; set; } = new List<FacturaDetalle>();
        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
    }
}
