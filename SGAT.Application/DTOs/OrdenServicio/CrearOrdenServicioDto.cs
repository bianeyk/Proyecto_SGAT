using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.OrdenServicio
{
    public class CrearOrdenServicioDto
    {
        public int IdEquipo { get; set; }
        public string? Diagnostico { get; set; }
        public string? Prioridad { get; set; } = "MEDIA";
        public DateTime? FechaPrometida { get; set; }
        public string? Observaciones { get; set; }
    }
}
