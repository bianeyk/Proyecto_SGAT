using SGAT.Application.DTOs.Producto;

namespace SGAT.Application.Interfaces.Services
{
    public interface IMarcaService
    {
        Task<IEnumerable<MarcaDto>> GetTodasAsync();

        Task<MarcaDto?> GetByIdAsync(int id);

        Task<IEnumerable<MarcaDto>> BuscarAsync(string termino);

        Task<MarcaDto> CrearAsync(MarcaDto dto);

        Task<MarcaDto> ActualizarAsync(int id, MarcaDto dto);

        Task<bool> ActivarDesactivarAsync(int id);
    }
}
