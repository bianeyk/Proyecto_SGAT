using SGAT.Domain.Entities;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IClienteRepository:IGenericRepository<Cliente>
    {
        Task<Cliente?> GetByNumDocAsync(string numDoc);
        Task<bool> NumDocExisteAsync(string numDoc);
        Task<IEnumerable<Cliente>> BuscarAsync(string termino);
    }
}
