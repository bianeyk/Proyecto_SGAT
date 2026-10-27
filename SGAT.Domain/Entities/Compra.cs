using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class Compra
    {
        public int IdCompra { get; set; }

        public int IdProveedor { get; set; }
        public string IdUsuario { get; set; } = string.Empty;

        public string? NumFactura { get; set; }
        public DateTime FechaCompra { get; set; } = DateTime.Now;
        public decimal Total { get; set; } = 0;
        public string? Observaciones { get; set; }

        // Navegacion
        public Proveedor Proveedor { get; set; } = null!;
        public ApplicationUser Usuario { get; set; } = null!;
        public ICollection<CompraDetalle> Detalles { get; set; } = new List<CompraDetalle>();
    }
}
