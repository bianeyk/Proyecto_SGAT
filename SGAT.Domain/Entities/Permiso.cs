using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class Permiso
    {
        public int IdPermiso { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string? Descripcion { get; set; }

        public ICollection<RolePermiso> RolesPermisos { get; set; }
            = new List<RolePermiso>();
    }
}
