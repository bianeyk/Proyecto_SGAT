using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class Proveedor
    {
        public int IdProveedor { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string? Contacto { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navegacion
        public ICollection<Compra> Compras { get; set; } = new List<Compra>();
        public ICollection<GarantiaProveedor> GarantiasProveedor { get; set; } = new List<GarantiaProveedor>();
        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
    }
}
