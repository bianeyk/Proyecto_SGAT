using System.ComponentModel.DataAnnotations;

namespace SGAT.Web.ViewModels.Cliente
{
    public class ClienteViewModel
    {
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [StringLength(50, ErrorMessage = "El documento no puede superar los 50 caracteres.")]
        [Display(Name = "Número de documento")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El documento solo debe contener números.")]
        public string NumDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(250, ErrorMessage = "El nombre no puede superar los 250 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s\-\'\.]+$", ErrorMessage = "No se permiten caracteres especiales como asteriscos (*).")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(20, ErrorMessage = "El teléfono no puede superar los 20 caracteres.")]
        [Phone(ErrorMessage = "Ingresa un teléfono válido.")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "El teléfono solo debe contener números.")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Ingresa un correo electrónico válido.")]
        [StringLength(150, ErrorMessage = "El correo no puede superar los 150 caracteres.")]
        [Display(Name = "Correo electrónico")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Ingrese un correo electrónico corporativo o personal válido.")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "La dirección no puede superar los 255 caracteres.")]
        [Display(Name = "Dirección")]
        [RegularExpression(@"^[a-zA-Z0-9áéíóúÁÉÍÓÚñÑüÜ\s#.,\/-]+$", ErrorMessage = "La dirección contiene caracteres no permitidos (no use * ni símbolos extraños).")]
        public string? Direccion { get; set; }

        [Display(Name = "Es colaborador")]
        public bool EsColaborador { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
