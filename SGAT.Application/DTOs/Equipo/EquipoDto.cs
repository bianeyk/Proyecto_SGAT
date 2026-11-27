namespace SGAT.Application.DTOs.Equipo;

public sealed class EquipoDto
{
    public int IdEquipo { get; init; }
    public int IdCliente { get; init; }
    public int IdCategoriaDispositivo { get; init; }
    public int IdMarca { get; init; }
    public string Cliente { get; init; } = "";
    public string DocumentoCliente { get; init; } = "";
    public string Categoria { get; init; } = "";
    public string Marca { get; init; } = "";
    public string Modelo { get; init; } = "";
    public string? Serial { get; init; }
    public bool SerialGenerado { get; init; }
    public string? Descripcion { get; init; }
    public bool Activo { get; init; }
    public bool TieneOrdenes { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public string Version { get; init; } = "";
}

public sealed record OpcionEquipoDto(int Id, string Nombre);
