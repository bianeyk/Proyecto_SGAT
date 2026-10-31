using System;

namespace SGAT.Application.DTOs.Tecnico
{
    public class OrdenTecnicoResumenDto
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public string Dispositivo { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string? FallaReportada { get; set; }
        public string Estado { get; set; } = string.Empty;
        public int EstadoValor { get; set; }
        public DateTime FechaIngreso { get; set; }
    }
}
