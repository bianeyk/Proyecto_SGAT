namespace SGAT.Application.DTOs.Equipo;

public sealed record ValidarRelacionesEquipoDto(
    int IdCliente,
    bool ClienteDebeEstarActivo,
    int IdMarca,
    bool MarcaDebeEstarActiva,
    int IdCategoriaDispositivo,
    bool CategoriaDebeEstarActiva);
