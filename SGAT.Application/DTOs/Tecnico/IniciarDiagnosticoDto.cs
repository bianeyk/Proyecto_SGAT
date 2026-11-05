using System.ComponentModel.DataAnnotations;

namespace SGAT.Application.DTOs.Tecnico
{
    public class IniciarDiagnosticoDto
    {
        [Required(ErrorMessage = "El ID de la orden es obligatorio.")]
        public int OrdenId { get; set; }

        [Required(ErrorMessage = "El ID del técnico es obligatorio.")]
        public string TecnicoId { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "La falla reportada no puede superar los 500 caracteres.")]
        public string? FallaReportada { get; set; }
    }
}
