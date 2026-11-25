using SGAT.Application.DTOs.Producto;
using SGAT.Application.DTOs.Paginacion;

namespace SGAT.Application.Interfaces.Services
{
    public interface IMarcaService
    {
        Task<IEnumerable<MarcaDto>> GetTodasAsync();

        Task<MarcaDto?> GetByIdAsync(int id);

        Task<IEnumerable<MarcaDto>> BuscarAsync(string termino);
        Task<PagedResult<MarcaDto>> GetPageAsync(string? termino, int pagina, int tamanoPagina);

        Task<MarcaDto> CrearAsync(MarcaDto dto);

        Task<MarcaDto> ActualizarAsync(int id, MarcaDto dto);

        Task<bool> ActivarDesactivarAsync(int id);
    }
}
