using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class RolePermiso
    {
        public string RoleId { get; set; } = string.Empty;

        public int IdPermiso { get; set; }

        public DateTime CreatedAt { get; set; }

        public ApplicationRole Role { get; set; } = null!;

        public Permiso Permiso { get; set; } = null!;
    }
}
