using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IProductoRepository:IGenericRepository<Producto>
    {
        Task<Producto?> GetByCodigoAsync(string codigo);
        Task<IEnumerable<Producto>> GetStockBajoAsync();
        Task<IEnumerable<Producto>> GetActivosAsync();
        Task<bool> CodigoExisteAsync(string codigo);
    }
}
