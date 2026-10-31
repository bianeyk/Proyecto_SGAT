using SGAT.Domain.Enums;
using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class OrdenServicio
    {
        public int IdOrdenServicio { get; set; }

        public int IdEquipo { get; set; }
        public string IdUsuarioCrea { get; set; } = string.Empty;
        public int IdEstadoServicio { get; set; }

        public string Codigo { get; set; } = string.Empty;
        public string? Diagnostico { get; set; }
        public decimal CostoTotal { get; set; } = 0;
        public decimal Abono { get; set; } = 0;
        public PrioridadEnum Prioridad { get; set; } = PrioridadEnum.MEDIA;
        public EstadoOrden EstadoOrden { get; set; } = EstadoOrden.PendienteRevision;

        public DateTime FechaIngreso { get; set; } = DateTime.Now;
        public DateTime? FechaPrometida { get; set; }
        public DateTime? FechaCierre { get; set; }

        public string? Observaciones { get; set; }

        // Navegacion
        public Equipo Equipo { get; set; } = null!;
        public ApplicationUser UsuarioCrea { get; set; } = null!;
        public EstadoServicio EstadoServicio { get; set; } = null!;
        public DiagnosticoTecnico? DiagnosticoTecnico { get; set; }
        public ICollection<BitacoraTecnica> Bitacoras { get; set; } = new List<BitacoraTecnica>();
        public ICollection<OrdenServicioDetalle> Detalles { get; set; } = new List<OrdenServicioDetalle>();
        public ICollection<OrdenServicioDetalleProducto> DetallesProducto { get; set; } = new List<OrdenServicioDetalleProducto>();
        public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
        public ICollection<HistorialServicio> HistorialesServicio { get; set; } = new List<HistorialServicio>();
        public ICollection<PagoCartera> PagosCartera { get; set; } = new List<PagoCartera>();
    }
}
