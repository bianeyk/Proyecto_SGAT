using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class Servicio
    {
        public int IdServicio { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string? DescripcionBase { get; set; }
        public decimal? PrecioBase { get; set; }
        public bool? Activo { get; set; } = true;

        public ICollection<OrdenServicioDetalle> OrdenesServicioDetalle { get; set; } = new List<OrdenServicioDetalle>();
    }
}
