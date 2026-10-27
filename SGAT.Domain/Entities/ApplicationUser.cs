using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public string NombreCompleto { get; set; }=string.Empty;

        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastLoginAt { get; set; }
        
        //Navegaciones

        public ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();

        public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();

        public ICollection<Compra> Compras { get; set; } = new List<Compra>();

        public ICollection<OrdenServicio> OrdenesCreadas { get; set; } = new List<OrdenServicio>();

        public ICollection<Factura> Facturas { get; set; } = new List<Factura>();

        public ICollection<HistorialServicio> HistorialesServicio { get; set; } = new List<HistorialServicio>();

        public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();

    }
}
