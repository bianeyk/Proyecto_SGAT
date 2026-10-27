using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class HistorialServicio
    {
        public int IdHistorialServicio { get; set; }

        public int IdOrdenServicio { get; set; }
        public int IdEstadoServicio { get; set; }
        public string IdUsuario { get; set; } = string.Empty;

        public string? Comentario { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navegacion
        public OrdenServicio OrdenServicio { get; set; } = null!;
        public EstadoServicio EstadoServicio { get; set; } = null!;
        public ApplicationUser Usuario { get; set; } = null!;
    }
}
