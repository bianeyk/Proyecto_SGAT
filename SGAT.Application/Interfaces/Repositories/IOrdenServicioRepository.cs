using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IOrdenServicioRepository : IGenericRepository<OrdenServicio>
    {
        Task<OrdenServicio?> GetByCodigoAsync(string codigo);
        Task<OrdenServicio?> GetConDetallesAsync(int id);
        Task<IEnumerable<OrdenServicio>> GetByClienteAsync(int idCliente);
        Task<IEnumerable<OrdenServicio>> GetByEstadoAsync(int idEstado);
        Task<IEnumerable<OrdenServicio>> GetByTecnicoAsync(string idUsuario);
        Task<string> GenerarCodigoAsync();

    }
}
