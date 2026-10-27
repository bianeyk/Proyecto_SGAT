using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class EstadoServicio
    {
        public int IdEstadoServicio { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public ICollection<OrdenServicio> OrdenesServicio { get; set; } = new List<OrdenServicio>();
        public ICollection<HistorialServicio> HistorialesServicio { get; set; } = new List<HistorialServicio>();
    }
}
