using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class OrdenServicioDetalle
    {
        public int IdOrdenServicioDetalle { get; set; }

        public int IdOrdenServicio { get; set; }
        public int IdServicio { get; set; }
        //Agregar propiedad de IdUsuarioCrea con respectiva propiedad de navegación
        public string IdUsuarioCrea { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;
        public string? Observacion { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navegacion
        public OrdenServicio OrdenServicio { get; set; } = null!;
        public Servicio Servicio { get; set; } = null!;
        public ApplicationUser UsuarioCrea { get; set; } = null!;
        public ICollection<FacturaDetalle> FacturaDetalles { get; set; } = new List<FacturaDetalle>();
    }
}
