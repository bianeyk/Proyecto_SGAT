namespace SGAT.Application.DTOs.Paginacion;

// Resultado estándar para devolver únicamente una página y el total de registros.
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
}
