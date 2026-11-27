namespace SGAT.Application.DTOs.Equipo;

public sealed class GuardarEquipoDto
{
    public int IdCliente { get; set; }
    public int IdCategoriaDispositivo { get; set; }
    public int IdMarca { get; set; }
    public string Modelo { get; set; } = "";
    public string? Serial { get; set; }
    public bool SerialGenerado { get; set; }
    public string? Descripcion { get; set; }
    public string Version { get; set; } = "";
}
