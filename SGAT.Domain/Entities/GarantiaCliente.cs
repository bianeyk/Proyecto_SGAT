using SGAT.SGAT.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Domain.Entities
{
    public class GarantiaCliente
    {
        public int IdGarantiaCliente { get; set; }

        public int IdFacturaDetalle { get; set; }

        public DateTime FechaInicio { get; set; } = DateTime.Now;
        public int GarantiaDias { get; set; }

        public DateTime FechaFinGarantia { get; set; } // columna calculada

        public EstadoGarantiaEnum Estado { get; set; } = EstadoGarantiaEnum.ACTIVA;
        public string? Observaciones { get; set; }

        // Navegacion
        public FacturaDetalle FacturaDetalle { get; set; } = null!;
    }
}
