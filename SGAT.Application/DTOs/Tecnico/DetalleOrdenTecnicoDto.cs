using System;
using System.Collections.Generic;

namespace SGAT.Application.DTOs.Tecnico
{
    public class DetalleOrdenTecnicoDto
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int EstadoValor { get; set; }
        public int EstadoServicioId { get; set; }
        public string EstadoServicioNombre { get; set; } = string.Empty;
        public string Prioridad { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; }
        public DateTime? FechaPrometida { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string? DiagnosticoInicial { get; set; }
        public string? Observaciones { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal Abono { get; set; }
        public decimal SaldoPendiente => Math.Max(0, CostoTotal - Abono);
        public string UsuarioCreaNombre { get; set; } = string.Empty;

        public ClienteTecnicoDto Cliente { get; set; } = new();
        public EquipoTecnicoDto Equipo { get; set; } = new();
        public DiagnosticoTecnicoDetalleDto? DiagnosticoActual { get; set; }
        public List<BitacoraTecnicaItemDto> HistorialNotas { get; set; } = new();
        public List<ServicioAplicadoTecnicoDto> ServiciosAplicados { get; set; } = new();
        public List<RepuestoAplicadoTecnicoDto> RepuestosAplicados { get; set; } = new();
    }

    public class ClienteTecnicoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string NumDoc { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Direccion { get; set; }
    }

    public class EquipoTecnicoDto
    {
        public int Id { get; set; }
        public string Categoria { get; set; } = string.Empty;
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public string? Serial { get; set; }
        public string? Descripcion { get; set; }
    }

    public class DiagnosticoTecnicoDetalleDto
    {
        public int Id { get; set; }
        public string? FallaReportada { get; set; }
        public string DiagnosticoDetallado { get; set; } = string.Empty;
        public string? SolucionAplicada { get; set; }
        public bool RequiereRepuestos { get; set; }
        public string? RepuestosSugeridos { get; set; }
        public decimal CostoManoDeObra { get; set; }
        public decimal CostoTotalEstimado { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFinalizacion { get; set; }
        public string TecnicoId { get; set; } = string.Empty;
        public string TecnicoNombre { get; set; } = string.Empty;
    }

    public class BitacoraTecnicaItemDto
    {
        public int Id { get; set; }
        public string Nota { get; set; } = string.Empty;
        public DateTime FechaRegistro { get; set; }
    }

    public class ServicioAplicadoTecnicoDto
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public string ServicioNombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string? Observacion { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string UsuarioCrea { get; set; } = string.Empty;
    }

    public class RepuestoAplicadoTecnicoDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string ProductoNombre { get; set; } = string.Empty;
        public int LoteId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string Estado { get; set; } = string.Empty;
    }
}
