using System.ComponentModel.DataAnnotations;

namespace SGAT.Web.ViewModels.Equipo;

public sealed class CategoriaDispositivoViewModel
{
    public int IdCategoriaDispositivo { get; set; }

    [Required(ErrorMessage = "El nombre de la categoría de dispositivo es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
    [Display(Name = "Nombre de la categoría de dispositivo")]
    public string Nombre { get; set; } = "";

    [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}
