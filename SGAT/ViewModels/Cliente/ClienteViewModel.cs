using System.ComponentModel.DataAnnotations;

namespace SGAT.Web.ViewModels.Cliente
{
    public class ClienteViewModel
    {
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [StringLength(50, ErrorMessage = "El documento no puede superar los 50 caracteres.")]
        [Display(Name = "Número de documento")]
        public string NumDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(250, ErrorMessage = "El nombre no puede superar los 250 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        [Phone(ErrorMessage = "Ingresa un teléfono válido.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        [Display(Name = "Correo electrónico")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "La dirección no puede superar los 255 caracteres.")]
        [Display(Name = "Dirección")]
        public string? Direccion { get; set; }

        [Display(Name = "Es colaborador")]
        public bool EsColaborador { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
