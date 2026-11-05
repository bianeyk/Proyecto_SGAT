using SGAT.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace SGAT.Application.DTOs.Tecnico
{
    public class RegistrarDiagnosticoDto
    {
        [Required(ErrorMessage = "El ID de la orden es obligatorio.")]
        public int OrdenId { get; set; }

        [Required(ErrorMessage = "El diagnóstico detallado es obligatorio.")]
        [MaxLength(2000, ErrorMessage = "El diagnóstico detallado no puede superar los 2000 caracteres.")]
        public string DiagnosticoDetallado { get; set; } = string.Empty;

        public bool RequiereRepuestos { get; set; }

        [MaxLength(1000, ErrorMessage = "Los repuestos sugeridos no pueden superar los 1000 caracteres.")]
        public string? RepuestosSugeridos { get; set; }

        [Range(0, 99999999.99, ErrorMessage = "El costo de mano de obra debe ser un valor válido mayor o igual a 0.")]
        public decimal CostoManoDeObra { get; set; }

        [Required(ErrorMessage = "El nuevo estado de la orden es obligatorio.")]
        public EstadoOrden NuevoEstado { get; set; }
    }
}
