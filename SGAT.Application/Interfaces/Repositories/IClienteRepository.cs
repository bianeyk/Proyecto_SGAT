using SGAT.Domain.Entities;
using SGAT.Application.DTOs.Paginacion;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IClienteRepository:IGenericRepository<Cliente>
    {
        Task<Cliente?> GetByNumDocAsync(string numDoc);
        Task<bool> NumDocExisteAsync(string numDoc);
        Task<PagedResult<Cliente>> GetPageAsync(string? termino, int pagina, int tamanoPagina);
    }
}
