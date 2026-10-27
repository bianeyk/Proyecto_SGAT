using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public bool Activo { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<RolePermiso> RolesPermisos { get; set; } = new List<RolePermiso>();
    }
}
