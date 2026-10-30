using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class Cliente
    {
        
        public int IdCliente { get; set; }

        public string IdUsuarioCrea { get; set; } = string.Empty;
       
        public string NumDoc { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public bool EsColaborador { get; set; } = false;

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        // Navegacion
        public ApplicationUser UsuarioCrea { get; set; } = null!;
        public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
        public ICollection<Factura> Facturas { get; set; } = new List<Factura>();
        public ICollection<PagoCartera> PagosCartera { get; set; } = new List<PagoCartera>();
    }

}
