using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IFacturaRepository:IGenericRepository<Factura>
    {
        Task<Factura?> GetByNumeroAsync(string numeroFactura);
        Task<Factura?> GetConDetallesAsync(int id);
        Task<IEnumerable<Factura>> GetByClienteAsync(int idCliente);
        Task<IEnumerable<Factura>> GetPendientesAsync();
        Task<string> GenerarNumeroFacturaAsync();
    }
}
