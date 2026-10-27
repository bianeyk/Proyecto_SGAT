using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class Factura
    {
        public int IdFactura { get; set; }

        public int IdCliente { get; set; }
        public int? IdOrdenServicio { get; set; }
        public string IdUsuario { get; set; } = string.Empty;

        public string NumeroFactura { get; set; } = string.Empty;
        public DateTime FechaFactura { get; set; } = DateTime.Now;
        public decimal Subtotal { get; set; } = 0;
        public decimal Iva { get; set; } = 0;
        public decimal Total { get; set; } = 0;
        public decimal SaldoPendiente { get; set; } = 0;

        //Cambiar la propiedad MetodoPago a un enum ya que estaba de tipo string
        public MetodoPagoEnum MetodoPago { get; set; } 
        public EstadoFacturaEnum Estado { get; set; } = EstadoFacturaEnum.PENDIENTE;
        public string? Observaciones { get; set; }
        public int GarantiaDias { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navegacion
        public Cliente Cliente { get; set; } = null!;
        public OrdenServicio? OrdenServicio { get; set; }
        public ApplicationUser Usuario { get; set; } = null!;
        public ICollection<FacturaDetalle> Detalles { get; set; } = new List<FacturaDetalle>();
        public ICollection<PagoCartera> PagosCartera { get; set; } = new List<PagoCartera>();
    }
}
