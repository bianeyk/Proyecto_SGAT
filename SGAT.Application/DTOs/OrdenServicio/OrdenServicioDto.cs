using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.DTOs.OrdenServicio
{
    public class OrdenServicioDto
    {
        public int IdOrdenServicio { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string Equipo { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public decimal CostoTotal { get; set; } 
        public decimal Abono { get; set; } 
        public DateTime FechaIngreso { get; set; } 
        public DateTime? FechaPrometida { get; set; } 
        public string? Diagnostico { get; set; } 
        public string TecnicoAsignado { get; set; } = string.Empty;

    }
}
