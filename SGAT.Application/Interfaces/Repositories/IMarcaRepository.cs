using SGAT.Domain.Entities;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IMarcaRepository : IGenericRepository<Marca>
    {
        Task<IEnumerable<Marca>> GetAllOrdenadasAsync();

        Task<Marca?> GetByNombreAsync(string nombre);

        Task<bool> NombreExisteAsync(string nombre, int? excluirId = null);

        Task<IEnumerable<Marca>> BuscarAsync(string termino);
    }
}
