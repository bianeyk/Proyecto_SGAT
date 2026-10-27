using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class Auditoria
    {
        public int IdAuditoria { get; set; }

        public string IdUsuario { get; set; } = string.Empty;

        public string Tabla { get; set; } = string.Empty;
        public int RegistroId { get; set; }
        public AccionAuditoriaEnum Accion { get; set; }
        public string? ValoresAnteriores { get; set; }
        public string? ValoresNuevos { get; set; }
        public string? Modulo { get; set; }
        public string? Observacion { get; set; }
        public string? IpAddress { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navegacion
        public ApplicationUser Usuario { get; set; } = null!;
    }
}
