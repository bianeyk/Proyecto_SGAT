using SGAT.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SGAT.Application.Interfaces.Repositories
{
    public interface IProductoRepository : IGenericRepository<Producto>
    {
        Task<Producto?> GetByCodigoAsync(string codigo);
        Task<IEnumerable<Producto>> GetStockBajoAsync();
        Task<IEnumerable<Producto>> GetActivosAsync();
        Task<bool> CodigoExisteAsync(string codigo);

        // Lecturas con categoría y marca cargadas para mostrarlas en las vistas.
        Task<IEnumerable<Producto>> GetAllConDetalleAsync();
        Task<Producto?> GetByIdConDetalleAsync(int id);

        // Catálogos activos para los desplegables del formulario.
        Task<IEnumerable<CategoriaProducto>> GetCategoriasActivasAsync();
        Task<IEnumerable<Marca>> GetMarcasActivasAsync();
        Task<bool> CategoriaExisteAsync(int idCategoria);
        Task<bool> MarcaExisteAsync(int idMarca);
    }
}
