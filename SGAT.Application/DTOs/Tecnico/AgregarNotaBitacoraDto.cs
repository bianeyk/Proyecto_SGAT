using System.ComponentModel.DataAnnotations;

namespace SGAT.Application.DTOs.Tecnico
{
    public class AgregarNotaBitacoraDto
    {
        [Required(ErrorMessage = "El ID de la orden es obligatorio.")]
        public int OrdenId { get; set; }

        [Required(ErrorMessage = "La nota es obligatoria.")]
        [MaxLength(1000, ErrorMessage = "La nota no puede superar los 1000 caracteres.")]
        public string Nota { get; set; } = string.Empty;
    }
}
