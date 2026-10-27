using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class Equipo
    {
        public int IdEquipo { get; set; }

        public string IdUsuario { get; set; } = string.Empty;
        public int IdCliente { get; set; }
        public int IdCategoriaDispositivo { get; set; }
        public int IdMarca { get; set; }

        public string Modelo { get; set; } = string.Empty;
        public string? Serial { get; set; }
        public string? Descripcion { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navegacion
        public ApplicationUser Usuario { get; set; } = null!;
        public Cliente Cliente { get; set; } = null!;
        public CategoriaDispositivo CategoriaDispositivo { get; set; } = null!;
        public Marca Marca { get; set; } = null!;
        public ICollection<OrdenServicio> OrdenesServicio { get; set; } = new List<OrdenServicio>();
    }
}
