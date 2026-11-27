namespace SGAT.Application.DTOs.Equipo;

public sealed record CategoriaDispositivoDto(
    int IdCategoriaDispositivo,
    string Nombre,
    string? Descripcion,
    bool Activo);
