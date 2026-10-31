using System.ComponentModel.DataAnnotations;

namespace SGAT.Application.DTOs.Tecnico
{
    public class FinalizarReparacionDto
    {
        [Required(ErrorMessage = "El ID de la orden es obligatorio.")]
        public int OrdenId { get; set; }

        [Required(ErrorMessage = "La solución aplicada es obligatoria.")]
        [MaxLength(2000, ErrorMessage = "La solución aplicada no puede superar los 2000 caracteres.")]
        public string SolucionAplicada { get; set; } = string.Empty;

        [Range(0, 99999999.99, ErrorMessage = "El costo final debe ser mayor o igual a 0.")]
        public decimal CostoFinal { get; set; }
    }
}
