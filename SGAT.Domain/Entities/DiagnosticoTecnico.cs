using System;

namespace SGAT.Domain.Entities
{
    public class DiagnosticoTecnico
    {
        public int Id { get; set; }

        public int OrdenId { get; set; }
        public string TecnicoId { get; set; } = string.Empty;

        public string? FallaReportada { get; set; }
        public string DiagnosticoDetallado { get; set; } = string.Empty;
        public string? SolucionAplicada { get; set; }
        public bool RequiereRepuestos { get; set; } = false;
        public string? RepuestosSugeridos { get; set; }

        public decimal CostoManoDeObra { get; set; } = 0;
        public decimal CostoTotalEstimado { get; set; } = 0;

        public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
        public DateTime? FechaFinalizacion { get; set; }

        // Navegación
        public OrdenServicio Orden { get; set; } = null!;
        public ApplicationUser Tecnico { get; set; } = null!;
    }
}
