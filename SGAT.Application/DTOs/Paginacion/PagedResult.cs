namespace SGAT.Application.DTOs.Paginacion;

// Resultado estándar para devolver únicamente una página y el total de registros.
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int TotalCount { get; init; }
    public int ActiveCount { get; init; }
    public int InactiveCount { get; init; }
    public int LowStockCount { get; init; }
}
