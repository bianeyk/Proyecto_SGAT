using SGAT.Application.DTOs.Equipo;
using SGAT.Application.DTOs.Paginacion;

namespace SGAT.Application.Interfaces.Services;

public interface ICategoriaDispositivoService
{
    Task<CategoriaDispositivoDto?> GetByIdAsync(int id);
    Task<PagedResult<CategoriaDispositivoDto>> GetPageAsync(string? termino, int pagina, int tamanoPagina);
    Task<CategoriaDispositivoDto> CrearAsync(CategoriaDispositivoDto dto);
    Task<CategoriaDispositivoDto> ActualizarAsync(int id, CategoriaDispositivoDto dto);
    Task<bool> ActivarDesactivarAsync(int id);
}
