using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class Marca
    {
        public int IdMarca { get; set; }

        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;

        public ICollection<Equipo> Equipos { get; set; } = new List<Equipo>();
    }
}
