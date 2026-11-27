using System.ComponentModel.DataAnnotations;
using SGAT.Application.DTOs.Equipo;

namespace SGAT.Web.ViewModels.Equipo;

public sealed class EquipoViewModel
{
    public int IdEquipo { get; set; }
    [Required(ErrorMessage = "Selecciona un cliente.")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un cliente válido.")]
    [Display(Name = "Cliente")]
    public int IdCliente { get; set; }
    [Required(ErrorMessage = "Selecciona una categoría de dispositivo.")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una categoría de dispositivo válida.")]
    [Display(Name = "Categoría de dispositivo")]
    public int IdCategoriaDispositivo { get; set; }
    [Required(ErrorMessage = "Selecciona una marca.")]
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una marca válida.")]
    [Display(Name = "Marca")]
    public int IdMarca { get; set; }
    [Required(ErrorMessage = "El modelo es obligatorio.")]
    [StringLength(100, ErrorMessage = "El modelo no puede superar los 100 caracteres.")]
    [Display(Name = "Modelo")]
    public string Modelo { get; set; } = "";
    [StringLength(100, ErrorMessage = "El serial no puede superar los 100 caracteres.")]
    [RegularExpression(@"^[A-Za-z0-9._/\-]+$", ErrorMessage = "Usa letras, números, puntos, guiones, barras o guiones bajos.")]
    [Display(Name = "Serial")]
    public string? Serial { get; set; }
    public bool SerialGenerado { get; set; }
    [StringLength(255, ErrorMessage = "La descripción no puede superar los 255 caracteres.")]
    [Display(Name = "Descripción")]
    public string? Descripcion { get; set; }
    public string Version { get; set; } = "";
    public bool TieneOrdenes { get; set; }
    public IReadOnlyList<OpcionEquipoDto> Clientes { get; set; } = [];
    public IReadOnlyList<OpcionEquipoDto> Marcas { get; set; } = [];
    public IReadOnlyList<OpcionEquipoDto> Categorias { get; set; } = [];
}
